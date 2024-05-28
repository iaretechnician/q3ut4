using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MarsFPSKit
{
    namespace UI
    {
        public class Kit_MenuSingleplayer : MonoBehaviour
        {
            /// <summary>
            /// Access to menu manager!
            /// </summary>
            public Kit_MenuManager menuManager;
            /// <summary>
            /// Id of this screen in the manager
            /// </summary>
            public int singleplayerScreenId;
            /// <summary>
            /// Go for the layout where we select the game modes
            /// </summary>
            public RectTransform layoutGo;
            /// <summary>
            /// Prefabs for the layout where we select the game modes
            /// </summary>
            public GameObject layoutPrefab;

            private void Start()
            {
                for (int i = 0; i < menuManager.game.allSingleplayerGameModes.Length; i++)
                {
                    int id = i;
                    Kit_PvE_GameModeBase gameMode = menuManager.game.allSingleplayerGameModes[id];

                    if (gameMode.menuPrefab)
                    {
                        GameObject menu = Instantiate(gameMode.menuPrefab);

                        Kit_MenuPveGameModeBase pveMenu = menu.GetComponent<Kit_MenuPveGameModeBase>();

                        if (pveMenu)
                        {
                            //Setup
                            pveMenu.SetupMenu(menuManager, 0, id);

                            //Create button
                            GameObject go = Instantiate(layoutPrefab, layoutGo, false);
                            //Set pos
                            go.transform.SetSiblingIndex(i);
                            //Get button
                            Button btn = go.GetComponentInChildren<Button>();
                            btn.onClick.AddListener(delegate { pveMenu.OpenMenu(); });
                            //Name
                            TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                            txt.text = gameMode.gameModeName;
                        }
                        else
                        {
                            Debug.Log("[Singleplayer] Game Mode " + gameMode.gameModeName + " has no menu script on its prefab", menu);
                        }
                    }
                    else
                    {
                        Debug.Log("[Singleplayer] Game Mode " + gameMode.gameModeName + " has no menu", gameMode);
                    }
                }
            }
        }
    }
}