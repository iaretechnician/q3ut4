using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Attach this to a trigger in order to swim inside that trigger!
    /// </summary>
    public class Kit_SwimmingTrigger : MonoBehaviour
    {
        /// <summary>
        /// Collider used for triggering the swim
        /// </summary>
        public Collider col;
        /// <summary>
        /// Plays when player enters
        /// </summary>
        public AudioClip playerEnterSound;
        /// <summary>
        /// Applied on exit
        /// </summary>
        public Vector3 exitAdjustment = new Vector3(0, 1f, 0);
        /// <summary>
        /// Should we handle buoyancy?
        /// </summary>
        public bool enableBouyancyForEnteringRigidbodies;

        private void OnTriggerEnter(Collider other)
        {
            if (enableBouyancyForEnteringRigidbodies)
            {
                if (other.GetComponent<Rigidbody>())
                {
                    if (!other.GetComponent<Buoyancy>())
                    {
                        Buoyancy by = other.gameObject.AddComponent<Buoyancy>();
                        by.SetWaterHeight(col.ClosestPointOnBounds(new Vector3(transform.position.x, Mathf.Infinity, transform.position.z)).y);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (enableBouyancyForEnteringRigidbodies)
            {
                if (other.GetComponent<Rigidbody>())
                {
                    if (other.GetComponent<Buoyancy>())
                    {
                        Destroy(other.GetComponent<Buoyancy>());
                    }
                }
            }
        }
    }
}