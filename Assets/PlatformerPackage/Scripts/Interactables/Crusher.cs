using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    Animator anim;
    Controller2D controller;
    PlatformController platformController;

    [Header("Specifications")]
    [Range(0, 5)]
    public float delayTime;
    float delayTimer;
    [Range(0, 40)]
    public float speed;
    public float acceleration;

    [Header("Movement")]
    Vector2 velocity;
    Direction direction;
    float threshold = 0.5f;
    bool hasHitWall;

    [Header("Waypoint System")]
    Vector2 currentWaypoint;
    int currentWaypointIndex;
    // public bool toWall;
    bool hasWent;
    // public float waitTime;
    // [Range(0, 2)]
    // public float easeAmount;
    public Vector3[] localWaypoints;

    Vector3[] globalWaypoints;
    // Start is called before the first frame update
    void Start()
    {
        // delayTimer = delayTime;
        // currentWaypointIndex = -1;

        // globalWaypoints = new Vector3[localWaypoints.Length];

        // for (int i = 0; i < localWaypoints.Length; i++)
        // {
        //     globalWaypoints[i] = localWaypoints[i] + transform.position;
        // }

        anim = GetComponent<Animator>();
        platformController = GetComponent<PlatformController>();
        controller = GetComponent<Controller2D>();

    }

    // Update is called once per frame
    void Update()
    {
        // if (delayTimer <= 0 && !hasWent)
        // {
        //     hasWent = true;
        //     ProcessWaypoint();
        // }
        // else
        // {
        //     delayTimer -= Time.deltaTime * EffectsManager.instance.timeScale;
        // }

        // if (hasWent)
        // {
        //     velocity = Movement.AccelerateToDirection(velocity, speed, acceleration, direction);


        //     if (Mathf.Abs(Vector2.Distance(transform.position, currentWaypoint)) < threshold) {
        //         velocity = Vector2.zero;
        //         hasWent = false;
        //         delayTimer = delayTime;
        //     }
        // }

        //     controller.Move(velocity);
        SetAnimation(platformController.velocityRec);

        if (controller.collisionInfo.below || controller.collisionInfo.left || controller.collisionInfo.above || controller.collisionInfo.right) {
            if (!hasHitWall) {
                anim.SetTrigger("Impact");
                hasHitWall = true;
            }
        }
        else 
        hasHitWall = false;
    }

    // void ProcessWaypoint()
    // {
    //     if (currentWaypointIndex + 1 > globalWaypoints.Length - 1)
    //     {
    //         currentWaypointIndex = 0;
    //     }
    //     else
    //     {
    //         currentWaypointIndex++;
    //     }

    //     currentWaypoint = globalWaypoints[currentWaypointIndex];

    //     if (currentWaypoint.x > transform.position.x)
    //     {
    //         direction = Direction.right;
    //     }
    //     if (currentWaypoint.x < transform.position.x) {
    //         direction = Direction.left;
    //     }
    //     if (currentWaypoint.y > transform.position.y) {
    //         direction = Direction.up;
    //     }
    //     if (currentWaypoint.y < transform.position.y) {
    //         direction = Direction.down;
    //     }

    // }

    public void SetAnimation(Vector2 velocity)
    {
        int direction = 0;

        if (velocity.x > 0)
            direction = 1;

        if (velocity.y < 0)
            direction = 2;

        if (velocity.x < 0)
            direction = 3;

        if (velocity.y > 0)
            direction = 4;

        anim.SetInteger("Direction", direction);
    }

    /***********************************************/
    /*          GIZMOS AND DEBUG                   */
    /***********************************************/
    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);

            }
        }
    }
}
