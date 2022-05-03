// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Parallax : MonoBehaviour
// {
//     float length, startPosition;
//     public GameObject cam;
//     [Range(0, 10)]
//     public float parallaxEffect;

//     private void Start()
//     {
//         startPosition = transform.position.x;
//         length = GetComponent<SpriteRenderer>().bounds.size.x;
//     }

//     private void FixedUpdate()
//     {
//         float temp = (cam.transform.position.x * (1 - parallaxEffect));
//         float positionY = cam.transform.position.y;

//         float dist = (cam.transform.position.x * parallaxEffect);

//         transform.position = new Vector3(startPosition + dist, positionY, cam.transform.position.z);

//         if (temp > startPosition - length) startPosition -= length;
//     }
// }
