using UnityEngine;

namespace MarsFPSKit
{
    [System.Serializable]
    public class StringToPrefab : SerializableDictionary<string, GameObject> { }

    /// <summary>
    /// This script is the base for a third person model behaviour
    /// </summary>
    public abstract class Kit_ThirdPersonPlayerModel : MonoBehaviour
    {
        /// <summary>
        /// Our animator that is used to animate this model
        /// </summary>
        public Animator anim;

        [Header("Weapon Placement")]
        //Every model needs to have transform objects for the weapons
        /// <summary>
        /// The transform for weapons that are in the player's hands
        /// </summary>
        public Transform weaponsInHandsGo;
        [Tooltip("The _TP prefab of weapons has an array of inverse kinematic left hand positions. This determines which one is used.")]
        /// <summary>
        /// The _TP prefab of weapons has an array of inverse kinematic left hand positions. This determines which one is used.
        /// </summary>
        public int inverseKinematicID;

        [Header("Sounds")]
        /// <summary>
        /// Audio source used for fire sounds
        /// </summary>
        public AudioSource soundFire;

        /// <summary>
        /// Audio Source used for reload sounds
        /// </summary>
        public AudioSource soundReload;

        /// <summary>
        /// Audio Source used for other sounds
        /// </summary>
        public AudioSource soundOther;

        /// <summary>
        /// Audio Source used for talking
        /// </summary>
        public AudioSource soundVoice;

        [Header("Name Above Head")]
        /// <summary>
        /// the collider which triggers our name to be displayed for enemies (if they are aiming at us)
        /// </summary>
        public Collider enemyNameAboveHeadTrigger;
        /// <summary>
        /// Where is our name going to be displayed?
        /// </summary>
        public Transform enemyNameAboveHeadPos;

        [Header("Customization")]
        /// <summary>
        /// All customizations
        /// </summary>
        public CustomizationSlot[] customizationSlots;

        /// <summary>
        /// The information that this player model belongs to
        /// </summary>
        [HideInInspector]
        public Kit_PlayerModelInformation information;

        [Tooltip("First person arms prefab to use for weapons that have this setting enabled")]
        /// <summary>
        /// First person arms prefab to use for weapons that have this setting enabled
        /// </summary>
        [Header("First Person Arms")]
        public StringToPrefab firstPersonArmsPrefab;


        /// <summary>
        /// The initial setup for this model, (for example setting up the animator)
        /// </summary>
        public abstract void SetupModel(Kit_PlayerBehaviour kpb);
        /// <summary>
        /// Called when the model should be set up for first person usage (Things like shadows only for example)
        /// </summary>
        public abstract void FirstPerson();
        /// <summary>
        /// Called when the model should be set up for third person usage
        /// </summary>
        public abstract void ThirdPerson();

        /// <summary>
        /// Uses given animset
        /// </summary>
        /// <param name="animType"></param>
        public abstract void SetAnimType(string animType, bool noTrans = false);

        /// <summary>
        /// Play a fire animation for given anim type
        /// </summary>
        /// <param name="animType"></param>
        public abstract void PlayWeaponFireAnimation(string animType);

        /// <summary>
        /// Play a reload animation for given anim type
        /// </summary>
        /// <param name="animType"></param>
        public abstract void PlayWeaponReloadAnimation(string animType, int type);

        /// <summary>
        /// Play change animation
        /// </summary>
        /// <param name="animType"></param>
        /// <param name="draw"></param>
        public abstract void PlayWeaponChangeAnimation(string animType, bool draw, float length);

        /// <summary>
        /// Play a universal weapon anim
        /// </summary>
        /// <param name="animType"></param>
        /// <param name="anim"></param>
        public abstract void PlayWeaponAnimation(string animType, string anim);

        /// <summary>
        /// Plays a melee animation (0 = Stab, 1 = Charge, 2 = Heal)
        /// </summary>
        /// <param name="animation"></param>
        public abstract void PlayMeleeAnimation(int animation, int state);

        /// <summary>
        /// Plays a grenade animation (0 = Pull; 1 = Throw)
        /// </summary>
        /// <param name="animation"></param>
        public abstract void PlayGrenadeAnimation(int animation);

        /// <summary>
        /// Stop all weapon animations
        /// </summary>
        public abstract void AbortWeaponAnimations();

        /// <summary>
        /// Called when we die (for everyone). Create ragdoll.
        /// </summary>
        public abstract void CreateRagdoll();

        /// <summary>
        /// Called when the player changed from first to third or third to first person view
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="isThirdPersonEnabled"></param>
        public abstract void FirstThirdPersonChanged(Kit_PlayerBehaviour pb, Kit_GameInformation.Perspective perspective);

        /// <summary>
        /// Enables the given customizations.
        /// </summary>
        /// <param name="enabledCustomizations"></param>
        public void SetCustomizations(int[] enabledCustomizations, Kit_PlayerBehaviour pb)
        {
            //Loop through all slots
            for (int i = 0; i < enabledCustomizations.Length; i++)
            {
                if (i < customizationSlots.Length)
                {
                    //Loop through all customizations for that slot
                    for (int o = 0; o < customizationSlots[i].customizations.Length; o++)
                    {
                        //Check if this customization is enabled
                        if (o == enabledCustomizations[i])
                        {
                            //Tell the behaviours they are active!
                            for (int p = 0; p < customizationSlots[i].customizations[o].customizationBehaviours.Length; p++)
                            {
                                customizationSlots[i].customizations[o].customizationBehaviours[p].Selected(pb, this);
                            }
                        }
                        else
                        {
                            //Tell the behaviours they are not active!
                            for (int p = 0; p < customizationSlots[i].customizations[o].customizationBehaviours.Length; p++)
                            {
                                customizationSlots[i].customizations[o].customizationBehaviours[p].Unselected(pb, this);
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Something must have gone wrong with the customizations. Enabled customizations is longer than all slots.");
                }
            }
        }

        /// <summary>
        /// Revert our colliders to their position revertBy seconds ago
        /// </summary>
        /// <param name="revertBy"></param>
        public abstract void SetLagCompensationTo(float revertBy);
    }
}
