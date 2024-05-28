using MarsFPSKit.Weapons;
using UnityEngine;

namespace MarsFPSKit
{
    public class WeaponsFromPlugin
    {
        public WeaponAttachmentBundle[] weaponsInSlot;
    }

    public class WeaponAttachmentBundle
    {
        public Kit_WeaponBase weapon;
        public int[] attachments;
    }

    public abstract class Kit_WeaponInjection : ScriptableObject
    {
        public virtual WeaponsFromPlugin WeaponsToInjectIntoWeaponManager(Kit_PlayerBehaviour player)
        {
            WeaponsFromPlugin weapons = new WeaponsFromPlugin();
            weapons.weaponsInSlot = new WeaponAttachmentBundle[0];
            return weapons;
        }

        public virtual void ReportSlotOfInjectedWeapons(Kit_PlayerBehaviour player, int slotWhereTheyWereInjected)
        {

        }
    }
}