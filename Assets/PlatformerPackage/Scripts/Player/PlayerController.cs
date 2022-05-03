using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//using Jesus.ManagerSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movements")]
    //public float moveSpeed = 10f;
    public float maxSpeed = 8f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Effects")]
    public Transform effectPos;
    public ParticleSystem dustEffect;
    public GameObject fadeEffect;
    public float fadeInterval;
    private float fadeTime;
    public GameObject jumpEffect;
    private SpriteRenderer spriteRenderer;


    [Header("Components")]
    public Rigidbody2D rb;
    public Animator anim;
    public LayerMask groundLayer;
    public LayerMask softPlatformLyr;

    [Header("Vertical Movement")]
    public float jumpForce = 8f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("Physics")]
    public float groundDrag = 4f;
    public float airDrag = 4f;
    public float groundAcceleration;
    public float airAcceleration;
    public float fallMultiplier = 5f;
    public float gravity = 1f;
    private float currentAcceleration;
    private float currentDrag;
    private float airVelocityY;

    [Header("Collision")]
    private bool grounded = false;
    private bool collidingSoftLyr = false;
    private bool hasCollidedSoftLyr = false;
    private GameObject softPlatforms = null;
    public float groundLength = 1.52f;
    public Vector3 colliderOffset;

    [Header("Input Management")]
    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        inputManager = FindObjectOfType<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.CanInput())
        {
            grounded = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            anim.SetFloat("InputX", Mathf.Abs(direction.x));
            anim.SetBool("Grounded", grounded);
            anim.SetFloat("VelocityY", rb.velocity.y);

            if (!grounded)
            {
                anim.SetFloat("AirVelocityY", rb.velocity.y);
                airVelocityY = rb.velocity.y;
            }


            if (Input.GetButtonDown("Jump"))
            {
                jumpTimer = Time.time + jumpDelay;
            }

            HandleEffects();

            if (!grounded)
            {
                if (fadeTime <= 0)
                {
                    CreateFadeEffect(fadeEffect, transform, spriteRenderer.sprite);
                    fadeTime = fadeInterval;
                }
                else
                    fadeTime -= Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        //Behavior for interacting with soft platforms
        //Check if collided, then toggle
        collidingSoftLyr = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, softPlatformLyr) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, softPlatformLyr);
        if (collidingSoftLyr && airVelocityY < 0.05)
            HandleSoftPlatforms();

        if (hasCollidedSoftLyr && !grounded && airVelocityY != 0 && !collidingSoftLyr){
            if (softPlatforms != null) {
                softPlatforms.GetComponent<TilemapCollider2D>().isTrigger = true;
                softPlatforms.layer = LayerMask.NameToLayer("SoftLayer");
                hasCollidedSoftLyr = false;
            }
        }


        if (inputManager.CanInput())
        {

            MoveCharacter(direction.x);

            if (jumpTimer > Time.time && grounded)
            {
                Jump();
            }
        }
    }

    void MoveCharacter(float horizontal)
    {
        ModifyPhysics();
        if (Mathf.Abs(horizontal) > 0)
            Accelerate(horizontal, currentAcceleration);
        else
            Decelerate(currentDrag);
        // rb.AddForce(Vector2.right * horizontal * moveSpeed);

        anim.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));

        if ((horizontal > 0 && rb.velocity.x > 0 && !facingRight) || (horizontal < 0 && rb.velocity.x < 0 && facingRight))
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    void Jump()
    {
        CreateEffect(jumpEffect, effectPos);
        //rb.velocity = new Vector2(rb.velocity.x, 0);
        //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        anim.SetTrigger("Jump");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpTimer = 0;

    }

    void ModifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (grounded)
        {
            currentAcceleration = groundAcceleration;
            currentDrag = groundDrag;
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                //  rb.drag = linearDrag;
            }
            else
            {
                //rb.drag = 0f;
            }
            rb.gravityScale = 0f;
        }
        else
        {
            currentAcceleration = airAcceleration;
            currentDrag = airDrag;
            rb.gravityScale = gravity;
            rb.drag = 0;

            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    void HandleEffects()
    {
        if (airVelocityY < 0 && grounded)
        {
            CreateEffect(jumpEffect, effectPos);
            airVelocityY = 0f;
        }

        if (!grounded)
        {

        }
    }


    void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);

        if (grounded)
            PlayEffect(dustEffect);
    }

    public void Decelerate(float rate)
    {
        float threshold = 0.5f;
        if (rb.velocity.x > threshold)
            //velocity.x -= speed_increment * rate;
            rb.velocity = new Vector2(rb.velocity.x - rate * Time.deltaTime, rb.velocity.y);

        else if (rb.velocity.x < -threshold)
            rb.velocity = new Vector2(rb.velocity.x + rate * Time.deltaTime, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void Accelerate(float direction, float rate)
    {
        rb.velocity = new Vector2(rb.velocity.x + (rate * Time.deltaTime) * direction, rb.velocity.y);
    }

    void PlayEffect(ParticleSystem effect)
    {
        dustEffect.Play();
    }

    void CreateEffect(GameObject effect, Transform pos)
    {
        // GameObject ps = Instantiate(effect, pos.position, pos.transform.rotation);
        // Destroy(ps, 1.1f);
        GameObject ps = ObjectPoolManager.instance.GetItem(effect, pos.position, pos.transform.rotation);
        ObjectPoolManager.instance.ClearObject(ps, 1.1f);
    }

    void CreateFadeEffect(GameObject effect, Transform pos, Sprite sprite)
    {
        //GameObject ps = GetItem(effect, pos.position, pos.transform.rotation);
        GameObject ps = ObjectPoolManager.instance.GetItem(effect, pos.position, pos.transform.rotation);
        ps.GetComponent<FadeSprite>().SetSprite(sprite);
        ObjectPoolManager.instance.ClearObject(ps, 1.1f);

        //Destroy(ps, 5f);
    }

    void HandleSoftPlatforms()
    {
        RaycastHit2D hitSoftLayer_left = Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, softPlatformLyr);
        RaycastHit2D hitSoftLayer_right = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, softPlatformLyr);

        if (hitSoftLayer_left) {
            softPlatforms = hitSoftLayer_left.collider.gameObject;
            hasCollidedSoftLyr = true;
            hitSoftLayer_left.collider.gameObject.layer = LayerMask.NameToLayer("Ground");
            hitSoftLayer_left.collider.gameObject.GetComponent<TilemapCollider2D>().isTrigger = false;
            return;
        }

        if (hitSoftLayer_right) {
            softPlatforms = hitSoftLayer_right.collider.gameObject;
            hasCollidedSoftLyr = true;
             hitSoftLayer_right.collider.gameObject.layer = LayerMask.NameToLayer("Ground");
            hitSoftLayer_right.collider.gameObject.GetComponent<TilemapCollider2D>().isTrigger = false;
            return;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);

    }
}
