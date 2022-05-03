using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SoundPlayer))]
[RequireComponent(typeof(TimeController))]
public class Player : MonoBehaviour
{
    [Header("Basic Movement")]
    [Header("Custom Physics Parameters")]
    public float moveSpeed = 6;
    [Header("Jumping")]
    public float maxJumpHeight = 4;
    [Range(0, 1)]
    public float doubleJumpHeightRatio;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    [Range(0.01f, 1)]
    public float fallGravityModifier = .5f;
    public float terminalVelocityY = 20;
    [Header("Acceleration")]
    [Range(0, 1)]
    public float accelerationAirborne = 0.1f;
    [Range(0, 1)]
    public float accelerationGrounded = 0.2f;
    [Range(0, 500)]
    public float accelerationJump = 0.1f;
    [Range(0, 500)]
    public float accelerationWallKick = 100f;

    [Header("JumpingParam")]
    float currentJumpVelocity;
    float currentWallKickVelocity;
    const float jumpDelay = 0.2f;
    float jumpTimer;
    bool hasPressedJump;
    bool hasDoubleJumped;
    bool hasWallJumped;

    [Header("Wall Jumping")]
    public float wallStickTime = .25f;
    public float wallSlideSpeedMax = 3;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    float timeToWallUnstick;
    [HideInInspector]
    public bool wallSliding;
    [HideInInspector]
    public int wallDirX;

    [Header("Dashing")]
    [Range(0, 10)]
    public float dashLength;
    [Range(0, 1)]
    public float dashApexTime;
    [Range(0, 2)]
    public float dashTime;
    [Range(0, 500)]
    public float dashAcceleration;
    public AttackData dashAttack;
    float currentDashTime;
    float dashSpeed;
    float dashInputTime;
    bool hasDashed;
    bool cloudJumping;
    bool canDash;

    [Header("Attacking")]
    float attackTimer;

    [Header("Effects")]
    public Transform effectPos;
    public ParticleSystem dustEffect;
    public GameObject fadeEffect;
    public float fadeInterval;
    private float fadeTime;
    public GameObject jumpEffect;
    //private SpriteRenderer spriteRenderer;

    [Header("Input")]
    Vector2 directionalInput;


    [HideInInspector]
    public bool facingRight = true;
    Vector3 velocity;
    Vector3 velocityOld;

    // Start is called before the first frame update
    [HideInInspector]
    public PlayerController2D controller;
    [Header("Essential Components")]
    public Transform pivotObject;
    Animator anim;
    TimeController timeMgr;
    SpriteRenderer sprite;
    MeleeAttack meleeAttack;

    [Header("Input Management")]
    private InputManager inputManager;
    bool canAct;

    [Header("Audio")]
    SoundPlayer soundPlayer;

    //For handling audio
    bool hasJumped;

    [Header("Editor Specifics")]
    [HideInInspector]
    public int toolBarTab;
    [HideInInspector]
    public string currentTab;

    /***********************************************/
    /***********************************************/
    /*              START FUNCTION                 */
    /***********************************************/
    /***********************************************/
    void Start()
    {
        //Set Components
        meleeAttack = GetComponent<MeleeAttack>();
        soundPlayer = GetComponent<SoundPlayer>();
        controller = GetComponent<PlayerController2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        inputManager = FindObjectOfType<InputManager>();
        timeMgr = GetComponent<TimeController>();

        //Set physics stuff
        controller.gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        controller.fallGravityModifier = fallGravityModifier;
        controller.maxJumpVelocity = Mathf.Abs(controller.gravity) * timeToJumpApex;
        controller.minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(controller.gravity) * minJumpHeight);
        float doubleJumpHeight = maxJumpHeight * doubleJumpHeightRatio;
        float doubleJumpApexTime = timeToJumpApex * doubleJumpHeightRatio;
        controller.doubleJumpVelocity = Mathf.Abs(controller.gravity) * doubleJumpApexTime;
        controller.dashVelocity = dashLength / dashApexTime;
    }

    /***********************************************/
    /***********************************************/
    /*              UPDATE FUNCTION                */
    /***********************************************/
    /***********************************************/

    void Update()
    {
        canAct = (inputManager.CanInput() && !PauseManager.instance.isPaused);
        // anim.speed = timeMgr.timeScale;

        if (canAct)
        {
            CalculateVelocity();
            HandleWallSliding();
            controller.Move(velocity * Time.deltaTime * timeMgr.timeScale, directionalInput);

            //Animation and effects
            if (Time.timeScale != 0)
            {
                anim.SetFloat("VelocityY", velocity.y);
                anim.SetFloat("VelocityX", Mathf.Abs(velocity.x));
                anim.SetBool("WallSlide", wallSliding);
                anim.SetBool("Dashing", hasDashed);
                anim.SetBool("CloudJumping", cloudJumping);

                if (attackTimer > 0)
                    attackTimer -= Time.deltaTime * timeMgr.timeScale;

                if (Mathf.Abs(velocity.x) < 0.5 && directionalInput.y == -1)
                {
                    anim.SetBool("Crouching", true);
                }
                else
                    anim.SetBool("Crouching", false);

                if (hasDashed || cloudJumping)
                {
                    if (fadeTime <= 0)
                    {
                        CreateFadeEffect(fadeEffect, transform, sprite.sprite);
                        fadeTime = fadeInterval;
                    }
                    else
                        fadeTime -= Time.deltaTime;
                }
            }

            // if (!controller.collisionInfo.left || !controller.collisionInfo.right) {
            //     anim.SetBool("WallSlide", false);
            // }
            if (controller.collisionInfo.above || controller.collisionInfo.below)
            {

                if (controller.collisionInfo.below && hasJumped)
                {
                    soundPlayer.PlaySound("land");

                    hasJumped = false;
                    hasDoubleJumped = false;
                    canDash = true;
                    cloudJumping = false;
                }

                if (controller.collisionInfo.slidingDownMaxSlope)
                {
                    float gravity = (velocity.y >= 0) ? -Mathf.Abs(controller.gravity) : -Mathf.Abs(controller.gravity) * fallGravityModifier;


                    velocity.y += controller.collisionInfo.slopeNormal.y * gravity * Time.deltaTime * timeMgr.timeScale * controller.gravityScale;
                }
                else
                {
                    velocity.y = 0;
                    currentJumpVelocity = 0;
                    hasPressedJump = false;
                    hasWallJumped = false;

                    if (jumpTimer > Time.time && controller.collisionInfo.below)
                    {
                        Debug.Log("Ko");
                        Jump(controller.maxJumpVelocity);
                    }
                }
            }
            else
            {
                if (Time.timeScale != 0)
                    anim.SetFloat("AirVelocityY", velocity.y);
                hasJumped = true;
            }

            if (Time.timeScale != 0)
                anim.SetBool("Grounded", controller.collisionInfo.below);

            velocityOld = velocity;
        }
        else
        {
            //Debug.Log("Ploom");
            // controller.Move(velocityOld);
        }
        // Debug.Log("Grounded: " + controller.collisionInfo.below);
    }

    /***********************************************/
    /***********************************************/
    /*              INPUT FUNCTIONS                */
    /***********************************************/
    /***********************************************/
    public void SetDirectionalInput(Vector2 input)
    {
        if (canAct)
            directionalInput = input;
        anim.SetFloat("InputX", Mathf.Abs(directionalInput.x));

    }

    public void OnJumpInputDown()
    {
        if (canAct && attackTimer <= 0 && !hasDashed)
        {
            jumpTimer = Time.time + jumpDelay;
            if (wallSliding)
            {
                WallKick();
            }
            if (controller.collisionInfo.below)
            {
                Jump(controller.maxJumpVelocity);
            }
            else
            {
                if (!hasDoubleJumped)
                {
                    Jump(controller.doubleJumpVelocity);
                    hasDoubleJumped = true;
                }
            }
        }
    }

    public void OnJumpInputUp()
    {
        Debug.Log("Lp");
        if (velocity.y > controller.minJumpVelocity && canAct && !cloudJumping)
        {
            if (!hasWallJumped)
            {
                velocity.y = controller.minJumpVelocity;
                currentJumpVelocity = controller.minJumpVelocity;
            }
        }
    }

    public void OnAttackInputDown()
    {
        if (canAct)
        {
            if (!wallSliding && attackTimer <= 0)
            {

                anim.SetTrigger("Attack");
                //attackTimer = 16f / 60f;
                attackTimer = meleeAttack.Attack(facingRight);

                soundPlayer.PlaySound("swordSwing");
            }
        }
    }

    public void OnDashInputDown(int direction)
    {
        if (canAct && !hasDashed && canDash && attackTimer <= 0)
        {
            cloudJumping = false;
            if (direction == 0)
                direction = (facingRight) ? 1 : -1;

            Dash(direction, controller.dashVelocity, dashTime);
            anim.SetTrigger("Dash");
            hasWallJumped = false;
            hasPressedJump = false;
            canDash = false;
            attackTimer = meleeAttack.Attack(dashAttack, facingRight);
        }
    }


    void DashCancel()
    {
        hasDashed = false;
        canDash = true;
        attackTimer = 0;
    }

    // bool DashHit()

    /***********************************************/
    /***********************************************/
    /*              PHYSICS EXCUTION FUNCTIONS     */
    /***********************************************/
    /***********************************************/

    void CloudJump()
    {
        Debug.Log("Dash Hit!");
        DashCancel();
        cloudJumping = true;
        int direction = (facingRight) ? 1 : -1;
        Dash(direction, controller.dashVelocity, dashTime / 1.5f);
        Jump(controller.maxJumpVelocity * .8f);
        //meleeAttack.Reset();
    }

    void Dash(int direction, float speed, float _dashTime)
    {
        hasDashed = true;
        velocity.x = 0;
        dashSpeed = speed * direction;
        dashInputTime = Time.time;
        currentDashTime = _dashTime;
    }

    void Jump(float jumpForce)
    {
        anim.SetTrigger("Jump");
        soundPlayer.PlaySound("jump");
        hasJumped = true;

        if (controller.collisionInfo.slidingDownMaxSlope)
        {

            //not jumping aginst max slope
            if (directionalInput.x != -Mathf.Sign(controller.collisionInfo.slopeNormal.x))
            {
                velocity.y = jumpForce * controller.collisionInfo.slopeNormal.y;
                velocity.x = jumpForce * controller.collisionInfo.slopeNormal.x;
            }
        }
        else
        {
            // velocity.y = jumpForce;
            if (!controller.collisionInfo.below)
                velocity.y = 0;
            currentJumpVelocity = jumpForce;

            if (!Input.GetButton("Jump") && !cloudJumping)
            {
                Debug.Log("Mp");
                currentJumpVelocity = controller.minJumpVelocity;
            }
            hasPressedJump = true;

        }
    }

    void WallKick()
    {
        hasWallJumped = true;
        anim.SetTrigger("Jump");
        soundPlayer.PlaySound("jump");
        hasPressedJump = true;

        if (wallDirX == directionalInput.x)
        {
            //velocity.x = -wallDirX * wallJumpClimb.x;
            //velocity.y = wallJumpClimb.y;
            currentWallKickVelocity = -wallDirX * wallJumpClimb.x;
            currentJumpVelocity = wallJumpClimb.y;
        }
        else if (directionalInput.x == 0)
        {
            //velocity.x = -wallDirX * wallJumpOff.x;
            //velocity.y = wallJumpOff.y;
            currentWallKickVelocity = -wallDirX * wallJumpOff.x;
            currentJumpVelocity = wallJumpOff.y;
        }
        else
        {
            // velocity.x = -wallDirX * wallLeap.x;
            // velocity.y = wallLeap.y;
            currentWallKickVelocity = -wallDirX * wallLeap.x;
            currentJumpVelocity = wallLeap.y;
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;

        if (meleeAttack.IsHitConfirm())
        {
            CloudJump();
            meleeAttack.Reset();
        }

        if (!hasWallJumped && !hasDashed)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref controller.velocityXSmoothing, (controller.collisionInfo.below) ? accelerationGrounded : accelerationAirborne);
        }
        else
        {
            if (!hasDashed)
            {
                //  Debug.Log("Flim");
                bool wallKicking = true;

                if (currentWallKickVelocity > 0)
                {
                    wallKicking = Movement.AccelerateToTarget(ref velocity.x, currentWallKickVelocity, accelerationWallKick);
                }
                else if (currentWallKickVelocity < 0)
                {
                    wallKicking = Movement.DecelerateToTarget(ref velocity.x, currentWallKickVelocity, accelerationWallKick);
                }

                hasWallJumped = !wallKicking;
            }
            else
            {

                if (!controller.collisionInfo.below)
                    velocity.y = 0;
                if (dashSpeed > 0)
                {
                    Movement.AccelerateToTarget(ref velocity.x, dashSpeed, dashAcceleration);
                }
                else if (dashSpeed < 0)
                {
                    Movement.DecelerateToTarget(ref velocity.x, dashSpeed, dashAcceleration);
                }
                if (Time.time - dashInputTime >= currentDashTime)
                    hasDashed = false;
            }
        }
        bool useGravity = true;

        if (hasPressedJump && !hasDashed)
        {
            if (!Input.GetButton("Jump") && !cloudJumping)
                currentJumpVelocity = controller.minJumpVelocity;
            useGravity = Movement.AccelerateToTarget(ref velocity.y, currentJumpVelocity, accelerationJump);
        }

        float gravity = (velocity.y >= 0) ? -Mathf.Abs(controller.gravity) : -Mathf.Abs(controller.gravity) * fallGravityModifier;

        if (useGravity && (!hasDashed))
        {
            velocity.y += gravity * Time.deltaTime * timeMgr.timeScale * controller.gravityScale;
            hasPressedJump = false;
        }

        if (velocity.y < -Mathf.Abs(terminalVelocityY))
            velocity.y = -Mathf.Abs(terminalVelocityY);

        controller.oldVelocityY = velocity.y;

        if (controller.collisionInfo.below)
        {
            if (((directionalInput.x > 0 && velocity.x > 0 && !facingRight) || (directionalInput.x < 0 && velocity.x < 0 && facingRight)) && attackTimer <= 0)
            {
                Flip();
            }
        }
        else
        {
            if (((velocity.x > 0 && !facingRight) || (velocity.x < 0 && facingRight)) && attackTimer <= 0)
            {
                Flip();
            }
        }
    }

    void HandleWallSliding()
    {
        wallSliding = false;
        wallDirX = (controller.collisionInfo.left) ? -1 : 1;
        if ((controller.collisionInfo.left || controller.collisionInfo.right) && !controller.collisionInfo.below && velocity.y < 0)
        {
            meleeAttack.Reset();

            wallSliding = true;
            hasDoubleJumped = false;
            cloudJumping = false;

            // if (jumpTimer > Time.time)
            //     WallKick();

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                controller.velocityXSmoothing = 0;
                velocity.x = 0;
                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime * timeMgr.timeScale;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

    }

    /***********************************************/
    /***********************************************/
    /*              EFFECTS HELPER FUNCTIONS       */
    /***********************************************/
    /***********************************************/
    public void TriggerParry()
    {
        anim.SetTrigger("Parry");
        // TriggerReflect();
        soundPlayer.PlaySound("parry");
        EffectsManager.instance.TriggerHitStop(CombatManager.instance.parryHitStop);
        attackTimer = meleeAttack.StartParryRecovery();
    }
    public void TriggerReflect()
    {
        soundPlayer.PlaySound("reflect");
        EffectsManager.instance.TriggerHitStop(CombatManager.instance.reflectHitStop);
    }

    void CreateFadeEffect(GameObject effect, Transform pos, Sprite sprite)
    {
        //GameObject ps = GetItem(effect, pos.position, pos.transform.rotation);
        GameObject ps = ObjectPoolManager.instance.GetItem(effect, pos.position, pos.transform.rotation);
        ps.GetComponent<FadeSprite>().SetSprite(sprite, !facingRight);
        ObjectPoolManager.instance.ClearObject(ps, 1.1f);

        //Destroy(ps, 5f);
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

        pivotObject.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);


        if (facingRight)
            sprite.flipX = false;
        else
            sprite.flipX = true;
    }
}
