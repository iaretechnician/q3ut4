using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_DamageConverterBase : ScriptableObject
    {
        public abstract void ConvertDamageFromRaycast(Kit_PlayerBehaviour applyEffectOn);

        public abstract void ConvertDamageFromBullet(Kit_PlayerBehaviour applyEffectOn);

        public abstract void ConvertDamageFromExplosion(Kit_PlayerBehaviour applyEffectOn, Vector3 explosionCenter);
    }
}