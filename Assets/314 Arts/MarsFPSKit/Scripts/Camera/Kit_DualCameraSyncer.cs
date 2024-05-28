using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_DualCameraSyncer : MonoBehaviour
    {
        /// <summary>
        /// Main Camera
        /// </summary>
        public Camera mainCamera;
        /// <summary>
        /// Weapon Camera
        /// </summary>
        public Camera weaponCamera;
        /// <summary>
        /// Directly copy fov?
        /// </summary>
        public bool copyFov;
        /// <summary>
        /// Lerp reference
        /// </summary>
        public Vector3 lerpFovReference = new Vector3(30f, 60f, 90f);
        /// <summary>
        /// Lerp reference.
        /// </summary>
        public Vector3 lerpFovCopy = new Vector3(30f, 50f, 65f);

        // Update is called once per frame
        void LateUpdate()
        {
            if (copyFov)
            {
                weaponCamera.fieldOfView = mainCamera.fieldOfView;
            }
            else
            {
                if (mainCamera.fieldOfView > lerpFovReference.y)
                {
                    weaponCamera.fieldOfView = Mathf.Lerp(lerpFovCopy.y, lerpFovCopy.z, Mathf.InverseLerp(lerpFovReference.y, lerpFovReference.z, mainCamera.fieldOfView));
                }
                else
                {
                    weaponCamera.fieldOfView = Mathf.Lerp(lerpFovCopy.x, lerpFovCopy.y, Mathf.InverseLerp(lerpFovReference.x, lerpFovReference.y, mainCamera.fieldOfView));
                }
            }
        }
    }
}