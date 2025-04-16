using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
// using Facebook.Unity;
using Nakama;
using ProjectCore.Events;
using UnityEngine;
using String = ProjectCore.Variables.String;

namespace FPSCommando.SocialFeature.Cloud.Internal
{
    [CreateAssetMenu(fileName = "NakamaServer", menuName = "FPSCommando/SocialFeature/Cloud/NakamaServer")]
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

        /*public async Task SyncWithFacebookID()
        {
            var fbID = FB.ClientToken;

            try
            {
                await Client.LinkFacebookAsync(Session, fbID,
                    retryConfiguration: SocialFeatureConfig.GetRetryConfiguration());
            }
            catch (ApiResponseException e)
            {
                Debug.LogError("[Nakama] Failed to Sync with Facebook ID: " + e.Message);
            }
        }

        public async Task AuthenticateWithFacebookID()
        {
            Debug.Log("[Nakama] Authenticating with Facebook ID");
            var fbID = FB.ClientToken;

            try
            {
                Session = await Client.AuthenticateFacebookAsync(fbID,
                    retryConfiguration: SocialFeatureConfig.GetRetryConfiguration());
                await Socket.ConnectAsync(Session, true, 30);

                NakamaServerConnected.Invoke();

                Debug.Log("[Nakama] Authenticated with Facebook ID: " + fbID);
            }
            catch (ApiResponseException ex)
            {
                Debug.LogError("[Nakama] Failed to authenticate with Facebook ID: " + ex.Message);
            }
        }*/

        public async Task SyncWithEmail(string email, string password)
        {
            try
            {
                await Client.LinkEmailAsync(Session, email, password, SocialFeatureConfig.GetRetryConfiguration());
            }
            catch (ApiResponseException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task AuthenticateWithEmail(string email, string password, CancellationToken token)
        {
            Debug.Log("[Nakama] Authenticating with email ID");
            try
            {
                Session = await Client.AuthenticateEmailAsync(email, password,
                    retryConfiguration: SocialFeatureConfig.GetRetryConfiguration(),
                    canceller: token);

                await Socket.ConnectAsync(Session, true, 10);

                NakamaServerConnected.Invoke();

                Debug.Log("[Nakama] Authenticated with email ID: " + email);
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

        public async Task<ISession> AuthenticateWithDeviceID(CancellationToken cancellationToken)
        {
            Debug.Log("[Nakama] Authenticating with device ID");
            var deviceID = SocialFeatureConfig.GetDeviceUdid();
            
            try
            {
                Session = await Client.AuthenticateDeviceAsync(deviceID,
                    retryConfiguration: SocialFeatureConfig.GetRetryConfiguration(),
                    canceller: cancellationToken);

                await Socket.ConnectAsync(Session, true, 10);

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

        private CancellationTokenSource _cancellationTokenSource;

        public async Task ValidateSession(bool forceSync = false)
        {
            if (Session == null)
            {
                Debug.LogError("[Nakama] Session is Invalid!!");

                Session = Nakama.Session.Restore(AuthToken, RefreshToken);
                await ValidateSocket();
            }

            if (Session == null || (Session.IsExpired && Session.IsRefreshExpired) || forceSync)
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

                    await ValidateSocket();
                }
                catch (ApiResponseException ex)
                {
                    Debug.LogError("[Nakama] Failed to validate session: " + ex.Message);
                }
            }
            else
            {
                Debug.Log("[Nakama] Session is Valid!!");
            }
            
            SaveSession();
        }

        public async Task ValidateSocket()
        {
            if (!Socket.IsConnected)
            {
                Debug.LogError("[Nakama] Socket is Invalid!!");
                await Socket.ConnectAsync(Session);
            }
            else
            {
                Debug.Log("[Nakama] Socket is Valid!!");
            }
        }

        private void SaveSession()
        {
            AuthToken.SetValue(Session.AuthToken);
            RefreshToken.SetValue(Session.RefreshToken);
        }

        #region UserProgressStorage

        // https://heroiclabs.com/docs/nakama/concepts/storage/collections
        
        public async Task SaveUserDataAsync(List<IApiWriteStorageObject> apiWriteStorageObjects)
        {
            await Client.WriteStorageObjectsAsync(Session, apiWriteStorageObjects.ToArray());
        }

        public async Task<object> GetUserDataAsync()
        {
            await ValidateSession();
            var result = await Client.ReadStorageObjectsAsync(Session, new IApiReadStorageObjectId[]
            {
                new StorageObjectId()
                {
                    Collection = "Save",
                    Key = "UserProgress",
                    UserId = Session.UserId
                }
            });
            return result;
        }

        public async Task DeleteUserDataAsync()
        {
            await ValidateSession();
            await Client.DeleteStorageObjectsAsync(Session, new[]
            {
                new StorageObjectId()
                {
                    Collection = "Save",
                    Key = "UserProgress",
                    UserId = Session.UserId
                }
            });
            Debug.LogError("[Nakama] Deleted user data");
        }

        #endregion

        public void OnDestroy()
        {
            Socket?.CloseAsync();
        }
    }
}