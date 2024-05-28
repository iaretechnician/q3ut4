using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_SimpleChatEntry : MonoBehaviour
    {
        /// <summary>
        /// The text for this entry
        /// </summary>
        public TextMeshProUGUI txt;

        /// <summary>
        /// Sets up this chat entry with given parameteres
        /// </summary>
        /// <param name="content"></param>
        /// <param name="col"></param>
        public void Setup(string content)
        {
            //Set it up
            txt.text = content; //Text
        }
    }
}
