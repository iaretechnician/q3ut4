using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this class to impement your own chat. It also includes a function to send a chat message properly. The master player controls it
    /// </summary>
    public abstract class Kit_ChatBase : MonoBehaviour
    {
        /// <summary>
        /// Displays a chat message. No checks required, they are done before by the master client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <param name="type">0 = Everyone; 1 = Team only</param>
        public abstract void DisplayChatMessage(Kit_Player sender, string message, byte type);

        /// <summary>
        /// Displays a chat message. No checks required, they are done before by the master client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <param name="type">0 = Everyone; 1 = Team only</param>
        public abstract void DisplayChatMessage(Kit_Bot sender, string message, byte type);

        /// <summary>
        /// A player left.
        /// </summary>
        /// <param name="player"></param>
        public abstract void PlayerLeft(Kit_Player player);

        /// <summary>
        /// A player joined
        /// </summary>
        /// <param name="player"></param>
        public abstract void PlayerJoined(Kit_Player player);

        /// <summary>
        /// A bot left.
        /// </summary>
        /// <param name="player"></param>
        public abstract void BotLeft(string botName);

        /// <summary>
        /// A bot joined
        /// </summary>
        /// <param name="player"></param>
        public abstract void BotJoined(string botName);

        /// <summary>
        /// Called when the pause menu was opened
        /// </summary>
        public virtual void PauseMenuOpened()
        {

        }

        /// <summary>
        /// Called when the pause menu was closed
        /// </summary>
        public virtual void PauseMenuClosed()
        {

        }

        /// <summary>
        /// Sends a chat message.
        /// </summary>
        /// <param name="content">What is the content of our chat message?</param>
        /// <param name="targets">0 = Everyone, 1 = Our team only (In team game modes)</param>
        public void SendChatMessage(string content, byte targets)
        {
            Kit_IngameMain.instance.CmdChatMessage(content, targets);
        }

        public void SendBotMessage(Kit_Bot botSender, string msg, byte type)
        {
            Kit_IngameMain.instance.RpcChatMessage(true, botSender.id, msg, type);
            Debug.Log("Bot message: " + botSender.name + ": " + msg);
        }
    }
}
