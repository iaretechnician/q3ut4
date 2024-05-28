using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this to implement your own in game leveling UI
    /// </summary>
    public abstract class Kit_LevelingUIBase : MonoBehaviour
    {
        /// <summary>
        /// Called when we achieved a level up in game
        /// </summary>
        /// <param name="newLevel"></param>
        public abstract void DisplayLevelUp(int newLevel);

        /// <summary>
        /// Display something else.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="txt"></param>
        public abstract void DisplayOther(Sprite sprite, string txt);
    }
}
