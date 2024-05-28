using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_LoadoutIKHelper : MonoBehaviour
    {
        /// <summary>
        /// Animator to apply IK to
        /// </summary>
        public Animator anim;
        /// <summary>
        /// Where is the left hand going to be 
        /// </summary>
        public Transform leftHandGoal;

        /// <summary>
        /// Should IK be applied?
        /// </summary>
        public bool applyIk;

        void OnAnimatorIK()
        {
            if (applyIk && leftHandGoal)
            {
                anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandGoal.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandGoal.rotation);
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            }
        }
    }
}