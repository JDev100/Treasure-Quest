using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoundaryManager : MonoBehaviour
{
    [Header("Preferences")]
    public bool lockToWorldCenter;

    [Header("Essential Components")]
    //private BoundingBox2D managerBox;
    private Transform player;
    public GameObject boundaryObject;
    private CameraFollowProto mainCamera;

    [Header("Boundary Setup")]
    public List<Boundary> boundarySetUp = new List<Boundary>();
    [HideInInspector]
    public Boundary currentBoundary;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<CameraFollowProto>().GetComponent<CameraFollowProto>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        foreach (Boundary boundary in boundarySetUp)
        {
            boundary.SetBounds();
        }
        AdjustBoundariesForLinks(ref boundarySetUp);

        // currentBoundary = boundarySetUp[0];
        if (player != null)
            currentBoundary = GetNextBoundary(player.position);


    }

    // Update is called once per frame
    void Update()
    {
        ManageBoundary();
        Debug.Log(currentBoundary.boundingBox.center);
    }

    void ManageBoundary()
    {
        if (player != null)
        {

            if (currentBoundary.boundingBox.min.x < player.position.x && player.position.x < currentBoundary.boundingBox.max.x &&
                currentBoundary.boundingBox.min.y < player.position.y && player.position.y < currentBoundary.boundingBox.max.y)
            {
                //boundaryObject.SetActive(true);
            }
            else
            {

                // boundaryObject.SetActive(false);
                currentBoundary = GetNextBoundary(player.position);
                bool lockX = (currentBoundary.boundingBox.size.x == 25);
                bool lockY = (currentBoundary.boundingBox.size.y == 13);
                // if (!lockY)
                //     lockX = true;
                mainCamera.StartScreenSwipe(currentBoundary.boundingBox, lockX, lockY);
            }
        }
    }

    public void LockToBoundary()
    {
        currentBoundary = GetNextBoundary(player.position);
    }

    Boundary GetNextBoundary(Vector2 playerPos)
    {
        for (int i = 0; i < boundarySetUp.Count; i++)
        {
            if (boundarySetUp[i].boundingBox.min.x < player.position.x && player.position.x < boundarySetUp[i].boundingBox.max.x &&
                boundarySetUp[i].boundingBox.min.y < player.position.y && player.position.y < boundarySetUp[i].boundingBox.max.y)
            {
                return boundarySetUp[i];
            }
        }
        return currentBoundary;
    }

    public void AdjustBoundariesForLinks(ref List<Boundary> boundaryList)
    {
        for (int i = 0; i < boundaryList.Count; i++)
        {
            if (i == 0 && lockToWorldCenter)
            {
                boundaryList[i].boundaryLink = Boundary.BoundaryLink.none;
                boundaryList[i].center = Vector2.zero;
                boundaryList[i].center += new Vector2(boundaryList[i].boundingBox.extents.x, boundaryList[i].boundingBox.extents.y);
            }
            else
            {
                switch (boundaryList[i].boundaryLink)
                {
                    case Boundary.BoundaryLink.left:
                        boundaryList[i].center = (boundaryList[i].anchorToCenter) ? boundaryList[i - 1].center : new Vector2(boundaryList[i - 1].center.x, boundaryList[i].center.y);
                        boundaryList[i].center -= new Vector2((boundaryList[i].size.x + boundaryList[i - 1].size.x) / 2, 0);
                        break;
                    case Boundary.BoundaryLink.right:
                        boundaryList[i].center = (boundaryList[i].anchorToCenter) ? boundaryList[i - 1].center : new Vector2(boundaryList[i - 1].center.x, boundaryList[i].center.y);
                        boundaryList[i].center += new Vector2((boundaryList[i].size.x + boundaryList[i - 1].size.x) / 2, 0);
                        break;
                    case Boundary.BoundaryLink.top:
                        boundaryList[i].center = (boundaryList[i].anchorToCenter) ? boundaryList[i - 1].center : new Vector2(boundaryList[i].center.x, boundaryList[i - 1].center.y);
                        boundaryList[i].center += new Vector2(0, (boundaryList[i].size.y + boundaryList[i - 1].size.y) / 2);
                        break;
                    case Boundary.BoundaryLink.bottom:
                        boundaryList[i].center = (boundaryList[i].anchorToCenter) ? boundaryList[i - 1].center : new Vector2(boundaryList[i].center.x, boundaryList[i - 1].center.y);
                        boundaryList[i].center -= new Vector2(0, (boundaryList[i].size.y + boundaryList[i - 1].size.y) / 2);
                        break;
                    case Boundary.BoundaryLink.topRight:
                        boundaryList[i].center = boundaryList[i - 1].center;
                        boundaryList[i].center += new Vector2((boundaryList[i].size.x + boundaryList[i - 1].size.x) / 2,
                                                                -Mathf.Abs(boundaryList[i].boundingBox.extents.y - boundaryList[i - 1].boundingBox.extents.y));
                        break;
                    case Boundary.BoundaryLink.bottomRight:
                        boundaryList[i].center = boundaryList[i - 1].center;
                        boundaryList[i].center += new Vector2((boundaryList[i].size.x + boundaryList[i - 1].size.x) / 2,
                                                                Mathf.Abs(boundaryList[i].boundingBox.extents.y - boundaryList[i - 1].boundingBox.extents.y));
                        break;
                    case Boundary.BoundaryLink.topLeft:
                        boundaryList[i].center = boundaryList[i - 1].center;
                        boundaryList[i].center -= new Vector2((boundaryList[i].size.x + boundaryList[i - 1].size.x) / 2,
                                                                Mathf.Abs(boundaryList[i].boundingBox.extents.y - boundaryList[i - 1].boundingBox.extents.y));
                        break;
                    case Boundary.BoundaryLink.bottomLeft:
                        boundaryList[i].center = boundaryList[i - 1].center;
                        boundaryList[i].center -= new Vector2((boundaryList[i].size.x + boundaryList[i - 1].size.x) / 2,
                                                                -Mathf.Abs(boundaryList[i].boundingBox.extents.y - boundaryList[i - 1].boundingBox.extents.y));
                        break;
                }
            }
        }
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


        foreach (Boundary boundary in boundarySetUp)
        {
            boundary.SetBounds();
        }
        AdjustBoundariesForLinks(ref boundarySetUp);

        for (int i = 0; i < boundarySetUp.Count; i++)
        {
            Gizmos.color = new Color(1, 0, 0);
            // if (i == 0)
            // {
            //     boundarySetUp[i].boundaryLink = Boundary.BoundaryLink.none;
            //     boundarySetUp[i].center = Vector2.zero;
            //     boundarySetUp[i].center += new Vector2(boundarySetUp[i].boundingBox.extents.x, boundarySetUp[i].boundingBox.extents.y);
            // }
            Gizmos.DrawWireCube(boundarySetUp[i].boundingBox.center, boundarySetUp[i].boundingBox.size);
        }

    }
}
[System.Serializable]
public class Boundary
{
    //Pick to clamp based on last
    public enum BoundaryLink { left, right, top, bottom, topLeft, topRight, bottomLeft, bottomRight, none }

    public bool anchorToCenter = true;

    public BoundaryLink boundaryLink = BoundaryLink.right;

    [HideInInspector]
    public Bounds boundingBox;

    public Vector2 size = new Vector2(1, 1);
    public Vector2 center = new Vector2(1, 1);

    public Boundary()
    {
        boundaryLink = BoundaryLink.none;
        boundingBox.extents = new Vector2(size.x / 2, size.y / 2);
        boundingBox.size = new Vector2(size.x, size.y);
        boundingBox.center = center;
    }

    public void SetBounds()
    {
        boundingBox.extents = new Vector2(size.x / 2, size.y / 2);
        boundingBox.size = new Vector2(size.x, size.y);
        boundingBox.center = center;
    }

    public void SetBounds(Vector2 _center, Vector2 _size)
    {
        boundingBox.extents = new Vector2(_size.x / 2, _size.y / 2);
        boundingBox.size = new Vector2(_size.x, _size.y);
        boundingBox.center = _center;
    }


}