using MarsFPSKit;
using MarsFPSKit.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_FlashbangExplosion : MonoBehaviour
    {
        /// <summary>
        /// Explosion radius
        /// </summary>
        public float radius = 5f;
        /// <summary>
        /// Ragdoll force
        /// </summary>
        public float ragdollForce = 50f;
        /// <summary>
        /// Explosion layers
        /// </summary>
        public LayerMask layers;
        /// <summary>
        /// Layers of the linecast
        /// </summary>
        public LayerMask linecastLayers;
        /// <summary>
        /// Damage at the closest
        /// </summary>
        public float maxTime = 5f;
        /// <summary>
        /// Damage at the furthest
        /// </summary>
        public float minTime = 1f;
        /// <summary>
        /// How long until it is destroyed?
        /// </summary>
        public float liveTime = 5f;
        /// <summary>
        /// Source that plays sounds
        /// </summary>
        public AudioSource source;
        /// <summary>
        /// Explosion particle system!
        /// </summary>
        public ParticleSystem system;
        /// <summary>
        /// Clips to play
        /// </summary>
        public AudioClip[] clips;

        void Start()
        {
            source.clip = clips[Random.Range(0, clips.Length)];
            source.Play();
            system.Play(true);
            Collider[] affectedByExplosion = Physics.OverlapSphere(transform.position, radius, layers.value, QueryTriggerInteraction.Collide);
            for (int i = 0; i < affectedByExplosion.Length; i++)
            {
                if (!Physics.Linecast(transform.position, affectedByExplosion[i].transform.position, linecastLayers))
                {
                    if (affectedByExplosion[i].GetComponent<Rigidbody>())
                    {
                        if (affectedByExplosion[i].GetComponent<Kit_ExplosionRigidbody>())
                        {
                            Kit_ExplosionRigidbody body = affectedByExplosion[i].GetComponent<Kit_ExplosionRigidbody>();
                            StartCoroutine(ApplyExplosionForceNetworked(body, ragdollForce, transform.position, radius));
                        }
                        else
                        {
                            affectedByExplosion[i].GetComponent<Rigidbody>().AddExplosionForce(ragdollForce, transform.position, radius);
                        }
                    }
                }
            }
        }

        public IEnumerator ApplyExplosionForceNetworked(Kit_ExplosionRigidbody body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
        {
            float time = 0f;
            while (body && body.body.isKinematic && time < 1f)
            {
                time += Time.deltaTime;
                yield return null;
            }

            if (body && !body.body.isKinematic)
            {
                body.body.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
        }

        public void Explode(bool botShot, uint idWhoShot, int gunID)
        {
            List<Kit_PlayerBehaviour> blindedPlayers = new List<Kit_PlayerBehaviour>();

            Collider[] affectedByExplosion = Physics.OverlapSphere(transform.position, radius, layers.value, QueryTriggerInteraction.Collide);
            for (int i = 0; i < affectedByExplosion.Length; i++)
            {
                if (NetworkServer.active)
                {
                    if (affectedByExplosion[i].GetComponent<Kit_PlayerDamageMultiplier>())
                    {
                        Kit_PlayerDamageMultiplier adm = affectedByExplosion[i].GetComponent<Kit_PlayerDamageMultiplier>();
                        if (affectedByExplosion[i].transform.root.GetComponent<Kit_PlayerBehaviour>())
                        {
                            Kit_PlayerBehaviour player = affectedByExplosion[i].transform.root.GetComponent<Kit_PlayerBehaviour>();
                            if (!Physics.Linecast(transform.position, player.playerCameraTransform.position, linecastLayers))
                            {
                                if (!Kit_IngameMain.instance.currentPvPGameModeBehaviour || (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.ArePlayersEnemies(idWhoShot, botShot, player, true)))
                                {
                                    if (!blindedPlayers.Contains(player))
                                    {
                                        blindedPlayers.Add(player);
                                        //Blind that player muhahaha!!!
                                        player.ServerBlind(Mathf.SmoothStep(maxTime, minTime, Vector3.Distance(transform.position, adm.transform.position) / radius), gunID, transform.position, botShot, idWhoShot);

                                        if (!botShot)
                                        {
                                            Kit_Player nPlayer = Kit_NetworkPlayerManager.instance.GetPlayerById(idWhoShot);

                                            if (nPlayer != null)
                                            {
                                                Kit_IngameMain.instance.TargetHitmarker(nPlayer.serverToClientConnection, 0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Destroy(gameObject, liveTime);
        }
    }
}