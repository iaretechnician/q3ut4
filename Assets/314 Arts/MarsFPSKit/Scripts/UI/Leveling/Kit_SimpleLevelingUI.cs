using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_SimpleLevelingUI : Kit_LevelingUIBase
    {
        /// <summary>
        /// Used to display the message
        /// </summary>
        public TextMeshProUGUI levelUpText;
        /// <summary>
        /// Used to display the unlocked item / level
        /// </summary>
        public Image levelUpImage;

        /// <summary>
        /// Animation used to fade
        /// </summary>
        public Animation animationFade;
        /// <summary>
        /// How long is the animation?
        /// </summary>
        public float animationLength = 3f;

        private bool isRoutineRunning = false;

        public override void DisplayLevelUp(int newLevel)
        {
            if (!isRoutineRunning)
            {
                //Create new text
                levelUpText.text = "Level up!" + "\n" + newLevel;
                //Start
                StartCoroutine(FadeLevelUp(newLevel));
            }
        }

        public override void DisplayOther(Sprite sprite, string txt)
        {
            if (!isRoutineRunning)
            {
                levelUpText.text = txt;
                levelUpImage.sprite = sprite;
                StartCoroutine(DisplayOtherThing());
            }
        }

        /// <summary>
        /// It can be display anything really
        /// </summary>
        /// <returns></returns>
        IEnumerator DisplayOtherThing()
        {
            isRoutineRunning = true;
            animationFade.gameObject.SetActive(true);
            //Enable animation
            animationFade.enabled = true;
            //Display level up
            animationFade.Rewind("Fade");
            animationFade.Play("Fade");
            //Wait
            yield return new WaitForSeconds(animationLength);
            //Disalbe
            animationFade.enabled = false;
            animationFade.gameObject.SetActive(false);
            isRoutineRunning = false;
        }

        IEnumerator FadeLevelUp(int newLvl)
        {
            isRoutineRunning = true;
            animationFade.gameObject.SetActive(true);
            //Enable animation
            animationFade.enabled = true;
            //Display level up
            animationFade.Rewind("Fade");
            //Set text
            levelUpText.text = "Level up!";
            //Set image
            levelUpImage.sprite = Kit_IngameMain.instance.gameInformation.leveling.levelUpIcons[newLvl - 1];
            animationFade.Play("Fade");
            //Wait
            yield return new WaitForSeconds(animationLength);
            //Get unlocked items
            UnlockInformation[] unlockedItems = Kit_IngameMain.instance.gameInformation.GetUnlockedItemsAtLevel(newLvl);
            //Display them
            for (int i = 0; i < unlockedItems.Length; i++)
            {
                //Display level up
                animationFade.Rewind("Fade");
                //Set text
                levelUpText.text = "New item unlocked: " + unlockedItems[i].name;
                //Set image
                levelUpImage.sprite = unlockedItems[i].img;
                //Play animation
                animationFade.Play("Fade");
                //Wait
                yield return new WaitForSeconds(animationLength);
            }
            //Disalbe
            animationFade.enabled = false;
            animationFade.gameObject.SetActive(false);
            isRoutineRunning = false;
        }
    }
}