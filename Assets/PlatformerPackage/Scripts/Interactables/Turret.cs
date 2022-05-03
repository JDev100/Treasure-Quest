using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("On Destruct")]
    [Range(0, 100)]
    public float yImpulse;
    [Range(0, 100)]
    public float xImpulse;

    [Header("Aim and Position")]
    public Direction direction;
    public Vector2 shootPos;
    Vector2 trueShootPos;

    [Header("Fire Rate")]
    [Range(0.3f, 5)]
    public float fireRate;
    public int delayFrames;
    float fireTimer;
    bool hasShot;

    [Header("Ammo")]
    public GameObject ammo;

    [Header("Essential Components")]
    Animator anim;
    Rigidbody2D rb;
    Vector2 shootDirection;
    bool active = true;



    private void Start()
    {
        anim = GetComponent<Animator>();

        trueShootPos = new Vector2(transform.position.x + shootPos.x, transform.position.y + shootPos.y);
        shootDirection = GetDirection(direction);
        fireTimer = fireRate;
    }

    private void Update()
    {
        if (active)
        {
            if (fireTimer < 0)
            {
                //Shoot();
                if (!hasShot)
                {
                    Shoot();
                    hasShot = true;
                }
            }
            else
            {
                fireTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (rb.velocity.y <= 0)
            {
                anim.SetTrigger("HitFloor");
            }
        }
    }

    public void Shoot()
    {
        Invoke("ShootProjectile", delayFrames / 60f);
        Debug.Log("Shooting");
        anim.SetTrigger("Shoot");

    }

    void ShootProjectile()
    {
        ProjectileController pc = ObjectPoolManager.instance.GetItem(ammo, trueShootPos).GetComponent<ProjectileController>();
        pc.SetDirection(shootDirection);
        pc.gameObject.SetActive(true);
        hasShot = false;
        fireTimer = fireRate;
    }

    public void Revive()
    {
        anim.SetTrigger("Revive");
        active = true;
    }

    public void OnDestruct()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("Destroy");
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


    /***********************************************/
    /*          GIZMOS AND DEBUG                   */
    /***********************************************/
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float size = 0.3f;

        float xOffset = (direction == Direction.right) ? 1 : -1;

        // GetComponent<SpriteRenderer>().s

        Gizmos.DrawLine(transform.position - new Vector3(shootPos.x, shootPos.y, 0) + new Vector3(0, 1, 0) * size, transform.position + new Vector3(shootPos.x, shootPos.y, 0) + new Vector3(0, 1, 0) * size);
        Gizmos.DrawLine(transform.position - new Vector3(shootPos.x, shootPos.y, 0) + new Vector3(-1, 0, 0) * size, transform.position + new Vector3(shootPos.x, shootPos.y, 0) + new Vector3(-1, 0, 0) * size);
        
        //Gizmos.DrawLine(shootPos - Vector2.left * size, shootPos + Vector2.left * size);
    }


    public Vector2 GetDirection(Direction direction)
    {
        Vector2 newDirection;
        switch (direction)
        {
            case Direction.left:
                newDirection = new Vector2(-1, 0);
                break;
            case Direction.right:
                newDirection = new Vector2(1, 0);
                break;
            // case Direction.up:
            // newDirection = new Vector2(0, 1);
            // break;
            // case Direction.down:
            // newDirection = new Vector2(0, -1);
            // break;
            default:
                newDirection = Vector2.zero;
                break;
        }

        return newDirection;
    }
}

public enum Direction { left, right, up, down, upRight, upLeft, downRight, downLeft };


