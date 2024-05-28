using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_Domination_FlagRuntime : NetworkBehaviour
    {
        [SyncVar]
        /// <summary>
        /// Who currently owns this flag?
        /// </summary>
        public int currentOwner;
        [SyncVar]
        /// <summary>
        /// What is currently happening with this flag?
        /// -1 = multiple teams are capping
        /// 0 = nothing
        /// 1 + x = capture process (team 0 + x)
        /// </summary>
        public int currentState;
        [SyncVar]
        /// <summary>
        /// Progress of capturing this flag
        /// </summary>
        public float captureProgress = 0f;

        /// <summary>
        /// Smoothed <see cref="captureProgress"/>
        /// </summary>
        public float smoothedCaptureProgress = 0f;
        [SyncVar]
        /// <summary>
        /// If one team is capturing the flag, this is how many they are (min = 1)
        /// </summary>
        public int playersCapturingFlag = 1;

        /// <summary>
        /// Spawn points for this flag
        /// </summary>
        public List<Kit_PlayerSpawn> spawnForFlag = new List<Kit_PlayerSpawn>();

        /// <summary>
        /// Nav points for capturing this flag!
        /// </summary>
        public List<Kit_BotNavPoint> navPointsForFlag = new List<Kit_BotNavPoint>();

        /// <summary>
        /// Position for the UI
        /// </summary>
        public Transform uiPosition;

        /// <summary>
        /// This is the renderer of the flag, where ownership materials will be applied
        /// </summary>
        public Renderer flagRenderer;

        /// <summary>
        /// Cloth object of flag
        /// </summary>
        public Cloth flagCloth;

        /// <summary>
        /// Minimap
        /// </summary>
        public SpriteRenderer minimapIcon;
        [SyncVar]
        public Vector3 externalAcceleration;
        [SyncVar]
        public Vector3 randomAcceleration;

        #region Runtime
        public List<Kit_PlayerBehaviour> playersInTrigger = new List<Kit_PlayerBehaviour>();
        #endregion

        public void Setup(Kit_Domination_Flag flag)
        {
            //Copy acceleration
            externalAcceleration = flag.externalAcceleration;
            randomAcceleration = flag.randomAcceleration;
        }

        public override void OnStartServer()
        {
            FindObjectOfType<Kit_PvP_GMB_DominationNetworkData>().flags.Add(this);
        }

        public override void OnStartClient()
        {
            var gameModeData = FindObjectOfType<Kit_PvP_GMB_DominationNetworkData>();

            if (!gameModeData.flags.Contains(this))
            {
                gameModeData.flags.Add(this);
            }

            //Copy acceleration
            flagCloth.externalAcceleration = externalAcceleration;
            flagCloth.randomAcceleration = randomAcceleration;
        }

        /// <summary>
        /// The owner of this flag changed
        /// </summary>
        /// <param name="newOwner"></param>
        public void UpdateFlag(int owner, Kit_PvP_GMB_Domination gameMode)
        {
            //Change material accordingly
            if (owner == 0)
            {
                flagRenderer.sharedMaterial = gameMode.flagMaterialNeutral;
            }
            else
            {
                flagRenderer.sharedMaterial = gameMode.flagMaterialTeams[owner - 1];
            }
        }

        #region Unity Calls
        void OnTriggerEnter(Collider other)
        {
            //First clean list
            playersInTrigger = playersInTrigger.Where(item => item != null).ToList();

            Kit_PlayerBehaviour pb = other.transform.root.GetComponent<Kit_PlayerBehaviour>();
            if (pb)
            {
                if (!playersInTrigger.Contains(pb)) playersInTrigger.Add(pb);
                //Tell game mode something changed
                (Kit_IngameMain.instance.currentPvPGameModeBehaviour as Kit_PvP_GMB_Domination).FlagStateChanged(this);
            }
        }

        public void PlayerDied()
        {
            //First clean list
            playersInTrigger = playersInTrigger.Where(item => item != null).ToList();
            //Tell game mode something changed
            (Kit_IngameMain.instance.currentPvPGameModeBehaviour as Kit_PvP_GMB_Domination).FlagStateChanged(this);
        }

        void OnTriggerExit(Collider other)
        {
            //First clean list
            playersInTrigger = playersInTrigger.Where(item => item != null).ToList();

            Kit_PlayerBehaviour pb = other.transform.root.GetComponent<Kit_PlayerBehaviour>();
            if (pb)
            {
                if (playersInTrigger.Contains(pb)) playersInTrigger.Remove(pb);
                //Tell game mode something changed
                (Kit_IngameMain.instance.currentPvPGameModeBehaviour as Kit_PvP_GMB_Domination).FlagStateChanged(this);
            }
        }
        #endregion
    }
}
