
using UnityEngine;

namespace MarsFPSKit
{
    [System.Serializable]
    public class MovementSyncData
    {

    }

    /// <summary>
    /// Used by <see cref="Kit_PlayerBehaviour"/> to calculate the movement. Drag into <see cref="Kit_PlayerBehaviour.movement"/> after you created a .asset file of it
    /// </summary>
    public abstract class Kit_MovementBase : Kit_WeaponInjection
    {
        public GameObject networkData;

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
        /// Visuals only
        /// </summary>
        /// <param name="pb"></param>
        public abstract void Visuals(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Callback for OnControllerColliderHit
        /// </summary>
        /// <param name="hit"></param>
        public virtual void OnControllerColliderHitRelay(Kit_PlayerBehaviour pb, ControllerColliderHit hit) //This is optional
        {

        }

        /// <summary>
        /// Returns a bool that determines if we can fire our weapons
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool CanFire(Kit_PlayerBehaviour pb);

        /// <summary>
        /// If running is true, this is true
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool IsRunning(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Returns the animation that should be currently played. 0 = Idle; 1 = Walk; 2 = Run;
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract int GetCurrentWeaponMoveAnimation(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Returns the speed that should be used for the walking animation
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract float GetCurrentWalkAnimationSpeed(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Retrieves the current (local!) movement direction
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract Vector3 GetMovementDirection(Kit_PlayerBehaviour pb);

        /// <summary>
        /// This is called for everyone in update
        /// </summary>
        /// <param name="pb"></param>
        public abstract void CalculateFootstepsUpdate(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Returns the velocity, either local or across the network.
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract Vector3 GetVelocity(Kit_PlayerBehaviour pb);

        /// <summary>
        /// A sound playing RPC was received (local or not local)
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="soundID"></param>
        /// <param name="arrayID"></param>
        public abstract void PlaySound(Kit_PlayerBehaviour pb, int soundID, int id2, int arrayID);

        /// <summary>
        /// An animation rpc was received (local or not local)
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="id"></param>
        public abstract void PlayAnimation(Kit_PlayerBehaviour pb, int id, int id2);

        /// <summary>
        /// Called when player enters a trigger
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="col"></param>
        public virtual void OnTriggerEnterRelay(Kit_PlayerBehaviour pb, Collider col)
        {

        }

        /// <summary>
        /// Called when player exited a trigger
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="col"></param>
        public virtual void OnTriggerExitRelay(Kit_PlayerBehaviour pb, Collider col)
        {

        }

        /// <summary>
        /// Called when camera enters a trigger
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="col"></param>
        public virtual void OnCameraTriggerEnterRelay(Kit_PlayerBehaviour pb, Collider col)
        {

        }

        /// <summary>
        /// Called when camera exited a trigger
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="col"></param>
        public virtual void OnCameraTriggerExitRelay(Kit_PlayerBehaviour pb, Collider col)
        {

        }

        public abstract void CreateSnapshot(Kit_PlayerBehaviour pb);
    }
}
