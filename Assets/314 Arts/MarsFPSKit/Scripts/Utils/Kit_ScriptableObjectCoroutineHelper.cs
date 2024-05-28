using MarsFPSKit.Weapons;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// A helper class to use coroutines from scriptable objects. The coroutines need to do checks if the instances supplied still exist though otherwise it might throw errors
    /// </summary>
    [DisallowMultipleComponent]
    public class Kit_ScriptableObjectCoroutineHelper : MonoBehaviour
    {
        public static Kit_ScriptableObjectCoroutineHelper instance;

        void Awake()
        {
            //The object should only exist once. Assign the instance
            instance = this;
        }

        public IEnumerator Kick(Transform trans, Vector3 target, float time)
        {
            Quaternion startRotation = trans.localRotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(target);
            float rate = 1.0f / time;
            float t = 0.0f;
            while (trans && t < 1.0f)
            {
                //Advance
                t += Time.deltaTime * rate;
                //Slerp to it 
                trans.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return null;
            }
        }

        private readonly Vector3 recoilClamp = new Vector3(90, 180, 180);

        public IEnumerator WeaponApplyRecoil(Kit_ModernWeaponScript behaviour, Kit_ModernWeaponScriptRuntimeData data, Kit_PlayerBehaviour pb, Vector2 target, float time)
        {
            Quaternion startRotation = pb.recoilApplyRotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(target.y, -target.x, 0f);
            float rate = 1.0f / time;
            float t = 0.0f;
            while (pb && behaviour && data != null && t < 1.0f)
            {
                //Advance
                t += Time.deltaTime * rate;
                //Slerp to it 
                pb.recoilApplyRotation = Kit_Helper.ClampRotation(Quaternion.Slerp(startRotation, endRotation, t), recoilClamp);
                yield return null;
            }
        }

        public IEnumerator NetworkReplaceWeapon(Kit_PlayerBehaviour pb, Kit_ModernWeaponManagerNetworkData runtimeData, int itemIndex, uint newItem)
        {
            //Find new data
            NetworkIdentity id = null;

            if (NetworkServer.active)
            {
                id = NetworkServer.spawned[newItem];
            }
            else
            {
                while (!NetworkClient.spawned.ContainsKey(newItem)) yield return null;
                id = NetworkClient.spawned[newItem];
            }

            runtimeData.weaponsInUseDataObjects[itemIndex] = id.GetComponent<Kit_WeaponRuntimeDataBase>();

            int weaponId = runtimeData.weaponsInUseDataObjects[itemIndex].id;
            //Get their behaviour modules
            Kit_WeaponBase weaponBehaviour = pb.gameInformation.allWeapons[weaponId];
            //Assign Behaviour
            runtimeData.weaponsInUseDataObjects[itemIndex].behaviour = weaponBehaviour;
            //Setup FP
            weaponBehaviour.SetupFirstPerson(pb, runtimeData.weaponsInUseDataObjects[itemIndex]);
            //Setup TP
            weaponBehaviour.SetupThirdPerson(pb, runtimeData.weaponsInUseDataObjects[itemIndex]);

            if (!runtimeData.quickUseInProgress && itemIndex == runtimeData.currentWeapon)
            {
                if (pb.isFirstPersonActive)
                {
                    Kit_IngameMain.instance.hud.DisplaySniperScope(false);
                }

                //Select current weapon
                runtimeData.weaponsInUseDataObjects[itemIndex].behaviour.DrawWeapon(pb, runtimeData.weaponsInUseDataObjects[itemIndex]);
                //Play Third person animation
                pb.thirdPersonPlayerModel.PlayWeaponChangeAnimation(runtimeData.weaponsInUseDataObjects[itemIndex].behaviour.thirdPersonAnimType, true, runtimeData.weaponsInUseDataObjects[itemIndex].behaviour.drawTime);
                runtimeData.switchInProgress = true;
                //Set phase
                runtimeData.switchPhase = 1;
                //Set time
                runtimeData.switchNextEnd = Time.time + runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.drawTime;
                //Play Third person animation
                pb.thirdPersonPlayerModel.PlayWeaponChangeAnimation(runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.thirdPersonAnimType, true, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.drawTime);
            }
        }

        public IEnumerator MeleeExecuteStab(Kit_ModernMeleeScript values, Kit_ModernMeleeScriptRuntimeData data, AttackSettings attackSettings, Kit_PlayerBehaviour pb, float revertBy)
        {
            //Relay to voice manager
            if (pb.voiceManager)
            {
                pb.voiceManager.MeleeUsed(pb, values.voiceMeleeSoundID);
            }

            if (attackSettings.stabWindupAnimationName != "")
            {
                if (pb.isFirstPersonActive)
                {
                    if (attackSettings.stabWindupAnimationName != "")
                    {
                        if (data.meleeRenderer.anim)
                        {
                            data.meleeRenderer.anim.Play(attackSettings.stabWindupAnimationName);
                        }
                        else if (data.meleeRenderer.legacyAnim)
                        {
                            data.meleeRenderer.legacyAnim.Play(attackSettings.stabWindupAnimationName);
                        }
                    }
                }

                //+TODO: Mirror Update
                //Call network
                //pb.photonView.RPC("MeleeStabNetwork", Photon.Pun.RpcTarget.Others, 0, 0);
                //Play third person reload anim
                pb.thirdPersonPlayerModel.PlayMeleeAnimation(0, 0);

                data.nextActionPossibleAt = Time.time + attackSettings.stabWindupTime + attackSettings.stabHitTime + attackSettings.stabMissTime;

                //Wait
                yield return new WaitForSeconds(attackSettings.stabWindupTime);
            }

            if (pb)
            {
                //Set Lag Compensation
                Kit_IngameMain.instance.SetLagCompensationTo(revertBy);

                Vector3 center = pb.playerCameraTransform.position - (pb.playerCameraTransform.forward * (attackSettings.stabReach / 2f));
                Vector3 dir = pb.playerCameraTransform.forward;

                RaycastHit[] hits = Physics.BoxCastAll(center, attackSettings.stabHalfExtents, dir, Quaternion.LookRotation(dir), attackSettings.stabReach, pb.weaponHitLayers.value, QueryTriggerInteraction.Collide).OrderBy(h => Vector3.Distance(pb.playerCameraTransform.position, h.point)).ToArray();

                //Reset
                Kit_IngameMain.instance.SetLagCompensationTo(0);

                int penetrationPowerLeft = attackSettings.stabPenetrationPower;
                //After penetration, only test for damage.
                bool penetratedOnce = false;

                //0 = Miss
                //1 = Hit Player
                //2 = Hit Object
                int result = 0;

                //Loop through all
                for (int i = 0; i < hits.Length; i++)
                {
                    //Check if we hits[i] ourselves
                    if (hits[i].transform.root != pb.transform.root)
                    {
                        //Check if we hits[i] a player
                        if (hits[i].transform.GetComponent<Kit_PlayerDamageMultiplier>() && Kit_IngameMain.instance.currentGameModeType == 2)
                        {
                            Kit_PlayerDamageMultiplier pdm = hits[i].transform.GetComponent<Kit_PlayerDamageMultiplier>();
                            if (hits[i].transform.root.GetComponent<Kit_PlayerBehaviour>())
                            {
                                Kit_PlayerBehaviour hitPb = hits[i].transform.root.GetComponent<Kit_PlayerBehaviour>();
                                //First check if we can actually damage that player
                                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.ArePlayersEnemies(pb, hitPb))
                                {
                                    //Check if he has spawn protection
                                    if (!hitPb.spawnProtection || hitPb.spawnProtection.CanTakeDamage(hitPb))
                                    {
                                        //Apply local damage, sample damage dropoff via distance
                                        if (pb.isServer)
                                        {
                                            hitPb.ServerDamage(attackSettings.stabDamage * pdm.damageMultiplier, values.gameGunID, pb.transform.position, dir, attackSettings.stabRagdollForce, hits[i].point, pdm.ragdollId, pb.isBot, pb.id);
                                            if (!pb.isBot) pb.TargetHitMarker(0);
                                        }
                                    }
                                    else if (pb.isServer)
                                    {
                                        //We hits[i] a player but his spawn protection is active
                                        if (!pb.isBot) pb.TargetHitMarker(1);
                                    }
                                }

                                if (!penetratedOnce)
                                    result = 1;

                                if (hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>())
                                {
                                    Kit_MeleePenetrateableObject penetration = hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>();
                                    if (penetrationPowerLeft >= penetration.cost)
                                    {
                                        penetrationPowerLeft -= penetration.cost;
                                        penetratedOnce = true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    //Just end
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (!penetratedOnce)
                            {
                                if (pb.isServer)
                                {
                                    pb.ClientImpactProcess(hits[i].point, hits[i].normal, hits[i].collider.tag);
                                }

                                if (hits[i].collider.GetComponentInParent<IKitDamageable>() != null)
                                {
                                    if (hits[i].collider.GetComponentInParent<IKitDamageable>().LocalDamage(attackSettings.stabDamage, values.gameGunID, pb.transform.position, dir, attackSettings.stabRagdollForce, hits[i].point, pb.isBot, pb.id))
                                    {
                                        if (pb.isServer)
                                        {
                                            //Since we hit a player, show the hitmarker
                                            if (!pb.isBot) pb.TargetHitMarker(0);
                                        }
                                    }
                                }
                            }

                            if (!penetratedOnce)
                                result = 2;

                            if (hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>())
                            {
                                Kit_MeleePenetrateableObject penetration = hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>();
                                if (penetrationPowerLeft >= penetration.cost)
                                {
                                    penetrationPowerLeft -= penetration.cost;
                                    penetratedOnce = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                //Just end
                                break;
                            }
                        }
                    }
                }

                if (result == 0)
                {
                    if (pb.isFirstPersonActive)
                    {
                        //Play animation
                        if (attackSettings.stabAnimationMissName != "")
                        {
                            if (data.meleeRenderer.anim)
                            {
                                data.meleeRenderer.anim.Play(attackSettings.stabAnimationMissName);
                            }
                            else if (data.meleeRenderer.legacyAnim)
                            {
                                data.meleeRenderer.legacyAnim.Play(attackSettings.stabAnimationMissName);
                            }
                        }
                        //Play sound
                        if (attackSettings.stabMissSound)
                        {
                            data.sounds.clip = attackSettings.stabMissSound;
                            data.sounds.Play();
                        }
                    }

                    //Call network
                    if (pb.isServer) pb.RpcMeleeStabNetwork(1, 0);
                    //Play third person reload anim
                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(0, 1);

                    data.nextActionPossibleAt = Time.time + attackSettings.stabMissTime;

                    yield return new WaitForSeconds(attackSettings.stabMissTime);
                }
                else if (result == 1)
                {
                    if (pb.isFirstPersonActive)
                    {
                        //Play animation
                        if (attackSettings.stabAnimationHitName != "")
                        {
                            if (data.meleeRenderer.anim)
                            {
                                data.meleeRenderer.anim.Play(attackSettings.stabAnimationHitName);
                            }
                            else if (data.meleeRenderer.legacyAnim)
                            {
                                data.meleeRenderer.legacyAnim.Play(attackSettings.stabAnimationHitName);
                            }
                        }
                        //Play sound
                        if (attackSettings.stabHitSound)
                        {
                            data.sounds.clip = attackSettings.stabHitSound;
                            data.sounds.Play();
                        }
                    }

                    //Call network
                    if (pb.isServer) pb.RpcMeleeStabNetwork(2, 0);
                    //Play third person reload anim
                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(0, 2);

                    data.nextActionPossibleAt = Time.time + attackSettings.stabHitTime;

                    yield return new WaitForSeconds(attackSettings.stabHitTime);
                }
                else if (result == 2)
                {
                    if (pb.isFirstPersonActive)
                    {
                        //Play animation
                        if (attackSettings.stabAnimationHitObjectName != "")
                        {
                            if (data.meleeRenderer.anim)
                            {
                                data.meleeRenderer.anim.Play(attackSettings.stabAnimationHitObjectName);
                            }
                            else if (data.meleeRenderer.legacyAnim)
                            {
                                data.meleeRenderer.legacyAnim.Play(attackSettings.stabAnimationHitObjectName);
                            }
                        }
                        //Play sound
                        if (attackSettings.stabHitObjectSound)
                        {
                            data.sounds.clip = attackSettings.stabHitObjectSound;
                            data.sounds.Play();
                        }
                    }

                    //Call network
                    if (pb.isServer) pb.RpcMeleeStabNetwork(3, 0);
                    //Play third person reload anim
                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(0, 3);

                    data.nextActionPossibleAt = Time.time + attackSettings.stabHitObjectTime;

                    yield return new WaitForSeconds(attackSettings.stabHitObjectTime);
                }

                if (pb && !pb.isBot)
                    data.startedRunAnimation = false;
            }
        }

        public IEnumerator MeleeExecuteCharge(Kit_ModernMeleeScript values, Kit_ModernMeleeScriptRuntimeData data, AttackSettings attackSettings, Kit_PlayerBehaviour pb, float revertBy)
        {
            //Relay to voice manager
            if (pb.voiceManager)
            {
                pb.voiceManager.MeleeUsed(pb, values.voiceMeleeSoundID);
            }

            if (attackSettings.chargeWindupAnimationName != "")
            {
                if (pb.isFirstPersonActive)
                {
                    if (attackSettings.chargeWindupAnimationName != "")
                    {
                        if (data.meleeRenderer.anim)
                        {
                            data.meleeRenderer.anim.Play(attackSettings.chargeWindupAnimationName);
                        }
                        else if (data.meleeRenderer.legacyAnim)
                        {
                            data.meleeRenderer.legacyAnim.Play(attackSettings.chargeWindupAnimationName);
                        }
                    }
                }

                data.nextActionPossibleAt = Time.time + attackSettings.chargeWindupTime + attackSettings.chargeHitTime + attackSettings.chargeMissTime;

                //Call network
                if (pb.isServer) pb.RpcMeleeChargeNetwork(1, 0);
                //Play third person reload anim
                pb.thirdPersonPlayerModel.PlayMeleeAnimation(1, 1);

                //Wait
                yield return new WaitForSeconds(attackSettings.chargeWindupTime);
            }

            if (pb)
            {
                //Set Lag Compensation
                Kit_IngameMain.instance.SetLagCompensationTo(revertBy);

                Vector3 center = pb.playerCameraTransform.position - (pb.playerCameraTransform.forward * (attackSettings.chargeReach / 2f));
                Vector3 dir = pb.playerCameraTransform.forward;

                RaycastHit[] hits = Physics.BoxCastAll(center, attackSettings.chargeHalfExtents, dir, Quaternion.LookRotation(dir), attackSettings.chargeReach, pb.weaponHitLayers.value, QueryTriggerInteraction.Collide).OrderBy(h => Vector3.Distance(pb.playerCameraTransform.position, h.point)).ToArray();

                //Reset
                Kit_IngameMain.instance.SetLagCompensationTo(0);

                int penetrationPowerLeft = attackSettings.chargePenetrationPower;
                //After penetration, only test for damage.
                bool penetratedOnce = false;

                //0 = Miss
                //1 = Hit Player
                //2 = Hit Object
                int result = 0;

                //Loop through all
                for (int i = 0; i < hits.Length; i++)
                {
                    //Check if we hits[i] ourselves
                    if (hits[i].transform.root != pb.transform.root)
                    {
                        //Check if we hits[i] a player
                        if (hits[i].transform.GetComponent<Kit_PlayerDamageMultiplier>() && Kit_IngameMain.instance.currentGameModeType == 2)
                        {
                            Kit_PlayerDamageMultiplier pdm = hits[i].transform.GetComponent<Kit_PlayerDamageMultiplier>();
                            if (hits[i].transform.root.GetComponent<Kit_PlayerBehaviour>())
                            {
                                Kit_PlayerBehaviour hitPb = hits[i].transform.root.GetComponent<Kit_PlayerBehaviour>();
                                //First check if we can actually damage that player
                                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.ArePlayersEnemies(pb, hitPb))
                                {
                                    //Check if he has spawn protection
                                    if (!hitPb.spawnProtection || hitPb.spawnProtection.CanTakeDamage(hitPb))
                                    {
                                        //Apply local damage, sample damage dropoff via distance
                                        if (pb.isServer)
                                        {
                                            hitPb.ServerDamage(Mathf.Lerp(attackSettings.chargeDamageStart, attackSettings.chargeDamageCharged, data.chargingProgress) * pdm.damageMultiplier, values.gameGunID, pb.transform.position, dir, attackSettings.chargeRagdollForce, hits[i].point, pdm.ragdollId, pb.isBot, pb.id);
                                            if (!pb.isBot) pb.TargetHitMarker(0);
                                        }
                                    }
                                    else if (pb.isServer)
                                    {
                                        //We hit a player but his spawn protection is active
                                        if (!pb.isBot) pb.TargetHitMarker(1);
                                    }
                                }

                                if (!penetratedOnce)
                                    result = 1;

                                if (hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>())
                                {
                                    Kit_MeleePenetrateableObject penetration = hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>();
                                    if (penetrationPowerLeft >= penetration.cost)
                                    {
                                        penetrationPowerLeft -= penetration.cost;
                                        penetratedOnce = true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    //Just end
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (!penetratedOnce)
                            {
                                if (pb.isServer)
                                {
                                    //Tell other players we hit something
                                    pb.ClientImpactProcess(hits[i].point, hits[i].normal, hits[i].collider.tag);
                                }

                                if (hits[i].collider.GetComponentInParent<IKitDamageable>() != null)
                                {
                                    if (hits[i].collider.GetComponentInParent<IKitDamageable>().LocalDamage(Mathf.Lerp(attackSettings.chargeDamageStart, attackSettings.chargeDamageCharged, data.chargingProgress), values.gameGunID, pb.transform.position, dir, attackSettings.chargeRagdollForce, hits[i].point, pb.isBot, pb.id))
                                    {
                                        if (pb.isServer)
                                        {
                                            if (!pb.isBot) pb.TargetHitMarker(0);
                                        }
                                    }
                                }
                            }

                            if (!penetratedOnce)
                                result = 2;

                            if (hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>())
                            {
                                Kit_MeleePenetrateableObject penetration = hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>();
                                if (penetrationPowerLeft >= penetration.cost)
                                {
                                    penetrationPowerLeft -= penetration.cost;
                                    penetratedOnce = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                //Just end
                                break;
                            }
                        }
                    }
                }

                if (result == 0)
                {
                    if (pb.isFirstPersonActive)
                    {
                        //Play animation
                        if (attackSettings.chargeAnimationMissName != "")
                        {
                            if (data.meleeRenderer.anim)
                            {
                                data.meleeRenderer.anim.Play(attackSettings.chargeAnimationMissName);
                            }
                            else if (data.meleeRenderer.legacyAnim)
                            {
                                data.meleeRenderer.legacyAnim.Play(attackSettings.chargeAnimationMissName);
                            }
                        }
                        //Play sound
                        if (attackSettings.chargeMissSound)
                        {
                            data.sounds.clip = attackSettings.chargeMissSound;
                            data.sounds.Play();
                        }
                    }

                    //Call network
                    if (pb.isServer) pb.RpcMeleeChargeNetwork(2, 0);
                    //Play third person reload anim
                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(1, 2);

                    data.nextActionPossibleAt = Time.time + attackSettings.chargeMissTime;

                    yield return new WaitForSeconds(attackSettings.chargeMissTime);
                }
                else if (result == 1)
                {
                    if (pb.isFirstPersonActive)
                    {
                        //Play animation
                        if (attackSettings.chargeAnimationHitName != "")
                        {
                            if (data.meleeRenderer.anim)
                            {
                                data.meleeRenderer.anim.Play(attackSettings.chargeAnimationHitName);
                            }
                            else if (data.meleeRenderer.legacyAnim)
                            {
                                data.meleeRenderer.legacyAnim.Play(attackSettings.chargeAnimationHitName);
                            }
                        }
                        //Play sound
                        if (attackSettings.chargeHitSound)
                        {
                            data.sounds.clip = attackSettings.chargeHitSound;
                            data.sounds.Play();
                        }
                    }

                    //Call network
                    if (pb.isServer) pb.RpcMeleeChargeNetwork(3, 0);
                    //Play third person reload anim
                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(1, 3);

                    data.nextActionPossibleAt = Time.time + attackSettings.chargeHitTime;

                    yield return new WaitForSeconds(attackSettings.chargeHitTime);
                }
                else if (result == 2)
                {
                    if (pb.isFirstPersonActive)
                    {
                        if (attackSettings.chargeAnimationHitObjectName != "")
                        {
                            //Play animation
                            if (data.meleeRenderer.anim)
                            {
                                data.meleeRenderer.anim.Play(attackSettings.chargeAnimationHitObjectName);
                            }
                            else if (data.meleeRenderer.legacyAnim)
                            {
                                data.meleeRenderer.legacyAnim.Play(attackSettings.chargeAnimationHitObjectName);
                            }
                        }
                        //Play sound
                        if (attackSettings.chargeHitObjectSound)
                        {
                            data.sounds.clip = attackSettings.chargeHitObjectSound;
                            data.sounds.Play();
                        }
                    }

                    //Call network
                    if (pb.isServer) pb.RpcMeleeChargeNetwork(4, 0);
                    //Play third person reload anim
                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(1, 4);

                    data.nextActionPossibleAt = Time.time + attackSettings.chargeHitObjectTime;

                    yield return new WaitForSeconds(attackSettings.chargeHitObjectTime);
                }

                data.chargingProgress = 0f;

                if (pb && !pb.isBot)
                    data.startedRunAnimation = false;
            }
        }

        public IEnumerator MeleeExecuteHeal(Kit_ModernMeleeScript values, Kit_ModernMeleeScriptRuntimeData data, AttackSettings attackSettings, Kit_PlayerBehaviour pb, float revertBy)
        {
            data.primaryHealingAmount--;

            if (pb.isFirstPersonActive)
            {
                //Play Animation
                if (data.meleeRenderer.anim)
                {
                    data.meleeRenderer.anim.Play(attackSettings.healAnimationName, 0, 0f);
                }
                else if (data.meleeRenderer.legacyAnim)
                {
                    data.meleeRenderer.legacyAnim.Play(attackSettings.healAnimationName);
                }

                //Play sound
                if (attackSettings.healSound)
                {
                    data.sounds.clip = attackSettings.healSound;
                    data.sounds.Play();
                }
            }

            data.nextActionPossibleAt = Time.time + attackSettings.healTimeOne + attackSettings.healTimeTwo;

            //Call network
            if (pb.isServer) pb.RpcMeleeHealNetwork(0);
            //Play third person reload anim
            pb.thirdPersonPlayerModel.PlayMeleeAnimation(2, 0);

            //Wait
            yield return new WaitForSeconds(attackSettings.healTimeOne);

            //Heal
            pb.vitalsManager.ApplyHeal(pb, attackSettings.healAmount);

            //Wait
            yield return new WaitForSeconds(attackSettings.healTimeTwo);
        }

        public IEnumerator MeleeExecuteCombo(Kit_ModernMeleeScript values, Kit_ModernMeleeScriptRuntimeData data, AttackSettings attackSettings, bool primary, Kit_PlayerBehaviour pb, float revertBy)
        {
            //Definie current combo
            int currentCombo = primary ? data.primaryComboCur : data.secondaryComboCur;

            //Relay to voice manager
            if (pb.voiceManager)
            {
                pb.voiceManager.MeleeUsed(pb, values.voiceMeleeSoundID);
            }

            if (attackSettings.combos[currentCombo].comboWindupAnimationName != "")
            {
                if (pb.isFirstPersonActive)
                {
                    if (attackSettings.combos[currentCombo].comboWindupAnimationName != "")
                    {
                        if (data.meleeRenderer.anim)
                        {
                            data.meleeRenderer.anim.Play(attackSettings.combos[currentCombo].comboWindupAnimationName);
                        }
                        else if (data.meleeRenderer.legacyAnim)
                        {
                            data.meleeRenderer.legacyAnim.Play(attackSettings.combos[currentCombo].comboWindupAnimationName);
                        }
                    }
                }

                //Call network
                if (pb.isServer) pb.RpcMeleeStabNetwork(0, 0);
                //Play third person reload anim
                pb.thirdPersonPlayerModel.PlayMeleeAnimation(0, 0);

                data.nextActionPossibleAt = Time.time + attackSettings.combos[currentCombo].comboWindupTime + attackSettings.combos[currentCombo].comboHitTime + attackSettings.combos[currentCombo].comboMissTime;

                //Wait
                yield return new WaitForSeconds(attackSettings.combos[currentCombo].comboWindupTime);
            }

            if (pb)
            {
                //Set Lag Compensation
                Kit_IngameMain.instance.SetLagCompensationTo(revertBy);

                Vector3 center = pb.playerCameraTransform.position - (pb.playerCameraTransform.forward * (attackSettings.combos[currentCombo].comboReach / 2f));
                Vector3 dir = pb.playerCameraTransform.forward;

                RaycastHit[] hits = Physics.BoxCastAll(center, attackSettings.combos[currentCombo].comboHalfExtents, dir, Quaternion.LookRotation(dir), attackSettings.combos[currentCombo].comboReach, pb.weaponHitLayers.value, QueryTriggerInteraction.Collide).OrderBy(h => Vector3.Distance(pb.playerCameraTransform.position, h.point)).ToArray();

                //Reset
                Kit_IngameMain.instance.SetLagCompensationTo(0);

                int penetrationPowerLeft = attackSettings.combos[currentCombo].comboPenetrationPower;
                //After penetration, only test for damage.
                bool penetratedOnce = false;

                //0 = Miss
                //1 = Hit Player
                //2 = Hit Object
                int result = 0;

                //Loop through all
                for (int i = 0; i < hits.Length; i++)
                {
                    //Check if we hits[i] ourselves
                    if (hits[i].transform.root != pb.transform.root)
                    {
                        //Check if we hits[i] a player
                        if (hits[i].transform.GetComponent<Kit_PlayerDamageMultiplier>() && Kit_IngameMain.instance.currentGameModeType == 2)
                        {
                            Kit_PlayerDamageMultiplier pdm = hits[i].transform.GetComponent<Kit_PlayerDamageMultiplier>();
                            if (hits[i].transform.root.GetComponent<Kit_PlayerBehaviour>())
                            {
                                Kit_PlayerBehaviour hitPb = hits[i].transform.root.GetComponent<Kit_PlayerBehaviour>();
                                //First check if we can actually damage that player
                                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.ArePlayersEnemies(pb, hitPb))
                                {
                                    //Check if he has spawn protection
                                    if (!hitPb.spawnProtection || hitPb.spawnProtection.CanTakeDamage(hitPb))
                                    {
                                        //Apply local damage, sample damage dropoff via distance
                                        if (pb.isServer)
                                        {
                                            hitPb.ServerDamage(attackSettings.combos[currentCombo].comboDamage * pdm.damageMultiplier, values.gameGunID, pb.transform.position, dir, attackSettings.combos[currentCombo].comboRagdollForce, hits[i].point, pdm.ragdollId, pb.isBot, pb.id);
                                            if (!pb.isBot) pb.TargetHitMarker(0);
                                        }
                                    }
                                    else if (pb.isServer)
                                    {
                                        //We hit a player but his spawn protection is active
                                        if (!pb.isBot) pb.TargetHitMarker(1);
                                    }
                                }

                                if (!penetratedOnce)
                                    result = 1;

                                if (hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>())
                                {
                                    Kit_MeleePenetrateableObject penetration = hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>();
                                    if (penetrationPowerLeft >= penetration.cost)
                                    {
                                        penetrationPowerLeft -= penetration.cost;
                                        penetratedOnce = true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    //Just end
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (!penetratedOnce)
                            {
                                if (pb.isServer)
                                {
                                    //Tell other players we hit something
                                    pb.ClientImpactProcess(hits[i].point, hits[i].normal, hits[i].collider.tag);
                                }

                                if (hits[i].collider.GetComponentInParent<IKitDamageable>() != null)
                                {
                                    if (hits[i].collider.GetComponentInParent<IKitDamageable>().LocalDamage(attackSettings.combos[currentCombo].comboDamage, values.gameGunID, pb.transform.position, dir, attackSettings.combos[currentCombo].comboRagdollForce, hits[i].point, pb.isBot, pb.id))
                                    {
                                        if (pb.isServer)
                                        {
                                            if (!pb.isBot) pb.TargetHitMarker(0);
                                        }
                                    }
                                }
                            }

                            if (!penetratedOnce)
                                result = 2;

                            if (hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>())
                            {
                                Kit_MeleePenetrateableObject penetration = hits[i].transform.GetComponent<Kit_MeleePenetrateableObject>();
                                if (penetrationPowerLeft >= penetration.cost)
                                {
                                    penetrationPowerLeft -= penetration.cost;
                                    penetratedOnce = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                //Just end
                                break;
                            }
                        }
                    }
                }

                //Wether we can proceed or not
                bool canProceed = false;

                if (result == 0)
                {
                    if (pb.isFirstPersonActive)
                    {
                        //Play animation
                        if (attackSettings.combos[currentCombo].comboAnimationMissName != "")
                        {
                            if (data.meleeRenderer.anim)
                            {
                                data.meleeRenderer.anim.Play(attackSettings.combos[currentCombo].comboAnimationMissName);
                            }
                            else if (data.meleeRenderer.legacyAnim)
                            {
                                data.meleeRenderer.legacyAnim.Play(attackSettings.combos[currentCombo].comboAnimationMissName);
                            }
                        }
                        //Play sound
                        if (attackSettings.combos[currentCombo].comboMissSound)
                        {
                            data.sounds.clip = attackSettings.combos[currentCombo].comboMissSound;
                            data.sounds.Play();
                        }
                    }

                    //Call network
                    if (pb.isServer) pb.RpcMeleeStabNetwork(1, 0);
                    //Play third person reload anim
                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(0, 1);

                    data.nextActionPossibleAt = Time.time + attackSettings.combos[currentCombo].comboMissTime;

                    //Set progress
                    canProceed = attackSettings.combos[currentCombo].canAchieveNextComboOnMiss;

                    yield return new WaitForSeconds(attackSettings.combos[currentCombo].comboMissTime);
                }
                else if (result == 1)
                {
                    if (pb.isFirstPersonActive)
                    {
                        //Play animation
                        if (attackSettings.combos[currentCombo].comboAnimationHitName != "")
                        {
                            if (data.meleeRenderer.anim)
                            {
                                data.meleeRenderer.anim.Play(attackSettings.combos[currentCombo].comboAnimationHitName);
                            }
                            else if (data.meleeRenderer.legacyAnim)
                            {
                                data.meleeRenderer.legacyAnim.Play(attackSettings.combos[currentCombo].comboAnimationHitName);
                            }
                        }
                        //Play sound
                        if (attackSettings.combos[currentCombo].comboHitSound)
                        {
                            data.sounds.clip = attackSettings.combos[currentCombo].comboHitSound;
                            data.sounds.Play();
                        }
                    }

                    //Call network
                    if (pb.isServer) pb.RpcMeleeStabNetwork(2, 0);
                    //Play third person reload anim
                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(0, 2);

                    data.nextActionPossibleAt = Time.time + attackSettings.combos[currentCombo].comboHitTime;

                    //Set progress
                    canProceed = attackSettings.combos[currentCombo].canAchieveNextComboOnHit;

                    yield return new WaitForSeconds(attackSettings.combos[currentCombo].comboHitTime);
                }
                else if (result == 2)
                {
                    if (pb.isFirstPersonActive)
                    {
                        //Play animation
                        if (attackSettings.combos[currentCombo].comboAnimationHitObjectName != "")
                        {
                            if (data.meleeRenderer.anim)
                            {
                                data.meleeRenderer.anim.Play(attackSettings.combos[currentCombo].comboAnimationHitObjectName);
                            }
                            else if (data.meleeRenderer.legacyAnim)
                            {
                                data.meleeRenderer.legacyAnim.Play(attackSettings.combos[currentCombo].comboAnimationHitObjectName);
                            }
                        }
                        //Play sound
                        if (attackSettings.combos[currentCombo].comboHitObjectSound)
                        {
                            data.sounds.clip = attackSettings.combos[currentCombo].comboHitObjectSound;
                            data.sounds.Play();
                        }
                    }

                    //Call network
                    if (pb.isServer) pb.RpcMeleeStabNetwork(3, 0);
                    //Play third person reload anim
                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(0, 3);

                    data.nextActionPossibleAt = Time.time + attackSettings.combos[currentCombo].comboHitObjectTime;

                    //Set progress
                    canProceed = attackSettings.combos[currentCombo].canAchieveNextComboOnHitObject;

                    yield return new WaitForSeconds(attackSettings.combos[currentCombo].comboHitObjectTime);
                }

                if (canProceed)
                {
                    if (primary)
                    {
                        if (attackSettings.combos.Length - 1 > currentCombo)
                        {
                            //Set time
                            data.primaryComboNextOnePossibleUntil = Time.time + attackSettings.combos[currentCombo].comboTimeForNextCombo;
                            //Advance combo
                            data.primaryComboCur = currentCombo + 1;
                        }
                        else
                        {
                            //Reset combo
                            data.primaryComboCur = 0;
                            data.primaryComboNextOnePossibleUntil = 0f;
                        }
                    }
                    else
                    {
                        if (attackSettings.combos.Length - 1 > currentCombo)
                        {
                            //Set time
                            data.secondaryComboNextOnePossibleUntil = Time.time + attackSettings.combos[currentCombo].comboTimeForNextCombo;
                            //Advance combo
                            data.secondaryComboCur = currentCombo + 1;
                        }
                        else
                        {
                            //Reset combo
                            data.secondaryComboCur = 0;
                            data.secondaryComboNextOnePossibleUntil = 0f;
                        }
                    }
                }

                if (pb && !pb.isBot)
                    data.startedRunAnimation = false;
            }
        }

        public IEnumerator WeaponBurstFire(Kit_ModernWeaponScript values, Kit_ModernWeaponScriptRuntimeData data, Kit_PlayerBehaviour pb, float delta, float revertBy, bool rpc)
        {
            int bulletsFired = 0;

            while (pb && values && data != null && bulletsFired < data.burstBulletsPerShot && data.bulletsLeft > 0)
            {
                //Fire
                bulletsFired++;

                values.FireOneShot(pb, data, delta, revertBy, rpc);

                yield return new WaitForSeconds(data.burstTimeBetweenShots);
            }
        }
    }
}
