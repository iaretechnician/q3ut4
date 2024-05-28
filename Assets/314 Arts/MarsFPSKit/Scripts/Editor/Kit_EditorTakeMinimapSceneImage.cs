using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;

public class Kit_EditorTakeMinimapSceneImage : EditorWindow
{
    enum Size { s512, s1024, s2048, s4096, s8192 }

    float mapSize = 10f;

    Camera takePicturecam;
    RenderTexture rt;

    private Size currentSize = Size.s2048;
    private Size lastSize;

    [MenuItem("MMFPSE/Scene/Take Image for Minimap")]
    private static void TakeSceneImage()
    {
        // Get existing open window or if none, make a new one:
        Kit_EditorTakeMinimapSceneImage window = (Kit_EditorTakeMinimapSceneImage)EditorWindow.GetWindow(typeof(Kit_EditorTakeMinimapSceneImage));
        window.Show();
        window.takePicturecam = new GameObject("[Minimap] Take Scene Image Camera").AddComponent<Camera>();
        window.takePicturecam.orthographic = true;
        //Rotate and position it correctly
        window.takePicturecam.transform.rotation = Quaternion.Euler(90, 0, 0);
        window.takePicturecam.transform.position = new Vector3(0, 100, 0);
        window.takePicturecam.cullingMask = ~(1 << 31);
        window.RecreateTargetTexture();
    }

    private void RecreateTargetTexture()
    {
        if (currentSize == Size.s512) takePicturecam.targetTexture = new RenderTexture(512, 512, 24);
        else if (currentSize == Size.s1024) takePicturecam.targetTexture = new RenderTexture(1024, 1024, 24);
        else if (currentSize == Size.s2048) takePicturecam.targetTexture = new RenderTexture(2048, 2048, 24);
        else if (currentSize == Size.s4096) takePicturecam.targetTexture = new RenderTexture(4096, 4096, 24);
        else if (currentSize == Size.s8192) takePicturecam.targetTexture = new RenderTexture(8192, 8192, 24);
    }

    void OnGUI()
    {
        GUILayout.Label("Resize map size so that your whole map fits inside the image");
        mapSize = EditorGUILayout.FloatField("Map size", mapSize);
        if (mapSize < 0) mapSize = 1f;
        GUILayout.Label("The larger your map size, the larger your texture size should be");
        currentSize = (Size)EditorGUILayout.EnumPopup("Texture size", currentSize);
        if (currentSize != lastSize)
        {
            RecreateTargetTexture();
            lastSize = currentSize;
        }

        GUILayout.Label(takePicturecam.targetTexture, GUILayout.MaxHeight(512), GUILayout.MaxWidth(512));

        if (GUILayout.Button("Create!"))
        {
            CreatePicture();
        }
    }

    void CreatePicture()
    {
        //Render
        takePicturecam.Render();
        //Create Texture to save
        Texture2D tex = new Texture2D(1, 1);
        //Create correct size
        if (currentSize == Size.s512) tex = new Texture2D(512, 512, TextureFormat.RGBA32, false);
        else if (currentSize == Size.s1024) tex = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
        else if (currentSize == Size.s2048) tex = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
        else if (currentSize == Size.s4096) tex = new Texture2D(4096, 4096, TextureFormat.RGBA32, false);
        else if (currentSize == Size.s8192) tex = new Texture2D(8192, 8192, TextureFormat.RGBA32, false);
        //Copy contents
        RenderTexture.active = takePicturecam.activeTexture;
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        //Save
        tex.Apply();
        RenderTexture.active = null;
        byte[] toSave = tex.EncodeToPNG();
        string scenePath = EditorSceneManager.GetActiveScene().path;
        scenePath = scenePath.Replace("Assets", "");
        scenePath = scenePath.Replace(".unity", "");
        File.WriteAllBytes(Application.dataPath + scenePath + "/Minimap.png", toSave);
        //Reset Scene Path
        scenePath = EditorSceneManager.GetActiveScene().path;
        scenePath = scenePath.Replace(".unity", "");
        //Import
        AssetDatabase.ImportAsset(scenePath + "/Minimap.png", ImportAssetOptions.Default);
        //Get Image
        Texture2D mapPictureInProject = AssetDatabase.LoadAssetAtPath(scenePath + "/Minimap.png", typeof(Texture2D)) as Texture2D;
        //Check if we already have a minimap
        GameObject go = GameObject.Find("Minimap Scene");
        if (!go)
        {
            go = new GameObject("Minimap Scene");
            go.AddComponent<SpriteRenderer>().sprite = Sprite.Create(mapPictureInProject, new Rect(0, 0, mapPictureInProject.width, mapPictureInProject.height), new Vector2(0.5f, 0.5f));
            //Rotate
            go.transform.rotation = Quaternion.Euler(90, 0, 0);
            //Move
            go.transform.position = new Vector3(0, 30, 0);
            //Scale
            go.transform.localScale = new Vector3(0.0974f * mapSize, 0.0974f * mapSize, 0.0974f * mapSize);
            //Assign layer
            go.layer = 31;
        }
        else
        {
            //Get Sprite Renderer
            go.GetComponent<SpriteRenderer>().sprite = Sprite.Create(mapPictureInProject, new Rect(0, 0, mapPictureInProject.width, mapPictureInProject.height), new Vector2(0.5f, 0.5f));
            //Rotate
            go.transform.rotation = Quaternion.Euler(90, 0, 0);
            //Move
            go.transform.position = new Vector3(0, 30, 0);
            //Scale
            go.transform.localScale = new Vector3(0.0974f * mapSize, 0.0974f * mapSize, 0.0974f * mapSize);
            //Assign layer
            go.layer = 31;
        }
    }

    void Update()
    {
        if (takePicturecam)
        {
            takePicturecam.orthographicSize = mapSize;
        }
    }

    void OnDestroy()
    {
        if (takePicturecam)
        {
            DestroyImmediate(takePicturecam.gameObject);
        }
    }
}
