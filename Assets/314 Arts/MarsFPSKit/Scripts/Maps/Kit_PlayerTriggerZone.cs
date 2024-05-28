using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this to apply damage or kill player with triggers
    /// </summary>
    public class Kit_PlayerTriggerZone : MonoBehaviour
    {
        [Tooltip("How much damage is dealed on enter")]
        /// <summary>
        /// How much damage is dealed on enter
        /// </summary>
        public float damageOnEnter = 0;
        [Tooltip("How much damage is dealed per second while we are in the trigger")]
        /// <summary>
        /// How much damage is dealed per second while we are in the trigger
        /// </summary>
        public float damageOnStayPerSecond = 0;
        [Tooltip("How much damage is dealed on exit")]
        /// <summary>
        /// How much damage is dealed on exit
        /// </summary>
        public float damageOnExit = 0;
        [Tooltip("Should a player be killed upon enter?")]
        /// <summary>
        /// Should a player be killed upon enter?
        /// </summary>
        public bool killOnEnter;
        [Tooltip("Should a player be killed upon exit?")]
        /// <summary>
        /// Should a player be killed upon exit?
        /// </summary>
        public bool killOnExit;
        [Tooltip("Which death sound to play on player")]
        /// <summary>
        /// Which death sound to play on player
        /// </summary>
        public int deathSoundCategory;

        private void OnTriggerEnter(Collider other)
        {
            Kit_PlayerBehaviour pb = other.transform.GetComponent<Kit_PlayerBehaviour>();
            if (pb)
            {
                //Check if its our
                if (NetworkServer.active)
                {
                    if (killOnEnter)
                    {
                        //Kill if requested
                        pb.vitalsManager.ApplyEnvironmentalDamage(pb, 1000f, deathSoundCategory);
                    }
                    else if (damageOnEnter > 0)
                    {
                        //Apply damage if > 0
                        pb.vitalsManager.ApplyEnvironmentalDamage(pb, damageOnEnter, deathSoundCategory);
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            Kit_PlayerBehaviour pb = other.transform.GetComponent<Kit_PlayerBehaviour>();
            if (pb)
            {
                //Check if its ours
                if (NetworkServer.active)
                {
                    if (damageOnStayPerSecond > 0f)
                    {
                        //Apply damage if > 0
                        pb.vitalsManager.ApplyEnvironmentalDamage(pb, damageOnStayPerSecond * Time.deltaTime, deathSoundCategory);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Kit_PlayerBehaviour pb = other.transform.GetComponent<Kit_PlayerBehaviour>();
            if (pb)
            {
                //Check if its our
                if (NetworkServer.active)
                {
                    if (killOnExit)
                    {
                        //Kill if requested
                        pb.vitalsManager.ApplyEnvironmentalDamage(pb, 1000f, deathSoundCategory);
                    }
                    else if (damageOnExit > 0)
                    {
                        //Apply damage if > 0
                        pb.vitalsManager.ApplyEnvironmentalDamage(pb, damageOnExit, deathSoundCategory);
                    }
                }
            }
        }
    }
}