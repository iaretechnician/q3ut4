using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace MarsFPSKit
{
    public class Kit_DeathCameraThirdPerson : Kit_DeathCameraBase
    {
        /// <summary>
        /// Where to look at
        /// </summary>
        public Transform lookAtTransform;
        /// <summary>
        /// How quickly does the camera react
        /// </summary>
        public float lookAtSmooth = 5f;
        /// <summary>
        /// Reference distance for FOV
        /// </summary>
        public float distanceFovReference = 30f;
        /// <summary>
        /// Smallest FOV the camera will fade to
        /// </summary>
        public float smallestFov = 40f;
        /// <summary>
        /// How long until we call the game mode death cam over? Alternatively the death cam ending will also call it.
        /// </summary>
        public float timeUntilGameModeDeathCameraOverCall = 3f;
        /// <summary>
        /// Did we call the game mode?
        /// </summary>
        private bool wasGameModeCalled;
        /// <summary>
        /// Where we died
        /// </summary>
        private Vector3 deathPos;
        /// <summary>
        /// Was this thing set up?
        /// </summary>
        private bool wasSetup;
        /// <summary>
        /// Time when to call game mode
        /// </summary>
        private float callGameModeTime = float.MaxValue;

        private void Update()
        {
            if (lookAtTransform && !Kit_IngameMain.instance.myPlayer && Kit_IngameMain.instance.mainCamera.transform.parent == null)
            {
                Kit_IngameMain.instance.mainCamera.transform.position = deathPos;
                //Kit_IngameMain.instance.mainCamera.transform.forward = Vector3.Slerp(Kit_IngameMain.instance.mainCamera.transform.forward, lookAtTransform.position - Kit_IngameMain.instance.mainCamera.transform.position, Time.deltaTime * lookAtSmooth);
                Kit_IngameMain.instance.mainCamera.transform.rotation = Quaternion.Slerp(Kit_IngameMain.instance.mainCamera.transform.rotation, Quaternion.LookRotation(lookAtTransform.position - Kit_IngameMain.instance.mainCamera.transform.position), Time.deltaTime * lookAtSmooth);

                Kit_IngameMain.instance.mainCamera.fieldOfView = Mathf.Lerp(Kit_IngameMain.instance.mainCamera.fieldOfView, Mathf.Lerp(Kit_GameSettings.baseFov, smallestFov, Vector3.Distance(Kit_IngameMain.instance.mainCamera.transform.position, lookAtTransform.position) / distanceFovReference), Time.deltaTime * lookAtSmooth);
            }

            if (Kit_IngameMain.instance.myPlayer)
            {
                enabled = false;
                Kit_IngameMain.instance.isCameraFovOverridden = false;
                wasSetup = false;
            }

            if (!wasGameModeCalled && wasSetup)
            {
                if (Time.time > callGameModeTime)
                {
                    if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                    {
                        //Call Game Mode
                        Kit_IngameMain.instance.currentPvPGameModeBehaviour.OnLocalPlayerDeathCameraEnded();
                    }
                    else if (Kit_IngameMain.instance.currentPvEGameModeBehaviour)
                    {
                        //Call Game Mode
                        Kit_IngameMain.instance.currentPvEGameModeBehaviour.OnLocalPlayerDeathCameraEnded();
                    }
                    wasGameModeCalled = true;
                }
            }
        }

        public override void SetupDeathCamera(Kit_ThirdPersonPlayerModel model)
        {
            //This is geared towards the modern player model
            Kit_ThirdPersonModernPlayerModel modernModel = model as Kit_ThirdPersonModernPlayerModel;
            Kit_IngameMain.instance.activeCameraTransform = null;
            Kit_IngameMain.instance.isCameraFovOverridden = true;
            deathPos = modernModel.kpb.playerCameraTransform.position;
            Kit_IngameMain.instance.mainCamera.transform.position = modernModel.kpb.playerCameraTransform.position;
            Kit_IngameMain.instance.mainCamera.transform.rotation = modernModel.kpb.playerCameraTransform.rotation;
            //Show
            for (int i = 0; i < modernModel.fpShadowOnlyRenderers.Length; i++)
            {
                //Make renderers visible again!
                modernModel.fpShadowOnlyRenderers[i].shadowCastingMode = ShadowCastingMode.On;
            }

            //Set time
            callGameModeTime = Time.time + timeUntilGameModeDeathCameraOverCall;
            //Enable call back
            enabled = true;
            //Set bool
            wasSetup = true;
        }

        private void OnDestroy()
        {
            if (wasSetup && !Kit_SceneSyncer.instance.isLoading)
            {
                Kit_IngameMain.instance.isCameraFovOverridden = false;
                if (!Kit_IngameMain.instance.myPlayer && Kit_IngameMain.instance.activeCameraTransform == null)
                {
                    Kit_IngameMain.instance.activeCameraTransform = Kit_IngameMain.instance.spawnCameraPosition;
                    if (!wasGameModeCalled)
                    {
                        if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                        {
                            //Call Game Mode
                            Kit_IngameMain.instance.currentPvPGameModeBehaviour.OnLocalPlayerDeathCameraEnded();
                        }
                        else if (Kit_IngameMain.instance.currentPvEGameModeBehaviour)
                        {
                            //Call Game Mode
                            Kit_IngameMain.instance.currentPvEGameModeBehaviour.OnLocalPlayerDeathCameraEnded();
                        }
                        wasGameModeCalled = true;
                    }
                }
            }
        }
    }
}