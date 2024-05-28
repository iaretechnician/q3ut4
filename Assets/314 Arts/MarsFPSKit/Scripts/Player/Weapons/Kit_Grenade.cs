
using MarsFPSKit.Networking;
using Mirror;
using System.Collections;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This script is the actual grenade
    /// </summary>
    public class Kit_Grenade : NetworkBehaviour
    {
        /// <summary>
        /// Time until the grenade explodes!
        /// </summary>
        public float explosionTime = 5f;

        /// <summary>
        /// Rigidbody of this grenade!
        /// </summary>
        public Rigidbody rb;

        /// <summary>
        /// Our collider
        /// </summary>
        public Collider col;

        /// <summary>
        /// Explosion that this grenade makes!
        /// </summary>
        public GameObject explosionPrefab;

        [SyncVar]
        /// <summary>
        /// Gun this was fired from!
        /// </summary>
        public int gunId;
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

        public override void OnStartServer()
        {
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


            //Destroy grenade after time
            StartCoroutine(DestroyGrenade());
        }

        public override void OnStartClient()
        {
            if (!NetworkServer.active)
            {
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

        IEnumerator DestroyGrenade()
        {
            //Wait
            yield return new WaitForSeconds(explosionTime);
            NetworkServer.Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (explosionPrefab)
            {
                GameObject go = Instantiate(explosionPrefab, transform.position, transform.rotation);
                if (go.GetComponent<Kit_Explosion>())
                {
                    go.GetComponent<Kit_Explosion>().Explode(ownerBot, ownerId, gunId);
                }
                if (go.GetComponent<Kit_FlashbangExplosion>())
                {
                    go.GetComponent<Kit_FlashbangExplosion>().Explode(ownerBot, ownerId, gunId);
                }
            }
        }
    }
}