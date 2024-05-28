using MarsFPSKit.Weapons;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace MarsFPSKit
{
    /// <summary>
    /// Creates random valid loadouts for bots
    /// </summary>
    [CreateAssetMenu(menuName = "MarsFPSKit/Bots/Loadout/Random")]
    public class Kit_BotRandomLoadoutManager : Kit_BotLoadoutManager
    {
        public override Loadout GetBotLoadout()
        {
            Loadout toReturn = new Loadout();
            //Find a primary
            Kit_WeaponBase[] primaries = Array.FindAll(Kit_IngameMain.instance.gameInformation.allWeapons, x => x.CanBeSelectedInLoadout() && x.weaponType == "Primary");
            Kit_WeaponBase primary = primaries[Random.Range(0, primaries.Length)];
            int primaryIndex = Array.IndexOf(Kit_IngameMain.instance.gameInformation.allWeapons, primary);
            //Find a secondary
            Kit_WeaponBase[] secondaries = Array.FindAll(Kit_IngameMain.instance.gameInformation.allWeapons, x => x.CanBeSelectedInLoadout() && x.weaponType == "Secondary");
            Kit_WeaponBase secondary = secondaries[Random.Range(0, secondaries.Length)];
            int secondaryIndex = Array.IndexOf(Kit_IngameMain.instance.gameInformation.allWeapons, secondary);

            toReturn.loadoutWeapons = new LoadoutWeapon[2];
            toReturn.loadoutWeapons[0] = new LoadoutWeapon();
            toReturn.loadoutWeapons[1] = new LoadoutWeapon();

            toReturn.loadoutWeapons[0].weaponID = primaryIndex;
            if (primary is Kit_ModernWeaponScript)
            {
                Kit_ModernWeaponScript mp = primary as Kit_ModernWeaponScript;
                int length = mp.attachmentSlots.Length;
                toReturn.loadoutWeapons[0].attachments = new int[length];
                for (int i = 0; i < toReturn.loadoutWeapons[0].attachments.Length; i++)
                {
                    toReturn.loadoutWeapons[0].attachments[i] = Random.Range(0, mp.attachmentSlots[i].availableAttachments.Length);
                }
            }
            else
            {
                toReturn.loadoutWeapons[0].attachments = new int[0];
            }

            toReturn.loadoutWeapons[1].weaponID = secondaryIndex;
            if (secondary is Kit_ModernWeaponScript)
            {
                Kit_ModernWeaponScript sp = secondary as Kit_ModernWeaponScript;
                int length = sp.attachmentSlots.Length;
                toReturn.loadoutWeapons[1].attachments = new int[length];
                for (int i = 0; i < toReturn.loadoutWeapons[1].attachments.Length; i++)
                {
                    toReturn.loadoutWeapons[1].attachments[i] = Random.Range(0, sp.attachmentSlots[i].availableAttachments.Length);
                }
            }
            else
            {
                toReturn.loadoutWeapons[1].attachments = new int[0];
            }

            toReturn.teamLoadout = new TeamLoadout[Kit_IngameMain.instance.gameInformation.allPvpTeams.Length];

            for (int i = 0; i < toReturn.teamLoadout.Length; i++)
            {
                toReturn.teamLoadout[i] = new TeamLoadout();
                toReturn.teamLoadout[i].playerModelID = Random.Range(0, Kit_IngameMain.instance.gameInformation.allPvpTeams[i].playerModels.Length);
                toReturn.teamLoadout[i].playerModelCustomizations = new int[Kit_IngameMain.instance.gameInformation.allPvpTeams[i].playerModels[toReturn.teamLoadout[i].playerModelID].prefab.GetComponent<Kit_ThirdPersonPlayerModel>().customizationSlots.Length];
            }

            /*
            //Assign
            toReturn.primaryWeapon = primaryIndex;
            if (primary.firstPersonPrefab.GetComponent<Weapons.Kit_WeaponRenderer>())
            {
                toReturn.primaryAttachments = new int[primary.firstPersonPrefab.GetComponent<Weapons.Kit_WeaponRenderer>().attachmentSlots.Length];
            }
            else
            {
                toReturn.primaryAttachments = new int[0];
            }
            toReturn.secondaryWeapon = secondaryIndex;
            if (secondary.firstPersonPrefab.GetComponent<Weapons.Kit_WeaponRenderer>())
            {
                toReturn.secondaryAttachments = new int[secondary.firstPersonPrefab.GetComponent<Weapons.Kit_WeaponRenderer>().attachmentSlots.Length];
            }
            else
            {
                toReturn.secondaryAttachments = new int[0];
            }
            */
            return toReturn;
        }
    }
}
