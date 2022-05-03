using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaycastController))]
public class Controller2D : RaycastController
{
    [Header("Collision Setup Parameters")]
    public float maxSlopeAngle = 80;

    [Header("Realtime Collision Info")]
    public CollisionInfo collisionInfo;

    [Header("Physics Parameters")]
    public float gravity = -50;
    public float fallGravityModifier = 0.5f;
    [HideInInspector]
    public float gravityScale = 1;
    [HideInInspector]
    public float velocityXSmoothing;
    [HideInInspector]
    public float maxJumpVelocity;
    [HideInInspector]
    public float doubleJumpVelocity;
    [HideInInspector]
    public float minJumpVelocity;
    [HideInInspector]
    public float dashVelocity;
    [HideInInspector]
    public Vector2 currentVelocity = Vector2.zero;
    [HideInInspector]
    public PhysicSim physicSim = new PhysicSim();
    //[HideInInspector]
    public Vector2 appliedVelocity = Vector2.zero;
    [HideInInspector]
    public float oldVelocityY;


    [HideInInspector]
    public Vector2 playerInput;

    bool moveExternal;

    /***********************************************/
    /*          START FUNCTION                     */
    /***********************************************/
    public override void Start()
    {
        base.Start();
        physicSim = new PhysicSim();
        collisionInfo.faceDir = 1;
    }

    private void FixedUpdate()
    {
        CalculateRaySpacing();
    }

    private void Update()
    {
        // if (gameObject.tag == "Player")
        //     Debug.Log("Grounded: " + collisionInfo.below);

        if (!moveExternal && appliedVelocity == Vector2.zero)
        {
            currentVelocity = Vector2.zero;
            Move(currentVelocity);
            HorizontalCollisions(collisionInfo.moveAmountOld);
            VerticalCollisions(collisionInfo.moveAmountOld);
        }

        if (physicSim.active)
        {
          //  Debug.Log("Plim");
            //if (!moveExternal)
            Move(appliedVelocity * Time.deltaTime * EffectsManager.instance.timeScale);
            physicSim.RunPhysicsSim(ref appliedVelocity, (collisionInfo.below), (collisionInfo.left || collisionInfo.right));
        }

        // if (!moveExternal)
        // {
        //    
        // }
        //if (appliedVelocity == Vector2.zero)
        transform.Translate(currentVelocity);
        // else
        //     transform.Translate(appliedVelocity);
        moveExternal = false;

    }
    /***********************************************/
    /***********************************************/
    /*          ESSENTIAL STRUCTURES               */
    /***********************************************/
    /***********************************************/
    [System.Serializable]
    public struct CollisionInfo
    {
        public bool above, below, belowOld;
        public bool left, right;
        public bool facingRight;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public bool overCliff;
        public bool overCliffOld;

        public Vector2 moveAmountOld;
        public int faceDir;
        public bool fallingThroughPlatform;

        public void Reset()
        {
            if (!PauseManager.instance.isPaused)
            {
                above = false;
                below = false;
                belowOld = below;
                left = right = false;
                climbingSlope = false;
                descendingSlope = false;
                slopeAngleOld = slopeAngle;
                slopeAngle = 0;
                slopeNormal = Vector2.zero;
                slidingDownMaxSlope = false;
                //overCliff = false;
            }
        }

        public void ResetHorizontal()
        {
            left = right = false;
        }

        public void ResetVertical()
        {
            above = below = false;
        }

    }

    /***********************************************/
    /*          HELPER FUNCTIONS                   */
    /***********************************************/
    void ResetFallingThroughPlatform()
    {
        collisionInfo.fallingThroughPlatform = false;
    }

    /***********************************************/
    /***********************************************/
    /*          PHYSICS EXECUTION METHODS          */
    /***********************************************/
    /***********************************************/
    //public Vector2 (Vector2 moveAmount, bool)

    public Vector2 Move(Vector2 moveAmount, bool standingOnPlatform = false)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);

        return moveAmount;
    }

    public virtual void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false, bool trueMove = true)
    {
        moveExternal = true;
        // if (gameObject.tag == "Player") {
        // Debug.Log("Velocity: " + moveAmount);
        // }

        // if (gameObject.tag == "Player") {
        //     Debug.Log(moveAmount.y);
        // }

        UpdateRaycastOrigins();
        collisionInfo.Reset();
        collisionInfo.moveAmountOld = moveAmount;
        playerInput = input;

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0)
        {
            collisionInfo.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
            VerticalCollisions(ref moveAmount);

        //if (!PauseManager.instance.isPaused)
        //transform.Translate(moveAmount);
        currentVelocity = moveAmount;

        if (standingOnPlatform)
        {
            collisionInfo.below = true;
        }
    }
    /***********************************************/
    /*          EXTERNAL MOVEMENT(NO INPUT)        */
    /***********************************************/
    public void TriggerKnockback(ForceSim forceCurve, bool facingRight, ForceMode forceMode)
    {
        if (!physicSim.active)
        {
            physicSim.simFallGravityModifier = fallGravityModifier;
            if (forceCurve.retainVertical)
            {
                physicSim.simGravity = gravity;
                physicSim.initialVelocity.y = oldVelocityY;
               //Debug.Log("Preserved Y: " + appliedVelocity.y);
            }
            physicSim.PrimePhysicsSim(forceCurve, ref appliedVelocity, facingRight, forceMode);
        }
    }

    /***********************************************/
    /*          COLLISION METHODS                  */
    /***********************************************/
    void HorizontalCollisions(Vector2 moveAmount)
    {
        Vector2 temp = moveAmount;
        HorizontalCollisions(ref temp);
    }
    void VerticalCollisions(Vector2 moveAmount)
    {
        Vector2 temp = moveAmount;
        VerticalCollisions(ref temp);
    }
    protected virtual RaycastHit2D HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisionInfo.faceDir;
        float raylength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            raylength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, raylength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                //CUSTOM: WATCH!!
                if (hit.collider.tag == "SoftLayer" && hit.distance >= 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisionInfo.descendingSlope)
                    {
                        collisionInfo.descendingSlope = false;
                        moveAmount = collisionInfo.moveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collisionInfo.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    raylength = hit.distance;

                    if (collisionInfo.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
                }

                return hit;
            }

        }

        return new RaycastHit2D();
    }

    protected virtual RaycastHit2D VerticalCollisions(ref Vector2 moveAmount)
    {
        // if (moveAmount.y <= 0)
        // {
        //     moveAmount.y = -skinWidth;
        // }

        float directionY = Mathf.Sign(moveAmount.y);
        float raylength = Mathf.Abs(moveAmount.y) + skinWidth;

        RaycastHit2D hitTrue = new RaycastHit2D();

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, raylength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);


            if (hit)
            {
                // Debug.Log("Hitting Wall: " + (hit.collider.tag != "SoftLayer"));
                // Debug.Log("Hitting Soft: " + (hit.collider.tag == "SoftLayer"));
                if (hit.collider.tag == "SoftLayer")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        // collisionInfo.fallingThroughPlatform = true;
                        // Invoke("ResetFallingThroughPlatform", .1f);
                        continue;
                    }
                    if (collisionInfo.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1)
                    {
                        collisionInfo.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .1f);
                        continue;
                    }
                }



                moveAmount.y = (hit.distance - skinWidth) * directionY;
                raylength = hit.distance;

                if (collisionInfo.climbingSlope)
                {
                    //  Debug.Log("Poo");
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisionInfo.below = collisionInfo.belowOld = directionY == -1;
                collisionInfo.above = directionY == 1;

                hitTrue = hit;
            }

        }

        if (collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            raylength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, raylength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisionInfo.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisionInfo.slopeAngle = slopeAngle;
                    collisionInfo.slopeNormal = hit.normal;

                }

                //return hit;
            }
        }

        return hitTrue;
    }

    /***********************************************/
    /*          SLOPE ADJUSTING METHODS            */
    /***********************************************/
    protected void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisionInfo.below = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
            collisionInfo.slopeNormal = slopeNormal;
        }
    }

    protected void DescendSlope(ref Vector2 moveAmount)
    {
        //Handle over maxSlope
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);

        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }
        if (!collisionInfo.slidingDownMaxSlope)
        {
            //Handle under maxSlope
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            collisionInfo.slopeAngle = slopeAngle;
                            collisionInfo.descendingSlope = true;
                            collisionInfo.below = true;
                            collisionInfo.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = hit.normal.x * ((Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad));

                collisionInfo.slopeAngle = slopeAngle;
                collisionInfo.slidingDownMaxSlope = true;
                collisionInfo.slopeNormal = hit.normal;
            }
        }
    }


}
