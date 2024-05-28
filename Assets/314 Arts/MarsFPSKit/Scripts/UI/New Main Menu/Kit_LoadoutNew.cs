using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_LoadoutNew : Kit_LoadoutBase
    {
        /// <summary>
        /// Main Menu
        /// </summary>
        private UI.Kit_MenuManager menu;

        private Kit_GameInformation game
        {
            get
            {
                if (Kit_IngameMain.instance) return Kit_IngameMain.instance.gameInformation;
                if (menu) return menu.game;

                return null;
            }
        }

        /// <summary>
        /// The loadouts
        /// </summary>
        public Loadout[] allLoadouts;
        /// <summary>
        /// The currently selected loadout
        /// </summary>
        public int currentlySelectedLoadout;
        [Header("Loadout Selection")]
        /// <summary>
        /// How many loadouts do we allow?
        /// </summary>
        public int amountOfLoadouts = 5;
        /// <summary>
        /// Screen id of the loadout selection
        /// </summary>
        public int loadoutScreenID = 0;
        /// <summary>
        /// Where the loadouts go
        /// </summary>
        public RectTransform loadoutGo;
        /// <summary>
        /// Prefab for the loadouts
        /// </summary>
        public GameObject loadoutPrefab;

        /// <summary>
        /// Main Category screen
        /// </summary>
        [Header("Main Category Selection")]
        public int mainCategoryScreenID = 1;

        /// <summary>
        /// Where the weapon category buttons go
        /// </summary>
        [Header("Weapon Category Selection")]
        public RectTransform weaponCategoryGo;
        /// <summary>
        /// Button prefab for weapon category selection
        /// </summary>
        public GameObject weaponCategoryPrefab;
        /// <summary>
        /// Screen ID for selecting the weapon category
        /// </summary>
        public int weaponCategoryScreenId = 6;
        /// <summary>
        /// The category that we selected last
        /// </summary>
        public int currentSelectedWeaponCategory;

        /// <summary>
        /// Where the weapon subcategory buttons go
        /// </summary>
        [Header("Weapon Sub-Category Selection")]
        public RectTransform weaponSubCategoryGo;
        /// <summary>
        /// Button prefab for weapon subcategory selection
        /// </summary>
        public GameObject weaponSubCategoryPrefab;
        /// <summary>
        /// Screen ID for selecting the weapon subcategory
        /// </summary>
        public int weaponSubCategoryScreenId = 7;
        /// <summary>
        /// Active sub category entries
        /// </summary>
        public List<GameObject> weaponSubCategoryActives = new List<GameObject>();

        /// <summary>
        /// Where the weapon buttons go
        /// </summary>
        [Header("Weapon Selection")]
        public RectTransform weaponSelectionGo;
        /// <summary>
        /// Button prefab for weapon selection
        /// </summary>
        public GameObject weaponSelectionPrefab;
        /// <summary>
        /// Screen ID for selecting the weapon
        /// </summary>
        public int weaponSelectionScreenId = 8;
        /// <summary>
        /// Active weapon entries
        /// </summary>
        public List<GameObject> weaponSelectionActives = new List<GameObject>();

        /// <summary>
        /// Where the weapon customization category buttons go
        /// </summary>
        [Header("Weapon Customization Category Selection")]
        public RectTransform weaponCustomizationCategoryGo;
        /// <summary>
        /// Button prefab for weapon customization category selection
        /// </summary>
        public GameObject weaponCustomizationCategoryPrefab;
        /// <summary>
        /// Screen ID for selecting the weapon customization category
        /// </summary>
        public int weaponCustomizationCategoryScreenId = 9;
        /// <summary>
        /// Active weapon customization category entries
        /// </summary>
        public List<GameObject> weaponCustomizationCategoryActives = new List<GameObject>();


        /// <summary>
        /// Where the weapon customization attachment buttons go
        /// </summary>
        [Header("Weapon Customization Attachment Selection")]
        public RectTransform weaponCustomizationAttachmentGo;
        /// <summary>
        /// Button prefab for weapon customization attachment selection
        /// </summary>
        public GameObject weaponCustomizationAttachmentPrefab;
        /// <summary>
        /// Screen ID for selecting the weapon customization attachments
        /// </summary>
        public int weaponCustomizationAttachmentScreenId = 10;
        /// <summary>
        /// Active weapon customization attachments entries
        /// </summary>
        public List<GameObject> weaponCustomizationAttachmentActives = new List<GameObject>();

        /// <summary>
        /// Currently active third person preview
        /// </summary>
        [Header("Weapon Preview")]
        public GameObject weaponPreviewObjectThirdPerson;
        /// <summary>
        /// Where the FP goes
        /// </summary>
        public Transform weaponPreviewFirstPersonGo;
        /// <summary>
        /// Currently active first person preview
        /// </summary>
        public GameObject weaponPreviewObjectFirstPerson;
        /// <summary>
        /// Camera that renders the weapon
        /// </summary>
        public Camera weaponPreviewRenderCam;

        /// <summary>
        /// Where the team buttons go
        /// </summary>
        [Header("Player Model Team Selection")]
        public RectTransform playerModelTeamGo;
        /// <summary>
        /// Button prefab for player model selection (team)
        /// </summary>
        public GameObject playerModelTeamPrefab;
        /// <summary>
        /// Screen ID for selecting the team to customize your player model
        /// </summary>
        public int playerModelTeamScreenId = 2;
        /// <summary>
        /// The team that we selected last
        /// </summary>
        public int currentSelectedTeamForPlayerModel;

        /// <summary>
        /// Where the player model buttons go
        /// </summary>
        [Header("Player Model Selection")]
        public RectTransform playerModelSelectionGo;
        /// <summary>
        /// Button prefab for player model selection
        /// </summary>
        public GameObject playerModelSelectionPrefab;
        /// <summary>
        /// Screen ID for selecting the model to use and customize
        /// </summary>
        public int playerModelSelectionScreenId = 3;
        /// <summary>
        /// Currently active buttons
        /// </summary>
        public List<GameObject> playerModelSelectionActives = new List<GameObject>();

        /// <summary>
        /// Where the player model buttons go
        /// </summary>
        [Header("Player Model Customization Category Selection")]
        public RectTransform playerModelCustomizationCategoryGo;
        /// <summary>
        /// Button prefab for player model selection
        /// </summary>
        public GameObject playerModelCustomizationCategoryPrefab;
        /// <summary>
        /// Screen ID for selecting the customization category
        /// </summary>
        public int playerModelCustomizationCategoryScreenId = 4;
        /// <summary>
        /// Currently active buttons
        /// </summary>
        public List<GameObject> playerModelCustomizationCategoryActives = new List<GameObject>();

        /// <summary>
        /// Where the player model buttons go
        /// </summary>
        [Header("Player Model Customization Selection")]
        public RectTransform playerModelCustomizationGo;
        /// <summary>
        /// Button prefab for player model selection
        /// </summary>
        public GameObject playerModelCustomizationPrefab;
        /// <summary>
        /// Screen ID for selecting the customization category
        /// </summary>
        public int playerModelCustomizationScreenId = 5;
        /// <summary>
        /// Currently active buttons
        /// </summary>
        public List<GameObject> playerModelCustomizationActives = new List<GameObject>();


        /// <summary>
        /// Where the actual visible player model goes
        /// </summary>
        [Header("Player Model Preview")]
        public Transform playerModelPreviewGo;
        /// <summary>
        /// Currently visible player model
        /// </summary>
        public Kit_ThirdPersonPlayerModel playerModelCurrent;
        /// <summary>
        /// Script to help with IK
        /// </summary>
        private Kit_LoadoutIKHelper playerModelIkHelper;
        /// <summary>
        /// The camera that renders the player model
        /// </summary>
        public Camera playerModelRenderCam;
        /// <summary>
        /// Render cam current target position
        /// </summary>
        public Vector3 playerModelRenderCamPosition;
        /// <summary>
        /// How far from the camera it should be
        /// </summary>
        public float playerModelRenderCamCustomizationDistance = 0.75f;

        /// <summary>
        /// The loadout submenus
        /// </summary>
        [Header("Menu")]
        public UI.MenuScreen[] subMenus;
        /// <summary>
        /// Currently visible subscreen
        /// </summary>
        private int currentSubScreen = 0;
        /// <summary>
        /// True if we are currently switching a screen
        /// </summary>
        private bool isSwitchingScreens;
        /// <summary>
        /// Where we are currently switching screens to
        /// </summary>
        private Coroutine currentlySwitchingScreensTo;

        private void Awake()
        {
            //Disable all the roots
            for (int i = 0; i < subMenus.Length; i++)
            {
                if (subMenus[i].root)
                {
                    //Disable
                    subMenus[i].root.SetActive(false);
                }
                else
                {
                    Debug.LogError("Submenu root at index " + i + " is not assigned.", this);
                }
            }

            //Enable Loadout Screen ID
            currentSubScreen = loadoutScreenID;
            subMenus[currentSubScreen].root.SetActive(true);

            //Disable cameras
            playerModelRenderCam.enabled = false;
            weaponPreviewRenderCam.enabled = false;

            //Cam default position
            playerModelRenderCamPosition = playerModelRenderCam.transform.parent.position;
        }

        private void Update()
        {
            //Smoothly move the player model camera where we want it to look at :)
            playerModelRenderCam.transform.position = Vector3.Lerp(playerModelRenderCam.transform.position, playerModelRenderCamPosition, Time.deltaTime * 5f);
        }

        public override void ForceClose()
        {
            if (menu)
            {
                if (menu.currentScreen == menuScreenId)
                {
                    menu.SwitchMenu(menu.mainScreen);
                }
            }

            if (Kit_IngameMain.instance)
            {
                if (Kit_IngameMain.instance.currentScreen == menuScreenId)
                {
                    Kit_IngameMain.instance.SwitchMenu(Kit_IngameMain.instance.pauseMenu.pauseMenuId);
                }
            }

            //Disable cameras
            playerModelRenderCam.enabled = false;
            weaponPreviewRenderCam.enabled = false;

            isOpen = false;
        }

        public override Loadout GetCurrentLoadout()
        {
            Loadout loadout = new Loadout();

            //We create a fully copy of the loadout that we selected so that modifying our loadout (attachments) does not update the attachments you currently have selected (on your player)
            loadout.loadoutWeapons = new LoadoutWeapon[allLoadouts[currentlySelectedLoadout].loadoutWeapons.Length];

            for (int i = 0; i < loadout.loadoutWeapons.Length; i++)
            {
                loadout.loadoutWeapons[i] = new LoadoutWeapon();
                loadout.loadoutWeapons[i].weaponID = allLoadouts[currentlySelectedLoadout].loadoutWeapons[i].weaponID;
                loadout.loadoutWeapons[i].attachments = new int[allLoadouts[currentlySelectedLoadout].loadoutWeapons[i].attachments.Length];

                for (int o = 0; o < loadout.loadoutWeapons[i].attachments.Length; o++)
                {
                    loadout.loadoutWeapons[i].attachments[o] = allLoadouts[currentlySelectedLoadout].loadoutWeapons[i].attachments[o];
                }
            }

            loadout.teamLoadout = new TeamLoadout[allLoadouts[currentlySelectedLoadout].teamLoadout.Length];

            for (int i = 0; i < loadout.teamLoadout.Length; i++)
            {
                loadout.teamLoadout[i] = new TeamLoadout();
                loadout.teamLoadout[i].playerModelID = allLoadouts[currentlySelectedLoadout].teamLoadout[i].playerModelID;

                loadout.teamLoadout[i].playerModelCustomizations = new int[allLoadouts[currentlySelectedLoadout].teamLoadout[i].playerModelCustomizations.Length];

                for (int o = 0; o < loadout.teamLoadout[i].playerModelCustomizations.Length; o++)
                {
                    loadout.teamLoadout[i].playerModelCustomizations[o] = allLoadouts[currentlySelectedLoadout].teamLoadout[i].playerModelCustomizations[o];
                }
            }

            return loadout;
        }

        public override void Initialize()
        {
            menu = FindObjectOfType<UI.Kit_MenuManager>();

            //Create Loadouts
            for (int i = 0; i < amountOfLoadouts; i++)
            {
                int id = i;
                //Create
                GameObject go = Instantiate(loadoutPrefab, loadoutGo, false);
                //Setup text
                TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                if (txt) txt.text = "Loadout #" + (id + 1).ToString();
                //Setup Button
                Button btn = go.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate { SelectLoadout(id); });
            }

            allLoadouts = new Loadout[amountOfLoadouts];

            //Setup default blank loadout
            for (int i = 0; i < allLoadouts.Length; i++)
            {
                int id = i;

                allLoadouts[id] = new Loadout();

                //Weapon selection
                allLoadouts[id].loadoutWeapons = new LoadoutWeapon[game.allWeaponCategories.Length];

                for (int j = 0; j < allLoadouts[id].loadoutWeapons.Length; j++)
                {
                    int jd = j;
                    allLoadouts[id].loadoutWeapons[jd] = new LoadoutWeapon();
                    //Set default one
                    allLoadouts[id].loadoutWeapons[jd].weaponID = game.defaultWeaponsInSlot[jd];

                    Weapons.Kit_ModernWeaponScript wpnRender = game.allWeapons[allLoadouts[id].loadoutWeapons[jd].weaponID] as Weapons.Kit_ModernWeaponScript;

                    //Kit_ModernWeaponScript attachments
                    if (wpnRender)
                    {
                        allLoadouts[id].loadoutWeapons[jd].attachments = new int[wpnRender.attachmentSlots.Length];
                    }
                    //No script with attachments.
                    else
                    {
                        allLoadouts[id].loadoutWeapons[jd].attachments = new int[0];
                    }
                }

                //Player Model selection
                allLoadouts[id].teamLoadout = new TeamLoadout[game.allPvpTeams.Length];

                for (int j = 0; j < allLoadouts[id].teamLoadout.Length; j++)
                {
                    //Setup Player Model Customization for default model
                    int jd = j;
                    allLoadouts[id].teamLoadout[jd] = new TeamLoadout();
                    allLoadouts[id].teamLoadout[jd].playerModelID = game.allPvpTeams[jd].playerModelDefault;
                    allLoadouts[id].teamLoadout[jd].playerModelCustomizations = new int[game.allPvpTeams[jd].playerModels[allLoadouts[id].teamLoadout[jd].playerModelID].prefab.GetComponent<Kit_ThirdPersonPlayerModel>().customizationSlots.Length];
                }
            }

            //Create Weapon Categories
            for (int i = 0; i < game.allWeaponCategories.Length; i++)
            {
                int id = i;
                //Create
                GameObject go = Instantiate(weaponCategoryPrefab, weaponCategoryGo, false);
                //Setup text
                TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                if (txt) txt.text = game.allWeaponCategories[id];
                //Setup Button
                Button btn = go.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate { WeaponSelectCategory(id); });
            }

            //Create Team selection for player model
            for (int i = 0; i < game.allPvpTeams.Length; i++)
            {
                int id = i;
                //Create
                GameObject go = Instantiate(playerModelTeamPrefab, playerModelTeamGo, false);
                //Setup text
                TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                if (txt) txt.text = game.allPvpTeams[id].teamName;
                //Setup Button
                Button btn = go.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate { PlayerModelSelectTeam(id); });
            }

            Load();
            //Redraw
            RedrawPlayerModel();
        }

        public override void Open()
        {
            if (amountOfLoadouts > 1)
            {
                if (menu)
                {
                    if (!menu.SwitchMenu(menuScreenId)) return;
                }

                if (Kit_IngameMain.instance)
                {
                    if (!Kit_IngameMain.instance.SwitchMenu(menuScreenId)) return;
                }
            }
            else
            {
                if (menu)
                {
                    if (!menu.SwitchMenu(menuScreenId)) return;
                }

                if (Kit_IngameMain.instance)
                {
                    if (!Kit_IngameMain.instance.SwitchMenu(menuScreenId)) return;
                }

                SwitchMenu(mainCategoryScreenID);
            }



            playerModelRenderCam.enabled = true;
            weaponPreviewRenderCam.enabled = true;

            isOpen = true;
        }

        public void Back()
        {
            if (currentSubScreen == loadoutScreenID)
            {
                ForceClose();
            }
            else if (currentSubScreen == mainCategoryScreenID)
            {
                if (amountOfLoadouts > 1)
                {
                    SwitchMenu(loadoutScreenID);
                }
                else
                {
                    ForceClose();
                }
            }
            else if (currentSubScreen == playerModelTeamScreenId)
            {
                SwitchMenu(mainCategoryScreenID);
            }
            else if (currentSubScreen == playerModelSelectionScreenId)
            {
                SwitchMenu(playerModelTeamScreenId);
            }
            else if (currentSubScreen == playerModelCustomizationCategoryScreenId)
            {
                if (!SwitchMenu(playerModelSelectionScreenId)) return;
            }
            else if (currentSubScreen == playerModelCustomizationScreenId)
            {
                if (!SwitchMenu(playerModelCustomizationCategoryScreenId)) return;
                //Reset
                playerModelRenderCamPosition = playerModelRenderCam.transform.parent.position;
            }
            else if (currentSubScreen == weaponCategoryScreenId)
            {
                SwitchMenu(mainCategoryScreenID);
            }
            else if (currentSubScreen == weaponSubCategoryScreenId)
            {
                SwitchMenu(weaponCategoryScreenId);
            }
            else if (currentSubScreen == weaponSelectionScreenId)
            {
                SwitchMenu(weaponSubCategoryScreenId);
            }
            else if (currentSubScreen == weaponCustomizationCategoryScreenId)
            {
                SwitchMenu(weaponSelectionScreenId);
            }
            else if (currentSubScreen == weaponCustomizationAttachmentScreenId)
            {
                SwitchMenu(weaponCustomizationCategoryScreenId);
            }

            Save();
        }

        public void SelectLoadout(int id)
        {
            currentlySelectedLoadout = id;
            SwitchMenu(mainCategoryScreenID);

            RedrawPlayerModel();
        }

        public void CategoryWeapons()
        {
            SwitchMenu(weaponCategoryScreenId);
        }

        public void WeaponSelectCategory(int id)
        {
            //Select category
            currentSelectedWeaponCategory = id;

            //Delete actives
            for (int i = 0; i < weaponSubCategoryActives.Count; i++)
            {
                Destroy(weaponSubCategoryActives[i]);
            }
            //Reset list
            weaponSubCategoryActives = new List<GameObject>();

            //Fetch subcategories for this weapon category
            List<string> subcategories = new List<string>();

            for (int i = 0; i < game.allWeapons.Length; i++)
            {
                if (game.allWeapons[i].CanBeSelectedInLoadout())
                {
                    //Check if weapon is for our category
                    if (game.allWeapons[i].weaponType == game.allWeaponCategories[currentSelectedWeaponCategory])
                    {
                        //Add our sub category if its not added yet
                        if (!subcategories.Contains(game.allWeapons[i].weaponLoadoutSubCategory))
                        {
                            subcategories.Add(game.allWeapons[i].weaponLoadoutSubCategory);
                        }
                    }
                }
            }

            for (int i = 0; i < subcategories.Count; i++)
            {
                int catId = i;
                //Create
                GameObject go = Instantiate(weaponSubCategoryPrefab, weaponSubCategoryGo, false);
                //Setup text
                TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                if (txt) txt.text = subcategories[catId];
                //Setup Button
                Button btn = go.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate { WeaponSelectSubcategory(id, subcategories[catId]); });
                //Add to list
                weaponSubCategoryActives.Add(go);
            }

            SwitchMenu(weaponSubCategoryScreenId);

            RedrawWeapon();
        }

        public void WeaponSelectSubcategory(int cat, string subCat)
        {
            //Destroy old ones
            for (int i = 0; i < weaponSelectionActives.Count; i++)
            {
                Destroy(weaponSelectionActives[i]);
            }
            //Reset list
            weaponSelectionActives = new List<GameObject>();

            for (int i = 0; i < game.allWeapons.Length; i++)
            {
                int id = i;
                if (game.allWeapons[id].CanBeSelectedInLoadout())
                {
                    //Check if weapon is for our category
                    if (game.allWeapons[id].weaponType == game.allWeaponCategories[currentSelectedWeaponCategory])
                    {
                        //Check if weapon is in that sub category
                        if (game.allWeapons[id].weaponLoadoutSubCategory == subCat)
                        {
                            //Create
                            GameObject go = Instantiate(weaponSelectionPrefab, weaponSelectionGo, false);
                            //Setup text
                            TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                            if (txt)
                            {
                                if (game.allWeapons[id].IsWeaponUnlocked(game))
                                {
                                    txt.text = game.allWeapons[id].weaponName;
                                }
                                else
                                {
                                    txt.text = game.allWeapons[id].weaponName + " [Unlocks at level " + game.allWeapons[id].levelToUnlockAt + "]";
                                }
                            }
                            //Setup Button
                            Button btn = go.GetComponentInChildren<Button>();
                            btn.onClick.AddListener(delegate { WeaponSelect(cat, id); });
                            //Add to list
                            weaponSelectionActives.Add(go);
                        }
                    }
                }
            }

            SwitchMenu(weaponSelectionScreenId);
        }

        public void WeaponSelect(int slot, int id)
        {
            //Check if weapon is unlocked
            if (game.allWeapons[id].IsWeaponUnlocked(game))
            {
                Weapons.Kit_ModernWeaponScript wpnRender = game.allWeapons[id] as Weapons.Kit_ModernWeaponScript;

                if (allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].weaponID != id)
                {         //Select
                    allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].weaponID = id;

                    //Kit_ModernWeaponScript attachments
                    if (wpnRender)
                    {
                        allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments = new int[wpnRender.attachmentSlots.Length];
                    }
                    //No script with attachments.
                    else
                    {
                        allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments = new int[0];
                    }
                }
                else
                {
                    if (wpnRender)
                    {
                        //Size doesnt match up
                        if (allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments.Length != wpnRender.attachmentSlots.Length)
                        {
                            allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments = new int[wpnRender.attachmentSlots.Length];
                        }
                        //Make sure no invalid selections are present
                        else
                        {
                            for (int i = 0; i < allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments.Length; i++)
                            {
                                //Clamp it up to a valid selection, 0 length is invalid configuration anyway
                                allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments[i] = Mathf.Clamp(allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments[i], 0, wpnRender.attachmentSlots[i].availableAttachments.Length - 1);
                            }
                        }
                    }
                    else
                    {
                        //No valid attachment possible
                        allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments = new int[0];
                    }
                }


                //Proceed to customization
                if (wpnRender && wpnRender.attachmentSlots.Length > 0)
                {
                    //Destroy
                    for (int i = 0; i < weaponCustomizationCategoryActives.Count; i++)
                    {
                        Destroy(weaponCustomizationCategoryActives[i]);
                    }
                    //Reset
                    weaponCustomizationCategoryActives = new List<GameObject>();

                    for (int i = 0; i < wpnRender.attachmentSlots.Length; i++)
                    {
                        int catId = i;
                        //Create
                        GameObject go = Instantiate(weaponCustomizationCategoryPrefab, weaponCustomizationCategoryGo, false);
                        //Setup text
                        TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                        if (txt) txt.text = wpnRender.attachmentSlots[catId].slotName;
                        //Setup Button
                        Button btn = go.GetComponentInChildren<Button>();
                        btn.onClick.AddListener(delegate { WeaponCustomizationSelectCategory(slot, id, catId); });
                        //Add to list
                        weaponCustomizationCategoryActives.Add(go);
                    }

                    SwitchMenu(weaponCustomizationCategoryScreenId);
                }

                //Redraw
                RedrawWeapon();
            }
        }

        public void WeaponCustomizationSelectCategory(int slot, int wep, int catId)
        {
            //Destroy
            for (int i = 0; i < weaponCustomizationAttachmentActives.Count; i++)
            {
                Destroy(weaponCustomizationAttachmentActives[i]);
            }
            //Reset
            weaponCustomizationAttachmentActives = new List<GameObject>();

            Weapons.Kit_ModernWeaponScript wpnRender = game.allWeapons[wep] as Weapons.Kit_ModernWeaponScript;

            if (wpnRender)
            {
                for (int i = 0; i < wpnRender.attachmentSlots[catId].availableAttachments.Length; i++)
                {
                    int id = i;
                    //Create
                    GameObject go = Instantiate(weaponCustomizationAttachmentPrefab, weaponCustomizationAttachmentGo, false);
                    //Setup text
                    TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                    if (txt) txt.text = wpnRender.attachmentSlots[catId].availableAttachments[id].displayName;
                    //Setup Button
                    Button btn = go.GetComponentInChildren<Button>();
                    btn.onClick.AddListener(delegate { SelectAttachment(slot, catId, id); });
                    //Add to list
                    weaponCustomizationAttachmentActives.Add(go);
                }

                SwitchMenu(weaponCustomizationAttachmentScreenId);
            }
        }

        public void SelectAttachment(int slot, int attachmentSlot, int attachment)
        {
            allLoadouts[currentlySelectedLoadout].loadoutWeapons[slot].attachments[attachmentSlot] = attachment;
            RedrawWeapon();
        }

        public void CategoryPlayerModel()
        {
            SwitchMenu(playerModelTeamScreenId);
        }

        public void PlayerModelSelectTeam(int team)
        {
            if (!SwitchMenu(playerModelSelectionScreenId)) return;
            currentSelectedTeamForPlayerModel = team;

            //Delete old ones
            for (int i = 0; i < playerModelSelectionActives.Count; i++)
            {
                Destroy(playerModelSelectionActives[i]);
            }
            //Reset list
            playerModelSelectionActives = new List<GameObject>();
            //List player models

            for (int i = 0; i < game.allPvpTeams[currentSelectedTeamForPlayerModel].playerModels.Length; i++)
            {
                int id = i;
                //Create
                GameObject go = Instantiate(playerModelSelectionPrefab, playerModelSelectionGo, false);
                //Setup text
                TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                if (txt) txt.text = game.allPvpTeams[currentSelectedTeamForPlayerModel].playerModels[id].displayName;
                //Setup Button
                Button btn = go.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate { PlayerModelSelect(team, id); });
                //Add to list
                playerModelSelectionActives.Add(go);
            }

            RedrawPlayerModel();
        }

        public void PlayerModelSelect(int team, int id)
        {
            Kit_ThirdPersonPlayerModel model = game.allPvpTeams[team].playerModels[id].prefab.GetComponent<Kit_ThirdPersonPlayerModel>();

            if (allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelID != id)
            {
                //Assign
                allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelID = id;
                //Customization slots
                allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelCustomizations = new int[model.customizationSlots.Length];
            }
            else
            {
                //Now this is where thigns can go wrong. We have to make SURE that the length is good AND that even if it is good, no invalid options are picked.

                //Check if length matches.
                if (allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelCustomizations.Length != model.customizationSlots.Length)
                {
                    allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelCustomizations = new int[model.customizationSlots.Length];
                }
                else
                {
                    //Make sure no invalid options are picked.

                    for (int i = 0; i < allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelCustomizations.Length; i++)
                    {
                        int cId = i;
                        //Clamp, a slot with 0 customizations is illegal anyway and a configuration error.
                        allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelCustomizations[cId] = Mathf.Clamp(allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelCustomizations[cId], 0, model.customizationSlots[cId].customizations.Length - 1);
                    }
                }
            }

            //Switch to customization menu :)
            if (allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelCustomizations.Length > 0)
            {
                if (!SwitchMenu(playerModelCustomizationCategoryScreenId)) return;

                for (int i = 0; i < playerModelCustomizationCategoryActives.Count; i++)
                {
                    Destroy(playerModelCustomizationCategoryActives[i]);
                }

                playerModelCustomizationCategoryActives = new List<GameObject>();

                for (int i = 0; i < model.customizationSlots.Length; i++)
                {
                    int cId = i;

                    //Create
                    GameObject go = Instantiate(playerModelCustomizationCategoryPrefab, playerModelCustomizationCategoryGo, false);
                    //Setup text
                    TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                    if (txt) txt.text = model.customizationSlots[cId].name;
                    //Setup Button
                    Button btn = go.GetComponentInChildren<Button>();
                    btn.onClick.AddListener(delegate { PlayerModelCustomizeSlot(team, id, cId); });
                    //Add to list
                    playerModelCustomizationCategoryActives.Add(go);
                }
            }

            RedrawPlayerModel();
        }

        public void PlayerModelCustomizeSlot(int team, int id, int slot)
        {
            SwitchMenu(playerModelCustomizationScreenId, true);

            //Get the model
            Kit_ThirdPersonPlayerModel model = game.allPvpTeams[team].playerModels[id].prefab.GetComponent<Kit_ThirdPersonPlayerModel>();
            //Look at the right thing
            playerModelRenderCamPosition = playerModelCurrent.customizationSlots[slot].uiPosition.position - playerModelRenderCam.transform.forward * playerModelRenderCamCustomizationDistance;

            for (int i = 0; i < playerModelCustomizationActives.Count; i++)
            {
                Destroy(playerModelCustomizationActives[i]);
            }

            playerModelCustomizationActives = new List<GameObject>();

            for (int i = 0; i < model.customizationSlots[slot].customizations.Length; i++)
            {
                int cId = i;

                //Create
                GameObject go = Instantiate(playerModelCustomizationPrefab, playerModelCustomizationGo, false);
                //Setup text
                TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                if (txt) txt.text = model.customizationSlots[slot].customizations[cId].name;
                //Setup Button
                Button btn = go.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate { PlayerModelChangeCustomize(team, slot, cId); });
                //Add to list
                playerModelCustomizationActives.Add(go);
            }
        }

        public void PlayerModelChangeCustomize(int team, int slot, int newCustomization)
        {
            //Update loadout
            allLoadouts[currentlySelectedLoadout].teamLoadout[team].playerModelCustomizations[slot] = newCustomization;
            //Only update customization
            RedrawPlayerModel(true);
        }

        public void RedrawPlayerModel(bool onlyUpdateCustomization = false)
        {
            if (!onlyUpdateCustomization)
            {
                if (playerModelCurrent)
                {
                    Destroy(playerModelCurrent.gameObject);
                }

                GameObject playerModel = Instantiate(game.allPvpTeams[currentSelectedTeamForPlayerModel].playerModels[allLoadouts[currentlySelectedLoadout].teamLoadout[currentSelectedTeamForPlayerModel].playerModelID].prefab, playerModelPreviewGo, false);

                playerModelCurrent = playerModel.GetComponent<Kit_ThirdPersonPlayerModel>();

                //Setup IK helper
                if (playerModelCurrent.anim)
                {
                    playerModelIkHelper = playerModelCurrent.anim.gameObject.AddComponent<Kit_LoadoutIKHelper>();
                    playerModelIkHelper.anim = playerModelCurrent.anim;
                    playerModelIkHelper.applyIk = false;
                }

                //Set Anim Type
                playerModelCurrent.SetAnimType(game.allWeapons[allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].weaponID].thirdPersonAnimType, true);

                RedrawWeapon();
            }
            //Apply customization
            playerModelCurrent.SetCustomizations(allLoadouts[currentlySelectedLoadout].teamLoadout[currentSelectedTeamForPlayerModel].playerModelCustomizations, null);
            RedrawWeapon();
        }

        public void RedrawWeapon()
        {
            if (weaponPreviewObjectThirdPerson)
            {
                //Destroy old one
                Destroy(weaponPreviewObjectThirdPerson);
            }

            //Create new third person one
            weaponPreviewObjectThirdPerson = Instantiate(game.allWeapons[allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].weaponID].thirdPersonPrefab, playerModelCurrent.weaponsInHandsGo, false);

            //Set Anim Type
            playerModelCurrent.SetAnimType(game.allWeapons[allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].weaponID].thirdPersonAnimType, true);

            Weapons.Kit_ThirdPersonWeaponRenderer tpWpnRender = weaponPreviewObjectThirdPerson.GetComponent<Weapons.Kit_ThirdPersonWeaponRenderer>();

            //Set attachments
            if (tpWpnRender)
            {
                tpWpnRender.SetAttachments(allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments, game.allWeapons[allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].weaponID] as Weapons.Kit_ModernWeaponScript, null, null);

                if (tpWpnRender.leftHandIK.Length > 0)
                {
                    playerModelIkHelper.leftHandGoal = tpWpnRender.leftHandIK[Mathf.Clamp(playerModelCurrent.inverseKinematicID, 0, tpWpnRender.leftHandIK.Length - 1)];
                    playerModelIkHelper.applyIk = true;
                }
                else
                {
                    playerModelIkHelper.applyIk = false;
                }
            }

            if (weaponPreviewObjectFirstPerson)
            {
                Destroy(weaponPreviewObjectFirstPerson);
            }

            weaponPreviewObjectFirstPerson = Instantiate(game.allWeapons[allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].weaponID].firstPersonPrefab, weaponPreviewFirstPersonGo, false);

            Weapons.Kit_WeaponRenderer wpnRender = weaponPreviewObjectFirstPerson.GetComponent<Weapons.Kit_WeaponRenderer>();

            if (wpnRender)
            {
                if (wpnRender.anim) wpnRender.anim.enabled = false;

                for (int i = 0; i < wpnRender.animAdditionals.Length; i++)
                {
                    wpnRender.animAdditionals[i].enabled = false;
                }

                if (wpnRender.legacyAnim) wpnRender.legacyAnim.enabled = false;

                for (int i = 0; i < wpnRender.hideInCustomiazionMenu.Length; i++)
                {
                    wpnRender.hideInCustomiazionMenu[i].enabled = false;
                }

                wpnRender.transform.localPosition = wpnRender.customizationMenuOffset;

                wpnRender.SetAttachments(allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].attachments, game.allWeapons[allLoadouts[currentlySelectedLoadout].loadoutWeapons[currentSelectedWeaponCategory].weaponID] as Weapons.Kit_ModernWeaponScript, null, null);
            }

            Weapons.Kit_MeleeRenderer mlRender = weaponPreviewObjectFirstPerson.GetComponent<Weapons.Kit_MeleeRenderer>();

            if (mlRender)
            {
                if (mlRender.anim) mlRender.anim.enabled = false;
                if (mlRender.legacyAnim) mlRender.legacyAnim.enabled = false;

                for (int i = 0; i < mlRender.hideInCustomiazionMenu.Length; i++)
                {
                    mlRender.hideInCustomiazionMenu[i].enabled = false;
                }

                mlRender.transform.localPosition = mlRender.customizationMenuOffset;
            }

            Weapons.Kit_GrenadeRenderer grRender = weaponPreviewObjectFirstPerson.GetComponent<Weapons.Kit_GrenadeRenderer>();

            if (grRender)
            {
                if (grRender.anim) grRender.anim.enabled = false;
                if (grRender.legacyAnim) grRender.legacyAnim.enabled = false;

                for (int i = 0; i < grRender.hideInCustomiazionMenu.Length; i++)
                {
                    grRender.hideInCustomiazionMenu[i].enabled = false;
                }

                grRender.transform.localPosition = grRender.customizationMenuOffset;
            }
        }

        #region Save Load
        /// <summary>
        /// Did we load at least ONCE already?
        /// </summary>
        private bool wasLoaded;

        public void Load()
        {
            for (int i = 0; i < allLoadouts.Length; i++)
            {
                int id = i;

                //Check if a saved loadout for this slot is present
                if (PlayerPrefs.HasKey("loadout_" + id + "_weapons"))
                {
                    int amountOfWeapons = PlayerPrefs.GetInt("loadout_" + id + "_weapons", 0);

                    //Slot configuration matches. Very important!
                    if (game.allWeaponCategories.Length == allLoadouts[id].loadoutWeapons.Length)
                    {
                        for (int j = 0; j < allLoadouts[id].loadoutWeapons.Length; j++)
                        {
                            int jd = j;

                            int savedWeapon = PlayerPrefs.GetInt("loadout_" + id + "_weapon_" + jd, 0);
                            string savedWeaponName = PlayerPrefs.GetString("loadout_" + id + "_weapon_" + jd + "_name", "");

                            //Check if loaded weapon is valid
                            if (savedWeapon < game.allWeapons.Length)
                            {
                                //Check if it is the same weapon
                                if (game.allWeapons[savedWeapon].weaponName == savedWeaponName)
                                {
                                    //It is the same weapon, assign it!
                                    allLoadouts[id].loadoutWeapons[jd].weaponID = savedWeapon;

                                    //Attachment loading
                                    int[] savedAttachments = PlayerPrefsExtended.GetIntArray("loadout_" + id + "_weapon_" + jd + "_attachments", 0, 0);

                                    Weapons.Kit_ModernWeaponScript wpnRender = game.allWeapons[savedWeapon] as Weapons.Kit_ModernWeaponScript;

                                    //Kit_ModernWeaponScript attachments
                                    if (wpnRender)
                                    {
                                        //Check if size matches
                                        if (savedAttachments.Length == wpnRender.attachmentSlots.Length)
                                        {
                                            //Size matches, assign
                                            allLoadouts[id].loadoutWeapons[jd].attachments = savedAttachments;
                                            //Then clamp!
                                            for (int o = 0; o < allLoadouts[id].loadoutWeapons[jd].attachments.Length; o++)
                                            {
                                                int od = o;
                                                allLoadouts[id].loadoutWeapons[jd].attachments[od] = Mathf.Clamp(allLoadouts[id].loadoutWeapons[jd].attachments[od], 0, wpnRender.attachmentSlots[od].availableAttachments.Length - 1);
                                            }
                                        }
                                        else
                                        {
                                            //Doesn't match, reset.
                                            allLoadouts[id].loadoutWeapons[jd].attachments = new int[wpnRender.attachmentSlots.Length];
                                        }
                                    }
                                    else
                                    {
                                        //No attachments supported, doesn't matter.
                                        allLoadouts[id].loadoutWeapons[jd].attachments = new int[0];
                                    }
                                }
                            }
                        }
                    }

                    int amountOfTeams = PlayerPrefs.GetInt("loadout_" + id + "_teams", 0);

                    //Team configuration matches
                    if (amountOfTeams == allLoadouts[id].teamLoadout.Length)
                    {
                        for (int j = 0; j < allLoadouts[id].teamLoadout.Length; j++)
                        {
                            int jd = j;
                            int savedPlayerModel = PlayerPrefs.GetInt("loadout_" + id + "_pm_" + jd, 0);

                            //Check if it's a valid one
                            if (savedPlayerModel < game.allPvpTeams[jd].playerModels.Length)
                            {
                                string savedPlayerModelName = PlayerPrefs.GetString("loadout_" + id + "_pm_" + jd + "_name", "");

                                //Check if it's still the same one
                                if (game.allPvpTeams[jd].playerModels[savedPlayerModel].displayName == savedPlayerModelName)
                                {
                                    //It is! Assign it!
                                    allLoadouts[id].teamLoadout[jd].playerModelID = savedPlayerModel;

                                    Kit_ThirdPersonPlayerModel model = game.allPvpTeams[jd].playerModels[savedPlayerModel].prefab.GetComponent<Kit_ThirdPersonPlayerModel>();

                                    //Load customization
                                    int[] savedPlayerModelCustomization = PlayerPrefsExtended.GetIntArray("loadout_" + id + "_pm_" + jd + "_customization", 0, 0);

                                    //Check if size of customizations matches
                                    if (savedPlayerModelCustomization.Length == model.customizationSlots.Length)
                                    {
                                        //Size matches, assign
                                        allLoadouts[id].teamLoadout[jd].playerModelCustomizations = savedPlayerModelCustomization;
                                        //Then clamp
                                        for (int o = 0; o < allLoadouts[id].teamLoadout[jd].playerModelCustomizations.Length; o++)
                                        {
                                            int od = o;
                                            allLoadouts[id].teamLoadout[jd].playerModelCustomizations[od] = Mathf.Clamp(allLoadouts[id].teamLoadout[jd].playerModelCustomizations[od], 0, model.customizationSlots[od].customizations.Length - 1);
                                        }
                                    }
                                    else
                                    {
                                        //It doesn't, create new array
                                        allLoadouts[id].teamLoadout[jd].playerModelCustomizations = new int[model.customizationSlots.Length];
                                    }
                                }
                            }
                        }
                    }
                }

            }

            int selectedLoadout = PlayerPrefs.GetInt("loadoutSelected", 0);

            //Check if it can be selected
            if (selectedLoadout < allLoadouts.Length)
            {
                currentlySelectedLoadout = selectedLoadout;
            }

            wasLoaded = true;
        }

        public void Save()
        {
            if (wasLoaded)
            {
                for (int i = 0; i < allLoadouts.Length; i++)
                {
                    int id = i;
                    PlayerPrefs.SetInt("loadout_" + id + "_weapons", allLoadouts[id].loadoutWeapons.Length);
                    for (int j = 0; j < allLoadouts[id].loadoutWeapons.Length; j++)
                    {
                        int jd = j;
                        PlayerPrefs.SetString("loadout_" + id + "_weapon_" + jd + "_name", game.allWeapons[allLoadouts[id].loadoutWeapons[jd].weaponID].weaponName);
                        PlayerPrefs.SetInt("loadout_" + id + "_weapon_" + jd, allLoadouts[id].loadoutWeapons[jd].weaponID);
                        PlayerPrefsExtended.SetIntArray("loadout_" + id + "_weapon_" + jd + "_attachments", allLoadouts[id].loadoutWeapons[jd].attachments);
                    }

                    PlayerPrefs.SetInt("loadout_" + id + "_teams", allLoadouts[id].teamLoadout.Length);

                    for (int j = 0; j < allLoadouts[id].teamLoadout.Length; j++)
                    {
                        int jd = j;
                        PlayerPrefs.SetString("loadout_" + id + "_pm_" + jd + "_name", game.allPvpTeams[jd].playerModels[allLoadouts[id].teamLoadout[jd].playerModelID].displayName);
                        PlayerPrefs.SetInt("loadout_" + id + "_pm_" + jd, allLoadouts[id].teamLoadout[jd].playerModelID);
                        PlayerPrefsExtended.SetIntArray("loadout_" + id + "_pm_" + jd + "_customization", allLoadouts[id].teamLoadout[jd].playerModelCustomizations);
                    }
                }

                PlayerPrefs.SetInt("loadoutSelected", currentlySelectedLoadout);
            }
        }
        #endregion

        #region SubMenu
        /// <summary>
        /// Switch to the given menu
        /// </summary>
        /// <param name="newMenu"></param>
        /// <returns></returns>
        public bool SwitchMenu(int newMenu)
        {
            if (!isSwitchingScreens)
            {
                //Start the coroutine
                currentlySwitchingScreensTo = StartCoroutine(SwitchRoutine(newMenu));
                //We are now switching
                return true;
            }

            //Not able to switch screens
            return false;
        }

        /// <summary>
        /// Switch to the given menu
        /// </summary>
        /// <param name="newMenu"></param>
        /// <returns></returns>
        public bool SwitchMenu(int newMenu, bool force)
        {
            if (!isSwitchingScreens || force)
            {
                if (force)
                {
                    if (currentlySwitchingScreensTo != null)
                    {
                        StopCoroutine(currentlySwitchingScreensTo);
                    }

                    //Make sure all correct ones ARE disabled
                    //Disable all the roots
                    for (int i = 0; i < subMenus.Length; i++)
                    {
                        if (i != currentSubScreen)
                        {
                            if (subMenus[i].root)
                            {
                                //Disable
                                subMenus[i].root.SetActive(false);
                            }
                        }
                    }
                }

                //Start the coroutine
                currentlySwitchingScreensTo = StartCoroutine(SwitchRoutine(newMenu));
                //We are now switching
                return true;
            }

            //Not able to switch screens
            return false;
        }

        private IEnumerator SwitchRoutine(int newMenu)
        {
            //Set bool
            isSwitchingScreens = true;
            //Fade out screen
            //Play Animation
            subMenus[currentSubScreen].anim.Play("Fade Out", 0, 0f);
            //Wait
            yield return new WaitForSeconds(subMenus[currentSubScreen].fadeOutLength);
            subMenus[currentSubScreen].root.SetActive(false);

            //Fade in new screen
            //Set screen
            currentSubScreen = newMenu;
            //Disable
            subMenus[currentSubScreen].root.SetActive(true);
            //Play Animation
            subMenus[currentSubScreen].anim.Play("Fade In", 0, 0f);
            //Wait
            yield return new WaitForSeconds(subMenus[currentSubScreen].fadeInLength);
            //Done
            isSwitchingScreens = false;
        }
        #endregion
    }
}