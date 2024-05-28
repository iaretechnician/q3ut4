using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MarsFPSKit
{
    namespace Optimization
    {
        [System.Serializable]
        public class GameObjectToGameObjectList : SerializableDictionary<GameObject, List<GameObject>> { }

        /// <summary>
        /// Default implementation for object pooling!
        /// </summary>
        public class Kit_ObjectPoolingDefault : Kit_ObjectPoolingBase
        {
            public GameObjectToGameObjectList pool;

            public override void DestroyInstantiateable(GameObject go)
            {
                go.transform.parent = transform;
                go.SetActive(false);
            }

            public override void EnqueueInstantiateable(GameObject prefab, int defaultInitialization)
            {
                if (prefab)
                {
                    if (!pool.Contains(prefab))
                    {
                        List<GameObject> preFill = new List<GameObject>();
                        for (int i = 0; i < defaultInitialization; i++)
                        {
                            GameObject go = Instantiate(prefab, transform, false);
                            go.SetActive(false);
                            preFill.Add(go);
                        }

                        pool.Add(prefab, preFill);
                    }
                }
            }

            public override GameObject GetInstantiateable(GameObject prefab, Vector3 pos, Quaternion rot)
            {
                if (!prefab) return null;

                List<GameObject> fill = pool[prefab];
                GameObject[] actives = fill.Where(x => x && !x.activeSelf).ToArray();

                if (actives.Length > 0)
                {
                    actives[0].transform.parent = null;
                    actives[0].transform.position = pos;
                    actives[0].transform.rotation = rot;
                    actives[0].SetActive(true);
                    return actives[0];
                }
                else
                {
                    GameObject go = Instantiate(prefab, pos, rot);
                    //Add to list
                    fill.Add(go);
                    return go;
                }
            }
        }
    }
}