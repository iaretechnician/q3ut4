using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_SaveCameraInChildren : MonoBehaviour
    {
        public void OnDestroy()
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam)
            {
                if (cam == Kit_IngameMain.instance.mainCamera)
                {
                    Kit_IngameMain.instance.activeCameraTransform = Kit_IngameMain.instance.spawnCameraPosition;
                }
            }
        }
    }
}