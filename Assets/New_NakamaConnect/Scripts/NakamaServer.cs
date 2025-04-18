using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using String = ProjectCore.Variables.String;

namespace ProjectCore.SocialFeature.Cloud.Internal
{
    [CreateAssetMenu(fileName = "NakamaServer", menuName = "FPSCommando/SocialFeature/Cloud/NakamaServer")]
    [InlineEditor]
    public class NakamaServer : ScriptableObject
    {
        [SerializeField] private String AuthToken;
        [SerializeField] private String RefreshToken;
        
        [SerializeField] private GameEvent NakamaServerConnected;

        [SerializeField] private SocialFeatureConfig SocialFeatureConfig;

        public IClient Client { get; private set; }
        public ISocket Socket { get; private set; }
        public ISession Session { get; private set; }

        public void Initialize()
        {
            InitializeNakamaServer();
        }

        private void InitializeNakamaServer()
        {
            var environment = SocialFeatureConfig.GeneralSettings.Environment;
            var data = SocialFeatureConfig.GetLocalServerData();
            
            switch (environment)
            {
                case Environment.Live:
                    data = SocialFeatureConfig.GetLiveServerData();
                    break;
                case Environment.Dev:
                    data = SocialFeatureConfig.GetDevServerData();
                    break;
                case Environment.Local:
                    data = SocialFeatureConfig.GetLocalServerData();
                    break;
            }

            Client = new Client(data.Scheme, data.Host, data.Port, data.Key)
            {
                GlobalRetryConfiguration = SocialFeatureConfig.GetRetryConfiguration(),
                Timeout = SocialFeatureConfig.GeneralSettings.RetryTimeOut
            };
            
            Socket = Client.NewSocket();
            
            Socket.Connected += () =>
            {
                Debug.Log("[Nakama] Socket Connected");
            };
            
            Socket.Closed += () =>
            {
                Debug.Log("[Nakama] Socket Disconnected");
            };
        }

        #region DeviceIdAuth

        public async Task<ISession> AuthenticateWithDeviceID(CancellationToken cancellationToken)
        {
            Debug.Log("[Nakama] Authenticating with device ID");
            var deviceID = SocialFeatureConfig.GetDeviceUdid();
            
            try
            {
                Session = await Client.AuthenticateDeviceAsync(deviceID,
                    retryConfiguration: SocialFeatureConfig.GetRetryConfiguration(),
                    canceller: cancellationToken);

                await Socket.ConnectAsync(Session, SocialFeatureConfig.CanAppearOnline(), 10);

                NakamaServerConnected.Invoke();

                Debug.Log("[Nakama] Authenticated with device ID: " + deviceID);

                return Session;
            }
            catch (Exception ex)
            {
                if (ex is ApiResponseException)
                {
                    Debug.LogError("[Nakama] Failed to authenticate with device ID: " + ex.Message);
                }
                else if (ex is OperationCanceledException)
                {
                    Debug.LogError("[Nakama] Connection TimedOut");
                }
                return null;
            }
        }

        public async Task UnlinkWithDeviceID()
        {
            Debug.Log("[Nakama] Unlinking with device ID");
            var deviceID = SocialFeatureConfig.GetDeviceUdid();

            try
            {
                await Client.UnlinkDeviceAsync(Session, deviceID, SocialFeatureConfig.GetRetryConfiguration());
                Debug.Log("[Nakama] Unlinked with device ID: " + deviceID);
            }
            catch (ApiResponseException e)
            {
                Debug.Log("[Nakama] Failed to unlink with device ID: " + e.Message);
            }
        }

        #endregion

        #region EmailAuth

        public async Task<string> LinkWithEmail(string email, string password, Action<bool> callback)
        {
            Debug.Log("[Nakama] Syncing with email ID");
            try
            {
                await Client.LinkEmailAsync(Session, email, password, SocialFeatureConfig.GetRetryConfiguration());
                await UnlinkWithDeviceID();
                Debug.Log("[Nakama] Synced with email ID: " + email);
                callback?.Invoke(true);
                return "Account Synced";
            }
            catch (ApiResponseException e)
            {
                Debug.LogError($"[Nakama] Failed to sync with email ID: m:{e.Message} c:{e.GrpcStatusCode}");
                await AuthenticateWithDeviceID(TaskUtil.RefreshToken(ref _cancellationTokenSource));
                callback?.Invoke(false);
                return e.Message;
            }
        }
        
        public async Task LinkWithDevice()
        {
            Debug.Log("[Nakama] Linking with device ID");
            var deviceID = SocialFeatureConfig.GetDeviceUdid();
            await Client.LinkDeviceAsync(Session, deviceID, SocialFeatureConfig.GetRetryConfiguration());
            Debug.Log("[Nakama] Linked with device ID: " + deviceID);
        }

        public async Task AuthenticateWithEmail(string email, string password, CancellationToken token)
        {
            Debug.Log("[Nakama] Authenticating with email ID");
            try
            {
                /*ISession tempSession = null; 
                if (Session != null)
                {
                    tempSession = Session;
                }*/
                
                Session = await Client.AuthenticateEmailAsync(email, password, create: false,
                    retryConfiguration: SocialFeatureConfig.GetRetryConfiguration(),
                    canceller: token);

                await Socket.ConnectAsync(Session, true, 10);

                var account = await Client.GetAccountAsync(Session);

                /*if (account.Devices.ToList().Count > 0)
                {
                    await Client.DeleteAccountAsync(tempSession, SocialFeatureConfig.GetRetryConfiguration());
                }*/

                var user = account.User;

                var devices = account.Devices.ToList();

                if (devices.All(device => device.Id != SocialFeatureConfig.GetDeviceUdid()))
                {
                    await LinkWithDevice();
                    Debug.Log("[Nakama] Linked with device ID: " + SocialFeatureConfig.GetDeviceUdid());
                }
                
                Debug.LogError("[Nakama] User: " + user.Id);
                Debug.LogError("[Nakama] Email: " + account.Email);

                var enu = account.Devices.GetEnumerator();
                
                while (enu.MoveNext())
                {
                    Debug.Log("[Nakama] Device: " + enu.Current);
                }

                Debug.Log("[Nakama] Authenticated with email ID: " + email);
                NakamaServerConnected.Invoke();
            }
            catch (Exception ex)
            {
                if (ex is ApiResponseException)
                {
                    Debug.LogError("[Nakama] Failed to authenticate with email ID: " + ex.Message);
                }
                else if (ex is OperationCanceledException)
                {
                    Debug.LogError("[Nakama] Connection TimedOut");
                }
            }
        }

        public async Task UnlinkWithEmail(string email, string password, Action<bool> callback = null)
        {
            Debug.Log("[Nakama] Unlinking with email ID");
            try
            {
                await ValidateSession();
                await Client.UnlinkEmailAsync(Session, email, password, SocialFeatureConfig.GetRetryConfiguration());
                Debug.Log("[Nakama] Unlinked with email ID: " + email);
                callback?.Invoke(true);
            }
            catch (ApiResponseException e)
            {
                Debug.Log("[Nakama] Failed to unlink with email ID: " + e.Message);
                callback?.Invoke(false);
            }
        }

        #endregion

        #region Helper Functions

        private CancellationTokenSource _cancellationTokenSource;

        public async Task<bool> ValidateSession(bool forceSync = false)
        {
            if (Session == null)
            {
                Debug.LogError("[Nakama] Session is Invalid!!");

                Session = Nakama.Session.Restore(AuthToken, RefreshToken);
                if (Session == null)
                {
                    Debug.LogError("[Nakama] Failed to restore session");
                    return false;
                }
                var isValidSocket = await ValidateSocket();
                return isValidSocket;
            }

            if ((Session.IsExpired && Session.IsRefreshExpired) || forceSync)
            {
                Debug.LogError("[Nakama] Session is Invalid!!");
                try
                {
                    if (forceSync)
                    {
                        Session = await AuthenticateWithDeviceID(TaskUtil.RefreshToken(ref _cancellationTokenSource));
                    }
                    else
                    {
                        Session = await Client.SessionRefreshAsync(Session);
                    }

                    if (Session != null) return await ValidateSocket();
                    
                    Debug.LogError("[Nakama] Failed to refresh session");
                    return false;
                }
                catch (ApiResponseException ex)
                {
                    Debug.LogError("[Nakama] Failed to validate session: " + ex.Message);
                    return false;
                }
            }
            
            Debug.Log("[Nakama] Session is Valid!!");
            SaveSession();
            return true;
        }

        private async Task<bool> ValidateSocket()
        {
            if (!Socket.IsConnected)
            {
                Debug.LogError("[Nakama] Socket is Invalid!!");
                try
                {
                    await Socket.ConnectAsync(Session, SocialFeatureConfig.CanAppearOnline());
                    return true;
                }
                catch (ApiResponseException exception)
                {
                    Debug.LogError("[Nakama] Socket Connection Failed: " + exception.Message);
                    return false;
                }
            }
            Debug.Log("[Nakama] Socket is Valid!!");
            return true;
        }

        private void SaveSession()
        {
            AuthToken.SetValue(Session.AuthToken);
            RefreshToken.SetValue(Session.RefreshToken);
        }

        #endregion

        #region UserProgressStorage

        // https://heroiclabs.com/docs/nakama/concepts/storage/collections
        
        public async Task SaveUserDataAsync(IApiWriteStorageObject[] apiWriteStorageObjects)
        {
            await Client.WriteStorageObjectsAsync(Session, apiWriteStorageObjects);
        }

        public async Task<object> GetUserDataAsync(IApiReadStorageObjectId[] apiReadObjects)
        {
            var result = await Client.ReadStorageObjectsAsync(Session, apiReadObjects);
            return result;
        }

        public async Task DeleteUserDataAsync(StorageObjectId[] apiDeleteObjects)
        {
            await Client.DeleteStorageObjectsAsync(Session, apiDeleteObjects);
            Debug.LogError("[Nakama] Deleted user data");
        }

        #endregion

        public void OnDestroy()
        {
            Socket?.CloseAsync();
        }
    }
}