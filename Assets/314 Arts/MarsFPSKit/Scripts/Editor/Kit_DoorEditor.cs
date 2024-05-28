using UnityEngine;
using UnityEditor;
using MarsFPSKit;
using System.Collections.Generic;


[CustomEditor(typeof(Kit_Door))]
public class Kit_DoorEditor : Editor
{
    public static bool rotateFoldout;

    public static bool animatedFoldout;

    public static bool settingsFoldout;

    public static bool doorCollisionFoldout;

    public static bool audioFoldout;

    public override void OnInspectorGUI()
    {
        Kit_Door door = target as Kit_Door;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        //First set type
        door.typeOfDoor = (DoorType)EditorGUILayout.EnumPopup("Door Type", door.typeOfDoor);
        EditorGUILayout.EndVertical();

        //Now custom settings
        if (door.typeOfDoor == DoorType.Rotate)
        {
            rotateFoldout = EditorGUILayout.Foldout(rotateFoldout, "Rotation settings");
            if (rotateFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                door.transformToRotate = EditorGUILayout.ObjectField("Transform to rotate", door.transformToRotate, typeof(Transform), true) as Transform;
                door.rotationClosed = EditorGUILayout.Vector3Field("Closed rotation", door.rotationClosed);
                door.rotationOpened = EditorGUILayout.Vector3Field("Opened rotation", door.rotationOpened);
                EditorGUILayout.EndVertical();
            }
        }
        else if (door.typeOfDoor == DoorType.Animated)
        {
            animatedFoldout = EditorGUILayout.Foldout(animatedFoldout, "Animation settings");
            if (animatedFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                door.anim = EditorGUILayout.ObjectField("Animator", door.anim, typeof(Animator), true) as Animator;
                EditorGUILayout.HelpBox("This animation must include opening / closing. First animation opens the door (until Animation Split Time) and then it should be closed again.", MessageType.Info);
                door.animationName = EditorGUILayout.TextField("Animation State name", door.animationName);
                door.animationSplitTime = EditorGUILayout.FloatField("Animation Split Time", door.animationSplitTime);
                EditorGUILayout.EndVertical();
            }
        }

        settingsFoldout = EditorGUILayout.Foldout(settingsFoldout, "General Settings");
        if (settingsFoldout)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            door.openTime = EditorGUILayout.FloatField("Opening time", door.openTime);
            door.closeTime = EditorGUILayout.FloatField("Closing time", door.closeTime);
            EditorGUILayout.EndVertical();
        }

        doorCollisionFoldout = EditorGUILayout.Foldout(doorCollisionFoldout, "Collision Settings");

        if (doorCollisionFoldout)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.HelpBox("If you assign these, your door will not push players or rigidbody objects around (Source Engine Style)", MessageType.Info);
            EditorGUILayout.HelpBox("This collider should be facing in the door's closing direction", MessageType.Info);
            door.doorColliderClosing = EditorGUILayout.ObjectField("Collider Closing", door.doorColliderClosing, typeof(Kit_DoorCollider), true) as Kit_DoorCollider;
            EditorGUILayout.HelpBox("This collider should be facing in the door's opening direction", MessageType.Info);
            door.doorColliderOpening = EditorGUILayout.ObjectField("Collider Opening", door.doorColliderOpening, typeof(Kit_DoorCollider), true) as Kit_DoorCollider;
            EditorGUILayout.EndVertical();
        }

        audioFoldout = EditorGUILayout.Foldout(audioFoldout, "Audio");
        if (audioFoldout)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            door.doorSoundSource = EditorGUILayout.ObjectField("Audio Source", door.doorSoundSource, typeof(AudioSource), true) as AudioSource;
            if (door.doorSoundSource)
            {
                door.doorClosingSound = EditorGUILayout.ObjectField("Closing Sound", door.doorClosingSound, typeof(AudioClip), false) as AudioClip;

                door.doorOpeningSound = EditorGUILayout.ObjectField("Opening Sound", door.doorOpeningSound, typeof(AudioClip), false) as AudioClip;
            }
            EditorGUILayout.EndVertical();
        }
    }
}