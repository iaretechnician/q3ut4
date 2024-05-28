using MarsFPSKit.Weapons;
using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    [System.Serializable]
    public class WeaponToSpawn
    {
        /// <summary>
        /// The ID of the weapon to spawn
        /// </summary>
        public int weaponID = 0;
        /// <summary>
        /// The attachments that this weapon will spawn with
        /// </summary>
        public int[] attachmentsOfThisWeapon = new int[1];

        /// <summary>
        /// The amount of bullets this weapon will spawn with
        /// </summary>
        public int bulletsLeft = 30;
        /// <summary>
        /// The amount of bullets left to reload this weapon will spawn with
        /// </summary>
        public int bulletsLeftToReload = 60;
    }

    public enum WeaponSpawnType { Once, RespawnAfterTaken, RespawnAfterTime }

    /// <summary>
    /// This script will spawn weapons if attached to an object with a photonview
    /// </summary>
    public class Kit_WeaponSpawner : MonoBehaviour
    {
        /// <summary>
        /// Drop prefab
        /// </summary>
        public GameObject dropPrefab;

        /// <summary>
        /// List of weapons that could spawn here
        /// </summary>
        public WeaponToSpawn[] weaponsToSpawn = new WeaponToSpawn[0];

        /// <summary>
        /// Respawn type of this weapon.
        /// </summary>
        public WeaponSpawnType spawnType = WeaponSpawnType.Once;

        /// <summary>
        /// If <see cref="spawnType"/> is set  to <see cref="WeaponSpawnType.RespawnAfterTaken"/> or <see cref="WeaponSpawnType.RespawnAfterTime"/>, this is used
        /// </summary>
        public float respawnTime = 10f;

        /// <summary>
        /// At which time will this weapon be respawned next
        /// </summary>
        private double nextRespawnTime;

        /// <summary>
        /// Was this weapon spawned already?
        /// </summary>
        private bool wasWeaponSpawned;

        [HideInInspector]
        public Kit_DropBehaviour currentlySpawnedWeapon;

        void Start()
        {
            if (weaponsToSpawn.Length <= 0)
            {
                Debug.LogError("Weapon spawner (" + this + ") has no weapons assigned", this);
                return;
            }

            if (NetworkServer.active)
            {
                if (!currentlySpawnedWeapon && !wasWeaponSpawned)
                {
                    //Spawn Random weapon
                    SpawnWeapon(Random.Range(0, weaponsToSpawn.Length));

                    //If we only want to spawn once, set bool
                    if (spawnType == WeaponSpawnType.Once)
                    {
                        wasWeaponSpawned = true;
                    }
                    else if (spawnType == WeaponSpawnType.RespawnAfterTime)
                    {
                        nextRespawnTime = NetworkTime.time + respawnTime;
                    }
                    else if (spawnType == WeaponSpawnType.RespawnAfterTaken)
                    {
                        //Set time
                        nextRespawnTime = NetworkTime.time + respawnTime;
                    }
                }
            }
        }

        void Update()
        {
            if (NetworkServer.active)
            {
                if (weaponsToSpawn.Length > 0)
                {
                    if (spawnType == WeaponSpawnType.RespawnAfterTime)
                    {
                        if (NetworkTime.time > nextRespawnTime)
                        {
                            //Destroy old
                            if (currentlySpawnedWeapon)
                            {
                                NetworkServer.Destroy(currentlySpawnedWeapon.gameObject);
                            }

                            //Spawn new
                            SpawnWeapon(Random.Range(0, weaponsToSpawn.Length));

                            //Set time
                            nextRespawnTime = NetworkTime.time + respawnTime;
                        }
                    }
                    else if (spawnType == WeaponSpawnType.RespawnAfterTaken)
                    {
                        if (currentlySpawnedWeapon)
                        {
                            //Set time
                            nextRespawnTime = NetworkTime.time + respawnTime;
                        }
                        else
                        {
                            if (NetworkTime.time > nextRespawnTime)
                            {
                                //Spawn new
                                SpawnWeapon(Random.Range(0, weaponsToSpawn.Length));

                                //Set time
                                nextRespawnTime = NetworkTime.time + respawnTime;
                            }
                        }
                    }
                }
            }
        }

        void SpawnWeapon(int id)
        {
            if (NetworkServer.active)
            {
                if (Kit_IngameMain.instance.gameInformation.allWeapons[weaponsToSpawn[id].weaponID].dropPrefab)
                {
                    GameObject go = Instantiate(dropPrefab, transform.position, transform.rotation);
                    Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                    dp.isSceneOwned = true;
                    dp.weaponID = weaponsToSpawn[id].weaponID;
                    dp.bulletsLeft = weaponsToSpawn[id].bulletsLeft;
                    dp.bulletsLeftToReload = weaponsToSpawn[id].bulletsLeftToReload;

                    for (int i = 0; i < weaponsToSpawn[id].attachmentsOfThisWeapon.Length; i++)
                    {
                        dp.attachments.Add(weaponsToSpawn[id].attachmentsOfThisWeapon[i]);
                    }

                    NetworkServer.Spawn(go);

                    currentlySpawnedWeapon = dp;
                }
            }
        }

        /// <summary>
        /// Should be called by game mode scripts when game was started in the middle!
        /// </summary>
        public void GameModeBeginMiddle()
        {
            if (NetworkServer.active)
            {
                //Destroy if necessary
                if (currentlySpawnedWeapon)
                {
                    NetworkServer.Destroy(currentlySpawnedWeapon.gameObject);
                }

                //Spawn Random weapon
                SpawnWeapon(Random.Range(0, weaponsToSpawn.Length));

                //If we only want to spawn once, set bool
                if (spawnType == WeaponSpawnType.Once)
                {
                    wasWeaponSpawned = true;
                }
                else if (spawnType == WeaponSpawnType.RespawnAfterTime)
                {
                    nextRespawnTime = NetworkTime.time + respawnTime;
                }
                else if (spawnType == WeaponSpawnType.RespawnAfterTaken)
                {
                    //Set time
                    nextRespawnTime = NetworkTime.time + respawnTime;
                }
            }
        }
    }
}