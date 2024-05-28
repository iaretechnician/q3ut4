using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_AfkLimitUISimple : Kit_AfkLimitUIBase
    {
        public GameObject root;
        /// <summary>
        /// The alpha of the UI
        /// </summary>
        public CanvasGroup uiAlpha;
        /// <summary>
        /// The text which displays the warning
        /// </summary>
        public TextMeshProUGUI uiText;

        void Start()
        {
            //Deactivate root
            root.SetActive(false);
        }

        public override void DisplayWarning(float timePlayerWasAfk, float kickIn, int warningNumber)
        {
            //Start coroutine
            StartCoroutine(WarningRoutine(timePlayerWasAfk, kickIn, warningNumber));
        }

        IEnumerator WarningRoutine(float timePlayerWasAfk, float kickIn, int warningNumber)
        {
            //Set alpha to 0
            uiAlpha.alpha = 0f;
            //Set text
            uiText.text = "Warning #" + warningNumber + ": You have been afk for " + timePlayerWasAfk.ToString("F0") + " seconds. You will be kicked in: " + kickIn.ToString("F0") + " seconds";
            //Activate root
            root.SetActive(true);
            //Alpha variable
            float a = 0f;
            //Fade in
            while (a < 1f)
            {
                a += Time.deltaTime * 2f;
                uiAlpha.alpha = a;
                yield return null;
            }
            //Wait 5 seconds
            yield return new WaitForSeconds(5f);
            //Fade out
            while (a > 0f)
            {
                a -= Time.deltaTime * 2f;
                uiAlpha.alpha = a;
                yield return null;
            }
            //Deactivate root
            root.SetActive(false);
        }
    }
}
