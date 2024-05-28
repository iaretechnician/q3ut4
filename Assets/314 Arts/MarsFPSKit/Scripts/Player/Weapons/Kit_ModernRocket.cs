using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class ModernRocketSetupData
        {
            /// <summary>
            /// Damage of this rocket
            /// </summary>
            public float damage = 30f;
            /// <summary>
            /// Gravity multiplier
            /// </summary>
            public float gravityMultiplier = 1f;
            /// <summary>
            /// Speed of this rocket
            /// </summary>
            public float speed = 200f;
            /// <summary>
            /// Direction of the rocket (spread already applied.)
            /// </summary>
            public Vector3 direction;
            /// <summary>
            /// Force to apply to ragdoll (if hit)
            /// </summary>
            public float ragdollForce = 500f;
            /// <summary>
            /// ID of the gun this rocket was fired with
            /// </summary>
            public int gameGunID;
            /// <summary>
            /// Lifetime of the rocket (in s)
            /// </summary>
            public float rocketLifeTime = 10f;
            /// <summary>
            /// Mask of things we can hit
            /// </summary>
            public LayerMask mask;
            /// <summary>
            /// Where does this rocket originate from?
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
        }

        public class Kit_ModernRocket : Kit_BulletBase
        {
            public GameObject explosionPrefab;
            /// <summary>
            /// Visible renderer for bullet
            /// </summary>
            public GameObject bulletRenderer;

            #region Runtime
            /// <summary>
            /// Settings received from weapon script
            /// </summary>
            private ModernRocketSetupData settings;
            /// <summary>
            /// Velocity of the rocket
            /// </summary>
            private Vector3 velocity;
            /// <summary>
            /// Next position of the rocket
            /// </summary>
            private Vector3 newPosition;
            /// <summary>
            /// Previous position of the rocket
            /// </summary>
            private Vector3 oldPosition;
            /// <summary>
            /// Temp direction
            /// </summary>
            private Vector3 tempDir;
            /// <summary>
            /// Temp distance
            /// </summary>
            private float tempDistance;
            /// <summary>
            /// Hit of raycast
            /// </summary>
            public RaycastHit tempHit;
            /// <summary>
            /// Last hit ID for fronthit
            /// </summary>
            private int lastHitID;
            /// <summary>
            /// Since when does the rocket exist?
            /// </summary>
            private float rocketExistTime;
            /// <summary>
            /// After which frame should the bullet be shown?
            /// </summary>
            private int frameOfShow;
            /// <summary>
            /// Did we show bullet?
            /// </summary>
            private bool wasBulletShown;
            #endregion

            public override void Setup( Kit_ModernWeaponScript ws, Kit_ModernWeaponScriptRuntimeData data, Kit_PlayerBehaviour pb, Vector3 dir)
            {
                //Setup
                ModernRocketSetupData mbsd = new ModernRocketSetupData();
                //Setup data
                mbsd.damage = data.baseDamage;
                mbsd.gravityMultiplier = data.bulletGravityMultiplier;
                mbsd.speed = data.bulletSpeed;
                mbsd.direction = dir;
                mbsd.ragdollForce = data.ragdollForce;
                mbsd.gameGunID = ws.gameGunID;
                mbsd.rocketLifeTime = data.bulletLifeTime;
                mbsd.mask = pb.weaponHitLayers;
                mbsd.shotFromPosition = pb.transform.position;
                mbsd.isServer = pb.isServer;
                mbsd.localOwner = pb;
                mbsd.ownerIsBot = pb.isBot;
                mbsd.ownerID = pb.id;

                //Get settings
                settings = mbsd;
                //Set position default
                newPosition = transform.position;
                oldPosition = transform.position;
                velocity = mbsd.speed * transform.forward;

                tempDir = Vector3.zero;
                tempDistance = 0f;
                lastHitID = 0;

                rocketExistTime = 0f;
                tempDir = Vector3.zero;
                tempDistance = 0f;
                lastHitID = 0;

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
                //Advance
                newPosition += (velocity + settings.direction + (Physics.gravity * settings.gravityMultiplier)) * Time.deltaTime;
                //Calculate direction
                tempDir = newPosition - oldPosition;
                //Calculate travelled distance
                tempDistance = tempDir.magnitude;
                //Divide
                tempDir /= tempDistance;
                //Check if we actually travelled
                if (tempDistance > 0f)
                {
                    RaycastHit[] hits = Physics.RaycastAll(oldPosition, tempDir, tempDistance, settings.mask);
                    hits = hits.OrderBy(h => h.distance).ToArray();
                    for (int i = 0; i < hits.Length; i++)
                    {
                        //Check if we hit ourselves
                        if (settings.localOwner && hits[i].transform.root != settings.localOwner.transform.root)
                        {
                            //Check if we hit last object again
                            if (hits[i].collider.GetInstanceID() != lastHitID)
                            {
                                //Set new position to hit position
                                newPosition = hits[i].point;
                                //Call function
                                OnHit(hits[i], tempDir);
                                break;
                            }
                        }
                    }
                }

                //Check if we should destroy
                rocketExistTime += Time.deltaTime;
                if (rocketExistTime > settings.rocketLifeTime && enabled) Kit_IngameMain.instance.objectPooling.DestroyInstantiateable(gameObject);

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

            void LateUpdate()
            {
                //Set position
                oldPosition = transform.position;
                //Move
                transform.position = newPosition;
            }
            #endregion

            #region Custom Calls
            void OnHit(RaycastHit hit, Vector3 dir)
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

                //Assign ID
                lastHitID = hit.collider.GetInstanceID();
                Kit_IngameMain.instance.objectPooling.DestroyInstantiateable(gameObject);
            }
            #endregion
        }
    }
}