
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace MarsFPSKit
{
    public class ModernVoiceData
    {
        /// <summary>
        /// The ID of the voice that this character uses
        /// </summary>
        public int id;

        /// <summary>
        /// When did we check for enemies the last time?
        /// </summary>
        public float nextScan;

        public List<Kit_PlayerBehaviour> enemyPlayersAwareOff = new List<Kit_PlayerBehaviour>();
    }

    [CreateAssetMenu(menuName = "MarsFPSKit/Voice Manager/Modern")]
    public class Kit_ModernVoiceManager : Kit_VoiceManagerBase
    {
        /// <summary>
        /// Chance in % that the character says somethin when spotting an enemy
        /// </summary>
        public int spottedChance = 50;
        /// <summary>
        /// Chance in % that the character says somethin when taking damage
        /// </summary>
        public int damageTakenChance = 50;
        /// <summary>
        /// Chance in % that the character says somethin when having killed an enemy
        /// </summary>
        public int enemyKilledChance = 50;
        /// <summary>
        /// Chance in % that the character says somethin when reloading
        /// </summary>
        public int reloadChance = 50;
        /// <summary>
        /// Chance in % that the character says somethin when throwin a grenade
        /// </summary>
        public int grenadeChance = 50;
        /// <summary>
        /// Chance in % that the character says somethin when using a melee weapon
        /// </summary>
        public int meleeChance = 50;

        public override void SetupOwner(Kit_PlayerBehaviour pb)
        {
            ModernVoiceData mvd = new ModernVoiceData();
            pb.voiceManagerData = mvd;
            if (pb.thirdPersonPlayerModel.information.voices.Length > 0 && pb.isServer)
            {
                mvd.id = Random.Range(0, pb.thirdPersonPlayerModel.information.voices.Length);
            }

            mvd.enemyPlayersAwareOff = new List<Kit_PlayerBehaviour>();
            spottingBoxSize = new Vector3(spottingBoxExtents.x, spottingBoxExtents.y, spottingMaxDistance / 2f);
        }

        public override void SetupOthers(Kit_PlayerBehaviour pb)
        {
            pb.voiceManagerData = new ModernVoiceData();
        }

        public override void PlayVoiceRpcReceived(Kit_PlayerBehaviour pb, int catId, int id, int idTwo = 0)
        {
            //Get runtime data
            if (pb.voiceManagerData != null && pb.voiceManagerData.GetType() == typeof(ModernVoiceData))
            {
                ModernVoiceData mvd = pb.voiceManagerData as ModernVoiceData;
                //Check if we're already talkin
                if (!pb.thirdPersonPlayerModel.soundVoice.isPlaying)
                {
                    if (catId == 0)
                    {
                        pb.thirdPersonPlayerModel.soundVoice.clip = pb.thirdPersonPlayerModel.information.voices[mvd.id].enemySpotted[id];
                        pb.thirdPersonPlayerModel.soundVoice.Play();
                    }
                    else if (catId == 1)
                    {
                        pb.thirdPersonPlayerModel.soundVoice.clip = pb.thirdPersonPlayerModel.information.voices[mvd.id].damageTakenProjectile[id];
                        pb.thirdPersonPlayerModel.soundVoice.Play();
                    }
                    else if (catId == 2)
                    {
                        pb.thirdPersonPlayerModel.soundVoice.clip = pb.thirdPersonPlayerModel.information.voices[mvd.id].damageTakenOthers[id];
                        pb.thirdPersonPlayerModel.soundVoice.Play();
                    }
                    else if (catId == 3)
                    {
                        pb.thirdPersonPlayerModel.soundVoice.clip = pb.thirdPersonPlayerModel.information.voices[mvd.id].enemyKilled[id];
                        pb.thirdPersonPlayerModel.soundVoice.Play();
                    }
                    else if (catId == 4)
                    {
                        pb.thirdPersonPlayerModel.soundVoice.clip = pb.thirdPersonPlayerModel.information.voices[mvd.id].reloading[id];
                        pb.thirdPersonPlayerModel.soundVoice.Play();
                    }
                    else if (catId == 5)
                    {
                        pb.thirdPersonPlayerModel.soundVoice.clip = pb.thirdPersonPlayerModel.information.voices[mvd.id].grenadeThrown[idTwo].clips[id];
                        pb.thirdPersonPlayerModel.soundVoice.Play();
                    }
                    else if (catId == 6)
                    {
                        pb.thirdPersonPlayerModel.soundVoice.clip = pb.thirdPersonPlayerModel.information.voices[mvd.id].meleeUsed[idTwo].clips[id];
                        pb.thirdPersonPlayerModel.soundVoice.Play();
                    }
                }
            }
        }

        public override void SpottedEnemy(Kit_PlayerBehaviour pb, Kit_PlayerBehaviour enemy)
        {
            if (pb.canControlPlayer)
            {
                if (pb.thirdPersonPlayerModel.information.voices.Length > 0 && pb.isServer)
                {
                    //Get runtime data
                    if (pb.voiceManagerData != null && pb.voiceManagerData.GetType() == typeof(ModernVoiceData))
                    {
                        ModernVoiceData mvd = pb.voiceManagerData as ModernVoiceData;

                        if (Random.Range(0, 100) > 100 - spottedChance)
                        {
                            if (pb.thirdPersonPlayerModel.information.voices[mvd.id].enemySpotted.Length > 0)
                            {
                                //Say something
                                pb.RpcPlayVoiceLine(0, Random.Range(0, pb.thirdPersonPlayerModel.information.voices[mvd.id].enemySpotted.Length));
                            }
                        }
                    }
                }
            }
        }

        public override void DamageTaken(Kit_PlayerBehaviour pb, DamageType dt)
        {
            if (pb.thirdPersonPlayerModel.information.voices.Length > 0 && pb.isServer)
            {
                //Get runtime data
                if (pb.voiceManagerData != null && pb.voiceManagerData.GetType() == typeof(ModernVoiceData))
                {
                    ModernVoiceData mvd = pb.voiceManagerData as ModernVoiceData;

                    if (Random.Range(0, 100) > 100 - damageTakenChance)
                    {
                        if (dt == DamageType.Projectile)
                        {
                            if (pb.thirdPersonPlayerModel.information.voices[mvd.id].damageTakenProjectile.Length > 0)
                            {
                                pb.RpcPlayVoiceLine(1, Random.Range(0, pb.thirdPersonPlayerModel.information.voices[mvd.id].damageTakenProjectile.Length));
                            }
                        }
                        else if (dt == DamageType.Other)
                        {
                            if (pb.thirdPersonPlayerModel.information.voices[mvd.id].damageTakenOthers.Length > 0)
                            {
                                pb.RpcPlayVoiceLine(2, Random.Range(0, pb.thirdPersonPlayerModel.information.voices[mvd.id].damageTakenOthers.Length));
                            }
                        }
                    }
                }
            }
        }

        public override void EnemyKilled(Kit_PlayerBehaviour pb)
        {
            if (pb.thirdPersonPlayerModel.information.voices.Length > 0 && pb.isServer)
            {
                //Get runtime data
                if (pb.voiceManagerData != null && pb.voiceManagerData.GetType() == typeof(ModernVoiceData))
                {
                    ModernVoiceData mvd = pb.voiceManagerData as ModernVoiceData;

                    if (Random.Range(0, 100) > 100 - enemyKilledChance)
                    {
                        if (pb.thirdPersonPlayerModel.information.voices[mvd.id].enemyKilled.Length > 0)
                        {
                            pb.RpcPlayVoiceLine(3, Random.Range(0, pb.thirdPersonPlayerModel.information.voices[mvd.id].enemyKilled.Length));
                        }
                    }
                }
            }
        }

        public override void Reloading(Kit_PlayerBehaviour pb)
        {
            if (pb.thirdPersonPlayerModel.information.voices.Length > 0 && pb.isServer)
            {
                //Get runtime data
                if (pb.voiceManagerData != null && pb.voiceManagerData.GetType() == typeof(ModernVoiceData))
                {
                    ModernVoiceData mvd = pb.voiceManagerData as ModernVoiceData;

                    if (Random.Range(0, 100) > 100 - reloadChance)
                    {
                        if (pb.thirdPersonPlayerModel.information.voices[mvd.id].reloading.Length > 0)
                        {
                            pb.RpcPlayVoiceLine(4, Random.Range(0, pb.thirdPersonPlayerModel.information.voices[mvd.id].reloading.Length));
                        }
                    }
                }
            }
        }

        public override void GrenadeThrown(Kit_PlayerBehaviour pb, int id)
        {
            if (pb.thirdPersonPlayerModel.information.voices.Length > 0 && pb.isServer)
            {
                //Get runtime data
                if (pb.voiceManagerData != null && pb.voiceManagerData.GetType() == typeof(ModernVoiceData))
                {
                    ModernVoiceData mvd = pb.voiceManagerData as ModernVoiceData;

                    if (Random.Range(0, 100) > 100 - grenadeChance)
                    {
                        if (pb.thirdPersonPlayerModel.information.voices[mvd.id].grenadeThrown[id].clips.Length > 0)
                        {
                            pb.RpcPlayVoiceLine(5, Random.Range(0, pb.thirdPersonPlayerModel.information.voices[mvd.id].grenadeThrown[id].clips.Length), id);
                        }
                    }
                }
            }
        }

        public override void MeleeUsed(Kit_PlayerBehaviour pb, int id)
        {
            if (pb.thirdPersonPlayerModel.information.voices.Length > 0 && pb.isServer)
            {
                //Get runtime data
                if (pb.voiceManagerData != null && pb.voiceManagerData.GetType() == typeof(ModernVoiceData))
                {
                    ModernVoiceData mvd = pb.voiceManagerData as ModernVoiceData;

                    if (Random.Range(0, 100) > 100 - meleeChance)
                    {
                        if (pb.thirdPersonPlayerModel.information.voices[mvd.id].meleeUsed[id].clips.Length > 0)
                        {
                            pb.RpcPlayVoiceLine(6, Random.Range(0, pb.thirdPersonPlayerModel.information.voices[mvd.id].meleeUsed[id].clips.Length), id);
                        }
                    }
                }
            }
        }

        public override int GetDeathSoundID(Kit_PlayerBehaviour pb, int cat)
        {
            if (pb.thirdPersonPlayerModel.information.voices.Length > 0 && pb.isServer)
            {
                //Get runtime data
                if (pb.voiceManagerData != null && pb.voiceManagerData.GetType() == typeof(ModernVoiceData))
                {
                    ModernVoiceData mvd = pb.voiceManagerData as ModernVoiceData;
                    if (cat < pb.thirdPersonPlayerModel.information.voices[mvd.id].deathSounds.Length)
                        return Random.Range(0, pb.thirdPersonPlayerModel.information.voices[mvd.id].deathSounds[cat].clips.Length);
                }
            }
            return -1;
        }

        public override void PlayDeathSound(Kit_PlayerBehaviour pb, int cat, int id)
        {
            if (cat >= 0 && id >= 0)
            {
                if (pb.thirdPersonPlayerModel.information.voices.Length > 0 && pb.isServer)
                {
                    //Get runtime data
                    if (pb.voiceManagerData != null && pb.voiceManagerData.GetType() == typeof(ModernVoiceData))
                    {
                        ModernVoiceData mvd = pb.voiceManagerData as ModernVoiceData;
                        pb.thirdPersonPlayerModel.soundVoice.clip = pb.thirdPersonPlayerModel.information.voices[mvd.id].deathSounds[cat].clips[Mathf.Clamp(id, 0, pb.thirdPersonPlayerModel.information.voices[mvd.id].deathSounds[cat].clips.Length - 1)];

                        GameObject go = Instantiate(pb.thirdPersonPlayerModel.soundVoice.gameObject, pb.thirdPersonPlayerModel.soundVoice.transform.position, pb.thirdPersonPlayerModel.soundVoice.transform.rotation);
                        go.GetComponent<AudioSource>().Play();
                        Destroy(go, 3);
                    }
                }
            }
        }

        /// <summary>
        /// How many seconds apart are our scans?
        /// </summary>
        public float scanTimer = 1f;

        [Header("Spotting")]
        public LayerMask spottingLayer;
        public LayerMask spottingCheckLayers;
        public float spottingMaxDistance = 50f;
        public Vector2 spottingBoxExtents = new Vector2(30, 30);
        private Vector3 spottingBoxSize;
        public float spottingFov = 90f;
        public float spottingRayDistance = 200f;

        public override void ScanForEnemies(Kit_PlayerBehaviour pb)
        {
            ModernVoiceData did = pb.voiceManagerData as ModernVoiceData;

            if (Time.time > did.nextScan)
            {
                Collider[] possiblePlayers = Physics.OverlapBox(pb.playerCameraTransform.position + pb.playerCameraTransform.forward * (spottingMaxDistance / 2), spottingBoxSize, pb.playerCameraTransform.rotation, spottingLayer.value);

                //Clean
                did.enemyPlayersAwareOff.RemoveAll(item => item == null);

                //Loop
                for (int i = 0; i < possiblePlayers.Length; i++)
                {
                    //Check if it is a player
                    Kit_PlayerBehaviour pnb = possiblePlayers[i].transform.root.GetComponent<Kit_PlayerBehaviour>();
                    if (pnb && pnb != pb)
                    {
                        if (CanSeePlayer(pb, did, pnb))
                        {
                            if (isEnemyPlayer(pb, did, pnb))
                            {
                                if (!did.enemyPlayersAwareOff.Contains(pnb))
                                {
                                    //Add to our known list
                                    did.enemyPlayersAwareOff.Add(pnb);
                                    //Call spotted
                                    if (pb.voiceManager)
                                    {
                                        pb.voiceManager.SpottedEnemy(pb, pnb);
                                    }
                                }
                            }
                        }
                    }
                }
                did.nextScan = Time.time + scanTimer;
            }
        }


        bool CanSeePlayer(Kit_PlayerBehaviour pb, ModernVoiceData did, Kit_PlayerBehaviour enemyPlayer)
        {
            if (enemyPlayer)
            {
                RaycastHit hit;
                Vector3 rayDirection = enemyPlayer.playerCameraTransform.position - new Vector3(0, 0.2f, 0f) - pb.playerCameraTransform.position;

                if ((Vector3.Angle(rayDirection, pb.playerCameraTransform.forward)) < spottingFov)
                {
                    if (Physics.Raycast(pb.playerCameraTransform.position, rayDirection, out hit, spottingRayDistance, spottingCheckLayers.value))
                    {
                        if (hit.collider.transform.root == enemyPlayer.transform.root)
                        {
                            return true;
                        }
                        else
                        {

                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool isEnemyPlayer(Kit_PlayerBehaviour pb, ModernVoiceData did, Kit_PlayerBehaviour enemyPlayer)
        {
            if (pb)
            {
                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                {
                    if (!Kit_IngameMain.instance.currentPvPGameModeBehaviour.isTeamGameMode) return true;
                    else
                    {
                        if (pb.myTeam != enemyPlayer.myTeam) return true;
                        else return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}