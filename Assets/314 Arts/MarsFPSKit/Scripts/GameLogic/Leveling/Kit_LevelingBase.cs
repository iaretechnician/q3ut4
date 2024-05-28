using UnityEngine;
using MarsFPSKit.UI;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this to implement your own leveling
    /// </summary>
    public abstract class Kit_LevelingBase : ScriptableObject
    {
        /// <summary>
        /// These are the icons used to display the level up. There should be as many as there are levels.
        /// </summary>
        public Sprite[] levelUpIcons;

        /// <summary>
        /// Called at the beginning of the game
        /// </summary>
        public abstract void Initialize(Kit_MenuManager menu);

        /// <summary>
        /// Called at certain points in the game where leveling data should be saved
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Adds XP
        /// </summary>
        /// <param name="xp"></param>
        public abstract void AddXp(int xp);

        /// <summary>
        /// Returns the current level
        /// </summary>
        /// <returns></returns>
        public abstract int GetLevel();

        /// <summary>
        /// Returns Max level
        /// </summary>
        /// <returns></returns>
        public abstract int GetMaxLevel();

        /// <summary>
        /// Returns progress to next level
        /// </summary>
        /// <returns></returns>
        public abstract float GetPercentageToNextLevel();
    }
}
