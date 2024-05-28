using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_CameraTrigger : MonoBehaviour
    {
        /// <summary>
        /// Rerefence to player
        /// </summary>
        public Kit_PlayerBehaviour pb;

        private void OnTriggerEnter(Collider other)
        {
            if (pb.movement)
            {
                pb.movement.OnCameraTriggerEnterRelay(pb, other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (pb.movement)
            {
                pb.movement.OnCameraTriggerExitRelay(pb, other);
            }
        }
    }
}