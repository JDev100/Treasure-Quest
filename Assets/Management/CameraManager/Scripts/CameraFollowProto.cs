using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraEffects))]
public class CameraFollowProto : MonoBehaviour
{
    [Header("Following Parameters")]
    public Controller2D target;
    public Vector2 focusAreaSize;

    [Header("Camera Movement")]
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTime;
    public float verticalSmoothTime;
    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;
    bool lookAheadStopped;
    bool lockX;
    bool lockY;

    [Header("Essential Components")]
    FocusArea focusArea;
    BoundaryManager boundaryMgr;
    Bounds cameraBox;
    Vector2 cameraBoxSize;

    [Header("Screen Sweep Values")]
    public float screenSwipeDuration;
    Vector2 startPos, endPos;
    Vector2 oldPosition;
    float timeStartedLerping;
    bool screenSwiping;

    // Camera camera;
    // Start is called before the first frame update

    /***********************************************/
    /***********************************************/
    /*          START FUNCTION                     */
    /***********************************************/
    /***********************************************/
    void Start()
    {
        //camera = Camera.main;
        //Set Essential Components
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller2D>();
        focusArea = new FocusArea(target.boxCollider.bounds, focusAreaSize);
        boundaryMgr = FindObjectOfType<BoundaryManager>();
    }

    // Update is called once per frame
    /***********************************************/
    /***********************************************/
    /*          LATE UPDATE FUNCTION               */
    /***********************************************/
    /***********************************************/

    void LateUpdate()
    {
        if (boundaryMgr == null)
        {
            boundaryMgr = FindObjectOfType<BoundaryManager>();
        }

        cameraBoxSize = new Vector2((Camera.main.aspect * Camera.main.orthographicSize) * 2, Camera.main.orthographicSize * 2);

        cameraBox.extents = new Vector2(cameraBoxSize.x / 2, cameraBoxSize.y / 2);

        if (target != null && InputManager.instance.CanInput())
        {
            focusArea.UpdateFocusArea(target.boxCollider.bounds);

            Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

            if (focusArea.velocity.x != 0)
            {
                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
                if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
                {
                    lookAheadStopped = false;
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else
                {
                    if (!lookAheadStopped)
                    {
                        lookAheadStopped = true;
                        targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                    }
                }

            }
            //targetLookAheadX = lookAheadDirX * lookAheadDstX;
            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTime);

            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
            focusPosition += Vector2.right * currentLookAheadX;



            Vector3 playerPosition = oldPosition = focusPosition;

            focusPosition = new Vector3(Mathf.Clamp(playerPosition.x, boundaryMgr.currentBoundary.boundingBox.min.x + cameraBox.extents.x, boundaryMgr.currentBoundary.boundingBox.max.x - cameraBox.extents.x),
                                      Mathf.Clamp(playerPosition.y, boundaryMgr.currentBoundary.boundingBox.min.y + cameraBox.extents.y, boundaryMgr.currentBoundary.boundingBox.max.y - cameraBox.extents.y),
                                      transform.position.z);

            if (lockX)
                focusPosition.x = boundaryMgr.currentBoundary.boundingBox.center.x;
            if (lockY)
                focusPosition.y = boundaryMgr.currentBoundary.boundingBox.center.y;




            cameraBox.center = transform.position;

            if (!screenSwiping)
            {
                // if (lockX || lockY) 
                transform.position = (Vector3)focusPosition + Vector3.forward * -10;
            }
            else
            {
                transform.position = ScreenSwipe(startPos, endPos, timeStartedLerping, screenSwipeDuration);
            }
        }
    }

    /***********************************************/
    /***********************************************/
    /*          HELPER FUNCTIONS                   */
    /***********************************************/
    /***********************************************/
    public void StartScreenSwipe(Bounds newBounds, bool _lockX, bool _lockY)
    {
        Debug.Log("Screen Swipe");
        lockX = _lockX;
        lockY = _lockY;

        timeStartedLerping = Time.time;
        startPos = transform.position;

        float xPos = transform.position.x;
        float yPos = transform.position.y;

        // if (!lockX)
        xPos = Mathf.Clamp(oldPosition.x, newBounds.min.x + cameraBox.extents.x, newBounds.max.x - cameraBox.extents.x);
        //  if (!lockY)
        yPos = Mathf.Clamp(oldPosition.y, newBounds.min.y + cameraBox.extents.y, newBounds.max.y - cameraBox.extents.y);

        xPos = Mathf.Clamp(xPos, newBounds.min.x + cameraBox.extents.x, newBounds.max.x - cameraBox.extents.x);
        yPos = Mathf.Clamp(yPos, newBounds.min.y + cameraBox.extents.y, newBounds.max.y - cameraBox.extents.y);

        endPos = new Vector3(xPos, yPos, transform.position.z);
        ScreenSwipe(startPos, endPos, timeStartedLerping, screenSwipeDuration);

        screenSwiping = true;
    }

    Vector3 ScreenSwipe(Vector2 _fromPos, Vector2 _toPos, float timeStartedLerping, float lerpTime = 1)
    {

        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        if (percentageComplete >= 1)
            screenSwiping = false;

        Vector3 result = Vector3.Lerp(_fromPos, _toPos, percentageComplete);
        result += new Vector3(0, 0, -10);

        return result;
    }

    /***********************************************/
    /***********************************************/
    /*          ESSENTIAL STRUCTURES               */
    /***********************************************/
    /***********************************************/
    [System.Serializable]
    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void UpdateFocusArea(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            bottom += shiftY;
            top += shiftY;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

    /***********************************************/
    /***********************************************/
    /*          GIZMOS                             */
    /***********************************************/
    /***********************************************/
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);

        //Vector2 boundsSize = new Vector2(cameraBox_2.size.)
        Gizmos.color = new Color(.8f, .8f, .4f, .5f);
        Gizmos.DrawCube(cameraBox.center, cameraBoxSize);
    }
}
