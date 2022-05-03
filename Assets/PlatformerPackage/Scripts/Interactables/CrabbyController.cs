using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabbyController : MonoBehaviour
{
    [Header("On Destruct")]
    [Range(0, 100)]
    public float yImpulse;
    [Range(0, 100)]
    public float xImpulse;

    [Header("Movement")]
    [Range(0, 10)]
    public float speed;
    [Range(0, 5)]
    public float waitTime;
    float waitTimer;
    public LayerMask groundLayer;
    public LayerMask ignoreLayer;
    public Transform wallCheck;
    public Transform groundCheck;
    float direction = -1;


    [Header("Essential Components")]
    string defaultTag;
    Animator anim;
    HashSet<Transform> victims = new HashSet<Transform>();
    Rigidbody2D rb;
    CircleCollider2D box;
    bool active;

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        defaultTag = gameObject.tag;
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        //anim.SetBool("Shooting")
        if (active)
        {
            if (waitTimer < 0)
            {

                // Collider2D[] hits = Physics2D.OverlapCircleAll(box.bounds.center, box.radius, victimMask);

                // foreach (Collider2D collider in hits)
                // {
                //     if (!victims.Contains((collider.transform.parent.transform)))
                //         victims.Add(collider.transform.parent.transform);
                // }

                // foreach (Transform transform in victims)
                // {
                //     MeleeAttack meleeAttack = transform.gameObject.GetComponent<MeleeAttack>();
                //     if (meleeAttack != null && transform.gameObject.tag == "Player")
                //         meleeAttack.whenHurt.Invoke();
                // }
                rb.velocity = new Vector2(speed * direction, 0);

                Collider2D hitWall = Physics2D.OverlapCircle(wallCheck.position, 0.05f, groundLayer);

                if (hitWall != null)
                {
                    FlipDirection();
                }

                Collider2D hitFloor = Physics2D.OverlapCircle(groundCheck.position, 0.05f, groundLayer);

                if (hitFloor == null)
                {
                    FlipDirection();
                }
            }
            else
            {
                waitTimer -= Time.deltaTime * EffectsManager.instance.timeScale;
            }
        }
    }

    void FlipDirection()
    {
        direction *= -1;
        waitTimer = waitTime;
        //GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        //transform.Rotate(new Vector3(0, 180, 0));
        StartCoroutine(ToggleFlip());
    }
    IEnumerator ToggleFlip()
    {
        yield return new WaitForSeconds(waitTime);
        transform.Rotate(new Vector3(0, 180, 0));

    }

    public void OnDestruct()
    {
        // gameObject.layer = 2;
        // gameObject.transform.GetChild(0).gameObject.layer = 2;
        box.enabled = false;
        anim = GetComponent<Animator>();
        anim.SetTrigger("Destroy");
        gameObject.tag = "Untagged";
        rb = GetComponent<Rigidbody2D>();
        active = false;

        Vector3 playerPos = FindObjectOfType<Player>().gameObject.transform.position;

        if (playerPos.x > transform.position.x)
        {
            rb.velocity = new Vector2(-xImpulse, yImpulse);
            return;
        }

        rb.velocity = new Vector2(xImpulse, yImpulse);
    }

    public void Revive()
    {
        // gameObject.layer = 14;
        // gameObject.transform.GetChild(0).gameObject.layer = 14;
        box.enabled = true;
        gameObject.tag = defaultTag;
        anim.SetTrigger("Revive");
        // active = true;
    }
}
