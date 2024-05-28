using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This script will relay Animator IK to <see cref="Kit_ThirdPersonModernPlayerModel"/>
    /// </summary>
    public class Kit_ThirdPersonModernIKRelay : MonoBehaviour
    {
        /// <summary>
        /// The player model to relay to
        /// </summary>
        public Kit_ThirdPersonModernPlayerModel pm;

        void OnAnimatorIK()
        {
            //Relay
            pm.OnAnimatorIKRelay();
        }
    }
}
