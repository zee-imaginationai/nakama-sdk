using System;
using System.Collections.Generic;
// using Facebook.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.SocialFeature.Cloud.Internal
{
    [CreateAssetMenu(fileName = "FacebookService", menuName = "FPSCommando/SocialFeature/Cloud/FacebookService")]
    public class FacebookService : ScriptableObject
    {
        /*public void Initialize()
        {
            if (FB.IsInitialized) return;
            
            try
            {
                ConnectFacebook();
            }
            catch (Exception exception)
            {
                Debug.LogError($"[FacebookService] Error Initializing Facebook Service: {exception.Message}");
            }
        }
        
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
            
            LoginToFacebook();
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
                if (result.AccessToken != null)
                {
                    Debug.LogError("[FACEBOOK: Logged In Successfully ]");
                    return;
                }
            });
        }
        
        private bool HasTokenExpired(AccessToken token)
        {
            return token.ExpirationTime > DateTime.Now.AddDays(1);
        }*/
    }
}