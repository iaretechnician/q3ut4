using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public abstract class Kit_WeaponRuntimeDataBase : NetworkBehaviour
        {
            public List<GameObject> instantiatedObjects = new List<GameObject>(); //Objects that were instantiated by this weapon. When it is replaced, they have to be cleaned up first.

            /// <summary>
            /// Assigned weapon behaviour
            /// </summary>
            public Kit_WeaponBase behaviour;

            [SyncVar]
            public int slot;

            [SyncVar]
            public int id;

            [SyncVar]
            public bool isInjectedFromPlugin;

            /// <summary>
            /// Attachments for this weapon
            /// </summary>
            public readonly SyncList<int> attachments = new SyncList<int>();

            /// <summary>
            /// This can be used for additional runtime data, for example for attachments.
            /// Like the flashlight can be turned on and off, and the bool has to be synced
            /// </summary>
            public readonly SyncList<uint> additionalDataBehaviors = new SyncList<uint>();

            public virtual void OnDestroy()
            {
                if (Kit_IngameMain.instance && isServer)
                {
                    //Destroy additional behaviors
                    for (int i = 0; i < additionalDataBehaviors.Count; i++)
                    {
                        if (NetworkServer.spawned.ContainsKey(additionalDataBehaviors[i]))
                        {
                            NetworkServer.Destroy(NetworkServer.spawned[additionalDataBehaviors[i]].gameObject);
                        }
                    }
                }
            }
        }
    }
}