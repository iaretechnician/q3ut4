using UnityEngine;
using System.Collections;


namespace MarsFPSKit
{
    /// <summary>
    /// Used by <see cref="Kit_PlayerBehaviour"/> to calculate the looking. Drag into <see cref="Kit_PlayerBehaviour.looking"/> after you created a .asset file of it
    /// </summary>
    public abstract class Kit_MouseLookBase : ScriptableObject
    {
        /// <summary>
        /// Initialize this module
        /// </summary>
        /// <param name="pb"></param>
        public abstract void InitializeServer(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Initialize this module
        /// </summary>
        /// <param name="pb"></param>
        public abstract void InitializeClient(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Update for not controller and not server
        /// </summary>
        /// <param name="pb"></param>
        public abstract void NotControllerUpdate(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Executed at 1 / send rate
        /// </summary>
        /// <param name="input"></param>
        public abstract void AuthorativeInput(Kit_PlayerBehaviour pb, Kit_PlayerInput input, float delta, double revertTime);

        /// <summary>
        /// Prediction input
        /// </summary>
        /// <param name="input"></param>
        public abstract void PredictionInput(Kit_PlayerBehaviour pb, Kit_PlayerInput input, float delta);

        /// <summary>
        /// Called once the player object has been assigned to the actual network player
        /// </summary>
        /// <param name="pb"></param>
        public abstract void TakeControl(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Visuals only
        /// </summary>
        /// <param name="pb"></param>
        public abstract void Visuals(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Did we reach our Y looking max value?
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool ReachedYMax(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Calculate the looking (Late Update)
        /// </summary>
        /// <param name="pb"></param>
        public virtual void CalculateLookLateUpdate(Kit_PlayerBehaviour pb) //This is optional
        {

        }

        /// <summary>
        /// Callback for OnControllerColliderHit
        /// </summary>
        /// <param name="hit"></param>
        public virtual void OnControllerColliderHitRelay(Kit_PlayerBehaviour pb, ControllerColliderHit hit) //This is optional
        {

        }

        /// <summary>
        /// This returns the speed multiplier
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract float GetSpeedMultiplier(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Returns the camera offset
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public virtual Vector3 GetCameraOffset(Kit_PlayerBehaviour pb)
        {
            return Vector3.zero;
        }

        /// <summary>
        /// Returns the camera offset
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public virtual Quaternion GetCameraRotationOffset(Kit_PlayerBehaviour pb)
        {
            return Quaternion.identity;
        }

        /// <summary>
        /// Returns the weapon offset
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public virtual Vector3 GetWeaponOffset(Kit_PlayerBehaviour pb)
        {
            return Vector3.zero;
        }

        /// <summary>
        /// Returns the weapon offset
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public virtual Quaternion GetWeaponRotationOffset(Kit_PlayerBehaviour pb)
        {
            return Quaternion.identity;
        }

        #region Perspective manager
        /// <summary>
        /// Gets the current perspective
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract Kit_GameInformation.Perspective GetPerspective(Kit_PlayerBehaviour pb);
        #endregion
    }
}
