using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarsFPSKit.Networking;
using Mirror;

namespace MarsFPSKit
{
    public class Kit_KillFeedEntry : MonoBehaviour
    {
        /// <summary>
        /// How long is this entry going to be visible?
        /// </summary>
        public float lifeTime = 4f;
        /// <summary>
        ///  How long is the fade out going to take?
        /// </summary>
        public float fadeOutTime = 1f;
        /// <summary>
        /// When was this entry created?
        /// </summary>
        private float timeOfAppearance;

        /// <summary>
        /// Displays the killer's name
        /// </summary>
        public TextMeshProUGUI killerText;
        /// <summary>
        /// Is being used when no icon is assigned
        /// </summary>
        public TextMeshProUGUI weaponText;
        /// <summary>
        /// This is preferred. Weapon Icon
        /// </summary>
        public Image weaponImage;
        /// <summary>
        /// If an entry is configured for this ragdoll ID, then this is displayed here
        /// </summary>
        public Image playerModelImage;
        /// <summary>
        /// Displays the name of the guy who was killed!
        /// </summary>
        public TextMeshProUGUI killedText;
        /// <summary>
        /// CanvasGroup for alpha change, so the text in <see cref="txt"/> can be colorful
        /// </summary>
        public CanvasGroup cg;

        public void SetUp(bool botKiller, uint killer, bool botKilled, uint killed, int gun, int playerModel, int ragdollId, Kit_KillFeedManager kfm)
        {
            timeOfAppearance = Time.time;

            string killerName = "";
            string killedName = "";

            //Determine their teams
            int killerTeam = -1;
            int killedTeam = -1;

            if (botKiller)
            {
                if (Kit_IngameMain.instance.currentBotManager)
                {
                    Kit_Bot killerBot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(killer);
                    if (killerBot != null)
                    {
                        killerName = killerBot.name;
                        killerTeam = killerBot.team;
                    }

                    if (Kit_IngameMain.instance.isServer && Kit_IngameMain.instance.currentBotManager.IsBotAlive(killerBot))
                    {
                        Kit_PlayerBehaviour pb = Kit_IngameMain.instance.currentBotManager.GetAliveBot(killerBot);
                        if (pb.voiceManager)
                        {
                            pb.voiceManager.EnemyKilled(pb);
                        }
                    }
                }
            }
            else
            {
                Kit_Player playerKiller = Kit_NetworkPlayerManager.instance.GetPlayerById(killer);

                if (playerKiller != null)
                {
                    killerName = playerKiller.name;
                    killerTeam = playerKiller.team;

                    //Play enemy killed voice
                    if (playerKiller.isLocal)
                    {
                        if (Kit_IngameMain.instance.myPlayer && Kit_IngameMain.instance.myPlayer.voiceManager)
                        {
                            Kit_IngameMain.instance.myPlayer.voiceManager.EnemyKilled(Kit_IngameMain.instance.myPlayer);
                        }
                    }
                }
            }

            if (botKilled)
            {
                if (Kit_IngameMain.instance.currentBotManager)
                {
                    Kit_Bot killedBot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(killed);
                    killedName = killedBot.name;
                    killedTeam = killedBot.team;
                }
            }
            else
            {
                Kit_Player playerKilled = Kit_NetworkPlayerManager.instance.GetPlayerById(killed);

                if (playerKilled != null)
                {
                    killedName = playerKilled.name;
                    killedTeam = playerKilled.team;
                }
            }

            //Determine the correct color
            Color killerColor = Kit_IngameMain.instance.gameInformation.allPvpTeams[killerTeam].teamColor;
            Color killedColor = Kit_IngameMain.instance.gameInformation.allPvpTeams[killedTeam].teamColor;

            //Hide this here so that we only have to unhide it later
            playerModelImage.gameObject.SetActive(false);

            //If we were killed with a normal weapon, get it from the game information
            if (gun >= 0)
            {
                if (Kit_IngameMain.instance.gameInformation.allWeapons[gun].weaponKillfeedImage)
                {
                    weaponImage.sprite = Kit_IngameMain.instance.gameInformation.allWeapons[gun].weaponKillfeedImage;
                    weaponText.gameObject.SetActive(false);
                    weaponImage.gameObject.SetActive(true);
                }
                else
                {
                    weaponText.text = " [" + Kit_IngameMain.instance.gameInformation.allWeapons[gun].weaponName + "]";
                    weaponText.gameObject.SetActive(true);
                    weaponImage.gameObject.SetActive(false);
                }

                //Check for player model config
                Kit_PlayerModelInformation pm = Kit_IngameMain.instance.gameInformation.allPvpTeams[killedTeam].playerModels[playerModel];

                for (int i = 0; i < pm.killFeedConfig.Length; i++)
                {
                    if (pm.killFeedConfig[i].idAtWhichToAppear == ragdollId)
                    {
                        playerModelImage.sprite = pm.killFeedConfig[i].toShow;
                        playerModelImage.gameObject.SetActive(true);
                        break;
                    }
                }
            }
            else
            {
                //Else check what caused it
                if (gun == -1)
                {
                    weaponText.text = " [Environment]";
                }
                else if (gun == -2)
                {
                    weaponText.text = " [Fall Damage]";
                }
                else if (gun == -3)
                {
                    weaponText.text = " [Suicide]";
                }

                //Show text
                weaponText.gameObject.SetActive(true);
                //Hide others
                weaponImage.gameObject.SetActive(false);
            }

            //Display killer's name
            killerText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(killerColor) + ">" + killerName + "</color>";
            //Display killed's name
            killedText.text = " <color=#" + ColorUtility.ToHtmlStringRGB(killedColor) + ">" + killedName + "</color>";
            //Update Layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            Canvas.ForceUpdateCanvases();

            enabled = true;
        }

        public void SetUp(bool botKiller, uint killer, bool botKilled, uint killed, string cause, int playerModel, int ragdollId, Kit_KillFeedManager kfm)
        {
            timeOfAppearance = Time.time;

            string killerName = "";
            string killedName = "";

            //Determine their teams
            int killerTeam = -1;
            int killedTeam = -1;

            if (botKiller)
            {
                if (Kit_IngameMain.instance.currentBotManager)
                {
                    Kit_Bot killerBot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(killer);
                    if (killerBot != null)
                    {
                        killerName = killerBot.name;
                        killerTeam = killerBot.team;
                    }
                    if (NetworkServer.active && Kit_IngameMain.instance.currentBotManager.IsBotAlive(killerBot))
                    {
                        Kit_PlayerBehaviour pb = Kit_IngameMain.instance.currentBotManager.GetAliveBot(killerBot);
                        if (pb.voiceManager)
                        {
                            pb.voiceManager.EnemyKilled(pb);
                        }
                    }
                }
            }
            else
            {
                Kit_Player playerKiller = Kit_NetworkPlayerManager.instance.GetPlayerById(killer);
                if (playerKiller != null)
                {
                    killerName = playerKiller.name;
                    killerTeam = playerKiller.team;

                    //Play enemy killed voice
                    if (playerKiller.isLocal)
                    {
                        if (Kit_IngameMain.instance.myPlayer && Kit_IngameMain.instance.myPlayer.voiceManager)
                        {
                            Kit_IngameMain.instance.myPlayer.voiceManager.EnemyKilled(Kit_IngameMain.instance.myPlayer);
                        }
                    }
                }
            }

            if (botKilled)
            {
                if (Kit_IngameMain.instance.currentBotManager)
                {
                    Kit_Bot killedBot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(killed);
                    killedName = killedBot.name;
                    killedTeam = killedBot.team;
                }
            }
            else
            {
                Kit_Player playerKilled = Kit_NetworkPlayerManager.instance.GetPlayerById(killed);

                if (playerKilled != null)
                {
                    killedName = playerKilled.name;
                    killedTeam = playerKilled.team;
                }
            }

            //Determine the correct color
            Color killerColor = Kit_IngameMain.instance.gameInformation.allPvpTeams[killerTeam].teamColor;
            Color killedColor = Kit_IngameMain.instance.gameInformation.allPvpTeams[killedTeam].teamColor;

            //Display killer's name
            killerText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(killerColor) + ">" + killerName + "</color>";
            //Display killed's name
            killedText.text = " <color=#" + ColorUtility.ToHtmlStringRGB(killedColor) + ">" + killedName + "</color>";

            //Hide these
            playerModelImage.gameObject.SetActive(false);
            weaponImage.gameObject.SetActive(false);
            //Use text
            weaponText.text = " [" + cause + "]";
            weaponText.gameObject.SetActive(true);

            //Update Layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            Canvas.ForceUpdateCanvases();

            enabled = true;
        }

        void Update()
        {
            if (Time.time > timeOfAppearance + lifeTime)
            {
                cg.alpha = Mathf.Clamp01(timeOfAppearance + lifeTime + fadeOutTime - Time.time);

                if (Time.time > timeOfAppearance + lifeTime + fadeOutTime)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                cg.alpha = 1f;
            }
        }
    }
}
