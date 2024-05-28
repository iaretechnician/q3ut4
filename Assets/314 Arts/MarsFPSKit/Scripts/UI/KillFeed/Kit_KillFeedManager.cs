using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_KillFeedManager : Kit_KillFeedBase
    {
        [Header("References")]
        /// <summary>
        /// The root transform of the entries
        /// </summary>
        public RectTransform killFeedGo;
        /// <summary>
        /// The entry prefab
        /// </summary>
        public GameObject killFeedPrefab;

        /// <summary>
        /// Add an entry to the killfeed
        /// </summary>
        /// <param name="killer">Who shot</param>
        /// <param name="killed">Who was killed</param>
        /// <param name="gun">Which weapon was used to kill</param>
        public override void Append(bool botKiller, uint killer, bool botKilled, uint killed, int gun, int playerModel, int ragdollId)
        {
            GameObject go = Instantiate(killFeedPrefab, killFeedGo, false);
            //Reset scale
            go.transform.localScale = Vector3.one;
            //Set it up
            go.GetComponent<Kit_KillFeedEntry>().SetUp(botKiller, killer, botKilled, killed, gun, playerModel, ragdollId, this);
        }

        /// <summary>
        /// Add an entry to the killfeed
        /// </summary>
        /// <param name="killer">Who shot</param>
        /// <param name="killed">Who was killed</param>
        /// <param name="gun">Which weapon was used to kill</param>
        public override void Append(bool botKiller, uint killer, bool botKilled, uint killed, string cause, int playerModel, int ragdollId)
        {
            GameObject go = Instantiate(killFeedPrefab, killFeedGo, false);
            //Reset scale
            go.transform.localScale = Vector3.one;
            //Set it up
            go.GetComponent<Kit_KillFeedEntry>().SetUp(botKiller, killer, botKilled, killed, cause, playerModel, ragdollId, this);
        }
    }
}
