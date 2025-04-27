using System;
using Nakama;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
using String = System.String;

namespace ProjectCore.CloudService.Nakama.Internal
{
    [CreateAssetMenu(fileName = "SocialFeatureConfig", menuName = "ProjectCore/SocialFeature/SocialFeatureConfig")]
    [InlineEditor]
    public class SocialFeatureConfig : ScriptableObject
    {
        [SerializeField] private CloudDBString UdidString;
        [SerializeField] private DBBool IsAppearOnline;
        
        public GeneralSettings GeneralSettings;
        public ServicesSettings ServicesSettings;

        private const string DEV_SERVER_SCHEME = "http";
        private const string DEV_SERVER_URL = "167.86.67.197";
        private const int DEV_SERVER_PORT = 7350;
        private const string DEV_SERVER_KEY = "defaultkey";
        
        private const string LIVE_SERVER_SCHEME = "https";
        private const string LIVE_SERVER_URL = "";
        private const int LIVE_SERVER_PORT = 7350;
        private const string LIVE_SERVER_KEY = "defaultkey";
        
        private const string LOCAL_SERVER_SCHEME = "http";
        private const string LOCAL_SERVER_URL = "127.0.0.1";
        private const int LOCAL_SERVER_PORT = 7350;
        private const string LOCAL_SERVER_KEY = "defaultkey";

        public bool CanAppearOnline() => IsAppearOnline.GetValue();
        
        public string GetDeviceUdid()
        {
            if (UdidString.GetValue() == String.Empty)
            {
                UdidString.SetValue(SystemInfo.deviceUniqueIdentifier);
            }
            return UdidString.GetValue();
        }

        public OriginData GetOriginData()
        {
            OriginData originData;
            switch (GeneralSettings.Environment)
            {
                case Environment.Live:
                    originData = new OriginData()
                    {
                        Scheme = LIVE_SERVER_SCHEME,
                        Host = LIVE_SERVER_URL,
                        Port = LIVE_SERVER_PORT,
                        Key = LIVE_SERVER_KEY
                    };
                    break;
                case Environment.Dev:
                    originData = new OriginData()
                    {
                        Scheme = DEV_SERVER_SCHEME,
                        Host = DEV_SERVER_URL,
                        Port = DEV_SERVER_PORT,
                        Key = DEV_SERVER_KEY
                    };
                    break;
                case Environment.Local:
                default:
                    originData = new OriginData()
                    {
                        Scheme = LOCAL_SERVER_SCHEME,
                        Host = LOCAL_SERVER_URL,
                        Port = LOCAL_SERVER_PORT,
                        Key = LOCAL_SERVER_KEY
                    };
                    break;
            }

            return originData;
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
    public class OriginData
    {
        public string Scheme;
        public string Host;
        public int Port;
        public string Key;
    }

    [Serializable]
    public class ServicesSettings
    {
        public bool EnableUserProfile;
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