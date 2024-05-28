using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Overrides the first person arms
    /// </summary>
    public class Kit_PlayerModelCustomizationFirstPersonArmsOverride : Kit_PlayerModelCustomizationBehaviour
    {
        [Tooltip("The new arms to use")]
        /// <summary>
        /// The new arms to use
        /// </summary>
        public StringToPrefab firstPersonArms;

        public override void Selected(Kit_PlayerBehaviour pb, Kit_ThirdPersonPlayerModel pm)
        {
            //Assign new arms
            pm.firstPersonArmsPrefab = firstPersonArms;
        }

        public override void Unselected(Kit_PlayerBehaviour pb, Kit_ThirdPersonPlayerModel pm)
        {
            //Dont do anything
        }
    }
}