// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Clickable : MonoBehaviour
// {
//     private Vector3 screenPoint;
//     private Vector3 offset;

//     private void Start()
//     {
//         GetComponent<BoxCollider2D>().size = new Vector2(2, 2);
//     }

//     void OnMouseDown()
//     {
//         screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

//         offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

//         // Debug.Log("Screen Point: " + screenPoint);
//     }

//     void OnMouseDrag()
//     {
//         if (LevelEditor.instance.currentMode == LevelEditor.EditorMode.moveTool || LevelEditor.instance.hasSelected)
//         {
//             LevelEditor.instance.ShowGridHighlight(true);
//             Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
//             Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
//             transform.position = cursorPosition;
//         }
//     }
//     private void OnMouseUp()
//     {
//         LevelEditor.instance.ShowGridHighlight(false);
//     }


// }
