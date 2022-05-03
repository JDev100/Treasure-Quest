using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

public class PNGToGameObject : EditorWindow
{
    private string filesFolderPath = "";
    //string folderPath = "";

    [MenuItem("Tools/PNG to GameObject")]

    public static void ShowWindow()
    {
        GetWindow<PNGToGameObject>("PNG to GameObject");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate GameObject from PNG", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("PNG Directory: ", GetRelativeFilePath(filesFolderPath));

        if (GUILayout.Button("Select PNG File from Folder"))
        {
            filesFolderPath = EditorUtility.OpenFolderPanel("Select PNG from Asset Folder...", Application.dataPath, "");
            if (!filesFolderPath.StartsWith(Application.dataPath))
            {
                filesFolderPath = "";
            }
        }

        // EditorGUILayout.LabelField("Destination Folder: ", GetRelativeFilePath(folderPath));

        // if (GUILayout.Button("Select Animation Clip Destination"))
        // {
        //     folderPath = EditorUtility.OpenFolderPanel("Select Destination Folder...", Application.dataPath, "");
        //     if (!folderPath.StartsWith(Application.dataPath))
        //     {
        //         folderPath = "";
        //     }
        // }

        EditorGUI.BeginDisabledGroup(filesFolderPath == string.Empty /* || folderPath == string.Empty */);

        if (GUILayout.Button("Generate GameObject"))
        {
            GenerateGameObjects();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        if (filesFolderPath == string.Empty)
        {
            EditorGUILayout.HelpBox("Please select the directory containing PNG you desire to create a GameObject from. ", MessageType.Info);
        }
        // if (folderPath == string.Empty)
        // {
        //     EditorGUILayout.HelpBox("Please select the folder you desire to place the generated Animation Clip. ", MessageType.Info);
        // }

    }

    void GenerateGameObjects()
    {
        string[] filesInDirectory = Directory.GetFiles(filesFolderPath);

        int index = 0;

        AnimationClip newAnimClip = new AnimationClip();
       // float frame = 0.0f;
        // Keyframe[] keys = new Keyframe[filesInDirectory.Length];

        foreach (string curFilePath in filesInDirectory)
        {
            float curProgress = (float)index / filesInDirectory.Length;
            EditorUtility.DisplayProgressBar("Generating GameObject From PNGs", (curProgress * 100).ToString("F0") + "% Complete", curProgress);


            if (!curFilePath.EndsWith(".png")) { index++; continue; }

            string relativeFilePath = GetRelativeFilePath(curFilePath);

            TextureImporter curTextureAsset = (TextureImporter)TextureImporter.GetAtPath(relativeFilePath);

            curTextureAsset.textureType = TextureImporterType.Sprite;
            int longestSide = GetLongestSide(curTextureAsset);
            curTextureAsset.maxTextureSize = GetMaxTextureSize(longestSide);
            curTextureAsset.spritePixelsPerUnit = 16;
            curTextureAsset.filterMode = FilterMode.Point;
            curTextureAsset.SaveAndReimport();

            Sprite curSprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativeFilePath);
            string[] splitFileName = curSprite.name.Split(new[] { '-' }, 2);
            if (!int.TryParse(splitFileName[0], out int orderInLayer))
            {
                Debug.LogError("Error: Cannot convert first part of file name to integer for file: " +
                                curSprite.name + ". Stopping operation.\n" +
                                "Ensure that the file name of all images begins with the layer index " +
                                "followed by an underscore. i.e. 01_ImageName.png");
                break;
            }

            GameObject newGO = new GameObject(splitFileName[1]);
            SpriteRenderer spRenderer = newGO.AddComponent<SpriteRenderer>();
            spRenderer.sprite = curSprite;
            spRenderer.sortingOrder = orderInLayer;

            //newGO.transform.parent = containerGO;
            newGO.transform.position = Vector3.zero;
            newGO.transform.rotation = Quaternion.identity;
            newGO.transform.localScale = Vector3.one;
            index++;
        }
        EditorUtility.ClearProgressBar();
        //string relativeFilePath = 
    }

    int GetLongestSide(TextureImporter curAsset)
    {
        int width, height;
        if (curAsset != null)
        {
            object[] args = new object[2] { 0, 0 };
            MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(curAsset, args);

            width = (int)args[0];
            height = (int)args[1];

            return width > height ? width : height;
        }
        return 0;
    }
    string GetRelativeFilePath(string fullPath)
    {
        if (fullPath == string.Empty) { return ""; }
        string relativeFilePath = "Assets" + fullPath.Substring(Application.dataPath.Length);
        return relativeFilePath;
    }

    int GetMaxTextureSize(int longestSide)
    {
        if (longestSide <= 32f)
        {
            return 32;
        }
        else if (longestSide <= 64f)
        {
            return 64;
        }

        else if (longestSide <= 128f)
        {
            return 128;
        }
        else if (longestSide <= 256f)
        {
            return 256;
        }
        else if (longestSide <= 512f)
        {
            return 512;
        }
        else if (longestSide <= 1024f)
        {
            return 1024;
        }
        else if (longestSide <= 2048f)
        {
            return 2048;
        }
        else if (longestSide <= 4096f)
        {
            return 4096;
        }
        else if (longestSide <= 8192f)
        {
            return 8192;
        }
        else
        {
            Debug.LogWarning("Warning: Image is larger than max size of 8192 pixels in one or more directions. " +
                              "Setting current TextureImporter's max size to max value of 8192. ");
            return 8192;
        }
    }
}

