using MarsFPSKit.Networking;
using Mirror;
using System.Collections;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This script is the actual smoke grenade
    /// </summary>
    public class Kit_GrenadeSmoke : NetworkBehaviour
    {
        /// <summary>
        /// Time until the grenade will throw smoke
        /// </summary>
        public float timeUntilSmoke = 5f;

        /// <summary>
        /// Time until the grenade is destroyed
        /// </summary>
        public float timeUntilDestroy = 20f;

        /// <summary>
        /// Rigidbody of this grenade!
        /// </summary>
        public Rigidbody rb;

        /// <summary>
        /// The smoke!!!
        /// </summary>
        public ParticleSystem smoke;
        [SyncVar(hook = "OnSmokeFiredChanged")]
        /// <summary>
        /// Master said to smoke?
        /// </summary>
        public bool smokeFired;

        [SyncVar]
        /// <summary>
        /// cached owner id
        /// </summary>
        public uint ownerId;
        [SyncVar]
        /// <summary>
        /// Is the owner a bot?
        /// </summary>
        public bool ownerBot;
        [SyncVar]
        public Vector3 initialPlayerVelocity;
        [SyncVar]
        public Vector3 forceToApply;
        [SyncVar]
        public Vector3 torqueToApply;

        IEnumerator Start()
        {
            if (isServer)
            {
                //Should be at start, but just to make sure!
                rb.isKinematic = false;
                //Wait
                yield return new WaitForSeconds(timeUntilSmoke);
                smokeFired = true;
                yield return new WaitForSeconds(timeUntilDestroy);
                //Then just destroy
                NetworkServer.Destroy(gameObject);
            }
            else
            {
                rb.isKinematic = true;
            }
        }

        void OnSmokeFiredChanged (bool was, bool isNow)
        {
            if (isNow)
            {
                smoke.transform.up = Vector3.up;
                smoke.Play(true);
            }
        }

        public override void OnStartServer()
        {
            Collider col = GetComponentInChildren<Collider>();

            if (!ownerBot)
            {
                Kit_PlayerBehaviour player = Kit_NetworkPlayerManager.instance.GetPlayerBehaviourById(ownerId);

                if (player)
                {
                    Physics.IgnoreCollision(player.cc, col);
                    Physics.IgnoreCollision(col, player.cc);
                }
            }
            else
            {
                Kit_PlayerBehaviour player = Kit_IngameMain.instance.currentBotManager.GetAliveBot(Kit_IngameMain.instance.currentBotManager.GetBotWithID(ownerId));

                if (player)
                {
                    Physics.IgnoreCollision(player.cc, col);
                    Physics.IgnoreCollision(col, player.cc);
                }
            }

            rb.velocity = initialPlayerVelocity;
            rb.AddRelativeForce(forceToApply);
            rb.AddRelativeTorque(torqueToApply);
        }
        }
}