// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using UnityEditor;

// public static class TileEditor
// {
//     public enum TileSpaceProfile
//     {
//         lone,
//     }

//     static List<PrimaryTileData> currentPrimaryTileDatas = new List<PrimaryTileData>();

//     /***********************************************/
//     /***********************************************/
//     /*          TILE CLASS CHECKING FUNCTIONS      */
//     /***********************************************/
//     /***********************************************/

//     static bool IsLoneTile(Tilemap tilemap, Vector3Int origin)
//     {
//         if (tilemap.HasTile(origin) ||
//         tilemap.HasTile(GetNearbyCell(origin, Direction.left)) ||
//         tilemap.HasTile(GetNearbyCell(origin, Direction.right)) ||
//         tilemap.HasTile(GetNearbyCell(origin, Direction.up)) ||
//         tilemap.HasTile(GetNearbyCell(origin, Direction.down)))
//             return false;
//         else
//             return true;
//     }

//     static bool IsPrimaryTile(Tilemap tilemap, Vector3Int origin)
//     {
//         if (tilemap.HasTile(origin))
//         {
//             string check = tilemap.GetTile(origin).name;
//             //Debug.Log(check);
//             string[] stringArr = check.Split('_');
//             for (int i = 0; i < stringArr.Length; i++)
//             {
//                 if (stringArr[i] == "P")
//                 {
//                     return true;
//                 }
//             }
//         }
//         return false;
//     }

//     static bool IsCornerTile(Tilemap tilemap, Vector3Int origin)
//     {
//         if (tilemap.HasTile(origin))
//         {
//             string check = tilemap.GetTile(origin).name;
//             //Debug.Log(check);
//             string[] stringArr = check.Split('_');
//             for (int i = 0; i < stringArr.Length; i++)
//             {
//                 if (stringArr[i] == "Top" || stringArr[i] == "Bottom")
//                 {
//                     return true;
//                 }
//             }
//         }
//         return false;
//     }

//     static string GetTileCode(Tilemap tilemap, Vector3Int origin)
//     {
//         if (tilemap.HasTile(origin))
//         {
//             string check = tilemap.GetTile(origin).name;
//             string[] stringArr = check.Split('_');
//             for (int i = 0; i < stringArr.Length; i++)
//             {
//                 if (stringArr[i] == "Top" || stringArr[i] == "Ground")
//                     return "TOP";
//                 if (stringArr[i] == "Bottom" || stringArr[i] == "Roof")
//                     return "BOTTOM";
//             }
//         }
//         return null;
//     }

//     static bool IsWallTile(Tilemap tilemap, Vector3Int origin)
//     {
//         if (tilemap.HasTile(origin))
//         {
//             string check = tilemap.GetTile(origin).name;
//             //Debug.Log(check);
//             string[] stringArr = check.Split('_');
//             for (int i = 0; i < stringArr.Length; i++)
//             {
//                 if (stringArr[i] == "Wall")
//                 {
//                     return true;
//                 }
//             }
//         }
//         return false;
//     }


//     static string IsNearGroundTile(Tilemap tilemap, Vector3Int origin)
//     {
//         if (tilemap.HasTile(GetNearbyCell(origin, Direction.left)) && tilemap.HasTile(GetNearbyCell(origin, Direction.right)))
//         {
//             return "LEFT_RIGHT";
//         }
//         else if (tilemap.HasTile(GetNearbyCell(origin, Direction.left)) && !tilemap.HasTile(GetNearbyCell(origin, Direction.right)))
//         {
//             return "LEFT";
//         }
//         else if (!tilemap.HasTile(GetNearbyCell(origin, Direction.left)) && tilemap.HasTile(GetNearbyCell(origin, Direction.right)))
//         {
//             return "RIGHT";
//         }
//         else
//             return "NOT";
//     }

//     static string IsAboveTile(Tilemap tilemap, Vector3Int origin)
//     {
//         Vector3Int downCell = GetNearbyCell(origin, Direction.down);

//         if (tilemap.HasTile(downCell) && IsPrimaryTile(tilemap, downCell))
//         {
//             Vector3Int downCell_2 = GetNearbyCell(downCell, Direction.down);
//             if (tilemap.HasTile(downCell_2) && IsWallTile(tilemap, downCell_2))
//             {
//                 return "OVER_WALL" + GetWallSide(tilemap, downCell_2);
//             }
//             else if (IsCornerTile(tilemap, downCell))
//                 return "OVER_CORNER" + "_" + GetTileCode(tilemap, downCell) + GetWallSide(tilemap, downCell);
//             else
//                 return "OVER_OTHER";
//         }
//         else
//             return "NOT";
//     }

//     static string IsUnderTile(Tilemap tilemap, Vector3Int origin)
//     {
//         Vector3Int upCell = GetNearbyCell(origin, Direction.up);
//         if (tilemap.HasTile(upCell) && IsPrimaryTile(tilemap, upCell))
//         {
//             //Vector3Int upCell_2 = GetNearbyCell(upCell, Direction.up);
//             if (IsWallTile(tilemap, upCell))
//             {
//                 return "UNDER" + GetWallSide(tilemap, upCell);
//             }
//             else if (IsCornerTile(tilemap, upCell))
//                 return "UNDER" + GetWallSide(tilemap, upCell);
//             else
//                 return "UNDER_OTHER";
//         }
//         else if (tilemap.HasTile(upCell) && !IsPrimaryTile(tilemap, upCell))
//         {
//             return "UNDER_OTHER";
//         }
//         return "NOT";
//     }

//     static string GetWallSide(Tilemap tilemap, Vector3Int location)
//     {
//         string check = tilemap.GetTile(location).name;
//         string[] stringArr = check.Split('_');
//         for (int i = 0; i < stringArr.Length; i++)
//         {
//             if (IsPrimaryTile(tilemap, location))
//             {
//                 if (stringArr[i] == "Left")
//                 {

//                     return "_" + "LEFT";
//                 }
//                 if (stringArr[i] == "Right")
//                 {
//                     return "_" + "RIGHT";
//                 }
//             }
//         }
//         return null;
//     }

//     /***********************************************/
//     /***********************************************/
//     /*      TILESET AUTO-CORRECTING FUNCTIONS      */
//     /***********************************************/
//     /***********************************************/
//     public static void AutoCorrectTiles(ref Tilemap tilemap, TileSet tileSet, Vector3Int origin)
//     {

//     }

//     /***********************************************/
//     /***********************************************/
//     /*          TILESET EDITING FUNCTIONS          */
//     /***********************************************/
//     /***********************************************/

//     public static void PlaceTile(ref Tilemap tilemap, TileSet tileSet, Vector3 location)
//     {
//         Vector3Int nearCell = tilemap.LocalToCell(location);

//         PrimaryTile primaryTile = CalculateTile(ref tilemap, tileSet, nearCell);
//         if (primaryTile != null)
//         {
//             PrimaryTileData primaryTileData = new PrimaryTileData(primaryTile, nearCell);
//             currentPrimaryTileDatas.Add(primaryTileData);
//             SetTileChain(ref tilemap, primaryTile, nearCell);
//         }
//     }

//     public static void RemoveTile(ref Tilemap tilemap, TileSet tileSet, Vector3 location)
//     {
//         Vector3Int nearCell = tilemap.LocalToCell(location);

//         PrimaryTile primaryTile = GetPrimaryTile(nearCell);

//         if (primaryTile != null) {
//             PrimaryTileData  primaryTileData = new PrimaryTileData(primaryTile, nearCell);
//             currentPrimaryTileDatas.Remove(primaryTileData);
            
//         }
//     }

//     static PrimaryTile GetPrimaryTile(Vector3Int origin)
//     {
//         for (int i = 0; i < currentPrimaryTileDatas.Count; i++)
//         {
//             if (currentPrimaryTileDatas[i].cellPosition == origin)
//             {
//                 return currentPrimaryTileDatas[i].primaryTile;
//             }
//         }
//         return null;
//     }

//     static PrimaryTile CalculateTile(ref Tilemap tilemap, TileSet tileSet, Vector3Int cellLocation)
//     {
//         if (IsLoneTile(tilemap, cellLocation))
//             return tileSet.groundSet[0];



//         if (!tilemap.HasTile(cellLocation) || !IsPrimaryTile(tilemap, cellLocation))
//         {
//             //Check if Vertical first

//             //Check over
//             if (IsAboveTile(tilemap, cellLocation) != "NOT")
//             {
//                 string resultCode = IsAboveTile(tilemap, cellLocation);

//                 switch (resultCode)
//                 {
//                     case "OVER_WALL_LEFT":
//                         Debug.Log("WL");
//                         SetTileChain(ref tilemap, tileSet.wallLeftSet[0], GetNearbyCell(cellLocation, Direction.down));
//                         return tileSet.topRight;

//                     case "OVER_WALL_RIGHT":
//                         Debug.Log("WR");
//                         SetTileChain(ref tilemap, tileSet.wallRightSet[0], GetNearbyCell(cellLocation, Direction.down));
//                         return tileSet.topLeft;

//                     case "OVER_CORNER_TOP_RIGHT":
//                         Debug.Log("CR");
//                         SetTileChain(ref tilemap, tileSet.wallRightSet[0], GetNearbyCell(cellLocation, Direction.down));
//                         return tileSet.topLeft;

//                     case "OVER_CORNER_TOP_LEFT":
//                         Debug.Log("CL");
//                         SetTileChain(ref tilemap, tileSet.wallLeftSet[0], GetNearbyCell(cellLocation, Direction.down));
//                         return tileSet.topRight;

//                     case "OVER_CORNER_BOTTOM_RIGHT":
//                         SetTileChain(ref tilemap, tileSet.topRight, GetNearbyCell(cellLocation, Direction.up));
//                         return tileSet.wallLeftSet[0];

//                     case "OVER_CORNER_BOTTOM_LEFT":
//                         SetTileChain(ref tilemap, tileSet.topLeft, GetNearbyCell(cellLocation, Direction.up));
//                         return tileSet.wallRightSet[0];

//                     case "OVER_OTHER":
//                         return tileSet.groundSet[0];
//                 }
//             }

//             //Check under
//             if (IsUnderTile(tilemap, cellLocation) != "NOT")
//             {
//                 string resultCode = IsUnderTile(tilemap, cellLocation);
//                 Vector3Int upCell = GetNearbyCell(cellLocation, Direction.up);

//                 switch (resultCode)
//                 {
//                     case "UNDER_OTHER":
//                         Vector3Int rightCell = GetNearbyCell(cellLocation, Direction.right);
//                         if (IsPrimaryTile(tilemap, rightCell) && IsCornerTile(tilemap, rightCell))
//                         {
//                             if (IsPrimaryTile(tilemap, GetNearbyCell(rightCell, Direction.right)))
//                                 SetTileChain(ref tilemap, tileSet.roofSet[0], rightCell);
//                             return (tileSet.bottomLeft);
//                         }

//                         Vector3Int leftCell = GetNearbyCell(cellLocation, Direction.right);
//                         if (IsPrimaryTile(tilemap, leftCell) && IsCornerTile(tilemap, leftCell))
//                         {
//                             if (IsPrimaryTile(tilemap, GetNearbyCell(leftCell, Direction.left)))
//                                 SetTileChain(ref tilemap, tileSet.roofSet[0], leftCell);
//                             return (tileSet.bottomRight);
//                         }

//                         return tileSet.blackSpace;

//                     case "UNDER_LEFT":
//                         if (tilemap.HasTile(GetNearbyCell(upCell, Direction.up)) && IsPrimaryTile(tilemap, GetNearbyCell(upCell, Direction.up)))
//                             SetTileChain(ref tilemap, tileSet.wallRightSet[0], upCell);
//                         return tileSet.bottomLeft;

//                     case "UNDER_RIGHT":
//                         if (tilemap.HasTile(GetNearbyCell(upCell, Direction.up)) && IsPrimaryTile(tilemap, GetNearbyCell(upCell, Direction.up)))
//                             SetTileChain(ref tilemap, tileSet.wallLeftSet[0], upCell);
//                         return tileSet.bottomRight;

//                         // case "UNDER_CORNER_LEFT":
//                         //     //Debug.Log("CL");
//                         //     return tileSet.wallRightSet[0];

//                         // case "UNDER_CORNER_RIGHT":
//                         //     //Debug.Log("CR");
//                         //     return tileSet.wallLeftSet[0];


//                         // case "UNDER_WALL_RIGHT":
//                         //     // Debug.Log("WR");
//                         //     return tileSet.wallRightSet[0];

//                         // case "UNDER_WALL_LEFT":
//                         //     // Debug.Log("WL");

//                         //     return tileSet.wallLeftSet[0];
//                 }
//             }

//             //Check if horizontal tile
//             if (IsNearGroundTile(tilemap, cellLocation) != "NOT")
//             {
//                 string resultCode = IsNearGroundTile(tilemap, cellLocation);


//                 Vector3Int leftCell = GetNearbyCell(cellLocation, Direction.left);
//                 Vector3Int rightCell = GetNearbyCell(cellLocation, Direction.right);
//                 switch (resultCode)
//                 {
//                     case "LEFT_RIGHT":

//                         if (!IsWallTile(tilemap, leftCell) && (!IsCornerTile(tilemap, leftCell)))
//                             SetTileChain(ref tilemap, tileSet.groundSet[0], GetNearbyCell(cellLocation, Direction.left));
//                         if (!IsWallTile(tilemap, rightCell) && !IsCornerTile(tilemap, rightCell))
//                             SetTileChain(ref tilemap, tileSet.groundSet[0], GetNearbyCell(cellLocation, Direction.right));
//                         return tileSet.groundSet[0];
//                     case "LEFT":
//                         Debug.Log("L");
//                         if (!IsWallTile(tilemap, leftCell))
//                         {
//                             if (IsCornerTile(tilemap, leftCell) && IsPrimaryTile(tilemap, GetNearbyCell(leftCell, Direction.up)))
//                             {
//                                 SetTileChain(ref tilemap, tileSet.wallLeftSet[0], leftCell);
//                                 return tileSet.topRight;
//                             }

//                             if (GetTileCode(tilemap, leftCell) == "BOTTOM")
//                             {
//                                 if (IsPrimaryTile(tilemap, GetNearbyCell(leftCell, Direction.left)))
//                                     SetTileChain(ref tilemap, tileSet.roofSet[0], leftCell);
//                                 return tileSet.bottomRight;
//                             }


//                             if ((tilemap.HasTile(GetNearbyCell(leftCell, Direction.left))))
//                                 SetTileChain(ref tilemap, tileSet.groundSet[0], GetNearbyCell(cellLocation, Direction.left));
//                             return tileSet.topRight;

//                         }
//                         else
//                         {
//                             if (!tilemap.HasTile(GetNearbyCell(rightCell, Direction.right)))
//                                 SetTileChain(ref tilemap, tileSet.bottomRight, rightCell);

//                             return tileSet.roofSet[0];
//                         }


//                     case "RIGHT":
//                         Debug.Log("R");
//                         if (!IsWallTile(tilemap, rightCell))
//                         {
//                             //Debug.Log("Corner: " + IsCornerTile(tilemap, rightCell));
//                             if (IsCornerTile(tilemap, rightCell) && IsPrimaryTile(tilemap, GetNearbyCell(rightCell, Direction.up)))
//                             {
//                                 SetTileChain(ref tilemap, tileSet.wallRightSet[0], rightCell);
//                                 return tileSet.topLeft;
//                             }

//                             if (GetTileCode(tilemap, rightCell) == "BOTTOM")
//                             {
//                                 if (IsPrimaryTile(tilemap, GetNearbyCell(rightCell, Direction.right)))
//                                     SetTileChain(ref tilemap, tileSet.roofSet[0], rightCell);
//                                 return tileSet.bottomLeft;
//                             }

//                             if ((tilemap.HasTile(GetNearbyCell(rightCell, Direction.right))))
//                                 SetTileChain(ref tilemap, tileSet.groundSet[0], GetNearbyCell(cellLocation, Direction.right));
//                             return tileSet.topLeft;

//                         }
//                         else
//                         {
//                             Debug.Log("Jom");
//                             if (!tilemap.HasTile(GetNearbyCell(leftCell, Direction.left)))
//                                 SetTileChain(ref tilemap, tileSet.bottomRight, rightCell);

//                             return tileSet.roofSet[0];
//                         }
//                 }
//             }
//         }

//         return null;
//     }

//     static void SetTileChain(ref Tilemap tilemap, PrimaryTile primaryTile, Vector3Int cellLocation)
//     {
//         tilemap.SetTile(cellLocation, primaryTile.tile);

//         if (primaryTile.attachedTiles != null)
//             for (int i = 0; i < primaryTile.attachedTiles.Length; i++)
//             {
//                 Direction direction = primaryTile.attachedTiles[i].direction;
//                 Tile tile = primaryTile.attachedTiles[i].tile;

//                 if (tile != null)
//                     tilemap.SetTile(GetNearbyCell(cellLocation, direction), tile);
//             }
//     }

//     static Vector3Int GetNearbyCell(Vector3Int cellLocation, Direction direction)
//     {
//         switch (direction)
//         {
//             case Direction.down:
//                 return new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z);
//             case Direction.downLeft:
//                 return new Vector3Int(cellLocation.x - 1, cellLocation.y - 1, cellLocation.z);
//             case Direction.downRight:
//                 return new Vector3Int(cellLocation.x + 1, cellLocation.y - 1, cellLocation.z);
//             case Direction.left:
//                 return new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z);
//             case Direction.right:
//                 return new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z);
//             case Direction.up:
//                 return new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z);
//             case Direction.upLeft:
//                 return new Vector3Int(cellLocation.x - 1, cellLocation.y + 1, cellLocation.z);
//             case Direction.upRight:
//                 return new Vector3Int(cellLocation.x + 1, cellLocation.y + 1, cellLocation.z);
//         }
//         return Vector3Int.zero;
//     }

//     /***********************************************/
//     /***********************************************/
//     /*          TILESET PRIMING FUNCTIONS          */
//     /***********************************************/
//     /***********************************************/

//     public static void SetTileSetByFileName(string folderName, ref TileSet tileSet)
//     {
//         string folderPath = "Assets/Resources/Bitmaps/" + folderName + "/";
//         List<PrimaryTile> primaryTileList = new List<PrimaryTile>();
//         List<AttachedTile> attachedTileList = new List<AttachedTile>();

//         string fileName = folderName + "_Ground_";

//         Debug.Log(folderPath + fileName);

//         SetTileSetArray(ref tileSet.groundSet, folderPath, fileName, true, Direction.down);

//         fileName = folderName + "_Wall_Left_";

//         SetTileSetArray(ref tileSet.wallLeftSet, folderPath, fileName, false, Direction.left);

//         fileName = folderName + "_Wall_Right_";

//         SetTileSetArray(ref tileSet.wallRightSet, folderPath, fileName, false, Direction.right);


//         fileName = folderName + "_Roof_";

//         SetTileSetArray(ref tileSet.roofSet, folderPath, fileName, false, Direction.up);


//         fileName = folderName + "_Top_Right_";
//         SetTileSetSingle(ref tileSet.topRight, folderPath, fileName, true, Direction.up);

//         fileName = folderName + "_Top_Left_";
//         SetTileSetSingle(ref tileSet.topLeft, folderPath, fileName, true, Direction.up);

//         fileName = folderName + "_Bottom_Right_";
//         SetTileSetSingle(ref tileSet.bottomRight, folderPath, fileName, true, Direction.up);

//         fileName = folderName + "_Bottom_Left_";
//         SetTileSetSingle(ref tileSet.bottomLeft, folderPath, fileName, true, Direction.up);

//         fileName = folderName + "_BlackSpace_";
//         SetTileSetSingle(ref tileSet.blackSpace, folderPath, fileName, true, Direction.up);
//     }

//     static void SetTileSetSingle(ref PrimaryTile tileSet, string folderPath, string fileName, bool ground, Direction attachDirection)
//     {
//         List<AttachedTile> attachedTileList = new List<AttachedTile>();
//         int secondaryIndex = 1;
//         bool secondarySetDone = false;

//         Tile tile = (Tile)AssetDatabase.LoadAssetAtPath(folderPath + fileName + "P.asset", typeof(Tile));

//         //Debug.Log(folderPath + fileName + "P.asset");
//         while (!secondarySetDone)
//         {
//             // Tile tile_2 = Resources.Load<Tile>(folderPath + fileName + primaryIndex.ToString() + "_" + secondaryIndex.ToString());
//             Tile tile_2 = (Tile)AssetDatabase.LoadAssetAtPath(folderPath + fileName + secondaryIndex.ToString() + ".asset", typeof(Tile));
//             //Debug.Log(folderPath + fileName + secondaryIndex.ToString() + ".asset");

//             if (tile_2 != null)
//             {
//                 // Debug.Log("Joom " + tile_2.name);
//                 Direction direction;
//                 if (ground)
//                     direction = (secondaryIndex == 1) ? Direction.up : Direction.down;
//                 else
//                     direction = attachDirection;
//                 AttachedTile aTile = new AttachedTile(tile_2, direction);
//                 attachedTileList.Add(aTile);
//                 secondaryIndex++;
//             }
//             else
//             {
//                 // Debug.Log("Joompf");
//                 secondaryIndex = 1;
//                 secondarySetDone = true;
//                 PrimaryTile pTile = new PrimaryTile(tile);
//                 if (attachedTileList.Count > 0)
//                 {
//                     pTile.attachedTiles = new AttachedTile[attachedTileList.Count];
//                     for (int i = 0; i < attachedTileList.Count; i++)
//                     {
//                         pTile.attachedTiles[i] = attachedTileList[i];
//                     }
//                     attachedTileList.Clear();
//                 }
//                 tileSet = new PrimaryTile();
//                 tileSet = pTile;
//                 //secondarySetDone = false;
//             }
//         }
//     }

//     static void SetTileSetArray(ref PrimaryTile[] tileSet, string folderPath, string fileName, bool ground, Direction attachDirection)
//     {
//         bool primarySetDone = false;
//         bool secondarySetDone = false;
//         int primaryIndex = 0;
//         int secondaryIndex = 1;
//         List<PrimaryTile> primaryTileList = new List<PrimaryTile>();
//         List<AttachedTile> attachedTileList = new List<AttachedTile>();
//         // Tile tile = Resources.Load<Tile>(folderPath + fileName + primaryIndex.ToString() + "_P.asset");

//         while (!primarySetDone)
//         {
//             Tile tile = (Tile)AssetDatabase.LoadAssetAtPath(folderPath + fileName + primaryIndex.ToString() + "_P.asset", typeof(Tile));

//             //Debug.Log(folderPath + fileName + primaryIndex.ToString() + "_P.asset");

//             if (tile != null)
//             {
//                 // Debug.Log("Floom " + tile.name);
//                 while (!secondarySetDone)
//                 {
//                     // Tile tile_2 = Resources.Load<Tile>(folderPath + fileName + primaryIndex.ToString() + "_" + secondaryIndex.ToString());
//                     Tile tile_2 = (Tile)AssetDatabase.LoadAssetAtPath(folderPath + fileName + primaryIndex.ToString() + "_" + secondaryIndex.ToString() + ".asset", typeof(Tile));

//                     if (tile_2 != null)
//                     {
//                         // Debug.Log("Joom " + tile_2.name);
//                         Direction direction;
//                         if (ground)
//                             direction = (secondaryIndex == 1) ? Direction.up : Direction.down;
//                         else
//                             direction = attachDirection;
//                         AttachedTile aTile = new AttachedTile(tile_2, direction);
//                         attachedTileList.Add(aTile);
//                         secondaryIndex++;
//                     }
//                     else
//                     {
//                         secondaryIndex = 1;
//                         secondarySetDone = true;
//                     }
//                 }
//                 PrimaryTile pTile = new PrimaryTile(tile);
//                 secondarySetDone = false;
//                 if (attachedTileList.Count > 0)
//                 {
//                     pTile.attachedTiles = new AttachedTile[attachedTileList.Count];
//                     for (int i = 0; i < attachedTileList.Count; i++)
//                     {
//                         pTile.attachedTiles[i] = attachedTileList[i];
//                     }
//                     attachedTileList.Clear();
//                 }
//                 primaryTileList.Add(pTile);
//                 primaryIndex++;
//             }
//             else
//             {
//                 tileSet = new PrimaryTile[primaryTileList.Count];
//                 for (int i = 0; i < primaryTileList.Count; i++)
//                 {
//                     // Debug.Log(primaryTileList[i].tile.name);
//                     tileSet[i] = primaryTileList[i];
//                 }
//                 primaryIndex = 0;
//                 primarySetDone = true;
//             }
//         }
//     }
// }

// /***********************************************/
// /***********************************************/
// /*      LEVEL EDITING STRUCTS AND CLASSES      */
// /***********************************************/
// /***********************************************/

// [System.Serializable]
// public class TileSet
// {
//     public PrimaryTile[] groundSet;
//     public PrimaryTile[] roofSet;
//     public PrimaryTile[] wallLeftSet;
//     public PrimaryTile[] wallRightSet;
//     public PrimaryTile blackSpace;
//     public PrimaryTile topLeft;
//     public PrimaryTile topRight;
//     public PrimaryTile bottomLeft;
//     public PrimaryTile bottomRight;
// }

// [System.Serializable]
// public class PrimaryTileData
// {
//     public PrimaryTile primaryTile;
//     public Vector3Int cellPosition;

//     public PrimaryTileData(PrimaryTile _primaryTile, Vector3Int _cellPosition) {
//         primaryTile = _primaryTile;
//         cellPosition = _cellPosition;
//     }
// }

// [System.Serializable]
// public class PrimaryTile
// {
//     public Tile tile;
//     public AttachedTile[] attachedTiles;

//     public PrimaryTile()
//     {

//     }
//     public PrimaryTile(Tile _tile)
//     {
//         tile = _tile;
//     }
//     public PrimaryTile(Tile _tile, AttachedTile[] _attachedTiles)
//     {
//         tile = _tile;
//         attachedTiles = _attachedTiles;
//     }
// }
// [System.Serializable]
// public class AttachedTile
// {
//     public Direction direction;
//     public Tile tile;

//     public AttachedTile(Tile _tile, Direction _direction)
//     {
//         tile = _tile;
//         direction = _direction;
//     }
// }


