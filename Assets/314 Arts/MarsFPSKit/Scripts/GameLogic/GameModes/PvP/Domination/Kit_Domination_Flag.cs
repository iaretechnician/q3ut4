using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This represents a flag for domination game mode!
    /// </summary>
    public class Kit_Domination_Flag : MonoBehaviour
    {
        /// <summary>
        /// External acceleration that will be applied to the flag's cloth
        /// </summary>
        public Vector3 externalAcceleration;
        /// <summary>
        /// Random acceleration that will be applied to the flag's cloth
        /// </summary>
        public Vector3 randomAcceleration;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            //Draw a cube to indicate
            Gizmos.DrawCube(transform.position + new Vector3(0, 1f, 0f), new Vector3(0.3f, 2f, 0.3f));
        }
    }
}
