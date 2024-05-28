using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This is really just a simple look at script that takes a camera instead of a transform, nothing special, really
    /// </summary>
    public class Kit_LoadoutLookAt : MonoBehaviour
    {
        public Camera camToLookAt;

        void Update()
        {
            transform.forward = Vector3.forward;

            /*
            if (camToLookAt)
            {
                transform.LookAt(camToLookAt.transform);
                transform.Rotate(0f, 180f, 0f);
            }
            */
        }
    }
}