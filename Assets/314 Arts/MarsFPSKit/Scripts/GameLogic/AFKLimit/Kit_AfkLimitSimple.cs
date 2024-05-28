using System;
using UnityEngine;

namespace MarsFPSKit
{
    [CreateAssetMenu(menuName = "MarsFPSKit/AFK Systems/Basic")]
    /// <summary>
    /// Implements a basic AFK system that checks for any input. If the afk limit is passed, we disconnect. It displays a number of warnings during that.
    /// </summary>
    public class Kit_AfkLimitSimple : Kit_AfkLimitBase
    {
        [Tooltip("After how many seconds should a warning be displayed?")]
        /// <summary>
        /// After how many seconds should a warning be displayed?
        /// </summary>
        public int warningEverySeconds;

        //RUNTIME DATA
        /// <summary>
        /// When did the system receive an input for the last time?
        /// </summary>
        private float lastInputTime;
        /// <summary>
        /// How many warnings have been displayed?
        /// </summary>
        private int currentNumberOfWarnings;
        /// <summary>
        /// The currently used AFK Limit
        /// </summary>
        private int currentAfkLimit;
        /// <summary>
        /// When will the next warning be displayed?
        /// </summary>
        private float nextWarning;
        //END

        public override void StartRelay(bool enabled, int afkLimit = 0)
        {
            //Set limit
            currentAfkLimit = afkLimit;
            //Reset values
            lastInputTime = Time.time;
            nextWarning = Time.time + warningEverySeconds;
            currentNumberOfWarnings = 0;
        }

        public override void UpdateRelay()
        {
            //Check if any key is pressed
            if (Input.anyKey)
            {
                //Set time
                lastInputTime = Time.time;
                //Reset warnings
                currentNumberOfWarnings = 0;
                nextWarning = Time.time + warningEverySeconds;
            }
            else
            {
                if (Time.time > lastInputTime + currentAfkLimit)
                {
                    //Disconnect
                    Kit_IngameMain.instance.Disconnect();
                }
                else if (Time.time > nextWarning)
                {
                    //Increase warnings
                    currentNumberOfWarnings++;
                    //Set time
                    nextWarning = Time.time + warningEverySeconds;
                    //Display warning
                    if (Kit_IngameMain.instance.afkLimitUI)
                    {
                        Kit_IngameMain.instance.afkLimitUI.DisplayWarning(Time.time - lastInputTime, (lastInputTime + currentAfkLimit) - Time.time, currentNumberOfWarnings);
                    }
                }
            }
        }
    }
}
