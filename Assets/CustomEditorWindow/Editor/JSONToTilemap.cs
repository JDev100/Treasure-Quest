using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using UnityEngine.Tilemaps;
using LitJson;



public class JSONToTilemap : EditorWindow
{
    public enum Layer { ground, spike, softGround, fallingPlatform }
    string jsonFilePath = "";
    string layerPNGFilePath = "";
    int pixelSize;
    bool shrinkOneTile;

    [MenuItem("Tools/JSON to TileMap")]

    public static void ShowWindow()
    {
        GetWindow<JSONToTilemap>("JSON to TileMap");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate TileMap from JSON", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("JSON Directory: ", GetRelativeFilePath(jsonFilePath));

        if (GUILayout.Button("Select JSON File from Folder"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON from Asset Folder...", Application.dataPath, "");
            if (!jsonFilePath.StartsWith(Application.dataPath))
            {
                jsonFilePath = "";
            }
        }

        shrinkOneTile = EditorGUILayout.BeginToggleGroup("Shrink One Tile", shrinkOneTile);
        EditorGUILayout.EndToggleGroup();


        // if (GUILayout.Button("Select Layer Image File from Folder"))
        // {
        //     layerPNGFilePath = EditorUtility.OpenFilePanel("Select PNG from Asset Folder...", Application.dataPath, "");
        //     if (!layerPNGFilePath.StartsWith(Application.dataPath))
        //     {
        //         layerPNGFilePath = "";
        //     }
        // }

        // if (GUILayout.Button("Select Layer JSON File from Folder"))
        // {
        //     layerInfoFilePath = EditorUtility.OpenFilePanel("Select Layer JSON from Asset Folder...", Application.dataPath, "");
        //     if (!layerInfoFilePath.StartsWith(Application.dataPath))
        //     {
        //         layerInfoFilePath = "";
        //     }
        // }
        // EditorGUILayout.LabelField("Destination Folder: ", GetRelativeFilePath(folderPath));

        // if (GUILayout.Button("Select Animation Clip Destination"))
        // {
        //     folderPath = EditorUtility.OpenFolderPanel("Select Destination Folder...", Application.dataPath, "");
        //     if (!folderPath.StartsWith(Application.dataPath))
        //     {
        //         folderPath = "";
        //     }
        // }

        EditorGUI.BeginDisabledGroup(jsonFilePath == string.Empty /* || folderPath == string.Empty */);


        // pixelSize = EditorGUILayout.IntField("Pixels per Unit", pixelSize);

        if (GUILayout.Button("Generate TileMap"))
        {
            GenerateTileMap();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        if (jsonFilePath == string.Empty)
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
        string[] filesInDirectory = Directory.GetFiles(GetLayerPNGFolder(jsonFilePath));

        // foreach (var item in filesInDirectory)
        // {
        //     Debug.Log(item);
        // }

        //int index = 0;
        // float frame = 0.0f;
        // Keyframe[] keys = new Keyframe[filesInDirectory.Length];
        string jsonString;
        // string layerJsonString;
        // JsonData layerData;
        string spritePath;
        JsonData itemData;

        jsonString = File.ReadAllText(jsonFilePath);
        itemData = JsonMapper.ToObject(jsonString);

        // layerJsonString = File.ReadAllText(layerInfoFilePath);
        // layerData = JsonMapper.ToObject(layerJsonString);
        //float curProgress = (float)index / filesInDirectory.Length;
        //  EditorUtility.DisplayProgressBar("Generating Tile Map From PNGs", (curProgress * 100).ToString("F0") + "% Complete", curProgress);


        // if (!filePath.EndsWith(".json")) { return; }
        // else
        // {
        //     gotJson = true;
        // }
        Debug.Log("JSON GET!");

        string relativeFilePath = GetRelativeFilePath(jsonFilePath);
        //string relativeLayerPath = GetRelativeFilePath(layerPNGFilePath);

        foreach (string file in filesInDirectory)
        {
            if (!file.EndsWith(".png")) continue;

            string relativePath = GetRelativeFilePath(file);
            //Layer Stuff
            TextureImporter layerTextureAsset = (TextureImporter)TextureImporter.GetAtPath(relativePath);
            layerTextureAsset.textureType = TextureImporterType.Sprite;
            layerTextureAsset.spriteImportMode = SpriteImportMode.Single;
            layerTextureAsset.spritePixelsPerUnit = 1;
            layerTextureAsset.filterMode = FilterMode.Point;
            int longestSide_Layer = GetLongestSide(layerTextureAsset);
            layerTextureAsset.maxTextureSize = GetMaxTextureSize(longestSide_Layer);
            layerTextureAsset.isReadable = true;
            //layerTextureAsset.textureCompression = TextureImporterCompression.;
            //Debug.Log(relativePath);
            layerTextureAsset.SaveAndReimport();
        }


        TextureImporter curTextureAsset = new TextureImporter();

        // if (!gotJson)
        // else
        // {

        // Debug.Log(GetRelativeFilePath(spritePath, relativeFilePath, true));
        GameObject tileMapGrid = new GameObject("TileMapGrid");
        Grid grid = tileMapGrid.AddComponent<Grid>();

        TilemapData[] tilemapDatas = new TilemapData[3];

        // //Default Grid
        // GameObject tileMapObject = new GameObject("TileMapDefault");
        // Tilemap tilemapDefault = tileMapObject.AddComponent<Tilemap>();
        // TilemapRenderer tilemapRenderer = tileMapObject.AddComponent<TilemapRenderer>();
        // tileMapObject.transform.SetParent(tileMapGrid.transform);
        TilemapData tilemapData_Default = new TilemapData("TileMap_Default");
        tilemapData_Default.tileMapObject.transform.SetParent(tileMapGrid.transform);
        tilemapDatas[0] = tilemapData_Default;


        // //Ground Grid
        // GameObject tileMapObject_Ground = new GameObject("TileMap_Ground");
        // tileMapObject_Ground.layer = 8;
        // Tilemap tilemap_Ground = tileMapObject_Ground.AddComponent<Tilemap>();
        // TilemapRenderer tilemapRenderer_Ground = tileMapObject_Ground.AddComponent<TilemapRenderer>();
        // TilemapCollider2D tilemapCollider_Ground = tileMapObject_Ground.AddComponent<TilemapCollider2D>();
        // tileMapObject_Ground.transform.SetParent(tileMapGrid.transform);
        TilemapData tilemapData_Ground = new TilemapData("TileMap_Ground", true);
        tilemapData_Ground.tileMapObject.transform.SetParent(tileMapGrid.transform);
        tilemapDatas[1] = tilemapData_Ground;
        tilemapData_Ground.tileMapObject.layer = 8;

        //Spikes Grid
        TilemapData tilemapData_Spikes = new TilemapData("TileMap_Spikes", true);
        tilemapData_Spikes.tileMapObject.transform.SetParent(tileMapGrid.transform);
        tilemapData_Spikes.tileMapObject.layer = 8;
        tilemapData_Spikes.tileMapObject.tag = "SpikeLayer";
        tilemapDatas[2] = tilemapData_Spikes;

        //Boundary SetUp
        GameObject boundaryManager = new GameObject("BoundaryManager");
        BoundaryManager boundaryMgrComponent = boundaryManager.AddComponent<BoundaryManager>();



        //Default Demographics
        int worldGridHeight = int.Parse(itemData["worldGridHeight"].ToString());

        for (int level = 0; level < itemData["levels"].Count; level++)
        {
            Vector3Int worldOffset = new Vector3Int();
            worldOffset.x = int.Parse(itemData["levels"][level]["worldX"].ToString());
            worldOffset.y = int.Parse(itemData["levels"][level]["worldY"].ToString());

            if (/* level > 0 && */ shrinkOneTile)
            {
                if (Mathf.Abs(worldOffset.x) > 0)
                worldOffset.x -= (worldOffset.x > 0) ? 2 * pixelSize : 2 * -pixelSize;

                if (Mathf.Abs(worldOffset.y) > 0)
                worldOffset.y -= (worldOffset.y > 0) ? 2 * pixelSize : 2 * -pixelSize;
            }

            string levelName = itemData["levels"][level]["identifier"].ToString();
            Sprite layerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GetLayerFromFolder(levelName, filesInDirectory));

            // if (layerSprite == null)
            //     Debug.Log("No Layer for: " + levelName);

            // else Debug.Log("Yes Layer for: " + levelName);

            for (int i = 0; i < itemData["levels"][level]["layerInstances"].Count; i++)
            {
                worldOffset /= int.Parse(itemData["levels"][level]["layerInstances"][i]["__gridSize"].ToString());

                if (itemData["levels"][level]["layerInstances"][i]["__type"].ToString() != "Entities")
                {
                    spritePath = itemData["levels"][level]["layerInstances"][i]["__tilesetRelPath"].ToString();
                    pixelSize = int.Parse(itemData["levels"][level]["layerInstances"][i]["__gridSize"].ToString());

                    // Debug.Log(pixelSize);

                    string relativeSpritePath = CorrectFilePathFromJSON(spritePath, relativeFilePath);
                    // // spritePath = GetRelativeFilePath(spritePath);
                    curTextureAsset = (TextureImporter)TextureImporter.GetAtPath(relativeSpritePath);


                    //  Debug.Log(GetRelativeFilePath(spritePath, relativeFilePath));

                    Boundary boundary = new Boundary();
                    boundary.boundaryLink = Boundary.BoundaryLink.none;
                    boundary.size.x = int.Parse(itemData["levels"][level]["pxWid"].ToString()) / pixelSize;
                    boundary.size.y = int.Parse(itemData["levels"][level]["pxHei"].ToString()) / pixelSize;

                    if (shrinkOneTile)
                    {
                        boundary.size.x -= 2;
                        boundary.size.y -= 2;
                    }

                    boundary.center.x = (boundary.size.x / 2) + worldOffset.x;
                    boundary.center.y = (boundary.size.y - (boundary.size.y + (boundary.size.y / 2 - 1))) - worldOffset.y;

                    if (shrinkOneTile) {
                        boundary.center.x += 1;
                        boundary.center.y -= 1;
                    }
                    boundaryMgrComponent.boundarySetUp.Add(boundary);



                    curTextureAsset.textureType = TextureImporterType.Sprite;
                    curTextureAsset.spriteImportMode = SpriteImportMode.Multiple;
                    // curTextureAsset.
                    //spr
                    int longestSide = GetLongestSide(curTextureAsset);
                    curTextureAsset.maxTextureSize = GetMaxTextureSize(longestSide);
                    curTextureAsset.spritePixelsPerUnit = pixelSize;
                    curTextureAsset.isReadable = true;
                    curTextureAsset.filterMode = FilterMode.Point;
                    //SpriteMetaData spriteMeta = new SpriteMetaData();
                    curTextureAsset.spritesheet = GetSpriteSheetFromPixelSize(curTextureAsset, pixelSize);
                    curTextureAsset.SaveAndReimport();

                    //curTextureAsset.

                    Sprite curSprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativeSpritePath);
                    if (curSprite = null)
                        Debug.Log(relativeFilePath);

                    string spritesheetPath = RemoveFileExtension(relativeSpritePath);
                    spritesheetPath = ConvertToResourcesPath(spritesheetPath);
                    Debug.Log(spritesheetPath);
                    Sprite[] curSprites = Resources.LoadAll<Sprite>(spritesheetPath);



                    for (int j = 0; j < itemData["levels"][level]["layerInstances"][i]["autoLayerTiles"].Count; j++)
                    {
                        int positionX = int.Parse(itemData["levels"][level]["layerInstances"][i]["autoLayerTiles"][j]["px"][0].ToString());
                        int positionY = int.Parse(itemData["levels"][level]["layerInstances"][i]["autoLayerTiles"][j]["px"][1].ToString());

                        int height = int.Parse(itemData["levels"][level]["pxHei"].ToString());
                        //int height = int.Parse
                        Vector2 position = new Vector2(positionX, positionY);
                        // position = new Vector2(positionX, W)
                        Vector3Int tilePosition = new Vector3Int((int)(position.x / pixelSize), (int)(position.y / pixelSize), 0);
                        tilePosition += worldOffset;

                        position = new Vector2(positionX, FlipOverCenter(height, positionY));
                        //Debug.Log("Position: " + position / pixelSize);

                        //bool isGroundTile = false;
                        string layerCode = "";
                        if (layerSprite != null)
                        {
                            Vector2Int intPosition = new Vector2Int((int)(position.x / pixelSize), (int)(position.y / pixelSize));
                            Color intColor = layerSprite.texture.GetPixel(intPosition.x, intPosition.y);
                            string intHexcode = "";

                            if (intColor != null)
                            {
                                intHexcode = "#" + ColorUtility.ToHtmlStringRGBA(intColor);
                                // isGroundTile = IsGroundTile(intHexcode, itemData);
                                layerCode = GetLayer(intHexcode, itemData);
                                //  Debug.Log("Layer: " + layerCode);
                            }
                        }
                        //Debug.Log(intHexcode);

                        // Debug.Log(isGroundTile);
                        // Color color = layerSprite.texture.GetPixel(1, 0);
                        // Debug.Log(ColorUtility.ToHtmlStringRGBA(color));


                        int sourceX = int.Parse(itemData["levels"][level]["layerInstances"][i]["autoLayerTiles"][j]["src"][0].ToString());
                        int sourceY = int.Parse(itemData["levels"][level]["layerInstances"][i]["autoLayerTiles"][j]["src"][1].ToString());

                        Vector2 source = new Vector2(sourceX, sourceY);

                        Sprite sprite = GetSpriteFromRect(curSprites, source);
                        Tile tile = ScriptableObject.CreateInstance<Tile>();
                        tile.sprite = sprite;
                        tile.name = sprite.name;

                        //Align tile according to layer
                        // if (isGroundTile)
                        // {
                        //     PlaceTile(ref tilemapData_Ground.tileMap, tilePosition, tile);
                        // }
                        // else
                        // {
                        //     PlaceTile(ref tilemapData_Default.tileMap, tilePosition, tile);
                        // }
                        bool setDefault = true;
                        foreach (TilemapData tilemapData in tilemapDatas)
                        {
                            // Debug.Log(tilemapData.tileMapObject.name);
                            if (tilemapData.tileMapObject.name.Contains(layerCode))
                            {
                                PlaceTile(ref tilemapData.tileMap, tilePosition, tile);
                                setDefault = false;
                            }
                        }
                        if (setDefault)
                        {
                            PlaceTile(ref tilemapData_Default.tileMap, tilePosition, tile);
                        }

                        // Debug.Log(tilemapDefault.CellToWorld(tilePosition));
                    }
                }
            }
        }
        //     index++;

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

    string GetLayerPNGFolder(string directory)
    {
        string[] splitArr = directory.Split('.');
        string[] splitArr2 = splitArr[0].Split('/');
        return splitArr[0] + "/" + "png";
    }

    string GetLayerFromFolder(string levelName, string[] folder)
    {
        foreach (string file in folder)
        {
            if (file.Contains(levelName) && file.Contains("int") && file.EndsWith(".png"))
            {
                //Debug.Log(levelName + " path: " + GetRelativeFilePath(file));
                return GetRelativeFilePath(file);
            }
        }
        return null;
    }

    string RemoveFileExtension(string path)
    {
        string[] s1 = path.Split('.');

        Debug.Log(s1[0]);
        return s1[0];
    }

    string ConvertToResourcesPath(string path)
    {
        string[] s1 = path.Split('/');

        string finalPath = "";

        for (int i = 0; i < s1.Length; i++)
        {
            if (s1[i] != "Assets" && s1[i] != "Resources")
                finalPath += s1[i] + (i < s1.Length - 1 ? "/" : "");
        }

        return finalPath;
    }

    string CorrectFilePathFromJSON(string path, string jsonPath)
    {

        // Debug.Log(path);
        //  Debug.Log(jsonPath);
        int numBack = 1;

        string[] s1 = path.Split('/');

        string secondPart = "";

        for (int i = 0; i < s1.Length; i++)
        {
            if (s1[i] == "..")
                numBack++;
            else
            {
                secondPart += s1[i] + (i < s1.Length - 1 ? "/" : "");
            }
        }

        string[] s2 = jsonPath.Split('/');

        int numToStop = s2.Length - numBack;

        string firstPart = "";

        for (int i = 0; i < numToStop; i++)
        {
            firstPart += s2[i] + "/";
        }

        string finalPath = firstPart + secondPart;

        // Debug.Log(finalPath);

        return finalPath;
    }
    SpriteMetaData[] GetSpriteSheetFromPixelSize(TextureImporter texture, int pixelSize)
    {
        List<SpriteMetaData> current = new List<SpriteMetaData>();

        int index = 0;
        Vector2 spriteSize = GetWidthAndHeight(texture);

        int columnSize = (int)spriteSize.x / pixelSize;
        int rowSize = (int)spriteSize.y / pixelSize;
        int centerY = (((int)spriteSize.y / pixelSize - 1) * pixelSize) / 2;

        for (int i = rowSize - 1; i > -1; i--)
        {
            for (int j = 0; j < columnSize; j++)
            {
                Vector2 position = new Vector2(j * pixelSize, i * pixelSize);
                SpriteMetaData metaData = new SpriteMetaData();
                metaData.name = "Tile_" + index.ToString();
                metaData.rect.position = position;
                metaData.rect.size = new Vector2(pixelSize, pixelSize);
                current.Add(metaData);
                index++;
            }
        }

        SpriteMetaData[] spriteSheet = new SpriteMetaData[current.Count];
        for (int i = 0; i < current.Count; i++)
        {
            spriteSheet[i] = current[i];
        }
        // Debug.Log(rowSize);
        // Debug.Log(columnSize);
        // Debug.Log(centerY);
        // Debug.Log(spriteSize);
        return spriteSheet;
    }

    Vector2 GetWidthAndHeight(TextureImporter curAsset)
    {
        int width, height;
        if (curAsset != null)
        {
            object[] args = new object[2] { 0, 0 };
            MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(curAsset, args);

            width = (int)args[0];
            height = (int)args[1];

            return new Vector2(width, height);
        }
        return Vector2.zero;
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

    Sprite GetSpriteFromRect(Sprite[] sprites, Vector2 pos)
    {
        foreach (Sprite sprite in sprites)
        {
            Vector2 adjustedVector = GetCorrectVectorFromJSON(sprite, pos);
            //Debug.Log(adjustedVector);
            if (adjustedVector == sprite.rect.position)
            {
                return sprite;
            }
        }
        return null;
    }

    // SpriteMetaData GetRectFromSpriteSheet(ref SpriteMetaData[] spriteSheet, Vector2 srcPos)
    // {
    //     foreach (SpriteMetaData metaData in spriteSheet)
    //     {
    //         if (metaData.rect.position == srcPos) {
    //             return metaData;
    //         }
    //     }

    //     return new SpriteMetaData();
    // }



    float FlipOverCenter(float max, float current)
    {
        float center = (max - pixelSize) / 2;
        return center + (center - current);
    }

    Vector2 GetCorrectVectorFromJSON(Sprite sprite, Vector2 originalPos)
    {
        // float center = (sprite.texture.height - pixelSize) / 2;
        // float newY = center + (center - originalPos.y);

        return new Vector2(originalPos.x, FlipOverCenter(sprite.texture.height, originalPos.y));
    }

    void PlaceTile(ref Tilemap tilemap, Vector3Int tilePosition, Tile tile)
    {
        Vector3 flipVector = tilemap.CellToWorld(tilePosition);
        flipVector.y *= -1;
        tilePosition = tilemap.WorldToCell(flipVector);
        tilemap.SetTile(tilePosition, tile);
    }

    //TO BE CHANGED
    string GetLayer(string hexCode, JsonData layerInfo)
    {
        //Debug.Log(hexCode.Substring(hexCode.Length - 2));
        //  Debug.Log(hexCode);
        foreach (JsonData layer in layerInfo["defs"]["layers"])
        {
            if (layer["intGridValues"] != null)
            {
                foreach (JsonData intGridValue in layer["intGridValues"])
                {

                    if (hexCode.Substring(hexCode.Length - 2) != "00")
                    {
                        if (hexCode.Substring(0, 7) == intGridValue["color"].ToString())
                        {
                            // if (intGridValue["identifier"].ToString() == "Ground")
                            // {
                            //     Debug.Log(intGridValue["identifier"].ToString());
                            //     return true;
                            // }
                            return intGridValue["identifier"].ToString();
                            // return true;
                        }
                    }
                }
            }
        }

        return "";
    }

}

public class TilemapData
{
    public GameObject tileMapObject;
    public Tilemap tileMap;
    public TilemapRenderer tilemapRenderer;
    public TilemapCollider2D tilemapCollider;

    public TilemapData(string name, bool hasCollider = false)
    {
        tileMapObject = new GameObject(name);
        tileMap = tileMapObject.AddComponent<Tilemap>();
        tilemapRenderer = tileMapObject.AddComponent<TilemapRenderer>();

        if (hasCollider)
        {
            tilemapCollider = tileMapObject.AddComponent<TilemapCollider2D>();
        }
    }
}

// public class TilemapDataPlus : TilemapData {
//     public TilemapCollider2D tilemapCollider;

//     // public TilemapDataPlus(string name) {
//     //     tileMapObject = new GameObject(name);

//     // }
// }