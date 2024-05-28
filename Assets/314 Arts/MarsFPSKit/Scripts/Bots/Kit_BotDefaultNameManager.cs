using UnityEngine;

namespace MarsFPSKit
{
    [CreateAssetMenu(menuName = "MarsFPSKit/Bots/Name Manager/Default")]
    public class Kit_BotDefaultNameManager : Kit_BotNameManager
    {
        /// <summary>
        /// Bot names to choose from
        /// </summary>
        public string[] botNames;

        public override string GetRandomName(Kit_BotManager bm)
        {
            if (botNames.Length <= 0) throw new System.Exception("No bot names to choose from!");
            return botNames[Random.Range(0, botNames.Length)];
        }
    }
}
