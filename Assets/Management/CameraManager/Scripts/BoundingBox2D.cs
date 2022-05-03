using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoundingBox2D : MonoBehaviour
{
    //public Vector2 center
    public Vector2 size;
    [HideInInspector]
    public Bounds boundingBox;
    // Start is called before the first frame update
    void Start()
    {
        boundingBox.center = transform.position;
        boundingBox.size = size;
        boundingBox.extents = new Vector2(size.x /2, size.y / 2);
    }

    // Update is called once per frame
    void Update()
    {
        boundingBox.center = transform.position;
    }
    
    /***********************************************/
    /***********************************************/
    /*          GIZMOS                             */
    /***********************************************/
    /***********************************************/
    void OnDrawGizmos()
    {
        // boundingBox.center = transform.position;
        // Gizmos.color = new Color(1, 0, 0, .1f);
        // Gizmos.DrawWireCube(boundingBox.center, size);
        
    }
}
