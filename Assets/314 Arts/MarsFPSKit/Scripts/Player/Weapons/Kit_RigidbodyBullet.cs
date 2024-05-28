using Mirror;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class RigidbodyBulletSettings
        {
            /// <summary>
            /// Damage we apply
            /// </summary>
            public float damage;
            /// <summary>
            /// Our lifetime
            /// </summary>
            public float bulletLifeTime;
            /// <summary>
            /// Does the object stay alive after death for some time?
            /// </summary>
            public bool staysAliveAfterDeath;
            /// <summary>
            /// Time we stay alive after death
            /// </summary>
            public float staysAliveAfterDeathTime;
            /// <summary>
            /// Shot from this direction
            /// </summary>
            public Vector3 direction;
            /// <summary>
            /// Where does this bullet originate from?
            /// </summary>
            public Vector3 shotFromPosition;
            /// <summary>
            /// Was this fired locally (should apply damage)
            /// </summary>
            public bool isServer;
            /// <summary>
            /// If <see cref="isServer"/> is true, this will be assigned
            /// </summary>
            public Kit_PlayerBehaviour localOwner;
            /// <summary>
            /// ID of owner
            /// </summary>
            public uint ownerID;
            /// <summary>
            /// Is the owner a bot?
            /// </summary>
            public bool ownerIsBot;
            /// <summary>
            /// Force to apply to ragdoll (if hit)
            /// </summary>
            public float ragdollForce = 500f;
            /// <summary>
            /// ID of the gun this bullet was fired with
            /// </summary>
            public int gameGunID;
        }

        public enum RigidbodyBulletOnImpact
        {
            Impact, Explosion
        }

        public class Kit_RigidbodyBullet : Kit_BulletBase
        {
            /// <summary>
            /// Our rigidbody
            /// </summary>
            public Rigidbody body;

            /// <summary>
            /// What do we do upon impact?
            /// </summary>
            public RigidbodyBulletOnImpact onImpact;

            /// <summary>
            /// Prefab
            /// </summary>
            public GameObject explosionPrefab;

            /// <summary>
            /// Visible renderer for bullet
            /// </summary>
            public GameObject bulletRenderer;

            #region Runtime
            /// <summary>
            /// Since when does the bullet exist?
            /// </summary>
            private float bulletExistTime;

            private RigidbodyBulletSettings settings;

            /// <summary>
            /// After which frame should the bullet be shown?
            /// </summary>
            private int frameOfShow;
            /// <summary>
            /// Did we show bullet?
            /// </summary>
            private bool wasBulletShown;
            #endregion

            public override void Setup(Kit_ModernWeaponScript ws, Kit_ModernWeaponScriptRuntimeData data, Kit_PlayerBehaviour pb, Vector3 dir)
            {
                //Setup
                RigidbodyBulletSettings rbs = new RigidbodyBulletSettings();
                //Setup data
                rbs.damage = data.baseDamage;
                rbs.direction = dir;
                rbs.ragdollForce = data.ragdollForce;
                rbs.gameGunID = ws.gameGunID;
                rbs.bulletLifeTime = data.bulletLifeTime;
                rbs.staysAliveAfterDeath = data.bulletStaysAliveAfterDeath;
                rbs.staysAliveAfterDeathTime = data.bulletStaysAliveAfterDeathTime;
                rbs.shotFromPosition = pb.transform.position;

                rbs.isServer = pb.isServer;
                rbs.localOwner = pb;
                rbs.ownerIsBot = pb.isBot;
                rbs.ownerID = pb.id;

                //Set data
                settings = rbs;

                //Reset force
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;

                //Apply force
                body.velocity = pb.movement.GetVelocity(pb);
                body.AddRelativeForce(0, 0, data.bulletSpeed, ForceMode.Impulse);

                bulletExistTime = 0f;

                frameOfShow = Time.frameCount + data.bulletHideForFrames;
                wasBulletShown = false;

                if (bulletRenderer)
                {
                    if (data.bulletHideForFrames > 0)
                    {
                        bulletRenderer.SetActiveOptimized(false);
                    }
                    else
                    {
                        bulletRenderer.SetActiveOptimized(true);
                        wasBulletShown = true;
                    }
                }
            }

            #region Unity Calls
            void Update()
            {

                //Check if we should destroy
                bulletExistTime += Time.deltaTime;
                if (bulletExistTime > settings.bulletLifeTime && enabled) Kit_IngameMain.instance.objectPooling.DestroyInstantiateable(gameObject);

                if (!wasBulletShown)
                {
                    if (Time.frameCount >= frameOfShow)
                    {
                        if (bulletRenderer)
                        {
                            bulletRenderer.SetActiveOptimized(true);
                        }

                        wasBulletShown = true;
                    }
                }
            }

            void OnCollisionEnter(Collision collision)
            {
                if (settings == null) return;

                //Check if we hit ourself
                if (settings.localOwner)
                {
                    for (int i = 0; i < collision.contacts.Length; i++)
                    {
                        if (collision.contacts[i].otherCollider.GetComponentInParent<Kit_RigidbodyBullet>()) return;
                        if (collision.contacts[i].otherCollider.transform.root == settings.localOwner.transform.root) return;
                    }
                }

                if (onImpact == RigidbodyBulletOnImpact.Impact)
                {
                    if (collision.contacts[0].otherCollider.transform.GetComponent<Kit_PlayerDamageMultiplier>() && Kit_IngameMain.instance.currentGameModeType == 2)
                    {
                        Kit_PlayerDamageMultiplier pdm = collision.contacts[0].otherCollider.transform.GetComponent<Kit_PlayerDamageMultiplier>();
                        if (collision.contacts[0].otherCollider.transform.root.GetComponent<Kit_PlayerBehaviour>())
                        {
                            if (settings.isServer)
                            {
                                Kit_PlayerBehaviour hitPb = collision.contacts[0].otherCollider.transform.root.GetComponent<Kit_PlayerBehaviour>();
                                //First check if we can actually damage that player
                                if ((settings.localOwner && Kit_IngameMain.instance.currentPvPGameModeBehaviour.ArePlayersEnemies(settings.localOwner, hitPb)) || (!settings.localOwner && Kit_IngameMain.instance.currentPvPGameModeBehaviour.ArePlayersEnemies(settings.ownerID, settings.ownerIsBot, hitPb, false)))
                                {
                                    //Check if he has spawn protection
                                    if (!hitPb.spawnProtection || hitPb.spawnProtection.CanTakeDamage(hitPb))
                                    {
                                        //Apply local damage
                                        if (hitPb.isServer) hitPb.ServerDamage(settings.damage * pdm.damageMultiplier, settings.gameGunID, settings.shotFromPosition, settings.direction, settings.ragdollForce, collision.contacts[0].point, pdm.ragdollId, settings.ownerIsBot, settings.ownerID);
                                        if (!settings.ownerIsBot)
                                        {
                                            //Since we hit a player, show the hitmarker
                                            Kit_IngameMain.instance.hud.DisplayHitmarker();
                                        }
                                    }
                                    else if (!settings.ownerIsBot)
                                    {
                                        //We hit a player but his spawn protection is active
                                        Kit_IngameMain.instance.hud.DisplayHitmarkerSpawnProtected();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (settings.isServer)
                        {
                            if (collision.contacts[0].otherCollider.GetComponentInParent<IKitDamageable>() != null)
                            {
                                if (collision.contacts[0].otherCollider.GetComponentInParent<IKitDamageable>().LocalDamage(settings.damage, settings.gameGunID, settings.shotFromPosition, settings.direction, settings.ragdollForce, collision.contacts[0].point, settings.ownerIsBot, settings.ownerID))
                                {
                                    if (!settings.ownerIsBot)
                                    {
                                        //Since we hit a player, show the hitmarker
                                        Kit_IngameMain.instance.hud.DisplayHitmarker();
                                    }
                                }
                            }
                        }
                    }

                    if (NetworkServer.active)
                    {
                        //Call
                        Kit_IngameMain.instance.ClientImpactProcess(collision.contacts[0].point, collision.contacts[0].normal, collision.contacts[0].otherCollider.tag);
                    }
                }
                else if (onImpact == RigidbodyBulletOnImpact.Explosion)
                {
                    if (explosionPrefab)
                    {
                        GameObject go = Instantiate(explosionPrefab, transform.position, transform.rotation);
                        if (go.GetComponent<Kit_Explosion>())
                        {
                            go.GetComponent<Kit_Explosion>().Explode(settings.ownerIsBot, settings.ownerID, settings.gameGunID);
                        }
                        if (go.GetComponent<Kit_FlashbangExplosion>())
                        {
                            go.GetComponent<Kit_FlashbangExplosion>().Explode(settings.ownerIsBot, settings.ownerID, settings.gameGunID);
                        }
                    }
                }

                if (settings.staysAliveAfterDeath)
                {
                    enabled = false;
                    if (settings.staysAliveAfterDeathTime > 0f)
                    {
                        Invoke("DestroyPooled", settings.staysAliveAfterDeathTime);
                    }
                }
                else
                {
                    Kit_IngameMain.instance.objectPooling.DestroyInstantiateable(gameObject);
                }
            }

            private void OnTriggerEnter(Collider other)
            {
                if (other.GetComponentInParent<Kit_RigidbodyBullet>()) return;

                if (settings == null) return;

                //Check if we hit ourself
                if (settings.localOwner)
                {
                    if (other.transform.root == settings.localOwner.transform.root) return;
                }

                if (onImpact == RigidbodyBulletOnImpact.Impact)
                {
                    //Check if we hit ourself
                    if (settings.localOwner)
                    {
                        if (other.transform.root == settings.localOwner.transform) return;
                    }

                    if (other.transform.GetComponent<Kit_PlayerDamageMultiplier>() && Kit_IngameMain.instance.currentGameModeType == 2)
                    {
                        Kit_PlayerDamageMultiplier pdm = other.transform.GetComponent<Kit_PlayerDamageMultiplier>();
                        if (other.transform.root.GetComponent<Kit_PlayerBehaviour>())
                        {
                            if (settings.isServer)
                            {
                                Kit_PlayerBehaviour hitPb = other.transform.root.GetComponent<Kit_PlayerBehaviour>();
                                //First check if we can actually damage that player
                                if ((settings.localOwner && Kit_IngameMain.instance.currentPvPGameModeBehaviour.ArePlayersEnemies(settings.localOwner, hitPb)) || (!settings.localOwner && Kit_IngameMain.instance.currentPvPGameModeBehaviour.ArePlayersEnemies(settings.ownerID, settings.ownerIsBot, hitPb, false)))
                                {
                                    //Check if he has spawn protection
                                    if (!hitPb.spawnProtection || hitPb.spawnProtection.CanTakeDamage(hitPb))
                                    {
                                        //Apply local damage
                                        if (hitPb.isServer) hitPb.ServerDamage(settings.damage * pdm.damageMultiplier, settings.gameGunID, settings.shotFromPosition, settings.direction, settings.ragdollForce, transform.position, pdm.ragdollId, settings.ownerIsBot, settings.ownerID);
                                        if (!settings.ownerIsBot)
                                        {
                                            //Since we hit a player, show the hitmarker
                                            Kit_IngameMain.instance.hud.DisplayHitmarker();
                                        }
                                    }
                                    else if (!settings.ownerIsBot)
                                    {
                                        //We hit a player but his spawn protection is active
                                        Kit_IngameMain.instance.hud.DisplayHitmarkerSpawnProtected();
                                    }
                                }
                            }
                        }

                        if (settings.staysAliveAfterDeath)
                        {
                            enabled = false;
                            if (settings.staysAliveAfterDeathTime > 0f)
                            {
                                Invoke("DestroyPooled", settings.staysAliveAfterDeathTime);
                            }
                        }
                        else
                        {
                            Kit_IngameMain.instance.objectPooling.DestroyInstantiateable(gameObject);
                        }

                        if (NetworkServer.active)
                        {
                            //Call
                            Kit_IngameMain.instance.ClientImpactProcess(transform.position, transform.forward, other.tag);
                        }
                    }
                }
            }
            #endregion

            #region Custom Calls
            void DestroyPooled()
            {
                Kit_IngameMain.instance.objectPooling.DestroyInstantiateable(gameObject);
            }
            #endregion
        }
    }
}