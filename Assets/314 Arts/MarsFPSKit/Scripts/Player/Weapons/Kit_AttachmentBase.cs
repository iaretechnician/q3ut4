using UnityEngine;
using UnityEngine.Serialization;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public abstract class Kit_AttachmentBase : ScriptableObject
        {
            [FormerlySerializedAs("attachmentName")]
            public string displayName;
        }
    }
}