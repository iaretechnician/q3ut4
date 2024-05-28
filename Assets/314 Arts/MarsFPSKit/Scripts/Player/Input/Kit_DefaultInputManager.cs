using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    public class DefaultInputData
    {

    }

    [CreateAssetMenu(menuName = "MarsFPSKit/Input Manager/Default")]
    /// <summary>
    /// This is the kit's default input manager
    /// </summary>
    public class Kit_DefaultInputManager : Kit_InputManagerBase
    {
        public string[] weaponSlotKeys;

        public override void InitializeServer(Kit_PlayerBehaviour pb)
        {
            DefaultInputData did = new DefaultInputData();
            pb.inputManagerData = did;
            pb.input.weaponSlotUses = new bool[weaponSlotKeys.Length];
        }

        public override void InitializeClient(Kit_PlayerBehaviour pb)
        {
            DefaultInputData did = new DefaultInputData();
            pb.inputManagerData = did;
            pb.input.weaponSlotUses = new bool[weaponSlotKeys.Length];
        }

        public override void WriteToPlayerInput(Kit_PlayerBehaviour pb)
        {
            if (pb.inputManagerData != null && pb.inputManagerData.GetType() == typeof(DefaultInputData))
            {
                DefaultInputData did = pb.inputManagerData as DefaultInputData;
                if (pb.enableInput)
                {
                    //Get all input
                    pb.input.hor = (float)System.Math.Round(Input.GetAxis("Horizontal"), 1);
                    pb.input.ver = (float)System.Math.Round(Input.GetAxis("Vertical"), 1);
                    pb.input.crouch = Input.GetButton("Crouch");
                    pb.input.sprint = Input.GetButton("Sprint");
                    pb.input.jump = Input.GetKey(KeyCode.Space);
                    pb.input.interact = Input.GetKey(KeyCode.F);

                    pb.input.rmb = Input.GetKey(KeyCode.Mouse1);
                    pb.input.reload = Input.GetKey(KeyCode.R);

                    //Changed  from pun to mirror: Mouse X and Mouse Y is now the desired look rotation
                    pb.input.mouseX += Input.GetAxisRaw("Mouse X") * pb.weaponManager.CurrentSensitivity(pb);
                    pb.input.mouseY += Input.GetAxisRaw("Mouse Y") * pb.weaponManager.CurrentSensitivity(pb);
                    pb.input.mouseY = Mathf.Clamp(pb.input.mouseY, -90, 90);

                    pb.input.leanLeft = Input.GetButton("Lean Left");
                    pb.input.leanRight = Input.GetButton("Lean Right");
                    pb.input.thirdPerson = Input.GetButton("Change Perspective");
                    pb.input.flashlight = Input.GetButton("Flashlight");
                    pb.input.laser = Input.GetButton("Laser");

                    if (MarsScreen.lockCursor)
                    {
                        pb.input.lmb = Input.GetMouseButton(0);
                    }
                    else
                    {
                        pb.input.lmb = false;
                    }

                    if (pb.input.weaponSlotUses == null || pb.input.weaponSlotUses.Length != weaponSlotKeys.Length) pb.input.weaponSlotUses = new bool[weaponSlotKeys.Length];

                    for (int i = 0; i < weaponSlotKeys.Length; i++)
                    {
                        int id = i;
                        pb.input.weaponSlotUses[id] = Input.GetButton(weaponSlotKeys[id]);
                    }
                }
                else
                {
                    //Get all input
                    pb.input.hor = 0f;
                    pb.input.ver = 0f;

                    pb.input.crouch = false;
                    pb.input.sprint = false;
                    pb.input.jump = false;
                    pb.input.interact = false;

                    pb.input.rmb = false;
                    pb.input.reload = false;

                    pb.input.leanLeft = false;
                    pb.input.leanRight = false;
                    pb.input.thirdPerson = false;
                    pb.input.flashlight = false;
                    pb.input.laser = false;

                    pb.input.lmb = false;


                    if (pb.input.weaponSlotUses == null || pb.input.weaponSlotUses.Length != weaponSlotKeys.Length) pb.input.weaponSlotUses = new bool[weaponSlotKeys.Length];

                    for (int i = 0; i < weaponSlotKeys.Length; i++)
                    {
                        int id = i;
                        pb.input.weaponSlotUses[id] = false;
                    }
                }

                //Set Camera (this is validated on the server)
                pb.input.clientCamPos = pb.playerCameraTransform.position;
                pb.input.clientCamForward = pb.playerCameraTransform.forward;
            }
        }
    }
}