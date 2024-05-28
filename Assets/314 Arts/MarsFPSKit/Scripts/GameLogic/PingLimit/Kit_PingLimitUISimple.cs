using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_PingLimitUISimple : Kit_PingLimitUIBase
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

        public override void DisplayWarning(ushort currentPing, ushort warningNumber)
        {
            //Start coroutine
            StartCoroutine(WarningRoutine(currentPing, warningNumber));
        }

        IEnumerator WarningRoutine(ushort currentPing, ushort warningNumber)
        {
            //Set alpha to 0
            uiAlpha.alpha = 0f;
            //Set text
            uiText.text = "Warning #" + warningNumber + ": Your ping " + currentPing + "ms is too high. Try to reduce it or you will be kicked!";
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
