using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace Utils
    {
        public class Kit_GetRidOfCanvas : MonoBehaviour
        {
            public void Start()
            {
                if (GetComponentInChildren<GraphicRaycaster>())
                {
                    Destroy(GetComponentInChildren<GraphicRaycaster>());
                }

                if (GetComponentInChildren<Canvas>())
                {
                    Destroy(GetComponentInChildren<Canvas>());
                }
            }
        }
    }
}