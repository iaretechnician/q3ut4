using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This implements a COD Style minimap
    /// </summary>
    public class Kit_MinimapDefault : Kit_MinimapBase
    {
        /// <summary>
        /// This is the camera used for the minimap
        /// </summary>
        public Camera minimapCamera;
        /// <summary>
        /// This is where the camera will be when its parented on the player
        /// </summary>
        public Vector3 minimapCameraRelativeToPlayer = new Vector3(0f, 50f, 0f);

        /// <summary>
        /// This is used to rotate the compass
        /// </summary>
        public RectTransform minimapCompass;

        /// <summary>
        /// Per 90° of rotation (player), how much should the compass move?
        /// </summary>
        public float movePerDirection = 160f;

        /// <summary>
        /// This is used for friendly players
        /// </summary>
        [Header("Prefabs")]
        public GameObject friendlyPlayerPrefab;
        /// <summary>
        /// This is used for enemy players
        /// </summary>
        public GameObject enemyPlayerPrefab;

        public Dictionary<Kit_PlayerBehaviour, Kit_MinimapMarker> activePlayers = new Dictionary<Kit_PlayerBehaviour, Kit_MinimapMarker>();

        public override void LocalPlayerSwitchedTeams()
        {
            List<Kit_PlayerBehaviour> newSpawns = new List<Kit_PlayerBehaviour>();
            //Delte all Markers
            foreach (KeyValuePair<Kit_PlayerBehaviour, Kit_MinimapMarker> mm in activePlayers)
            {
                if (mm.Value)
                {
                    Destroy(mm.Value.gameObject);
                    newSpawns.Add(mm.Key);
                }
            }
            //Reset
            activePlayers = new Dictionary<Kit_PlayerBehaviour, Kit_MinimapMarker>();
            //Respawn all
            for (int i = 0; i < newSpawns.Count; i++)
            {
                PlayerSpawned(newSpawns[i]);
            }
        }

        public override void LocalPlayerDied(Kit_PlayerBehaviour pb)
        {
            //Unparent camera
            minimapCamera.transform.parent = null;
            //Disable camera
            minimapCamera.enabled = false;
        }

        public override void LocalPlayerSpawned(Kit_PlayerBehaviour pb)
        {
            //Parent camera
            minimapCamera.transform.parent = pb.transform;
            //Move
            minimapCamera.transform.localPosition = minimapCameraRelativeToPlayer;
            //Rotate
            minimapCamera.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            //Enable
            minimapCamera.enabled = true;
        }

        public override void LocalPlayerUpdate(Kit_PlayerBehaviour pb)
        {
            Vector3 vector = minimapCompass.localPosition;
            vector.x = -(pb.transform.localEulerAngles.y / 90f) * movePerDirection;
            minimapCompass.localPosition = vector;
        }

        public override void PlayerDied(Kit_PlayerBehaviour pb)
        {
            if (pb)
            {
                if (activePlayers.ContainsKey(pb))
                {
                    //Destroy sprite
                    Destroy(activePlayers[pb].gameObject);
                    //Remove from dictionary
                    activePlayers.Remove(pb);
                }
            }
        }

        public override void PlayerFriendlyUpdate(Kit_PlayerBehaviour pb)
        {

        }

        public override void PlayerShoots(Kit_PlayerBehaviour pb, bool silenced)
        {
            if (pb)
            {
                if (!silenced)
                {
                    if (activePlayers.ContainsKey(pb))
                    {
                        //Check if we're enemies
                        if (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.AreWeEnemies(pb.isBot, pb.id))
                        {
                            activePlayers[pb].EnemyShot();
                        }
                    }
                }
            }
        }

        public override void PlayerSpawned(Kit_PlayerBehaviour pb)
        {
            if (pb)
            {
                if (!activePlayers.ContainsKey(pb))
                {
                    //Check if player is enemy or friendly
                    if (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.AreWeEnemies(pb.isBot, pb.id))
                    {
                        GameObject newPlayersIcons = Instantiate(enemyPlayerPrefab, pb.transform);
                        //Get Renderer
                        Kit_MinimapMarker marker = newPlayersIcons.GetComponent<Kit_MinimapMarker>();
                        //Setup
                        marker.Setup(Kit_MinimapMarker.MarkerMode.Enemy, pb);
                        //Add to dicitonary
                        activePlayers.Add(pb, marker);
                    }
                    else
                    {
                        GameObject newPlayersIcons = Instantiate(friendlyPlayerPrefab, pb.transform);
                        //Get Renderer
                        Kit_MinimapMarker marker = newPlayersIcons.GetComponent<Kit_MinimapMarker>();
                        //Setup
                        marker.Setup(Kit_MinimapMarker.MarkerMode.Friendly, pb);
                        //Add to dicitonary
                        activePlayers.Add(pb, marker);
                    }
                }
            }
        }

        public override void Setup()
        {
            //Disable camera
            minimapCamera.enabled = false;
        }
    }
}