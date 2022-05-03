using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[RequireComponent(typeof(Rigidbody2D))]
public class BombController : ProjectileController
{
    [Header("Throw Arc")]
    [Range(0, 1)]
    public float length;
    [Range(0, 1)]
    public float height;

    [Header("Bomb Specifications")]
    public LayerMask victimMask;
    public float blastRadius;
    public Vector2 blastOffset;
    public float detonateTime;
    float detonateTimer;
    bool exploded = false;
    bool detonated = false;
    HashSet<Transform> victims = new HashSet<Transform>();
    Rigidbody2D rb;
    Animator anim;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!exploded)
        {
            if (detonateTimer <= 0 && !exploded)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + new Vector3(blastOffset.x, blastOffset.y, 0), blastRadius, victimMask);

                foreach (Collider2D collider in hits)
                {
                    if (!victims.Contains((collider.transform.parent.transform)))
                        victims.Add(collider.transform.parent.transform);
                }

                foreach (Transform transform in victims)
                {
                    MeleeAttack meleeAttack = transform.gameObject.GetComponent<MeleeAttack>();
                    if (meleeAttack != null)
                        meleeAttack.whenHurt.Invoke();
                }
                anim.SetTrigger("Explode");
                Invoke ("Destruct", 2f);
                exploded = true;
            }
            else
            {
                detonateTimer -= Time.deltaTime * EffectsManager.instance.timeScale;
            }
        }

    }

    void Destruct() {
        ObjectPoolManager.instance.ClearObject(this.gameObject);
    }

    private void OnEnable()
    {
        OnEnableSpecifics();
        anim = GetComponent<Animator>();
        anim.SetTrigger("Revive");
    }

    public override void SetDirection(Vector2 _direction)
    {
        base.SetDirection(_direction);

        float directionIndex = direction.x;

        Vector2 velocity = new Vector2(length * directionIndex * speed, height * speed);
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = velocity;
        detonateTimer = detonateTime;
        victims.Clear();
        exploded = false;
        anim.SetTrigger("Trigger");
    }

    /***********************************************/
    /*          GIZMOS AND DEBUG                   */
    /***********************************************/
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // float size = 0.3f;

        Gizmos.DrawWireSphere(transform.position + new Vector3(blastOffset.x, blastOffset.y, 0), blastRadius);
    }
}
