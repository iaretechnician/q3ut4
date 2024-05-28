using System;
using System.Collections.Generic;
using MarsFPSKit.Weapons;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MarsFPSKit
{
    public class Kit_PlayerHUD : Kit_PlayerHUDBase
    {
        /// <summary>
        /// This is the root object of hideable HUD-elements
        /// </summary>
        public GameObject root;

        /// <summary>
        /// Reference to our canvas
        /// </summary>
        public Canvas canvas;

        /// <summary>
        /// The root of the HP display
        /// </summary>
        [Header("Health")]
        public GameObject healthRoot;
        /// <summary>
        /// How much HP do we have left?
        /// </summary>
        public TextMeshProUGUI healthText;

        /// <summary>
        /// Root of bullets
        /// </summary>
        [Header("Ammo")]
        public GameObject bulletsRoot;
        /// <summary>
        /// How many bullets are left in the magazine?
        /// </summary>
        public TextMeshProUGUI bulletsLeft;
        /// <summary>
        /// How many bullets do we have left to reload?
        /// </summary>
        public TextMeshProUGUI bulletsLeftToReload; //It's a stylistic decision to split it up, you can do it in one text, if you like.

        [Header("Crosshair")]
        /// <summary>
        /// The root object of the crosshair, so that it can be hidden if needed.
        /// </summary>
        public GameObject crosshairRoot;
        /// <summary>
        /// The left part of the crosshair
        /// </summary>
        public Image crosshairLeft;
        /// <summary>
        /// The right part of the crosshair
        /// </summary>
        public Image crosshairRight;
        /// <summary>
        /// The upper part of the crosshair
        /// </summary>
        public Image crosshairUp;
        /// <summary>
        /// The lower part of the crosshair
        /// </summary>
        public Image crosshairDown;
        /// <summary>
        /// Root
        /// </summary>
        public RectTransform crosshairMoveRoot;

        [Header("Bloody Screen")]
        /// <summary>
        /// The bloody screen effect when getting hit
        /// </summary>
        public Image bloodyScreen;

        [Header("Hitmarker")]
        public Image hitmarkerImage;
        /// <summary>
        /// How long is a hitmarker going to be displayed?
        /// </summary>
        public float hitmarkerTime;
        /// <summary>
        /// Sound that is going to be played when we hit someone
        /// </summary>
        public AudioClip hitmarkerSound;
        /// <summary>
        /// Audio source for <see cref="hitmarkerSound"/>
        /// </summary>
        public AudioSource hitmarkerAudioSource;
        /// <summary>
        /// At which <see cref="Time.time"/> is the hitmarker going to be completely invisible
        /// </summary>
        private float hitmarkerLastDisplay;
        /// <summary>
        /// Hitmarker color cache.
        /// </summary>
        private Color hitmarkerColor;

        [Header("Hitmarker Spawn Protected")]
        public Image hitmarkerSpawnProtectionImage;
        /// <summary>
        /// How long is a hitmarker going to be displayed?
        /// </summary>
        public float hitmarkerSpawnProtectionTime;
        /// <summary>
        /// Sound that is going to be played when we hit someone
        /// </summary>
        public AudioClip hitmarkerSpawnProtectionSound;
        /// <summary>
        /// Audio source for <see cref="hitmarkerSound"/>
        /// </summary>
        public AudioSource hitmarkerSpawnProtectionAudioSource;
        /// <summary>
        /// At which <see cref="Time.time"/> is the hitmarker going to be completely invisible
        /// </summary>
        private float hitmarkerSpawnProtectionLastDisplay;
        /// <summary>
        /// Hitmarker color cache.
        /// </summary>
        private Color hitmarkerSpawnProtectionColor;

        [Header("Damage Indicator")]
        /// <summary>
        /// The transform which is going to be rotated on the UI
        /// </summary>
        public RectTransform indicatorRotate;
        /// <summary>
        /// The image of the indicator to apply the alpha to
        /// </summary>
        public Image indicatorImage;
        /// <summary>
        /// An object which the player's position is going to be copied to. Parent of the helper.
        /// </summary>
        public Transform indicatorHelperRoot;
        /// <summary>
        /// A helper transform which looks at the last direction we were shot from
        /// </summary>
        public Transform indicatorHelper;
        /// <summary>
        /// How long is the damage indicator going to be visible?
        /// </summary>
        public float indicatorVisibleTime = 5f;
        /// <summary>
        /// Current alpha of the indicator
        /// </summary>
        private float indicatorAlpha;
        /// <summary>
        /// Current position we were shot from last time
        /// </summary>
        private Vector3 indicatorLastPos;

        [Header("Sniper Scope")]
        /// <summary>
        /// The root object of the sniper scope
        /// </summary>
        public GameObject sniperScopeRoot;
        /// <summary>
        /// A help boolean to only set the <see cref="sniperScopeRoot"/> active once
        /// </summary>
        private bool wasSniperScopeActive;

        [Header("Waiting for Players")]
        /// <summary>
        /// Root object of the 'Waiting for players'
        /// </summary>
        public GameObject waitingForPlayersRoot;

        [Header("Player Name Markers")]
        public List<Kit_PlayerMarker> allPlayerMarkers = new List<Kit_PlayerMarker>();
        /// <summary>
        /// Prefab for player markers
        /// </summary>
        public GameObject playerMarkerPrefab;
        /// <summary>
        /// Where do the player markers go?
        /// </summary>
        public RectTransform playerMarkerGo;
        /// <summary>
        /// Color used for friendly markers
        /// </summary>
        public Color friendlyMarkerColor = Color.white;
        /// <summary>
        /// Color used for enemy markers
        /// </summary>
        public Color enemyMarkerColor = Color.red;

        [Header("Spawn Protection")]
        /// <summary>
        /// The root object of the spawn protection
        /// </summary>
        public GameObject spRoot;
        /// <summary>
        /// This displays the time left of the spawn protection
        /// </summary>
        public TextMeshProUGUI spText;

        [Header("Weapon Pickup")]
        /// <summary>
        /// This displays the weapon pickup
        /// </summary>
        public GameObject weaponPickupRoot;
        /// <summary>
        /// This displays the weapon that is being picked up
        /// </summary>
        public TextMeshProUGUI weaponPickupText;

        [Header("Interaction")]
        /// <summary>
        /// This displays the interaction
        /// </summary>
        public GameObject interactionRoot;
        /// <summary>
        /// This displays the weapon that is being picked up
        /// </summary>
        public TextMeshProUGUI interactionText;

        /// <summary>
        /// Canvas group to fade in / out the auxiliary bar
        /// </summary>
        [Header("Stamina Bar")]
        public CanvasGroup staminaGroup;
        /// <summary>
        /// Bar to fill with stamina
        /// </summary>
        public Image staminaProgress;
        /// <summary>
        /// How fast will stamina fade in / out
        /// </summary>
        public float staminaAlphaFadeSpeed = 2f;

        /// <summary>
        /// Canvas group to fade in / out the auxiliary bar
        /// </summary>
        [Header("Auxiliary Bar")]
        public CanvasGroup auxiliaryGroup;
        /// <summary>
        /// Bar to fill with auxiliary
        /// </summary>
        public Image auxiliaryProgress;
        /// <summary>
        /// How fast will auxiliary fade in / out
        /// </summary>
        public float auxiliaryAlphaFadeSpeed = 2f;
        /// <summary>
        /// When was it used?
        /// </summary>
        public float auxiliaryUsedAt;

        /// <summary>
        /// Image that displays it!
        /// </summary>
        [Header("Movement Icon")]
        public Image movementIcon;
        /// <summary>
        /// Displayed when we are standing
        /// </summary>
        public Sprite movementStanding;
        /// <summary>
        /// Displayed when we are crouching
        /// </summary>
        public Sprite movementCrouching;

        /// <summary>
        /// This is just white!
        /// </summary>
        [Header("Flashbang Blind")]
        public Image flashbangWhite;
        /// <summary>
        /// This displays the screenshot!
        /// </summary>
        public RawImage flashbangScreenshot;
        /// <summary>
        /// How much time is left until we recover from the blind?
        /// </summary>
        private float flashbangTimeLeft;
        /// <summary>
        /// Sound that plays the high pitched noise
        /// </summary>
        public AudioSource flashbangSource;

        /// <summary>
        /// Prefab for weapon display
        /// </summary>
        [Header("Weapon Display")]
        public GameObject weaponDisplayPrefab;
        /// <summary>
        /// Where they go!
        /// </summary>
        public RectTransform weaponDisplayGo;
        /// <summary>
        /// List of active weapon displays!
        /// </summary>
        public List<Image> weaponDisplayActives = new List<Image>();
        /// <summary>
        /// When weapon is selected
        /// </summary>
        public Color weaponDisplaySelectedColor = Color.black;
        /// <summary>
        /// When weapon is not selected
        /// </summary>
        public Color weaponDisplayUnselectedColor = Color.white;

        /// <summary>
        /// Prefab for weapon display
        /// </summary>
        [Header("Weapon Quick Use Display")]
        public GameObject weaponQuickUseDisplayPrefab;
        /// <summary>
        /// Where they go!
        /// </summary>
        public RectTransform weaponQuickUseDisplayGo;
        /// <summary>
        /// List of active weapon displays!
        /// </summary>
        public List<Image> weaponQuickUseDisplayActives = new List<Image>();

        /// <summary>
        /// Are we underwater?
        /// </summary>
        [Header("Underwater Post Processing")]
        public GameObject underwaterPostProcessing;

        /// <summary>
        /// Text for leaving battlefield!
        /// </summary>
        [Header("Leaving Battlefield")]
        public TextMeshProUGUI leavingBattlefieldText;

        #region Unity Calls
        void Awake()
        {
            //Cache color
            hitmarkerColor = hitmarkerImage.color;
            //SpawnProtection
            hitmarkerSpawnProtectionColor = hitmarkerSpawnProtectionImage.color;
        }

        void Update()
        {
            //Update hitmarker alpha
            hitmarkerColor.a = Mathf.Clamp01(hitmarkerLastDisplay - Time.time);
            //Set the color
            hitmarkerImage.color = hitmarkerColor;

            //Update hitmarker SP alpha
            hitmarkerSpawnProtectionColor.a = Mathf.Clamp01(hitmarkerSpawnProtectionLastDisplay - Time.time);
            //Set the color
            hitmarkerSpawnProtectionImage.color = hitmarkerSpawnProtectionColor;

            //Check if stamina shall be displayed
            if (!Mathf.Approximately(staminaProgress.fillAmount, 1f))
            {
                if (staminaGroup.alpha < 1f)
                {
                    //Increase alpha
                    staminaGroup.alpha += Time.deltaTime * staminaAlphaFadeSpeed;
                }
            }
            else
            {
                if (staminaGroup.alpha > 0f)
                {
                    //Decrase alpha
                    staminaGroup.alpha -= Time.deltaTime * staminaAlphaFadeSpeed;
                }
            }

            //Check if auxiliary shall be displayed
            if (auxiliaryUsedAt + 3 > Time.time)
            {
                if (auxiliaryGroup.alpha < 1f)
                {
                    //Increase alpha
                    auxiliaryGroup.alpha += Time.deltaTime * auxiliaryAlphaFadeSpeed;
                }
            }
            else
            {
                if (auxiliaryGroup.alpha > 0f)
                {
                    //Decrase alpha
                    auxiliaryGroup.alpha -= Time.deltaTime * auxiliaryAlphaFadeSpeed;
                }
            }
        }
        #endregion

        #region Custom Calls
        /// <summary>
        /// Shows or hides the HUD. Some parts (such as the hitmarker) will always be visible.
        /// </summary>
        /// <param name="visible"></param>
        public override void SetVisibility(bool visible)
        {
            //Update the active state of root, but only if it doesn't have it already.
            if (root)
            {
                if (visible)
                {
                    if (!root.activeSelf) root.SetActive(true);
                }
                else
                {
                    if (root.activeSelf) root.SetActive(false);
                    //Hide spawn protection too
                    if (spRoot.activeSelf) spRoot.SetActive(false);
                    //Hide underwater too
                    DisplayUnderwater(false);
                    //Hide Battlefield
                    DisplayLeavingBattlefield(-1);
                }
            }
        }

        public override void DisplayLeavingBattlefield(float timeLeft)
        {
            if (timeLeft < 0)
            {
                leavingBattlefieldText.enabled = false;
            }
            else
            {
                leavingBattlefieldText.text = "YOU ARE LEAVING THE BATTLEFIELD. YOU WILL DIE IN " + timeLeft.ToString("F1");
                leavingBattlefieldText.enabled = true;
            }
        }

        public override void DisplayUnderwater(bool isUnderwater)
        {
            //Just show/hide post processing :)
            underwaterPostProcessing.SetActiveOptimized(isUnderwater);
        }

        public override void DisplayMovementState(int state)
        {
            if (state == 0)
            {
                movementIcon.sprite = movementStanding;
            }
            else if (state == 1)
            {
                movementIcon.sprite = movementCrouching;
            }
        }

        public override void SetWaitingStatus(bool isWaiting)
        {
            if (waitingForPlayersRoot.activeSelf != isWaiting)
            {
                //Set to the required state
                waitingForPlayersRoot.SetActive(isWaiting);
            }
        }

        public override void PlayerStart(Kit_PlayerBehaviour pb)
        {
            indicatorAlpha = 0f;
            //Update state
            wasSniperScopeActive = false;
            //Set state accordingly
            sniperScopeRoot.SetActive(false);
            staminaGroup.alpha = 0f;
            auxiliaryGroup.alpha = 0f;
            flashbangTimeLeft = 0f;
            flashbangScreenshot.color = new Color(1, 1, 1, 0f);
            flashbangWhite.color = new Color(1, 1, 1, 0f);
            //Start sound
            flashbangSource.volume = 0f;
            flashbangSource.loop = true;
            flashbangSource.Play();
        }

        public override void PlayerEnd(Kit_PlayerBehaviour pb)
        {
            if (flashbangSource)
            {
                //Set sound to 0
                flashbangSource.volume = 0f;
                flashbangSource.Stop();
            }
        }

        public override void PlayerUpdate(Kit_PlayerBehaviour pb)
        {
            //Position damage indicator
            indicatorHelperRoot.position = pb.transform.position;
            indicatorHelperRoot.rotation = pb.transform.rotation;
            //Look at
            indicatorHelper.LookAt(indicatorLastPos);
            //Decrease alpha
            if (indicatorAlpha > 0f) indicatorAlpha -= Time.deltaTime;
            //Set alpha
            indicatorImage.color = new Color(1f, 1f, 1f, indicatorAlpha);
            //Set rotation 
            indicatorRotate.localRotation = Quaternion.Euler(0f, 0f, -indicatorHelper.localEulerAngles.y);

            if (flashbangTimeLeft >= 0)
            {
                //Set Color
                flashbangScreenshot.color = new Color(1, 1, 1, flashbangTimeLeft / 2f);
                flashbangWhite.color = new Color(1, 1, 1, Mathf.Clamp(flashbangTimeLeft / 3f, 0, 0.6f));
                flashbangSource.volume = flashbangTimeLeft;

                flashbangTimeLeft -= Time.deltaTime;
            }
            else
            {
                flashbangScreenshot.color = new Color(1, 1, 1, 0f);
                flashbangWhite.color = new Color(1, 1, 1, 0f);
                flashbangSource.volume = 0f;
            }
        }

        /// <summary>
        /// Displays the hitmarker for <see cref="hitmarkerTime"/> seconds
        /// </summary>
        public override void DisplayHitmarker()
        {
            hitmarkerLastDisplay = Time.time + hitmarkerTime; //Set time of the hitmarker, which makes it visible automatically
            //Play sound
            if (hitmarkerSound)
            {
                hitmarkerAudioSource.clip = hitmarkerSound;
                hitmarkerAudioSource.PlayOneShot(hitmarkerSound);
            }
        }

        public override void DisplayHitmarkerSpawnProtected()
        {
            hitmarkerSpawnProtectionLastDisplay = Time.time + hitmarkerSpawnProtectionTime; //Set time of the hitmarker, which makes it visible automatically
            //Play sound
            if (hitmarkerSpawnProtectionSound)
            {
                hitmarkerSpawnProtectionAudioSource.clip = hitmarkerSpawnProtectionSound;
                hitmarkerSpawnProtectionAudioSource.PlayOneShot(hitmarkerSpawnProtectionSound);
            }
        }

        /// <summary>
        /// Display hit points in the HUD
        /// </summary>
        /// <param name="hp">Amount of hitpoints</param>
        public override void DisplayHealth(float hp)
        {
            if (hp >= 0f)
            {
                if (!healthRoot.activeSelf) healthRoot.SetActive(true);
                //Display the HP
                healthText.text = hp.ToString("F0"); //If you want decimals, change it to F1, F2, etc...
            }
            else
            {
                if (healthRoot.activeSelf) healthRoot.SetActive(false);
            }
        }

        /// <summary>
        /// Display ammo count in the HUD
        /// </summary>
        /// <param name="bl">Bullets left (On the left side)</param>
        /// <param name="bltr">Bullets left to reload (On the right side)</param>
        public override void DisplayAmmo(int bl, int bltr, bool show = true)
        {
            if (show)
            {
                if (bl >= 0)
                {
                    //Set text for bullets left
                    bulletsLeft.text = bl.ToString("F0");
                }
                else
                {
                    bulletsLeft.text = "";
                }
                if (bltr >= 0)
                {
                    //Set text for bullets left to reload
                    bulletsLeftToReload.text = bltr.ToString("F0");
                }
                else
                {
                    bulletsLeftToReload.text = "";
                }

                if (!bulletsRoot.activeSelf) bulletsRoot.SetActive(true);
            }
            else
            {
                if (bulletsRoot.activeSelf) bulletsRoot.SetActive(false);
            }
        }

        public override void DisplayCrosshair(float size, bool overrideShow)
        {
            //For zero or smaller,
            if (size <= 0f && !overrideShow)
            {
                //Hide it
                crosshairLeft.enabled = false;
                crosshairRight.enabled = false;
                crosshairUp.enabled = false;
                crosshairDown.enabled = false;
            }
            else
            {
                //Show it
                crosshairLeft.enabled = true;
                crosshairRight.enabled = true;
                crosshairUp.enabled = true;
                crosshairDown.enabled = true;

                //Position all crosshair parts accordingly
                crosshairLeft.rectTransform.anchoredPosition = new Vector2 { x = size };
                crosshairRight.rectTransform.anchoredPosition = new Vector2 { x = -size };
                crosshairUp.rectTransform.anchoredPosition = new Vector2 { y = size };
                crosshairDown.rectTransform.anchoredPosition = new Vector2 { y = -size };
            }
        }

        public override void MoveCrosshairTo(Vector3 pos)
        {
            crosshairMoveRoot.anchoredPosition3D = pos;
        }

        public override void DisplayWeaponsAndQuickUses(Kit_PlayerBehaviour pb, Kit_ModernWeaponManagerNetworkData runtimeData)
        {
            List<WeaponDisplayData> weaponDisplayData = new List<WeaponDisplayData>();
            List<WeaponQuickUseDisplayData> weaponQuickUseDisplayData = new List<WeaponQuickUseDisplayData>();

            //Get Data from Weapon Manager!
            for (int i = 0; i < runtimeData.weaponsInUseSync.Count; i++)
            {
                Kit_WeaponRuntimeDataBase weaponData = runtimeData.GetWeapon(i);
                //Get from weapons!
                WeaponDisplayData wdd = weaponData.behaviour.GetWeaponDisplayData(pb, weaponData);
                WeaponQuickUseDisplayData wqudd = weaponData.behaviour.GetWeaponQuickUseDisplayData(pb, weaponData);

                //Add if weapon supports it!
                if (wdd != null)
                {
                    //Check if this weapon is selected atm!
                    if (runtimeData.currentWeapon == i)
                    {
                        wdd.selected = true;
                    }
                    else
                    {
                        wdd.selected = false;
                    }
                    weaponDisplayData.Add(wdd);
                }

                //Add if weapon supports it!
                if (wqudd != null)
                {
                    weaponQuickUseDisplayData.Add(wqudd);
                }
            }

            //Make sure list length if correct!
            if (weaponDisplayData.Count != weaponDisplayActives.Count)
            {
                while (weaponDisplayData.Count != weaponDisplayActives.Count)
                {
                    if (weaponDisplayActives.Count > weaponDisplayData.Count)
                    {
                        Destroy(weaponDisplayActives[weaponDisplayActives.Count - 1].gameObject);
                        //Remove
                        weaponDisplayActives.RemoveAt(weaponDisplayActives.Count - 1);
                    }
                    else if (weaponDisplayActives.Count < weaponDisplayData.Count)
                    {
                        //Add new
                        GameObject go = Instantiate(weaponDisplayPrefab, weaponDisplayGo, false);
                        //Get
                        Image img = go.GetComponent<Image>();
                        //Add
                        weaponDisplayActives.Add(img);
                    }
                }
            }

            //Now length is correct, redraw!
            for (int i = 0; i < weaponDisplayData.Count; i++)
            {
                weaponDisplayActives[i].sprite = weaponDisplayData[i].sprite;
                //Set correct color
                if (weaponDisplayData[i].selected)
                {
                    weaponDisplayActives[i].color = weaponDisplaySelectedColor;
                }
                else
                {
                    weaponDisplayActives[i].color = weaponDisplayUnselectedColor;
                }
            }

            int totalQuickUseDisplayLength = 0;

            for (int i = 0; i < weaponQuickUseDisplayData.Count; i++)
            {
                totalQuickUseDisplayLength += weaponQuickUseDisplayData[i].amount;
            }

            //Make sure list length if correct!
            if (totalQuickUseDisplayLength != weaponQuickUseDisplayActives.Count)
            {
                while (totalQuickUseDisplayLength != weaponQuickUseDisplayActives.Count)
                {
                    if (weaponQuickUseDisplayActives.Count > totalQuickUseDisplayLength)
                    {
                        Destroy(weaponQuickUseDisplayActives[weaponQuickUseDisplayActives.Count - 1].gameObject);
                        //Remove
                        weaponQuickUseDisplayActives.RemoveAt(weaponQuickUseDisplayActives.Count - 1);
                    }
                    else if (weaponQuickUseDisplayActives.Count < totalQuickUseDisplayLength)
                    {
                        //Add new
                        GameObject go = Instantiate(weaponQuickUseDisplayPrefab, weaponQuickUseDisplayGo, false);
                        //Get
                        Image img = go.GetComponent<Image>();
                        //Add
                        weaponQuickUseDisplayActives.Add(img);
                    }
                }
            }

            int currentIndex = 0;

            //Now length is correct, redraw!
            for (int i = 0; i < weaponQuickUseDisplayData.Count; i++)
            {
                for (int o = 0; o < weaponQuickUseDisplayData[i].amount; o++)
                {
                    weaponQuickUseDisplayActives[currentIndex].sprite = weaponQuickUseDisplayData[i].sprite;
                    currentIndex++;
                }
            }
        }

        public override void DisplayHurtState(float state)
        {
            //Update bloody screen
            bloodyScreen.color = new Color(1, 1, 1, state);
        }

        public override void DisplayShot(Vector3 from)
        {
            //Set pos
            indicatorLastPos = from;
            //Set alpha
            indicatorAlpha = indicatorVisibleTime;
        }

        /// <summary>
        /// Should we grab the screen for flashbang?
        /// </summary>
        bool grab = false;

        float flashbangTimeForGrab;

        public void OnEnable()
        {
            // register the callback when enabling object
            Camera.onPostRender += FlashbangPostRender;
        }

        public void OnDisable()
        {
            // remove the callback when disabling object
            Camera.onPostRender -= FlashbangPostRender;
        }

        private void FlashbangPostRender(Camera cam)
        {
            //Check if its the main camera
            if (cam.CompareTag("MainCamera"))
            {
                if (grab)
                {
                    Texture2D tex = new Texture2D(Screen.width, Screen.height);
                    tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                    tex.Apply();
                    flashbangScreenshot.texture = tex;
                    //Reset the grab state
                    grab = false;
                    //Set time, this needs to be here otherwise the screenshot will be white too!
                    flashbangTimeLeft = flashbangTimeForGrab;
                }
            }
        }

        public override void DisplayBlind(float time)
        {
            //Set time
            flashbangTimeForGrab = time;
            grab = true;

            //Play if not
            flashbangSource.loop = true;
            flashbangSource.Play();

            Debug.Log("Blinded");
        }

        public override void DisplaySniperScope(bool display)
        {
            //Check if the state changed
            if (display != wasSniperScopeActive)
            {
                //Update state
                wasSniperScopeActive = display;
                //Set state accordingly
                sniperScopeRoot.SetActive(display);
            }
        }

        public override void DisplayWeaponPickup(bool displayed, int weapon = -1)
        {
            if (displayed)
            {
                if (!weaponPickupRoot.activeSelf)
                    weaponPickupRoot.SetActive(true);
                if (weapon >= 0)
                {
                    //Set name
                    weaponPickupText.text = "Press [F] to pickup: " + Kit_IngameMain.instance.gameInformation.allWeapons[weapon].weaponName;
                }
            }
            else
            {
                if (weaponPickupRoot.activeSelf)
                    weaponPickupRoot.SetActive(false);
            }
        }

        public override void DisplayInteraction(bool display, string txt = "")
        {
            if (display)
            {
                if (!interactionRoot.activeSelf) interactionRoot.SetActive(true);
                //Set
                interactionText.text = "Press [F] to: " + txt;
            }
            else
            {
                if (interactionRoot.activeSelf) interactionRoot.SetActive(false);
            }
        }

        public override void DisplayStamina(float stamina)
        {
            //Set progress
            staminaProgress.fillAmount = (stamina / 100f);
        }

        public override void DisplayAuxiliaryBar(float fill)
        {
            if (fill > 0)
            {
                //Set progress
                auxiliaryProgress.fillAmount = fill;
                auxiliaryUsedAt = Time.time;
            }
            else
                auxiliaryUsedAt = 0f;
        }

        public override int GetUnusedPlayerMarker()
        {
            for (int i = 0; i < allPlayerMarkers.Count; i++)
            {
                //Check if its not used
                if (!allPlayerMarkers[i].used)
                {
                    //If its not, set it to used
                    allPlayerMarkers[i].used = true;
                    //Activate its root
                    allPlayerMarkers[i].markerRoot.gameObject.SetActive(true);
                    //And return its id
                    return i;
                }
            }
            //If not, add a new one and return that one
            GameObject newMarker = Instantiate(playerMarkerPrefab, playerMarkerGo, false);
            //Reset scale
            newMarker.transform.localScale = Vector3.one;
            //Add
            allPlayerMarkers.Add(newMarker.GetComponent<Kit_PlayerMarker>());
            allPlayerMarkers[allPlayerMarkers.Count - 1].used = true;
            allPlayerMarkers[allPlayerMarkers.Count - 1].markerRoot.gameObject.SetActive(true);
            return allPlayerMarkers.Count - 1;
        }

        public override void ReleasePlayerMarker(int id)
        {
            if (allPlayerMarkers[id].markerRoot)
            {
                //Deactivate its root
                allPlayerMarkers[id].markerRoot.gameObject.SetActive(false);
            }
            //And set it to unused
            allPlayerMarkers[id].used = false;
        }

        public override void UpdatePlayerMarker(int id, PlayerNameState state, Vector3 worldPos, string playerName)
        {
            //Get screen pos
            Vector3 canvasPos = canvas.WorldToCanvas(worldPos, Kit_IngameMain.instance.mainCamera);
            //Set
            allPlayerMarkers[id].markerRoot.anchoredPosition3D = canvasPos;
            //Check if it is visible at all
            if (canvasPos.z > 0)
            {
                //Check the state
                if (state == PlayerNameState.friendlyClose)
                {
                    //Set name
                    allPlayerMarkers[id].markerText.text = playerName;
                    //Set color
                    allPlayerMarkers[id].markerText.color = friendlyMarkerColor;
                    //Display name
                    allPlayerMarkers[id].markerText.enabled = true;
                    //Dont display marker
                    allPlayerMarkers[id].markerArrow.enabled = false;
                }
                else if (state == PlayerNameState.friendlyFar)
                {
                    //Display marker
                    allPlayerMarkers[id].markerArrow.enabled = true;
                    //Dont display name
                    allPlayerMarkers[id].markerText.enabled = false;
                }
                else if (state == PlayerNameState.enemy)
                {
                    //Set name
                    allPlayerMarkers[id].markerText.text = playerName;
                    //Set color
                    allPlayerMarkers[id].markerText.color = enemyMarkerColor;
                    //Display name
                    allPlayerMarkers[id].markerText.enabled = true;
                    //Dont display marker
                    allPlayerMarkers[id].markerArrow.enabled = false;
                }
                else
                {
                    //Hide all
                    allPlayerMarkers[id].markerText.enabled = false;
                    allPlayerMarkers[id].markerArrow.enabled = false;
                }
            }
            //If its not...
            else
            {
                //...hide all
                allPlayerMarkers[id].markerText.enabled = false;
                allPlayerMarkers[id].markerArrow.enabled = false;
            }
        }

        public override void UpdateSpawnProtection(bool isActive, float timeLeft)
        {
            if (isActive)
            {
                //Activate root
                if (!spRoot.activeSelf) spRoot.SetActive(true);
                //Set time
                spText.text = timeLeft.ToString("F1");
            }
            else
            {
                //Deactivate root
                if (spRoot.activeSelf) spRoot.SetActive(false);
            }
        }
        #endregion
    }
}
