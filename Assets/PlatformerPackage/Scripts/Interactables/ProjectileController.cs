using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleRaycastController))]
public class ProjectileController : CircleRaycastController
{
    [Header("Custom Physics Parameters")]
    public bool canReflect = true;
    public bool invincible = false;
    bool canAttack = false;
    public LayerMask reflectLayer;
    bool reflected = false;
    public float speed;
    protected Vector2 direction = Vector2.up;

    [Header("Effects")]
    public GameObject bulletSpark;

    Vector2 velocity;

    public CollisionInfo collisionInfo;

    [Header("Essential Components")]
    SoundPlayer soundPlayer;

    /***********************************************/
    /*          START FUNCTION                     */
    /***********************************************/
    public virtual void Start()
    {
        soundPlayer = GetComponent<SoundPlayer>();
        // base.Start();
        //ObjectPoolManager.instance.GetItem
        Invoke("ActivateAttack", 2f);
    }

    private void OnEnable()
    {
        OnEnableSpecifics();
    }

    protected virtual void OnEnableSpecifics()
    {
        reflected = false;
        velocity = Vector2.zero;
        collisionInfo.isHit = false;
        Invoke("ActivateAttack", 2f);

    }
    /***********************************************/
    /*          UPDATE FUNCTION                    */
    /***********************************************/
    private void Update()
    {
        //direction = transform.up;
        velocity = direction * speed;



        Move(velocity * Time.deltaTime);

        if (collisionInfo.horizontal)
            velocity.x = 0;
        if (collisionInfo.vertical)
            velocity.y = 0;
    }
    /***********************************************/
    /***********************************************/
    /*          PHYSICS EXECUTION FUNCTIONS        */
    /***********************************************/
    /***********************************************/
    public virtual void Move(Vector2 moveAmount)
    {
        UpdateRaycastInfo();
        collisionInfo.Reset();
        collisionInfo.moveAmountOld = moveAmount;

        Collisions(ref moveAmount);
        transform.Translate(moveAmount);
    }

    /***********************************************/
    /***********************************************/
    /*          COLLISION EXECUTION FUNCTIONS      */
    /***********************************************/
    /***********************************************/
    public virtual void Collisions(ref Vector2 velocity)
    {
        RaycastHit2D hit = Physics2D.CircleCast(raycastInfo.center, raycastInfo.radius, velocity.normalized, velocity.magnitude, collisionMask);
        RaycastHit2D hit_2 = Physics2D.CircleCast(raycastInfo.center, raycastInfo.radius, velocity.normalized, velocity.magnitude, reflectLayer);

        if (hit_2 && !reflected && canReflect)
        {
            float directionX = (hit_2.collider.transform.position.x > transform.position.x) ? -1 : 1;
            if (direction.x < 0 && directionX == 1 || direction.x > 0 && directionX == -1)
                direction.x *= -1;
            //direction.y *= -1;
            reflected = true;
            soundPlayer.PlaySound("reflect");
        }


        // Vector2 rayOrig
        //Debug.DrawRay()

        if (hit && !collisionInfo.isHit)
        {

            // if (hit.collider.gameObject.tag == "Player" && canAttack)
            //     LevelManager.instance.PlayerGameOver();
            if (hit && canAttack)
            {
                MeleeAttack melee = hit.collider.gameObject.transform.parent.gameObject.GetComponent<MeleeAttack>();
                if (melee != null)
                    melee.whenHurt.Invoke();
            }

            if (!invincible || hit.collider.gameObject.tag != "Player")
            {
                Quaternion effectRotation = new Quaternion(-90, 0, 0, 0);
                if (direction.x < 0)
                    effectRotation.y = 180;

                GameObject sfx = ObjectPoolManager.instance.GetItem(bulletSpark, hit.point, effectRotation);
                ObjectPoolManager.instance.ClearObject(sfx, 1.5f);
                ObjectPoolManager.instance.ClearObject(gameObject, 0.1f);
                Vector2 angle = hit.normal;
                // if (velocity.y == 0)
                velocity.x = (hit.distance - skinWidth) * -angle.x;
                //  if (velocity.x == 0)
                velocity.y = (hit.distance - skinWidth) * -angle.y;


                collisionInfo.horizontal = (Mathf.Abs(angle.x) > 0 && hit.normal.y == 0);
                collisionInfo.vertical = (Mathf.Abs(angle.y) > 0 && hit.normal.x == 0);
                collisionInfo.isHit = true;
            }
        }
    }



    /***********************************************/
    /***********************************************/
    /*          HELPER FUNCTIONS                   */
    /***********************************************/
    /***********************************************/
    public virtual void SetDirection(Vector2 _direction)
    {
        direction = _direction;
    }

    public void NegateAttack()
    {
        canAttack = false;
        Invoke("ActivateAttack", 1f);
    }

    void ActivateAttack()
    {
        canAttack = true;
    }
}

[RequireComponent(typeof(CircleCollider2D))]
[System.Serializable]
public class CircleRaycastController : MonoBehaviour
{
    public LayerMask collisionMask;
    public CircleCollider2D circleCollider;
    [HideInInspector]
    public RaycastInfo raycastInfo;

    public const float skinWidth = 0.015f;
    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public struct RaycastInfo
    {
        public Vector2 center;
        public Vector2 top;
        public Vector2 bottom;
        public Vector2 left;
        public Vector2 right;
        public float radius;
    }

    [System.Serializable]
    public struct CollisionInfo
    {
        public bool isHit;
        public bool horizontal;
        public bool vertical;
        public Vector2 moveAmountOld;
        public bool hittingAngle;

        public void Reset()
        {
            // isHit = false;
            horizontal = false;
            vertical = false;
        }
    }

    public void UpdateRaycastInfo()
    {
        Bounds bounds = circleCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastInfo.center = new Vector2(bounds.center.x, bounds.center.y);
        raycastInfo.radius = circleCollider.radius;
    }
}
