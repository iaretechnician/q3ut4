using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    public class OutOfMapPlayer
    {
        /// <summary>
        /// Player that is out of map :)
        /// </summary>
        public Kit_PlayerBehaviour pb;

        /// <summary>
        /// How much lifetime does he have left?
        /// </summary>
        public float timeLeft;
    }

    /// <summary>
    /// Add this to a trigger in order to display "you are leaving the battlefield" and kill the player after X seconds
    /// </summary>
    public class Kit_OutOfMapTrigger : MonoBehaviour
    {
        [Range(1f, 60f)]
        /// <summary>
        /// How much time until death?
        /// </summary>
        public float timeUntilDeath = 10f;
        /// <summary>
        /// Death sound category to use
        /// </summary>
        public int deathSoundCategory;
        /// <summary>
        /// These are the players that are out of map :)
        /// </summary>
        private List<OutOfMapPlayer> playersOutOfMap = new List<OutOfMapPlayer>();

        private void OnTriggerEnter(Collider other)
        {
            Kit_PlayerBehaviour pb = other.transform.root.GetComponent<Kit_PlayerBehaviour>();
            if (pb)
            {
                //Check if he is part of this
                for (int i = 0; i < playersOutOfMap.Count; i++)
                {
                    if (playersOutOfMap[i].pb == pb)
                    {
                        break;
                    }
                }

                //He is not, add him!
                OutOfMapPlayer oomp = new OutOfMapPlayer();
                oomp.pb = pb;
                oomp.timeLeft = timeUntilDeath;
                //Add
                playersOutOfMap.Add(oomp);

                if (pb.isLocalPlayer && !pb.isBot)
                {
                    //Update hud in order to show it
                    Kit_IngameMain.instance.hud.DisplayLeavingBattlefield(timeUntilDeath);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Kit_PlayerBehaviour pb = other.transform.root.GetComponent<Kit_PlayerBehaviour>();
            if (pb)
            {
                for (int i = 0; i < playersOutOfMap.Count; i++)
                {
                    if (playersOutOfMap[i].pb == pb)
                    {
                        if (playersOutOfMap[i].pb.isLocalPlayer && !playersOutOfMap[i].pb.isBot)
                        {
                            //Update hud in order to hide it
                            Kit_IngameMain.instance.hud.DisplayLeavingBattlefield(-1);
                        }

                        //Remove that
                        playersOutOfMap.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void Update()
        {
            //Reverse loop
            for (int i = playersOutOfMap.Count - 1; i >= 0; i--)
            {
                if (playersOutOfMap[i].pb)
                {
                    //Decrease time
                    if (playersOutOfMap[i].timeLeft > 0)
                        playersOutOfMap[i].timeLeft -= Time.deltaTime;

                    if (NetworkServer.active)
                    {
                        //Check if time ran out
                        if (playersOutOfMap[i].timeLeft <= 0)
                        {
                            //Kill him if it's ours
                            playersOutOfMap[i].pb.vitalsManager.ApplyEnvironmentalDamage(playersOutOfMap[i].pb, 1000, deathSoundCategory);
                        }
                    }

                    if (playersOutOfMap[i].pb.isLocalPlayer && !playersOutOfMap[i].pb.isBot)
                    {
                        //Update hud
                        Kit_IngameMain.instance.hud.DisplayLeavingBattlefield(playersOutOfMap[i].timeLeft);
                    }
                }
                else
                {
                    //Remove
                    playersOutOfMap.RemoveAt(i);
                }
            }
        }
    }
}