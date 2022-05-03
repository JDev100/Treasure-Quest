// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;
// using UnityEditor;
// using UnityEngine.Tilemaps;


// public class LevelEditor : MonoBehaviour
// {
//     public enum EditorMode { handTool, moveTool, tilePlacement, enemyPlacement, musicSelect };
//     public enum TileEditorMode { auto, box, absolute };

//     public static LevelEditor instance;


//     [Header("UI")]
//     float cameraSpeedMouse = 10;
//     float cameraSpeedWASD = 1;
//     float cameraZoomSpeed = 5;
//     public Texture2D handToolTexture;
//     public Texture2D eraserToolTexture;
//     public Sprite grideHighlight;
//     public List<GameObject> buttons = new List<GameObject>();
//     [HideInInspector]
//     public EditorMode currentMode = EditorMode.handTool;
//     int modeIndex = 0;


//     [Header("Level Editing")]
//     Grid grid;
//     public Transform visualGrid;
//     bool levelEditingActive = false;
//     public Tilemap levelToEdit;
//     public bool hasSelected;
//     GameObject gridHighlighter;

//     [Header("Tile Editing")]
//     TileEditorMode currentTileEditorMode = TileEditorMode.auto;
//     public string tileSetFolder;
//     TileSet tileSet;

//     [Header("Interactables")]
//     public List<GameObject> mobCompendium;
//     GameObject objectBuffer;
//     GameObject objectMarker;
//     [HideInInspector]
//     List<string> currentInteractableSetup = new List<string>();
//     int mobIndex = 0;

//     private void Awake()
//     {
//         if (instance == null)
//         {
//             instance = this;
//         }
//         else
//         {
//             Destroy(instance);
//         }
//     }

//     void Start()
//     {
//         tileSet = new TileSet();
//         TileEditor.SetTileSetByFileName(tileSetFolder, ref tileSet);

//         grid = GetComponent<Grid>();
//         Camera.main.transform.position = new Vector3(0, 0, -100);

//         objectBuffer = new GameObject("ObjectBuffer");

//         objectMarker = new GameObject("Object_Marker",
//                                        new System.Type[] { typeof(SpriteRenderer) });

//         gridHighlighter = new GameObject("Grid_Highlighter", new System.Type[] { typeof(SpriteRenderer) });

//         objectMarker.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .5f);
//         gridHighlighter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

//         gridHighlighter.GetComponent<SpriteRenderer>().sprite = grideHighlight;
//         cameraSpeedMouse = 510f;
//         cameraSpeedWASD = 51f;
//         // EventSystem.current.SetSelectedGameObject(null);
//         // EventSystem.current.SetSelectedGameObject(buttons[0]);

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         //Test
//         if (Input.GetKeyDown(KeyCode.L))
//         {
//             LoadLevel();
//         }

//         //Switch mode based on numpad
//         if (Input.GetKeyDown(KeyCode.Alpha1))
//         {
//             currentMode = (EditorMode)0;
//             modeIndex = 0;
//         }
//         if (Input.GetKeyDown(KeyCode.Alpha2))
//         {
//             currentMode = (EditorMode)1;
//             modeIndex = 1;
//         }
//         if (Input.GetKeyDown(KeyCode.Alpha3))
//         {
//             currentMode = (EditorMode)2;
//             modeIndex = 2;
//         }
//         if (Input.GetKeyDown(KeyCode.Alpha4))
//         {
//             currentMode = (EditorMode)3;
//             modeIndex = 3;
//         }

//         //Deactivate or Activate ObjectMarker based on mode
//         if (currentMode == EditorMode.enemyPlacement)
//         {
//             objectMarker.SetActive(true);
//         }
//         else
//         {
//             objectMarker.SetActive(false);

//         }

//         gridHighlighter.GetComponent<SpriteRenderer>().sortingOrder = -1;

//         //Snap visual grid to cameraposition

//         Vector3Int nearCell = grid.LocalToCell(Camera.main.transform.position);
//         Vector3 finalPos = grid.CellToLocal(nearCell);
//         visualGrid.transform.position = new Vector3(finalPos.x, finalPos.y, visualGrid.transform.position.z);


//         EventSystem.current.SetSelectedGameObject(null);
//         EventSystem.current.SetSelectedGameObject(buttons[modeIndex]);
//         HandleEditorMode(currentMode);
//     }
//     /***********************************************/
//     /***********************************************/
//     /*          LEVEL EDITING                      */
//     /***********************************************/
//     /***********************************************/
//     public void SetEditorMode(int mode)
//     {
//         currentMode = (LevelEditor.EditorMode)mode;
//         if (mode < 5)
//             modeIndex = mode;
//     }
//     void HandleEditorMode(EditorMode mode)
//     {
//         switch (mode)
//         {
//             case EditorMode.handTool:
//                 HandTool(true, true, true);
//                 break;
//             case EditorMode.moveTool:
//                 HandTool(false, true, true);
//                 break;
//             case EditorMode.tilePlacement:
//                 TileEditorTool();
//                 HandTool(false, false, false);
//                 break;
//             case EditorMode.enemyPlacement:
//                 HandTool(false, false, false);
//                 PlaceTool(mobCompendium, ref mobIndex);
//                 break;
//         }
//     }
//     void HandTool(bool canLeftClick = false, bool canRightClick = true, bool canZoom = true)
//     {
//         bool clicking;

//         bool leftClick = (canLeftClick) ? Input.GetMouseButton(0) : false;
//         bool rightClick = (canRightClick) ? Input.GetMouseButton(1) : false;

//         //  if (canLeftClick)
//         clicking = (leftClick ||
//                            rightClick ||
//                             Input.GetMouseButton(2));

//         if (Input.GetAxis("Mouse ScrollWheel") > 0)
//         {
//             if (Input.GetKey(KeyCode.LeftControl) && canZoom)
//             {
//                 cameraSpeedMouse += 100;
//                 cameraSpeedWASD += 10;
//                 // cameraZoomSpeed += 1;
//             }
//             else
//             {
//                 if (canZoom)
//                 {
//                     Camera.main.orthographicSize -= cameraZoomSpeed;
//                 }
//                 else
//                 {
//                     if (Input.GetKey(KeyCode.LeftControl))
//                     {
//                         Camera.main.orthographicSize -= cameraZoomSpeed;
//                     }
//                 }
//             }
//         }
//         if (Input.GetAxis("Mouse ScrollWheel") < 0)
//         {
//             if (Input.GetKey(KeyCode.LeftControl) && canZoom)
//             {
//                 cameraSpeedMouse -= 100;
//                 cameraSpeedWASD -= 10;
//                 // cameraZoomSpeed -= 1;
//             }
//             else
//             {
//                 if (canZoom)
//                 {
//                     Camera.main.orthographicSize += cameraZoomSpeed;
//                 }
//                 else
//                 {
//                     if (Input.GetKey(KeyCode.LeftControl))
//                     {
//                         Camera.main.orthographicSize += cameraZoomSpeed;
//                     }
//                 }
//             }
//         }

//         Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3, 40);
//         cameraSpeedMouse = Mathf.Clamp(cameraSpeedMouse, 10f, 1000f);
//         cameraSpeedWASD = Mathf.Clamp(cameraSpeedWASD, 1f, 100);
//         //cameraZoomSpeed = Mathf.Clamp(cameraZoomSpeed, 1f, 10f);




//         if (clicking)
//         {
//             Cursor.SetCursor(handToolTexture, Vector2.zero, CursorMode.Auto);
//             if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0)
//             {
//                 Camera.main.transform.position += new Vector3(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * cameraSpeedMouse,
//                                                                 -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * cameraSpeedMouse, 0);
//             }
//             else if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse Y") < 0)
//             {
//                 Camera.main.transform.position += new Vector3(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * cameraSpeedMouse,
//                                                                 -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * cameraSpeedMouse, 0);
//             }

//         }
//         else
//         {
//             if (currentMode != EditorMode.enemyPlacement)
//                 Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

//             if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0)
//             {
//                 Camera.main.transform.position += new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * cameraSpeedWASD,
//                                                                 Input.GetAxisRaw("Vertical") * Time.deltaTime * cameraSpeedWASD, 0);
//             }
//             else if (Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0)
//             {
//                 Camera.main.transform.position += new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * cameraSpeedWASD,
//                                                                 Input.GetAxisRaw("Vertical") * Time.deltaTime * cameraSpeedWASD, 0);
//             }
//         }
//     }

//     void PlaceTool(List<GameObject> list, ref int index)
//     {
//         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//         Vector3Int nearCell = grid.LocalToCell(mousePos);
//         Vector3 finalPos = grid.GetCellCenterLocal(nearCell);


//         objectMarker.transform.position = new Vector3(finalPos.x, finalPos.y, objectMarker.transform.position.z);

//         // Debug.Log("Dir: " + dir + "Worldp: " + mousePos);
//         // Vector3 dirNormal


//         Sprite sprite = list[index].GetComponent<SpriteRenderer>().sprite;
//         if (sprite == null)
//             sprite = list[index].GetComponentInChildren<SpriteRenderer>().sprite;

//         objectMarker.GetComponent<SpriteRenderer>().sprite = sprite;

//         if (!Input.GetKey(KeyCode.LeftControl))
//         {
//             if (Input.GetAxis("Mouse ScrollWheel") > 0)
//             {
//                 ScrollList(mobCompendium, ref index, true);
//             }
//             else if (Input.GetAxis("Mouse ScrollWheel") < 0)
//             {
//                 ScrollList(mobCompendium, ref index, false);
//             }
//         }

//         //Place on left click
//         if (Input.GetMouseButtonDown(0) && GetObjectAtMouse() == null)
//         {
//             GameObject newObjectMarker = new GameObject(list[index].name,
//                                      new System.Type[] { typeof(ObjectMarker), typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Clickable) });

//             newObjectMarker.transform.position = new Vector3(finalPos.x, finalPos.y, 0.0f);
//             newObjectMarker.GetComponent<ObjectMarker>().SetInteractable(list[index], new Vector3(finalPos.x, finalPos.y, 0.0f));
//             newObjectMarker.GetComponent<SpriteRenderer>().sprite = sprite;
//             newObjectMarker.tag = "ObjectMarker";
//             newObjectMarker.transform.parent = objectBuffer.transform;

//             Interactable interactable = newObjectMarker.GetComponent<ObjectMarker>().GetInteractable();
//             currentInteractableSetup.Add(ConvertInteractableToString(interactable));
//             Debug.Log(currentInteractableSetup[currentInteractableSetup.Count - 1]);
//         }
//         else if (Input.GetMouseButton(0) && (GetObjectAtMouse() || hasSelected))
//         {
//             hasSelected = true;
//             objectMarker.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
//             //  Debug.Log("Floom");

//             //HandTool(true, false, true);
//         }
//         else if (Input.GetMouseButton(1))
//         {
//             Cursor.SetCursor(eraserToolTexture, Vector2.zero, CursorMode.Auto);
//             objectMarker.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

//             //Debug.Log("Pressing Down");
//             GameObject go = GetObjectAtMouse();
//             if (go != null)
//             {
//                 if (go.tag == "ObjectMarker")
//                 {
//                     string interactable = ConvertInteractableToString(go.GetComponent<ObjectMarker>().GetInteractable());
//                     currentInteractableSetup.Remove(interactable);
//                     Destroy(go);
//                 }
//             }
//         }
//         else
//         {
//             objectMarker.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);

//             Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
//             hasSelected = false;
//         }
//     }

//     public void TileEditorTool()
//     {
//         if (Input.GetMouseButton(0))
//         {
//             Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//             TileEditor.PlaceTile(ref levelToEdit, tileSet, mousePos);
//         }
//     }

//     /***********************************************/
//     /***********************************************/
//     /*          LEVEL EDITING HELPER FUNCTIONS     */
//     /***********************************************/
//     /***********************************************/
//     void ScrollList(List<GameObject> list, ref int index, bool next = true)
//     {
//         if (next)
//         {
//             index = (index + 1 < list.Count) ? index + 1 : 0;
//         }
//         else
//         {
//             index = (index - 1 > 0) ? index - 1 : list.Count - 1;
//         }
//     }

//     GameObject GetObjectAtMouse()
//     {
//         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//         Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

//         RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

//         // Debug.Log(mousePos);

//         if (hit)
//         {
//             //Debug.Log(hit.collider.gameObject.name);
//             return hit.collider.gameObject;
//         }
//         return null;
//     }

//     public void ShowGridHighlight(bool show = false)
//     {
//         if (show)
//         {
//             gridHighlighter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
//             Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//             Vector3Int nearCell = grid.LocalToCell(mousePos);
//             Vector3 finalPos = grid.GetCellCenterLocal(nearCell);


//             gridHighlighter.transform.position = new Vector3(finalPos.x, finalPos.y, gridHighlighter.transform.position.z);
//         }
//         else
//         {
//             gridHighlighter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
//         }

//     }

//     /***********************************************/
//     /***********************************************/
//     /*          SAVE AND LOAD                      */
//     /***********************************************/
//     /***********************************************/
//     public void SaveLevel()
//     {
//         SaveSystem.SaveLevel(currentInteractableSetup);
//     }
//     public void LoadLevel()
//     {
//         foreach (Transform child in objectBuffer.transform)
//             Destroy(child.gameObject);

//         LevelData levelData = SaveSystem.LoadLevel();

//         currentInteractableSetup.Clear();

//         for (int i = 0; i < levelData.interactables.Length; i++)
//         {
//             string[] splitComponents = levelData.interactables[i].Split();
//             string path = splitComponents[0];
//             float xPosition = float.Parse(splitComponents[1]);
//             float yPosition = float.Parse(splitComponents[2]);
//             float zPosition = float.Parse(splitComponents[3]);


//             GameObject newObjectMarker = new GameObject(AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)).name,
//                                        new System.Type[] { typeof(ObjectMarker), typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Clickable) });

//             newObjectMarker.transform.position = new Vector3(xPosition, yPosition, zPosition);
//             newObjectMarker.GetComponent<ObjectMarker>().SetInteractable((GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)), new Vector3(xPosition, yPosition, zPosition));

//             GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
//             newObjectMarker.GetComponent<SpriteRenderer>().sprite = go.GetComponent<SpriteRenderer>().sprite;
//             newObjectMarker.tag = "ObjectMarker";
//             newObjectMarker.transform.parent = objectBuffer.transform;
//             Interactable interactable = newObjectMarker.GetComponent<ObjectMarker>().GetInteractable();
//             currentInteractableSetup.Add(ConvertInteractableToString(interactable));
//         }
//     }

//     /***********************************************/
//     /***********************************************/
//     /*          SAVE AND LOAD HELPER FUNCTIONS     */
//     /***********************************************/
//     /***********************************************/
//     string ConvertInteractableToString(Interactable interactable)
//     {
//         string path = AssetDatabase.GetAssetPath(interactable.gameObject);
//         string positionX = string.Format("{0:G}", interactable.location.x);
//         string positionY = string.Format("{0:G}", interactable.location.y);
//         string positionZ = string.Format("{0:G}", interactable.location.z);

//         string objectMarker = path + " " + positionX + " " + positionY + " " + positionZ;

//         return objectMarker;
//     }


//     /***********************************************/
//     /***********************************************/
//     /*          ANIMATION HELPER FUNCTIONS         */
//     /***********************************************/
//     /***********************************************/
//     public void ToggleSideBar()
//     {
//         GetComponent<Animator>().SetTrigger("ToggleSidebar");
//     }
// }

