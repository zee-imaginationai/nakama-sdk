using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if FB
using Facebook.Unity;
#endif
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.SocialFeature.Cloud.Internal
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "FacebookService", menuName = "ProjectCore/SocialFeature/Cloud/FacebookService")]
    public class FacebookService : ScriptableObject
    {
        [SerializeField] private DBString FbAccessToken;
        [SerializeField] private DBBool FbLoggedIn;
        
        public async Task Initialize()
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
                Debug.LogError("[FacebookService] FB.IsInitialized is FALSE !");
                InitializeFacebook(token);
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
                        // Fire a FB connect success Event;
                    }
                });
            }
        }

        private void InitializeFacebook(AccessToken accessToken = null)
        {
            Debug.Log("[FacebookService] Initializing fb]");
            FB.Init(delegate
            {
                FB.ActivateApp();
                if (!FB.IsLoggedIn || accessToken == null)
                {
                    OnFbInitialized();
                }
            });
        }

        private void OnFbInitialized()
        {
            Debug.Log("[FacebookService] Fb Initialized]");
            
            // Fire a FB Initialized Event

            if (!FbLoggedIn)
            {
                LoginToFacebook();
            }
            else
            {
                FB.Android.RetrieveLoginStatus((ILoginStatusResult result) =>
                {
                    if (!string.IsNullOrEmpty(result.Error)) {
                        Debug.Log("Error: " + result.Error);
                    } else if (result.Failed) {
                        Debug.Log("Failure: Access Token could not be retrieved");
                    }
                    else
                    {
                        // Successfully logged user in
                        // A popup notification will appear that says "Logged in as <User Name>"
                        Debug.Log("Success: " + result.AccessToken.UserId);
                    }
                });
            }
        }
        
        [Button]
        private void LoginToFacebook()
        {
            List<string> scopes = new List<string>();
            scopes.Add("public_profile");
            scopes.Add("email");
            
            if (FB.IsLoggedIn && !HasTokenExpired(AccessToken.CurrentAccessToken))
            {
                return;
            }
            
            FB.LogInWithReadPermissions(scopes, delegate(ILoginResult result)
            {
                if (FB.IsLoggedIn)
                {
                    FbAccessToken.SetValue(result.AccessToken.TokenString);
                    FbLoggedIn.SetValue(FB.IsLoggedIn);
                    Debug.Log("[FacebookService] Logged In Successfully" + $" :: Access Token: {result.AccessToken} :: Expire Time: {result.AccessToken.ExpirationTime}");
                    return;
                }
                else
                {
                    Debug.LogError("[FacebookService] Login Failed " + result.Error);
                }
            });
        }
        
        private bool HasTokenExpired(AccessToken token)
        {
            return token.ExpirationTime > DateTime.Now.AddDays(1);
        }
#endif
    }
}