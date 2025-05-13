using System;
using System.Linq;
using Nakama;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
using String = System.String;

namespace ProjectCore.Integrations.Internal
{
    [CreateAssetMenu(fileName = "ServerConfig", menuName = "ProjectCore/Integrations/ServerConfig")]
    [InlineEditor]
    public class ServerConfig : ScriptableObject
    {
        [SerializeField] private DBString UdidString;
        [SerializeField] private DBBool IsAppearOnline;
        
        [ListDrawerSettings(NumberOfItemsPerPage = 1, ShowPaging = true)]
        [SerializeField] private NetworkData[] NetworkData;
        
        public GeneralSettings GeneralSettings;

        public string AuthToken { get; private set; }
        public string RefreshToken { get; private set; }
        
        public void SetAuthTokens(string authToken, string refreshToken)
        {
            AuthToken = authToken;
            RefreshToken = refreshToken;
        }
        
        public bool CanAppearOnline() => IsAppearOnline.GetValue();
        
        public string GetDeviceUdid()
        {
            if (UdidString.GetValue() == String.Empty)
            {
                UdidString.SetValue(SystemInfo.deviceUniqueIdentifier);
            }
            return UdidString.GetValue();
        }

        public NetworkData GetNetworkData()
        {
            return NetworkData.FirstOrDefault(x => x.Environment == GeneralSettings.Environment);
        }
        
        public RetryConfiguration GetRetryConfiguration()
        {
            var retryConfiguration = new RetryConfiguration(GeneralSettings.BaseRetryDelay,
                GeneralSettings.MaxRetries, delegate
                {
                    Debug.Log("[Nakama] about to retry");
                });
            return retryConfiguration;
        }
    }

    [Serializable]
    public class NetworkData
    {
        public Environment Environment;
        public string Scheme;
        public string Host;
        public int Port;
        public string Key;
    }

    [Serializable]
    public class GeneralSettings
    {
        public Environment Environment;
        public bool EnableSocialKit;

        public int BaseRetryDelay;
        public int MaxRetries;
        public int RetryTimeOut;
    }

    [Serializable]
    public enum Environment
    {
        Live,
        Dev,
        Local
    }
}