using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_DropRenderer : MonoBehaviour
        {
            #region New Attachment System
            /// <summary>
            /// Assign the sockets for the attachments to spawn in here aswell as the renderers that skins can change here
            /// </summary>
            [Tooltip("Assign the sockets for the attachments to spawn in here aswell as the renderers that skins can change here")]
            public AttachmentSocket[] attachmentAndSkinSockets;
            #endregion

            /// <summary>
            /// Enables the given attachments.
            /// </summary>
            /// <param name="enabledAttachments"></param>
            public void SetAttachments(Kit_ModernWeaponScript ws, int[] enabledAttachments)
            {
                List<Kit_SkinInfo> skinsInUse = new List<Kit_SkinInfo>();

                for (int i = 0; i < enabledAttachments.Length; i++)
                {
                    if (ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]].GetType() == typeof(Kit_SkinInfo))
                    {
                        Kit_SkinInfo skin = ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]] as Kit_SkinInfo;
                        skinsInUse.Add(skin);
                    }
                }

                //Loop through all slots
                for (int i = 0; i < enabledAttachments.Length; i++)
                {
                    if (ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]].GetType() == typeof(Kit_AttachmentInfo))
                    {
                        Kit_AttachmentInfo attachment = ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]] as Kit_AttachmentInfo;
                        AttachmentPrefabSet prefabsToUse = attachment.defaultSet;

                        for (int o = 0; o < skinsInUse.Count; o++)
                        {
                            if (attachment.skinOverride.ContainsKey(skinsInUse[o]))
                            {
                                prefabsToUse = attachment.skinOverride[skinsInUse[o]];
                            }
                        }

                        if (attachmentAndSkinSockets[i].socket)
                        {
                            if (prefabsToUse.fpPrefab)
                            {
                                GameObject creation = null;

                                if (prefabsToUse.dropPrefabOverride)
                                {
                                    Instantiate(prefabsToUse.dropPrefabOverride, attachmentAndSkinSockets[i].socket, false);
                                }
                                else
                                {
                                    creation = Instantiate(prefabsToUse.fpPrefab, attachmentAndSkinSockets[i].socket, false);
                                }
                                Kit_AttachmentVisualBase[] attachmentVisualBehaviours = creation.GetComponentsInChildren<Kit_AttachmentVisualBase>();

                                //Tell the behaviours they are active!
                                for (int p = 0; p < attachmentVisualBehaviours.Length; p++)
                                {
                                    int id = i;
                                    attachmentVisualBehaviours[p].Selected(null, AttachmentUseCase.Drop, ws, null, id);

                                    if (attachmentVisualBehaviours[p].GetType() == typeof(Kit_AttachmentAimOverride))
                                    {
                                        Kit_AttachmentAimOverride ar = attachmentVisualBehaviours[p] as Kit_AttachmentAimOverride;
                                        if (ar.dualRenderScopeCam) ar.dualRenderScopeCam.enabled = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]].GetType() == typeof(Kit_SkinInfo))
                    {
                        Kit_SkinInfo skin = ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]] as Kit_SkinInfo;
                        if (skin.setToRendererWithIndexDropOverride.Length > 0)
                        {
                            if (skin.setToRendererWithIndexDropOverride.Length != attachmentAndSkinSockets[i].renderersToChange.Length) Debug.LogWarning("Skin material length and the amount of renderers in slot #" + i + " do not match up. Trying best to assign.");

                            for (int o = 0; o < Mathf.Min(skin.setToRendererWithIndexDropOverride.Length, attachmentAndSkinSockets[i].renderersToChange.Length); o++)
                            {
                                attachmentAndSkinSockets[i].renderersToChange[o].sharedMaterials = skin.setToRendererWithIndexDropOverride[o].materialsToSet;
                            }
                        }
                        else
                        {
                            if (skin.setToRendererWithIndexFp.Length != attachmentAndSkinSockets[i].renderersToChange.Length) Debug.LogWarning("Skin material length and the amount of renderers in slot #" + i + " do not match up. Trying best to assign.");

                            for (int o = 0; o < Mathf.Min(skin.setToRendererWithIndexFp.Length, attachmentAndSkinSockets[i].renderersToChange.Length); o++)
                            {
                                attachmentAndSkinSockets[i].renderersToChange[o].sharedMaterials = skin.setToRendererWithIndexFp[o].materialsToSet;
                            }
                        }
                    }
                }

                /*
                try
                {
                    //Loop through all slots
                    for (int i = 0; i < enabledAttachments.Length; i++)
                    {
                        if (i < attachmentSlots.Length)
                        {
                            //Loop through all attachments for that slot

                            for (int o = 0; o < attachmentSlots[i].attachments.Length; o++)
                            {
                                //Check if this attachment is enabled
                                if (o == enabledAttachments[i])
                                {
                                    //Tell the behaviours they are active!
                                    for (int p = 0; p < attachmentSlots[i].attachments[o].attachmentBehaviours.Length; p++)
                                    {
                                        attachmentSlots[i].attachments[o].attachmentBehaviours[p].Selected(null, AttachmentUseCase.Drop);
                                    }
                                }
                                else
                                {
                                    //Tell the behaviours they are not active!
                                    for (int p = 0; p < attachmentSlots[i].attachments[o].attachmentBehaviours.Length; p++)
                                    {
                                        attachmentSlots[i].attachments[o].attachmentBehaviours[p].Unselected(null, AttachmentUseCase.Drop);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Something must have gone wrong with the attachments. Enabled attachments is longer than all slots.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("There was an error with the attachments: " + e);
                }
                */
            }
        }
    }
}
