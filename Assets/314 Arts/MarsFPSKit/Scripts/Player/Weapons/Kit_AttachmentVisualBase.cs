using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        /// <summary>
        /// Enum used to describe attachment case
        /// </summary>
        public enum AttachmentUseCase { FirstPerson, ThirdPerson, Drop }

        public abstract class Kit_AttachmentVisualBase : MonoBehaviour
        {
            /// <summary>
            /// If this attachment requires syncing, the third person equivalent (where "SyncFromFirstPerson" will be called), will be assigned here by the system.
            /// </summary>
            public Kit_AttachmentVisualBase thirdPersonEquivalent;

            /// <summary>
            /// Does this attachment require syncing?
            /// </summary>
            /// <returns></returns>
            public virtual bool RequiresSyncing()
            {
                return false;
            }

            /// <summary>
            /// Does this behaviour require interaction ?
            /// </summary>
            /// <returns></returns>
            public virtual bool RequiresInteraction()
            {
                return false;
            }

            /// <summary>
            /// Interaction function
            /// </summary>
            /// <param name="pb"></param>
            public virtual void Interaction(Kit_PlayerBehaviour pb)
            {

            }

            /// <summary>
            /// For local sync in third person (and bots as master client), this is called from first person to sync.
            /// </summary>
            /// <param name="obj"></param>
            public virtual void SyncFromFirstPerson(object obj)
            {

            }

            /// <summary>
            /// Called when this attachment is selected
            /// </summary>
            public virtual void Selected(Kit_PlayerBehaviour pb, AttachmentUseCase auc, Kit_ModernWeaponScript script, Kit_ModernWeaponScriptRuntimeData data, int slot)
            {

            }

            /// <summary>
            /// Sets visibility if its selected
            /// </summary>
            /// <param name="visible"></param>
            public virtual void SetVisibility(Kit_PlayerBehaviour pb, AttachmentUseCase auc, bool visible)
            {

            }
        }
    }
}