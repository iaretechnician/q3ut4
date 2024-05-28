using UnityEngine;

namespace MarsFPSKit
{
    namespace Optimization
    {
        public abstract class Kit_ObjectPoolingBase : MonoBehaviour
        {
            /// <summary>
            /// Start Object Pooling for given prefab
            /// </summary>
            /// <param name="prefab"></param>
            /// <param name="defaultInitialization"></param>
            public abstract void EnqueueInstantiateable(GameObject prefab, int defaultInitialization);

            /// <summary>
            /// Gets one of these
            /// </summary>
            /// <param name="prefab"></param>
            /// <returns></returns>
            public abstract GameObject GetInstantiateable(GameObject prefab, Vector3 pos, Quaternion rot);

            /// <summary>
            /// Destroys one of these.
            /// </summary>
            /// <param name="go"></param>
            public abstract void DestroyInstantiateable(GameObject go);
        }
    }
}