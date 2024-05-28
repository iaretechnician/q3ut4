
using System;
using UnityEngine;

namespace MarsFPSKit
{
    public class BasicMouseLookRuntimeData
    {
        public float mouseX; //Rotation on Unity Y-Axis (Player Object)
        public float mouseY; //Rotation on Unity X-Axis (Camera/Weapons)

        public float recoilMouseX; //Recoil on x axis
        public float recoilMouseY; //Recoil on y axis

        public float finalMouseX; //Rotation on Unity Y-Axis with recoil applied
        public float finalMouseY; //Rotation on Unity X-Axis with recoil applied

        /// <summary>
        /// How are we currently leaning -1 (L) to 1 (R)
        /// </summary>
        public float leaningState;

        public Quaternion leaningSmoothState = Quaternion.identity;

        /// <summary>
        /// Last input of the third person button
        /// </summary>
        public bool lastThirdPersonButton;

        /// <summary>
        /// Were we aiming?
        /// </summary>
        public bool wasAimingLast;

        /// <summary>
        /// 0 = First person pos
        /// 1 = Third person pos
        /// </summary>
        public float firstPersonThirdPersonBlend;

        /// <summary>
        /// Hit for perspective
        /// </summary>
        public RaycastHit perspectiveClippingAvoidmentHit;

        /// <summary>
        /// Hit for crosshair
        /// </summary>
        public RaycastHit worldPositionCrosshair;

        /// <summary>
        /// The currently desired perspective
        /// </summary>
        public Kit_GameInformation.Perspective desiredPerspective = Kit_GameInformation.Perspective.FirstPerson;

        /// <summary>
        /// The currently active perspective
        /// </summary>
        public Kit_GameInformation.Perspective currentPerspective = Kit_GameInformation.Perspective.FirstPerson;
    }

    /// <summary>
    /// Implements a basic Mouse Look on the X and Y axis
    /// </summary>
    [CreateAssetMenu(menuName = "MarsFPSKit/Looking/Basic Mouse Look")]
    public class Kit_BasicMouseLook : Kit_MouseLookBase
    {

        //The basic sensitivity (Can not be changed by the user by default)
        [Header("Basic Sensitivity")]
        [Tooltip("Input multiplier for x rotation")]
        public float basicSensitivityX = 1f;
        [Tooltip("Input multiplier for y rotation")]
        public float basicSensitivityY = 1f;

        [Header("Limits")]
        public float minY = -85f; //Minimum for y looking
        public float maxY = 85f; //Maximum for y looking

        [Header("Leaning")]
        /// <summary>
        /// Is leaning enabled?
        /// </summary>
        public bool leaningEnabled;

        public Vector3 leftLeanPos;
        public Vector3 leftLeanRot;
        public Vector3 leftLeanWepPos;
        public Vector3 leftLeanWepRot;

        public Vector3 rightLeanPos;
        public Vector3 rightLeanRot;
        public Vector3 rightLeanWepPos;
        public Vector3 rightLeanWepRot;

        public float leaningSpeedMultiplier = 0.7f;

        public float leaningRotationSmoothSpeed = 15f;

        /// <summary>
        /// How fast is the leaning state going to thcange
        /// </summary>
        public float leaningSpeed = 2f;

        /// <summary>
        /// Position of the camera in first person
        /// </summary>
        [Header("Perspective")]
        public Vector3 cameraFirstPersonPosition;
        /// <summary>
        /// Position of the camera in third person
        /// </summary>
        public Vector3 cameraThirdPersonPosition;

        /// <summary>
        /// How fast is the transition from first to third person (and vice versa)?
        /// </summary>
        public float firstThirdPersonChangeSpeed = 4f;

        /// <summary>
        /// Should clipping be avoided in third person?
        /// </summary>
        [Header("Clipping Avoidment")]
        public bool enableCameraClippingAvoidment = true;
        /// <summary>
        /// Mask for clipping avoidment
        /// </summary>
        public LayerMask clippingAvoidmentMask;
        /// <summary>
        /// Multiplied with normal = offset
        /// </summary>
        public float clippingAvoidmentCorrection = 0.01f;

        /// <summary>
        /// Should the crosshair be repositioned in third person view to match where we are actually aiming?
        /// </summary>
        [Header("World Position Crosshair")]
        public bool enableWorldPositionCrosshair = true;
        /// <summary>
        /// Mask of the world position crosshair
        /// </summary>
        public LayerMask worldPositionCrosshairMask;
        /// <summary>
        /// Maximum distance that the world position crosshair can hit
        /// </summary>
        public float worldPositionCrosshairMaxDistance = 100f;

        public override void InitializeServer(Kit_PlayerBehaviour pb)
        {
            //Assign
            pb.customMouseLookData = new BasicMouseLookRuntimeData();
            //Get our custom data
            BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;
        }

        public override void InitializeClient(Kit_PlayerBehaviour pb)
        {
            //Assign
            pb.customMouseLookData = new BasicMouseLookRuntimeData();
            //Get our custom data
            BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;
            //Set initial look pos
            data.mouseX = pb.transform.localEulerAngles.y;
            //Set default
            if (Kit_IngameMain.instance.gameInformation.perspectiveMode == Kit_GameInformation.PerspectiveMode.FirstPersonOnly || Kit_IngameMain.instance.gameInformation.perspectiveMode == Kit_GameInformation.PerspectiveMode.Both && Kit_IngameMain.instance.gameInformation.defaultPerspective == Kit_GameInformation.Perspective.FirstPerson)
            {
                //Set desired
                data.desiredPerspective = Kit_GameInformation.Perspective.FirstPerson;
                //Set current
                data.currentPerspective = Kit_GameInformation.Perspective.FirstPerson;
                //Set Blend
                data.firstPersonThirdPersonBlend = 0f;
            }
            else if (Kit_IngameMain.instance.gameInformation.perspectiveMode == Kit_GameInformation.PerspectiveMode.ThirdPersonOnly || Kit_IngameMain.instance.gameInformation.perspectiveMode == Kit_GameInformation.PerspectiveMode.Both && Kit_IngameMain.instance.gameInformation.defaultPerspective == Kit_GameInformation.Perspective.ThirdPerson)
            {
                //Set desired
                data.desiredPerspective = Kit_GameInformation.Perspective.ThirdPerson;
                //Set current
                data.currentPerspective = Kit_GameInformation.Perspective.ThirdPerson;
                //Set Blend
                data.firstPersonThirdPersonBlend = 1f;
            }
        }

        public override void TakeControl(Kit_PlayerBehaviour pb)
        {
            //Get our custom data
            BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;

            if (pb.isFirstPersonActive)
            {
                //Update
                UpdatePerspectiveScripts(pb, data.currentPerspective);
            }
        }

        public override void AuthorativeInput(Kit_PlayerBehaviour pb, Kit_PlayerInput input, float delta, double revertTime)
        {
            //Get our custom data
            BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;

            data.mouseY = pb.input.mouseY;
            data.mouseX = pb.input.mouseX;

            if (leaningEnabled)
            {
                if (!pb.movement.IsRunning(pb))
                {
                    if (pb.input.leanLeft)
                    {
                        data.leaningState = Mathf.MoveTowards(data.leaningState, -1f, Time.deltaTime * leaningSpeed);
                    }
                    else if (pb.input.leanRight)
                    {
                        data.leaningState = Mathf.MoveTowards(data.leaningState, 1f, Time.deltaTime * leaningSpeed);
                    }
                    else
                    {
                        data.leaningState = Mathf.MoveTowards(data.leaningState, 0f, Time.deltaTime * leaningSpeed);
                    }
                }
                else
                {
                    data.leaningState = Mathf.MoveTowards(data.leaningState, 0f, Time.deltaTime * leaningSpeed);
                }
            }
            else
            {
                data.leaningState = 0f;
            }

            //Calculate recoil
            if (pb.recoilApplyRotation.eulerAngles.x < 90)
            {
                data.recoilMouseY = pb.recoilApplyRotation.eulerAngles.x;
            }
            else
            {
                data.recoilMouseY = pb.recoilApplyRotation.eulerAngles.x - 360;
            }

            if (pb.recoilApplyRotation.eulerAngles.y < 90)
            {
                data.recoilMouseX = -pb.recoilApplyRotation.eulerAngles.y;
            }
            else
            {
                data.recoilMouseX = -(pb.recoilApplyRotation.eulerAngles.y - 360);
            }

            if (data.recoilMouseY + data.mouseY > maxY)
            {
                data.mouseY -= ((data.recoilMouseY + data.mouseY) - maxY);
            }

            //Clamp y input
            data.mouseY = Mathf.Clamp(data.mouseY, minY, maxY);
            //Apply reocil
            data.finalMouseY = Mathf.Clamp(data.mouseY + data.recoilMouseY, minY, maxY);

            //Simplify x input
            data.mouseX %= 360;
            //Apply recoil
            data.finalMouseX = data.mouseX + data.recoilMouseX;

            if (!pb.isBot) //Bots cannot handle input like this, so they will need to assign it themselves.
            {
                //Apply rotation
                pb.transform.rotation = Quaternion.Euler(new Vector3(0, data.finalMouseX, 0f));
                //Smooth leaning
                data.leaningSmoothState = Quaternion.Slerp(data.leaningSmoothState, GetCameraRotationOffset(pb), Time.deltaTime * leaningRotationSmoothSpeed);
                pb.mouseLookObject.localRotation = Quaternion.Euler(-data.finalMouseY, 0, 0) * data.leaningSmoothState;
            }
            else
            {
                data.finalMouseY = -pb.mouseLookObject.localEulerAngles.x;
                if (data.finalMouseY < -180) data.finalMouseY += 360;
                data.mouseY = data.finalMouseY;
            }

            #region Perspective Management
            if (Kit_IngameMain.instance.gameInformation.perspectiveMode == Kit_GameInformation.PerspectiveMode.Both)
            {
                //Check if input changed
                if (data.lastThirdPersonButton != pb.input.thirdPerson)
                {
                    data.lastThirdPersonButton = pb.input.thirdPerson;

                    //Check if button is pressed
                    if (pb.input.thirdPerson)
                    {
                        //Change perspective
                        if (data.desiredPerspective == Kit_GameInformation.Perspective.FirstPerson) data.desiredPerspective = Kit_GameInformation.Perspective.ThirdPerson;
                        else data.desiredPerspective = Kit_GameInformation.Perspective.FirstPerson;
                    }
                }
            }
            #endregion
        }

        public override void PredictionInput(Kit_PlayerBehaviour pb, Kit_PlayerInput input, float delta)
        {
            AuthorativeInput(pb, input, delta, 0);
        }

        public override void NotControllerUpdate(Kit_PlayerBehaviour pb)
        {

        }

        public override void Visuals(Kit_PlayerBehaviour pb)
        {
            //Get our custom data
            BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;

            if (Kit_IngameMain.instance.gameInformation.thirdPersonAiming == Kit_GameInformation.ThirdPersonAiming.GoIntoFirstPerson && pb.weaponManager.IsAiming(pb) || pb.weaponManager.ForceIntoFirstPerson(pb))
            {
                if (data.firstPersonThirdPersonBlend >= 0) data.firstPersonThirdPersonBlend -= Time.deltaTime / pb.weaponManager.AimInTime(pb);
            }
            else if (data.desiredPerspective == Kit_GameInformation.Perspective.FirstPerson)
            {
                if (data.firstPersonThirdPersonBlend >= 0) data.firstPersonThirdPersonBlend -= Time.deltaTime * firstThirdPersonChangeSpeed;
            }
            else
            {
                if (data.firstPersonThirdPersonBlend <= 1) data.firstPersonThirdPersonBlend += Time.deltaTime * firstThirdPersonChangeSpeed;
            }

            //Clamp
            data.firstPersonThirdPersonBlend = Mathf.Clamp(data.firstPersonThirdPersonBlend, 0f, 1f);

            if (Mathf.Approximately(data.firstPersonThirdPersonBlend, 0f) && data.currentPerspective != Kit_GameInformation.Perspective.FirstPerson)
            {
                //Set
                data.currentPerspective = Kit_GameInformation.Perspective.FirstPerson;
                //Update
                UpdatePerspectiveScripts(pb, Kit_GameInformation.Perspective.FirstPerson);
            }

            if (!Mathf.Approximately(data.firstPersonThirdPersonBlend, 0f) && data.currentPerspective != Kit_GameInformation.Perspective.ThirdPerson)
            {
                //Set
                data.currentPerspective = Kit_GameInformation.Perspective.ThirdPerson;
                //Update
                UpdatePerspectiveScripts(pb, Kit_GameInformation.Perspective.ThirdPerson);
            }

            if (Kit_IngameMain.instance.activeCameraTransform == pb.playerCameraTransform)
            {
                if (enableCameraClippingAvoidment)
                {
                    if (data.currentPerspective == Kit_GameInformation.Perspective.ThirdPerson)
                    {
                        if (Physics.Linecast(pb.playerCameraTransform.transform.position, pb.playerCameraTransform.TransformPoint(cameraThirdPersonPosition), out data.perspectiveClippingAvoidmentHit, clippingAvoidmentMask.value, QueryTriggerInteraction.Ignore))
                        {
                            Kit_IngameMain.instance.mainCamera.transform.localPosition = Vector3.Lerp(cameraFirstPersonPosition, Kit_IngameMain.instance.mainCamera.transform.parent.InverseTransformPoint(data.perspectiveClippingAvoidmentHit.point + data.perspectiveClippingAvoidmentHit.normal * clippingAvoidmentCorrection), data.firstPersonThirdPersonBlend);
                        }
                        else
                        {
                            Kit_IngameMain.instance.mainCamera.transform.localPosition = Vector3.Lerp(cameraFirstPersonPosition, cameraThirdPersonPosition, data.firstPersonThirdPersonBlend);
                        }
                    }
                    else
                    {
                        Kit_IngameMain.instance.mainCamera.transform.localPosition = Vector3.Lerp(cameraFirstPersonPosition, cameraThirdPersonPosition, data.firstPersonThirdPersonBlend);
                    }
                }
                else
                {
                    Kit_IngameMain.instance.mainCamera.transform.localPosition = Vector3.Lerp(cameraFirstPersonPosition, cameraThirdPersonPosition, data.firstPersonThirdPersonBlend);
                }
            }

            if (enableWorldPositionCrosshair)
            {
                if (data.currentPerspective == Kit_GameInformation.Perspective.ThirdPerson)
                {
                    //This is currently not supported
                    /*
                    if (Kit_IngameMain.instance.gameInformation.thirdPersonCameraShooting)
                    {
                        if (Physics.Raycast(Kit_IngameMain.instance.mainCamera.transform.position, Kit_IngameMain.instance.mainCamera.transform.forward, out data.worldPositionCrosshair, worldPositionCrosshairMaxDistance, worldPositionCrosshairMask.value))
                        {
                            //Get screen pos
                            Vector3 canvasPos = Kit_IngameMain.instance.canvas.WorldToCanvas(data.worldPositionCrosshair.point, Kit_IngameMain.instance.mainCamera);
                            Kit_IngameMain.instance.hud.MoveCrosshairTo(canvasPos);
                        }
                        else
                        {
                            //Get screen pos
                            Vector3 canvasPos = Kit_IngameMain.instance.canvas.WorldToCanvas(Kit_IngameMain.instance.mainCamera.transform.position + Kit_IngameMain.instance.mainCamera.transform.forward * 100f, Kit_IngameMain.instance.mainCamera);
                            Kit_IngameMain.instance.hud.MoveCrosshairTo(canvasPos);
                        }
                    }
                    else
                    */
                    {
                        if (Physics.Raycast(pb.playerCameraTransform.transform.position, pb.playerCameraTransform.transform.forward, out data.worldPositionCrosshair, worldPositionCrosshairMaxDistance, worldPositionCrosshairMask.value))
                        {
                            //Get screen pos
                            Vector3 canvasPos = Kit_IngameMain.instance.canvas.WorldToCanvas(data.worldPositionCrosshair.point, Kit_IngameMain.instance.mainCamera);
                            Kit_IngameMain.instance.hud.MoveCrosshairTo(canvasPos);
                        }
                        else
                        {
                            //Get screen pos
                            Vector3 canvasPos = Kit_IngameMain.instance.canvas.WorldToCanvas(pb.playerCameraTransform.transform.position + pb.playerCameraTransform.transform.forward * 100f, Kit_IngameMain.instance.mainCamera);
                            Kit_IngameMain.instance.hud.MoveCrosshairTo(canvasPos);
                        }
                    }
                }
                else
                {
                    //Reset to center
                    Kit_IngameMain.instance.hud.MoveCrosshairTo(Vector3.zero);
                }
            }
        }

        void UpdatePerspectiveScripts(Kit_PlayerBehaviour pb, Kit_GameInformation.Perspective perspective)
        {
            if (pb.isFirstPersonActive)
            {
                //Tell weapon manager
                pb.weaponManager.FirstThirdPersonChanged(pb, perspective);
                //Tell player model
                pb.thirdPersonPlayerModel.FirstThirdPersonChanged(pb, perspective);
            }
        }

        public override bool ReachedYMax(Kit_PlayerBehaviour pb)
        {
            //Check if correct object is used by the player
            if (pb.customMouseLookData != null && pb.customMouseLookData.GetType() == typeof(BasicMouseLookRuntimeData))
            {
                //Get our custom data
                BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;

                //Check if we reached min or max value on Y
                if (Mathf.Approximately(data.finalMouseY, minY)) return true;
                else if (Mathf.Approximately(data.finalMouseY, maxY)) return true;
            }

            return false;
        }

        public override float GetSpeedMultiplier(Kit_PlayerBehaviour pb)
        {
            //Check if correct object is used by the player
            if (pb.customMouseLookData != null && pb.customMouseLookData.GetType() == typeof(BasicMouseLookRuntimeData))
            {
                //Get our custom data
                BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;
                return Mathf.Lerp(1f, leaningSpeedMultiplier, Mathf.Abs(data.leaningState));
            }
            return 1f;
        }

        public override Vector3 GetCameraOffset(Kit_PlayerBehaviour pb)
        {
            //Check if correct object is used by the player
            if (pb.customMouseLookData != null && pb.customMouseLookData.GetType() == typeof(BasicMouseLookRuntimeData))
            {
                //Get our custom data
                BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;
                if (data.leaningState > 0)
                {
                    return Vector3.Lerp(Vector3.zero, rightLeanPos, data.leaningState);
                }
                else
                {
                    return Vector3.Lerp(Vector3.zero, leftLeanPos, Mathf.Abs(data.leaningState));
                }
            }
            return base.GetCameraOffset(pb);
        }

        public override Quaternion GetCameraRotationOffset(Kit_PlayerBehaviour pb)
        {
            //Check if correct object is used by the player
            if (pb.customMouseLookData != null && pb.customMouseLookData.GetType() == typeof(BasicMouseLookRuntimeData))
            {
                //Get our custom data
                BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;
                if (data.leaningState > 0)
                {
                    return Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(rightLeanRot), data.leaningState);
                }
                else
                {
                    return Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(leftLeanRot), Mathf.Abs(data.leaningState));
                }
            }
            return base.GetCameraRotationOffset(pb);
        }

        public override Vector3 GetWeaponOffset(Kit_PlayerBehaviour pb)
        {
            if (pb.weaponManager.IsAiming(pb))
            {
                return Vector3.zero;
            }
            else
            {
                //Check if correct object is used by the player
                if (pb.customMouseLookData != null && pb.customMouseLookData.GetType() == typeof(BasicMouseLookRuntimeData))
                {
                    //Get our custom data
                    BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;
                    if (data.leaningState > 0)
                    {
                        return Vector3.Lerp(Vector3.zero, rightLeanWepPos, data.leaningState);
                    }
                    else
                    {
                        return Vector3.Lerp(Vector3.zero, leftLeanWepPos, Mathf.Abs(data.leaningState));
                    }
                }
            }
            return base.GetWeaponOffset(pb);
        }

        public override Quaternion GetWeaponRotationOffset(Kit_PlayerBehaviour pb)
        {
            if (pb.weaponManager.IsAiming(pb))
            {
                return Quaternion.identity;
            }
            else
            {
                //Check if correct object is used by the player
                if (pb.customMouseLookData != null && pb.customMouseLookData.GetType() == typeof(BasicMouseLookRuntimeData))
                {
                    //Get our custom data
                    BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;
                    if (data.leaningState > 0)
                    {
                        return Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(rightLeanWepRot), data.leaningState);
                    }
                    else
                    {
                        return Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(leftLeanWepRot), Mathf.Abs(data.leaningState));
                    }
                }
            }
            return base.GetWeaponRotationOffset(pb);
        }

        public override Kit_GameInformation.Perspective GetPerspective(Kit_PlayerBehaviour pb)
        {
            if (pb.isFirstPersonActive)
            {
                //Check if correct object is used by the player
                if (pb.customMouseLookData != null && pb.customMouseLookData.GetType() == typeof(BasicMouseLookRuntimeData))
                {
                    //Get our custom data
                    BasicMouseLookRuntimeData data = pb.customMouseLookData as BasicMouseLookRuntimeData;
                    //Return whats saved
                    return data.currentPerspective;
                }
            }
            else
            {
                return Kit_GameInformation.Perspective.ThirdPerson;
            }
            return Kit_GameInformation.Perspective.FirstPerson;
        }
    }
}
