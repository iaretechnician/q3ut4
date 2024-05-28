using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        public class Kit_UIRebuildOnEnable : MonoBehaviour
        {
            public void OnEnable()
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            }
        }
    }
}