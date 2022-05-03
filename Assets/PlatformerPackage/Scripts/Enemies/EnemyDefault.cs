using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EnemyController2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyInputDefault))]
[RequireComponent(typeof(HealthManager))]

public class EnemyDefault : MonoBehaviour
{
    public enum EnemyDirection { none, left, right, up, down }


    [Header("Basic Movement")]
    [Header("Custom Physics Parameters")]
    float moveSpeed = 6;
    [Header("Acceleration")]
    public float accelerationAirborne = 0.1f;
    public float accelerationGrounded = 0.2f;

    [Header("Input")]
    Vector2 directionalInput; //AI input
    public EnemyDirection currentDirection = EnemyDirection.left;


    [Header("Essential Parameters")]
    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public Vector3 velocity;
    bool isAlive = true;


    [Header("Essential Components")]
    
    public EnemyController2D controller;
    
    public Transform rendererObject;
    
    public Animator anim;
    
    public SpriteRenderer sprite;
    
    public EnemyInputDefault enemyInput;
    
    public HealthManager healthManager;
    
    public TimeController timeMgr;
    
    public MeleeAttack meleeAttack;
    
    public SoundPlayer soundPlayer;

    [HideInInspector]
    public Transform targetPlayer;


    bool canAct;

    /***********************************************/
    /***********************************************/
    /*              START FUNCTION                 */
    /***********************************************/
    /***********************************************/

    void Start()
    {
       // healthManager = GetComponent<HealthManager>();
        targetPlayer = FindObjectOfType<Player>().GetComponent<Transform>();
        // controller = GetComponent<EnemyController2D>();
        // anim = GetComponentInChildren<Animator>();
        // if (anim == null)
        // anim = rendererObject.GetComponentInChildren<Animator>();
        // sprite = GetComponentInChildren<SpriteRenderer>();
        // if (sprite == null)
        // sprite = GetComponentInChildren<SpriteRenderer>();
        // enemyInput = GetComponent<EnemyInputDefault>();
        // timeMgr = GetComponent<TimeController>();
    }


    /***********************************************/
    /***********************************************/
    /*              UPDATE FUNCTION                */
    /***********************************************/
    /***********************************************/
    void Update()
    {
        //Debug.Log("Velocity: " + velocity);
        canAct = (isAlive && !PauseManager.instance.isPaused);

        if (canAct && controller != null)
        {
            directionalInput = GetDirectionalInput(currentDirection);
            float targetVelocityX = directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref controller.velocityXSmoothing, (controller.collisionInfo.below) ? accelerationGrounded : accelerationAirborne);
            velocity.y += controller.gravity * Time.deltaTime * timeMgr.timeScale * controller.gravityScale;

            if ((velocity.x > 0 && !facingRight) || (velocity.x < 0 && facingRight))
            {
                Flip();
            }

            controller.Move(velocity * Time.deltaTime * timeMgr.timeScale, directionalInput, facingRight);

            anim.SetFloat("VelocityX", Mathf.Abs(velocity.x));

            if (controller.collisionInfo.above || controller.collisionInfo.below)
            {

                if (controller.collisionInfo.slidingDownMaxSlope)
                {
                    velocity.y += controller.collisionInfo.slopeNormal.y * -controller.gravity * Time.deltaTime * timeMgr.timeScale * controller.gravityScale;
                }
                else
                {
                    velocity.y = 0;
                }
            }


        }

    }
    /***********************************************/
    /***********************************************/
    /*            INPUT HELPER FUNCTIONS           */
    /***********************************************/
    /***********************************************/
    public void SetMoveSpeed(float speed)
    {
        if (canAct)
            moveSpeed = speed;
    }

    public void SetCurrentDirection(EnemyDirection direction)
    {
        if (canAct)
            currentDirection = direction;
    }

    public Vector2 GetDirectionalInput(EnemyDirection direction)
    {
        if (canAct)
        {
            if (direction == EnemyDirection.right)
            {
                return new Vector2(1, 0);
            }
            else if (direction == EnemyDirection.left)
            {
                return new Vector2(-1, 0);
            }
            else
                return Vector2.zero;
        }
        return Vector2.zero;
    }

    /***********************************************/
    /***********************************************/
    /*            ANIMATION HELPER FUNCTIONS       */
    /***********************************************/
    /***********************************************/
    void Flip()
    {
        facingRight = !facingRight;

        controller.collisionInfo.facingRight = facingRight;

        rendererObject.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);

    }
}



