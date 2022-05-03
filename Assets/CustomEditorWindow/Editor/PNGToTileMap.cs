using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using UnityEngine.Tilemaps;

public class PNGToTileMap : EditorWindow
{
    string filesFolderPath = "";

    [MenuItem("Tools/PNG to TileMap")]

    public static void ShowWindow()
    {
        GetWindow<PNGToTileMap>("PNG to TileMap");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate TileMap from PNG", EditorStyles.boldLabel);

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

        if (GUILayout.Button("Generate TileMap"))
        {
            GenerateTileMap();
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

    void GenerateTileMap()
    {
        string[] filesInDirectory = Directory.GetFiles(filesFolderPath);

        int index = 0;
        // float frame = 0.0f;
        // Keyframe[] keys = new Keyframe[filesInDirectory.Length];

        foreach (string curFilePath in filesInDirectory)
        {
            float curProgress = (float)index / filesInDirectory.Length;
            EditorUtility.DisplayProgressBar("Generating Tile Map From PNGs", (curProgress * 100).ToString("F0") + "% Complete", curProgress);


            if (!curFilePath.EndsWith(".png")) { index++; continue; }

            string relativeFilePath = GetRelativeFilePath(curFilePath);

            TextureImporter curTextureAsset = (TextureImporter)TextureImporter.GetAtPath(relativeFilePath);

            curTextureAsset.textureType = TextureImporterType.Sprite;
            curTextureAsset.spriteImportMode = SpriteImportMode.Multiple;
            // curTextureAsset.
            //spr
            //int longestSide = GetLongestSide(curTextureAsset);
            // curTextureAsset.maxTextureSize = GetMaxTextureSize(longestSide);
            curTextureAsset.spritePixelsPerUnit = 32;
            curTextureAsset.filterMode = FilterMode.Point;
            //SpriteMetaData spriteMeta = new SpriteMetaData();
            
            //curTextureAsset.spritesheet;
            curTextureAsset.SaveAndReimport();

            //Sprite[] curSprites = AssetDatabase.LoadAllAssetsAtPath(GetRelativeFilePath(curFilePath));
            Sprite[] curSprites = Resources.LoadAll<Sprite>(GetSpriteFromMultiple(curFilePath));

            GameObject tileMapGrid = new GameObject("TileMapGrid");
            Grid grid = tileMapGrid.AddComponent<Grid>();

            GameObject tileMapObject = new GameObject("TileMap");
            Tilemap tilemap = tileMapObject.AddComponent<Tilemap>();
            TilemapRenderer tilemapRenderer = tileMapObject.AddComponent<TilemapRenderer>();
            tileMapObject.transform.SetParent(tileMapGrid.transform);

            Vector3Int vector = new Vector3Int(0,0,0);
            foreach (Sprite sprite in curSprites)
            {
                Debug.Log(sprite.rect);
                vector = new Vector3Int((int)sprite.rect.x, (int)sprite.rect.y, 0);
                vector/= 32;
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = sprite;
                tile.name = sprite.name;

                tilemap.SetTile(vector, tile);
            }
                index++;
        }
        EditorUtility.ClearProgressBar();
        //string relativeFilePath =
    }

    string GetRelativeFilePath(string fullPath)
    {
        if (fullPath == string.Empty) { return ""; }

        string relativeFilePath = "Assets" + fullPath.Substring(Application.dataPath.Length);
        //Debug.Log("Assets" + fullPath.Substring(Application.dataPath.Length));
        return relativeFilePath;
    }
    string GetSpriteFromMultiple(string fullPath)
    {
        if (fullPath == string.Empty) { return ""; }

        string path = fullPath.Substring(Application.dataPath.Length);
        string[] stringArr = path.Split('/');
        // foreach(string str in stringArr)
        // Debug.Log(str);

        string finalString = stringArr[2] + "/" + stringArr[3] + "/";
        string str = stringArr[4];
        str = str.Replace('\\', '/');
        string[] str2 = str.Split('.');
        finalString += str2[0];

        string relativeFilePath = finalString;
        Debug.Log(relativeFilePath);
        return relativeFilePath;
    }

    SpriteMetaData[] GetSpriteSheetFromPixelSize(TextureImporter texture, int pixelSize, Sprite original) {
        List<SpriteMetaData> current = new List<SpriteMetaData>();

        //texture.tex
        return null;
    }
}

