
using Mirror;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_DropBehaviour : NetworkBehaviour
        {
            public Rigidbody body;

            public GameObject rendererRoot;
            /// <summary>
            /// How long is this weapon drop going to be live?
            /// </summary>
            public float lifeTime = 30f;

            [SyncVar]
            public int weaponID;
            [SyncVar]
            public int bulletsLeft;
            [SyncVar]
            public int bulletsLeftToReload;
            public readonly SyncList<int> attachments = new SyncList<int>();
            [SyncVar]
            public bool isSceneOwned;

            public override void OnStartServer()
            {
                //Instantiate renderer
                GameObject pr = Instantiate(Kit_IngameMain.instance.gameInformation.allWeapons[weaponID].dropPrefab, rendererRoot.transform);
                //Set scale
                pr.transform.localScale = Vector3.one;
                if (Kit_IngameMain.instance.gameInformation.allWeapons[weaponID] is Kit_ModernWeaponScript)
                {
                    //Get Renderer
                    Kit_DropRenderer render = pr.GetComponent<Kit_DropRenderer>();
                    //Setup Attachments
                    render.SetAttachments(Kit_IngameMain.instance.gameInformation.allWeapons[weaponID] as Kit_ModernWeaponScript, attachments.ToArray());
                }

                if (lifeTime > 0)
                {
                    StartCoroutine(DestroyTimed());
                }
            }

            public override void OnStartClient()
            {
                if (!isServer)
                {
                    //Instantiate renderer
                    GameObject pr = Instantiate(Kit_IngameMain.instance.gameInformation.allWeapons[weaponID].dropPrefab, rendererRoot.transform);
                    //Set scale
                    pr.transform.localScale = Vector3.one;
                    if (Kit_IngameMain.instance.gameInformation.allWeapons[weaponID] is Kit_ModernWeaponScript)
                    {
                        //Get Renderer
                        Kit_DropRenderer render = pr.GetComponent<Kit_DropRenderer>();
                        //Setup Attachments
                        render.SetAttachments(Kit_IngameMain.instance.gameInformation.allWeapons[weaponID] as Kit_ModernWeaponScript, attachments.ToArray());
                    }
                }
            }

            IEnumerator DestroyTimed()
            {
                yield return new WaitForSeconds(lifeTime);
                NetworkServer.Destroy(gameObject);
            }


            public void PickedUp()
            {
                if (NetworkServer.active)
                {
                    NetworkServer.Destroy(gameObject);
                }
            }
        }
    }
}
