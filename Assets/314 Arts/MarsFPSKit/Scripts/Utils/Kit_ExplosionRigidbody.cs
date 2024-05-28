using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Due to networking delays, this little helper is required to have very cool ragdolls!
    /// </summary>
    public class Kit_ExplosionRigidbody : MonoBehaviour
    {
        public Rigidbody body;
    }
}