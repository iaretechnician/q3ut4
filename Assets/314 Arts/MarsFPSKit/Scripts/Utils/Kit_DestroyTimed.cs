using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MarsFPSKit
{
    /// <summary>
    /// An utils class to destroy an object after <see cref="destroyAfter"/>
    /// </summary>
    public class Kit_DestroyTimed : MonoBehaviour
    {
        public float destroyAfter = 5f;

        public bool objectPooled;

        // Use this for initialization
        void OnEnable()
        {
            if (!objectPooled)
            {
                //Just destroy after set seconds
                Destroy(gameObject, destroyAfter);
            }
            else
            {
                Invoke("DestroyPooled", destroyAfter);
            }
        }

        void DestroyPooled()
        {
            Kit_IngameMain.instance.objectPooling.DestroyInstantiateable(gameObject);
        }
    }
}
