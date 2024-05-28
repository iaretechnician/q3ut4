using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using static MarsFPSKit.UI.Kit_MenuLogin;

namespace MarsFPSKit
{
    namespace Services
    {
        /// <summary>
        /// This handles the UGS login.
        /// </summary>
        public class Kit_UGS : MonoBehaviour
        {
            public static Kit_UGS instance;
            /// <summary>
            /// Are we currently logged in?
            /// </summary>
            public bool isLoggedIn;

            async void Awake()
            {
                if (instance)
                {
                    Destroy(gameObject);
                    return;
                }

                //Assign singleton
                instance = this;
                DontDestroyOnLoad(gameObject);

                try
                {
                    Debug.Log("<color=magenta>[MMFPSE UGS Manager]</color> Initializing UGS.", this);
                    await UnityServices.InitializeAsync();
                    Debug.Log("<color=magenta>[MMFPSE UGS Manager]</color> Signing in anonymously.", this);
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log("<color=magenta>[MMFPSE UGS Manager]</color> Logged into UGS, player ID: " + AuthenticationService.Instance.PlayerId, this);
                    isLoggedIn = true;
                }
                catch (Exception e)
                {
                    isLoggedIn = false;
                    Debug.Log("<color=magenta>[MMFPSE UGS Manager]</color> " + e.ToString(), this);
                }
            }
        }
    }
}