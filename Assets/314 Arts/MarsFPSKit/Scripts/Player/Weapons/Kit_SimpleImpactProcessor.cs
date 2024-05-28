using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    [System.Serializable]
    public class BulletImpact
    {
        /// <summary>
        /// Impact particle
        /// </summary>
        public GameObject[] impactParticle;
        /// <summary>
        /// Can this impact particle be parented
        /// </summary>
        public bool canBeParented = true;
        /// <summary>
        /// Bullet mark material
        /// </summary>
        public Material[] materials;
    }

    [System.Serializable]
    public class StringToBulletImpact : SerializableDictionary<string, BulletImpact> { }

    /// <summary>
    /// This is a simple impact processor. It only instantiates bullet marks / particles. A more advanced one could use object pooling.
    /// </summary>
    [CreateAssetMenu(menuName = "MarsFPSKit/Impact Processors/Simple")]
    public class Kit_SimpleImpactProcessor : Kit_ImpactParticleProcessor
    {
        [Header("Material impacts")]
        /// <summary>
        /// Impact particles array
        /// </summary>
        public StringToBulletImpact bulletImpacts;

        /// <summary>
        /// The Prefab for the bullet markss. Needs to have <see cref="Kit_BulletMarks"/> on it 
        /// </summary>
        public GameObject bulletMarksPrefab;

        /// <summary>
        /// How much is the bullet marks object moved into the normal's direction?
        /// </summary>
        public float bulletMarksNormalOffset;

        /// <summary>
        /// Bullet mark lifetime in seconds
        /// </summary>
        public float bulletMarksLifetime = 60f;

        /// <summary>
        /// How many entries pre-created for object pooling?
        /// </summary>
        public int defaultObjectPoolingFill = 30;

        public override void StartImpactProcessor()
        {
            if (bulletMarksPrefab)
            {
                Kit_IngameMain.instance.objectPooling.EnqueueInstantiateable(bulletMarksPrefab, defaultObjectPoolingFill);
            }

            foreach (BulletImpact bi in bulletImpacts.Values)
            {
                for (int i = 0; i < bi.impactParticle.Length; i++)
                {
                    Kit_IngameMain.instance.objectPooling.EnqueueInstantiateable(bi.impactParticle[i], defaultObjectPoolingFill / bi.impactParticle.Length);
                }
            }
        }

        public override void ProcessImpact(Vector3 pos, Vector3 normal, string materialType, Transform parent = null)
        {
            BulletImpact impactToUse = null;

            if (bulletImpacts.Contains(materialType)) impactToUse = bulletImpacts[materialType];
            else impactToUse = bulletImpacts["Concrete"];

            Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
            GameObject go = Kit_IngameMain.instance.objectPooling.GetInstantiateable(impactToUse.impactParticle[Random.Range(0, impactToUse.impactParticle.Length)], pos, rot); //Instantiate appropriate particle
            if (parent && impactToUse.canBeParented) go.transform.parent = parent; //Set parent if we have one
                                                      //The instantiated GO should destroy itself

            //Play Particles
            ParticleSystem ps = go.GetComponentInChildren<ParticleSystem>();
            if (ps)
            {
                ps.Play(true);
            }

            if (impactToUse.materials.Length > 0)
            {
                //Bullet marks
                GameObject bm = Kit_IngameMain.instance.objectPooling.GetInstantiateable(bulletMarksPrefab, pos + normal * bulletMarksNormalOffset, rot);
                //The instantiated GO should destroy itself
                //Set material
                bm.GetComponent<Kit_BulletMarks>().SetMaterial(impactToUse.materials[Random.Range(0, impactToUse.materials.Length)], bulletMarksLifetime);
                bm.transform.localScale = bulletMarksPrefab.transform.localScale;
                if (parent)
                {
                    bm.transform.parent = parent; //Set parent if we have one
                }
            }
        }
    }
}
