using System;
using ProjectCore.Events;
#if FB
using System.Threading.Tasks;
using Facebook.Unity;
#endif
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.Integrations.FacebookService
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "FacebookService", menuName = "ProjectCore/Integrations/FacebookService")]
    public class FacebookService : ScriptableObject
    {
        [SerializeField] private DBString FbAccessToken;
        [SerializeField] private DBBool FbLoggedIn;
        
        [SerializeField] private Float FbLoadingProgress;

        [SerializeField] private GameEvent FacebookInitializedEvent;
        [SerializeField] private GameEventWithBool FacebookConnectEvent;
        
        // link: https://developers.facebook.com/docs/unity/reference/current
        
        public void Initialize()
        {
#if FB
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                return;
            }
            
            try
            {
                ConnectFacebook();
            }
            catch (Exception exception)
            {
                Debug.LogError($"[FacebookService] Error Initializing Facebook Service: {exception.Message}");
            }
#endif
        }

#if FB
        private void ConnectFacebook()
        {
            AccessToken token = AccessToken.CurrentAccessToken;
            
            if (!FB.IsInitialized)
            {
                Debug.LogError("[FacebookService] FB.IsInitialized not Initialized!");
                InitializeFacebook();
                return;
            }

            if (!FB.IsLoggedIn)
            {
                LoginToFacebook();
                return;
            }

            if (token != null && HasTokenExpired(token))
            {
                FB.Mobile.RefreshCurrentAccessToken((result) =>
                {
                    if (result == null)
                    {
                        LoginToFacebook();
                    }
                    else
                    {
                        Debug.Log("[FacebookService] Token Refreshed");
                    }
                });
            }
        }

        private void InitializeFacebook()
        {
            Debug.Log("[FacebookService] Initializing fb");
            FbLoadingProgress.SetValue(1f);
            FB.Init(OnFbInitialized);
        }

        private void OnFbInitialized()
        {
            Debug.Log("[FacebookService] Fb Initialized");
            
            // Fire a FB Initialized Event
            FacebookInitializedEvent.Invoke();
        }

        private void LoginToFacebook()
        {
            if (FB.IsLoggedIn && !HasTokenExpired(AccessToken.CurrentAccessToken))
                return;
            
            var scopes = new []
            {
                "public_profile",
                "email"
            };
            
            FB.LogInWithReadPermissions(scopes, delegate(ILoginResult result)
            {
                if (FB.IsLoggedIn)
                {
                    FbAccessToken.SetValue(result.AccessToken.TokenString);
                    FbLoggedIn.SetValue(FB.IsLoggedIn);
                    Debug.Log("[FacebookService] Logged In Successfully");
                    FacebookConnectEvent.Raise(true);
                    return;
                }
                Debug.LogError("[FacebookService] Login Failed " + result.Error);
            });
        }

        private void RetrieveLogin()
        {
            FB.Android.RetrieveLoginStatus((ILoginStatusResult result) =>
            {
                if (!string.IsNullOrEmpty(result.Error))
                {
                    Debug.LogError("Error: " + result.Error);
                }
                else if (result.Failed)
                {
                    Debug.LogError("Failure: Access Token could not be retrieved");
                }
                else
                {
                    // Successfully logged user in
                    FbAccessToken.SetValue(result.AccessToken.TokenString);
                    Debug.Log("Success: " + result.AccessToken);
                    FacebookConnectEvent.Raise(true);
                }
            });
        }

        private bool HasTokenExpired(AccessToken token)
        {
            return token.ExpirationTime > DateTime.Now.AddDays(1);
        }

        [Button]
        public void LogoutFacebook()
        {
            try
            {
                FB.LogOut();
                
                Debug.Log("[FacebookService] Logged Out Successfully");
                
                FbLoggedIn.SetValue(false);
                FacebookConnectEvent.Raise(false);
            }
            catch (Exception ex)
            {
                Debug.LogError("[FacebookService] Error Logging Out: " + ex.Message);
            }
        }

        [Button]
        public async Task LoginFacebook()
        {
            if (FB.IsLoggedIn || FbLoggedIn)
            {
                Debug.Log("[FacebookService] Already Logged In");
                return;
            }

#if !UNITY_EDITOR
            if (FbLoggedIn)
                RetrieveLogin();
            else
                LoginToFacebook();
#else
            LoginToFacebook();
#endif

            await Task.CompletedTask;
        }
#endif
    }
}