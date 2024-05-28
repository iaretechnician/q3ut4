
using Mirror;
using System;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Implements a basic vitals
    /// </summary>
    [CreateAssetMenu(menuName = "MarsFPSKit/Vitals/Simple")]
    public class Kit_SimpleVitals : Kit_VitalsBase
    {
        public float bloodyScreenTime = 3f;
        /// <summary>
        /// First person hit reactions will be applied if this is set to true
        /// </summary>
        public bool hitReactionEnabled = true;
        /// <summary>
        /// Intensity of the hit reactions
        /// </summary>
        public float hitReactionsIntensity = 1.2f;
        /// <summary>
        /// How fast will we recover from the hit reactions?
        /// </summary>
        public float hitReactionsReturnSpeed = 5f;

        /// <summary>
        /// ID for fall death CAT
        /// </summary>
        public int fallDamageSoundCatID;
        /// <summary>
        /// ID for out of map death CAT
        /// </summary>
        public int outOfMapSoundCatID;

        public override void ApplyHeal(Kit_PlayerBehaviour pb, float heal)
        {
            Kit_SimpleVitalsNetworkData vrd = pb.vitalsNetworkData as Kit_SimpleVitalsNetworkData;
            if (vrd)
            {
                vrd.hitPoints = Mathf.Clamp(vrd.hitPoints + heal, 0, 100f);
            }
        }

        public override void ApplyDamage(Kit_PlayerBehaviour pb, float dmg, bool botShot, uint idWhoShot, int gunID, Vector3 shotFrom)
        {
            Kit_SimpleVitalsNetworkData vrd = pb.vitalsNetworkData as Kit_SimpleVitalsNetworkData;
            if (vrd)
            {
                //Check if we can take damage
                if (!pb.spawnProtection || pb.spawnProtection.CanTakeDamage(pb))
                {
                    //Check for hitpoints
                    if (vrd.hitPoints > 0)
                    {
                        //Apply damage
                        vrd.hitPoints -= dmg;

                        //Hit reactions
                        if (hitReactionEnabled)
                        {
                            Vector3 dir = (pb.playerCameraTransform.InverseTransformDirection(Vector3.Cross(pb.playerCameraTransform.forward, pb.transform.position - shotFrom))).normalized * hitReactionsIntensity;
                            dir *= Mathf.Clamp(dmg / 30f, 0.3f, 1f);

                            Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(pb.playerCameraHitReactionsTransform, dir, 0.1f));
                            Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(pb.weaponsHitReactions, dir * 2f, 0.1f));
                        }
                        //Play voice
                        if (pb.voiceManager)
                        {
                            pb.voiceManager.DamageTaken(pb, Kit_VoiceManagerBase.DamageType.Projectile);
                        }
                        //Set damage effect
                        vrd.hitAlpha = 2f;
                        //Check for death
                        if (vrd.hitPoints <= 0)
                        {
                            //Call the die function on pb
                            pb.Die(botShot, idWhoShot, gunID);
                        }
                    }
                }
            }
        }

        public override void ApplyDamage(Kit_PlayerBehaviour pb, float dmg, bool botShot, uint idWhoShot, string deathCause, Vector3 shotFrom)
        {
            Kit_SimpleVitalsNetworkData vrd = pb.vitalsNetworkData as Kit_SimpleVitalsNetworkData;
            if (vrd)
            {
                //Check if we can take damage
                if (!pb.spawnProtection || pb.spawnProtection.CanTakeDamage(pb))
                {
                    //Check for hitpoints
                    if (vrd.hitPoints > 0)
                    {
                        //Apply damage
                        vrd.hitPoints -= dmg;

                        //Hit reactions
                        if (hitReactionEnabled)
                        {
                            Vector3 dir = (pb.playerCameraTransform.InverseTransformDirection(Vector3.Cross(pb.playerCameraTransform.forward, pb.transform.position - shotFrom))).normalized * hitReactionsIntensity;
                            dir *= Mathf.Clamp(dmg / 30f, 0.3f, 1f);

                            Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(pb.playerCameraHitReactionsTransform, dir, 0.1f));
                            Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(pb.weaponsHitReactions, dir * 2f, 0.1f));
                        }

                        //Play voice
                        if (pb.voiceManager)
                        {
                            pb.voiceManager.DamageTaken(pb, Kit_VoiceManagerBase.DamageType.Projectile);
                        }
                        //Set damage effect
                        vrd.hitAlpha = 2f;
                        //Check for death
                        if (vrd.hitPoints <= 0)
                        {
                            //Call the die function on pb
                            pb.Die(botShot, idWhoShot, deathCause);
                        }
                    }
                }
            }
        }

        public override void ApplyFallDamage(Kit_PlayerBehaviour pb, float dmg)
        {
            Kit_SimpleVitalsNetworkData vrd = pb.vitalsNetworkData as Kit_SimpleVitalsNetworkData;
            if (vrd)
            {
                //Check for hitpoints
                if (vrd.hitPoints > 0)
                {
                    pb.deathSoundCategory = fallDamageSoundCatID;
                    if (pb.voiceManager)
                    {
                        pb.deathSoundID = pb.voiceManager.GetDeathSoundID(pb, pb.deathSoundCategory);
                    }
                    //Apply damage
                    vrd.hitPoints -= dmg;
                    //Set damage effect
                    vrd.hitAlpha = 2f;
                    //Play voice
                    if (pb.voiceManager)
                    {
                        pb.voiceManager.DamageTaken(pb, Kit_VoiceManagerBase.DamageType.Other);
                    }
                    //Check for death
                    if (vrd.hitPoints <= 0)
                    {
                        //Reset player force
                        pb.ragdollForce = 0f;
                        //Call the die function on pb
                        pb.Die(-2);
                    }
                }
            }
        }

        public override void ApplyEnvironmentalDamage(Kit_PlayerBehaviour pb, float dmg, int deathSoundCategory)
        {
            Kit_SimpleVitalsNetworkData vrd = pb.vitalsNetworkData as Kit_SimpleVitalsNetworkData;
            if (vrd)
            {
                //Check for hitpoints
                if (vrd.hitPoints > 0)
                {
                    pb.deathSoundCategory = deathSoundCategory;
                    if (pb.voiceManager)
                    {
                        pb.deathSoundID = pb.voiceManager.GetDeathSoundID(pb, pb.deathSoundCategory);
                    }
                    //Apply damage
                    vrd.hitPoints -= dmg;
                    //Set damage effect
                    vrd.hitAlpha = 2f;
                    //Play voice
                    if (pb.voiceManager)
                    {
                        pb.voiceManager.DamageTaken(pb, Kit_VoiceManagerBase.DamageType.Other);
                    }
                    //Check for death
                    if (vrd.hitPoints <= 0)
                    {
                        //Reset player force
                        pb.ragdollForce = 0f;
                        //Call the die function on pb
                        pb.Die(-1);
                    }
                }
            }
        }

        public override void Suicide(Kit_PlayerBehaviour pb)
        {
            Kit_SimpleVitalsNetworkData vrd = pb.vitalsNetworkData as Kit_SimpleVitalsNetworkData;
            if (vrd)
            {
                //Reset player force
                pb.ragdollForce = 0f;
                //Call the die function on pb
                pb.Die(-3);
            }
        }

        public override void CustomUpdate(Kit_PlayerBehaviour pb)
        {
            Kit_SimpleVitalsNetworkData vrd = pb.vitalsNetworkData as Kit_SimpleVitalsNetworkData;
            if (vrd)
            {
                //Clamp
                vrd.hitPoints = Mathf.Clamp(vrd.hitPoints, 0f, 100f);
                //Decrease hit alpha
                if (vrd.hitAlpha > 0)
                {
                    vrd.hitAlpha -= (Time.deltaTime * 2) / bloodyScreenTime;
                }

                if (pb.isFirstPersonActive)
                {
                    //Update hud
                    Kit_IngameMain.instance.hud.DisplayHealth(vrd.hitPoints);
                    Kit_IngameMain.instance.hud.DisplayHurtState(vrd.hitAlpha);
                }
                //Return hit reactions
                if (hitReactionEnabled)
                {
                    pb.playerCameraHitReactionsTransform.localRotation = Quaternion.Slerp(pb.playerCameraHitReactionsTransform.localRotation, Quaternion.identity, Time.deltaTime * hitReactionsReturnSpeed);
                    pb.weaponsHitReactions.localRotation = Quaternion.Slerp(pb.weaponsHitReactions.localRotation, Quaternion.identity, Time.deltaTime * hitReactionsReturnSpeed);
                }

                if (pb.isServer)
                {
                    //Check if we are lower than death threshold
                    if (pb.transform.position.y <= Kit_IngameMain.instance.mapDeathThreshold)
                    {
                        pb.deathSoundCategory = outOfMapSoundCatID;
                        if (pb.voiceManager)
                        {
                            pb.deathSoundID = pb.voiceManager.GetDeathSoundID(pb, pb.deathSoundCategory);
                        }
                        pb.Die(-1);
                    }
                }
            }
        }

        public override void InitializeServer(Kit_PlayerBehaviour pb)
        {
            GameObject data = Instantiate(networkData, Vector3.zero, Quaternion.identity);
            Kit_SimpleVitalsNetworkData nData = data.GetComponent<Kit_SimpleVitalsNetworkData>();
            pb.vitalsNetworkData = nData;
            nData.ownerPlayerNetworkId = pb.netId;
            nData.hitPoints = 100f;
            if (pb.isBot)
                NetworkServer.Spawn(data);
            else
                NetworkServer.Spawn(data, pb.gameObject);
        }

        public override void InitializeClient(Kit_PlayerBehaviour pb)
        {
            if (!pb.vitalsNetworkData)
            {
                //Find ours
                pb.vitalsNetworkData = FindObjectsOfType<Kit_SimpleVitalsNetworkData>().Where(x => x.ownerPlayerNetworkId == pb.netId).FirstOrDefault();
            }
        }
    }
}
