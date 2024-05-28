using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This is used for the instantiation of impact particles / bullet marks
    /// </summary>
    public abstract class Kit_ImpactParticleProcessor : ScriptableObject
    {
        /// <summary>
        /// Start
        /// </summary>
        /// <param name=""></param>
        public virtual void StartImpactProcessor()
        {

        }

        /// <summary>
        /// Used to process impact particles, here you could add object pooling
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="normal"></param>
        /// <param name="materialType"></param>
        public abstract void ProcessImpact(Vector3 pos, Vector3 normal, string materialType, Transform parentObject = null);
    }
}