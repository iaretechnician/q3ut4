using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Leveling/PlayerPrefs based")]
        public class Kit_LevelingPlayerPrefs : Kit_LevelingBase
        {
            [Header("Settings")]
            public int maxLevel = 20;

            /// <summary>
            /// How much xp do we need for the first level up (lvl 1 to lvl 2)
            /// </summary>
            public int xpNeededForLevelTwo = 500;
            /// <summary>
            /// How much xp do we need to jump from maxlevel-1 to maxlevel?
            /// </summary>
            public int xpNeededForMaxLevel = 20000;

            public int currentLevel = 1;
            public int currentXp;

            public int[] xpNeeded;

            public override void AddXp(int xp)
            {
                currentXp += xp;
                RecalculateLevelWithLevelUp();
            }

            public override int GetLevel()
            {
                return currentLevel;
            }

            public override int GetMaxLevel()
            {
                return maxLevel;
            }

            public override float GetPercentageToNextLevel()
            {
                if (currentLevel >= maxLevel) return 1f;
                else
                {
                    return (float)currentXp / xpNeeded[Mathf.Clamp(currentLevel, 0, xpNeeded.Length - 1)];
                }
            }

            public override void Initialize(Kit_MenuManager menu)
            {
                //Calculate XP needed
                xpNeeded = new int[maxLevel - 1];
                for (int i = 0; i < maxLevel - 1; i++)
                {
                    xpNeeded[i] = Mathf.RoundToInt(Mathf.Lerp(xpNeededForLevelTwo, xpNeededForMaxLevel, (float)i / (maxLevel - 2)));
                }
                //Load XP
                currentXp = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_xp", 0);
                //Recalc Level
                RecalculateLevel();
            }

            private void RecalculateLevel()
            {
                int newLvl = 0;

                for (int i = 0; i < xpNeeded.Length; i++)
                {
                    if (currentXp > xpNeeded[i]) newLvl++;
                    else break;
                }

                if (newLvl + 1 > currentLevel)
                {
                    Save();
                }

                currentLevel = newLvl + 1;
            }

            private void RecalculateLevelWithLevelUp()
            {
                int newLvl = 0;

                for (int i = 0; i < xpNeeded.Length; i++)
                {
                    if (currentXp >= xpNeeded[i]) newLvl++;
                    else break;
                }

                if (newLvl + 1 > currentLevel)
                {
                    //Level up!
                    if (Kit_IngameMain.instance.levelingUi)
                    {
                        Kit_IngameMain.instance.levelingUi.DisplayLevelUp(newLvl + 1);
                    }
                    Save();
                }

                currentLevel = newLvl + 1;
            }

            public override void Save()
            {
                PlayerPrefs.SetInt(Kit_GameSettings.userName + "_xp", currentXp);
            }
        }
    }
}