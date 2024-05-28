using UnityEngine;
using System.Collections.Generic;

namespace MarsFPSKit
{
    public abstract class Kit_MapVotingUIBase : MonoBehaviour
    {
        /// <summary>
        /// How many map / game mode combos are available to vote?
        /// </summary>
        public int amountOfAvailableVotes = 4;

        /// <summary>
        /// Sets up the UI for given combo
        /// </summary>
        /// <param name="combos"></param>
        public abstract void SetupVotes(Kit_MapVotingBehaviour behaviour);

        /// <summary>
        /// Redraws the votes with the values from the given behaviour
        /// </summary>
        /// <param name="behaviour"></param>
        public abstract void RedrawVotes(Kit_MapVotingBehaviour behaviour);

        /// <summary>
        /// Hide the UI
        /// </summary>
        public abstract void Hide();
    }
}