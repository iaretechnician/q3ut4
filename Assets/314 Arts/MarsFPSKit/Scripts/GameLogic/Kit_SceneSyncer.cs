using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MarsFPSKit
{
    /// <summary>
    /// This custom scene manager makes sure that scenes are properly synced. In contrast to the default manager from Photon, reloading a scene is possible.
    /// </summary>
    public class Kit_SceneSyncer : MonoBehaviour
    {
        public static Kit_SceneSyncer instance;
        /// <summary>
        /// Game Information
        /// </summary>
        public Kit_GameInformation information;
        /// <summary>
        /// The canvas that displays the loading screen
        /// </summary>
        public GameObject loadingCanvas;
        /// <summary>
        /// The image that displays the progress
        /// </summary>
        public Image loadingBar;
        /// <summary>
        /// Background image of map
        /// </summary>
        public Image backgroundImage;
        [HideInInspector]
        /// <summary>
        /// Is loading in progress?
        /// </summary>
        public bool isLoading;

        [Header("Fade")]
        public Animator fadeAnim;
        /// <summary>
        /// How long does it take to fade in?
        /// </summary>
        public float fadeInLength = 0.25f;
        /// <summary>
        /// How long does it take to fade out?
        /// </summary>
        public float fadeOutLength = 0.25f;

        void Awake()
        {
            if (!instance)
            {
                //Assign instance
                instance = this;
                //Make sure it doens't get destroyed
                DontDestroyOnLoad(this);
                //Hide canvas
                loadingCanvas.SetActive(false);
            }
            else
            {
                //Only one of this instance may be active at a time
                Destroy(gameObject);
            }
        }

        public IEnumerator DisplayAsyncSceneLoading(string scene)
        {
            while (Kit_NetworkManager.loadingSceneAsync == null) yield return null;

            isLoading = true;
            Kit_MapInformation mapInfo = information.GetMapInformationFromSceneName(scene);
            if (mapInfo && mapInfo.loadingImage)
            {
                backgroundImage.sprite = mapInfo.loadingImage;
                backgroundImage.enabled = true;
            }
            else
            {
                backgroundImage.enabled = false;
            }
            //Reset progress
            loadingBar.fillAmount = 0f;
            //Show canvas
            loadingCanvas.SetActive(true);
            fadeAnim.Play("Fade In", 0, 0f);
            //Wait for anim
            yield return new WaitForSeconds(fadeInLength);

            AsyncOperation loading = Kit_NetworkManager.loadingSceneAsync;
            while (loading != null && !loading.isDone)
            {
                loadingBar.fillAmount = loading.progress;
                yield return null;
            }
            loadingBar.fillAmount = 1f;
            fadeAnim.Play("Fade Out", 0, 0f);
            //Wait for anim
            yield return new WaitForSeconds(fadeOutLength);
            //Hide canvas again
            loadingCanvas.SetActive(false);
            //Unset bool
            isLoading = false;
        }

        IEnumerator LoadLocalSceneAsync(string scene)
        {
            isLoading = true;
            Kit_MapInformation mapInfo = information.GetMapInformationFromSceneName(scene);
            if (mapInfo && mapInfo.loadingImage)
            {
                backgroundImage.sprite = mapInfo.loadingImage;
                backgroundImage.enabled = true;
            }
            else
            {
                backgroundImage.enabled = false;
            }
            //Reset progress
            loadingBar.fillAmount = 0f;
            //Show canvas
            loadingCanvas.SetActive(true);
            fadeAnim.Play("Fade In", 0, 0f);
            //Wait for anim
            yield return new WaitForSeconds(fadeInLength);
            AsyncOperation loading = SceneManager.LoadSceneAsync(scene);
            while (!loading.isDone)
            {
                loadingBar.fillAmount = loading.progress;
                yield return null;
            }
            fadeAnim.Play("Fade Out", 0, 0f);
            //Wait for anim
            yield return new WaitForSeconds(fadeOutLength);
            //Hide canvas again
            loadingCanvas.SetActive(false);
            //Unset bool
            isLoading = false;
        }

        /// <summary>
        /// Network loads a scene
        /// </summary>
        /// <param name="scene"></param>
        public void LoadScene(string scene)
        {
            Debug.Log("Trying to load scene now: " + scene);

            if (!Kit_NetworkManager.instance.isNetworkActive)
            {
                StartCoroutine(LoadLocalSceneAsync(scene));
            }
            else
            {
                if (!NetworkServer.isLoadingScene)
                {
                    //Relay to network manager
                    Kit_NetworkManager.instance.ServerChangeScene(scene);
                }
            }
        }
    }
}
