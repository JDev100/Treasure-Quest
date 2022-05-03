using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeleeAttack : MonoBehaviour
{
    [Header("Event Specifications")]
    public UnityEvent whenHurt;    //By other combatant
    public UnityEvent onReflect;
    public UnityEvent onParry;
    //public UnityEvent whenRecover;

    [Header("VFX")]
    public GameObject hitEffect;
    public GameObject hitConfirmEffect;
    public GameObject parryEffect;
    bool flipX;

    [Header("Attack")]
    public AttackData defaultAttack;
    AttackData currentAttack;
    [HideInInspector]
    public bool canAttack;

    [Header("Parry")]
    [HideInInspector]
    public float inputTime;
    [HideInInspector]
    public float startupTime;
    [HideInInspector]
    public float attackTime;
    [HideInInspector]
    public bool hurt;
    [HideInInspector]
    public bool invincible;

    [Header("HitStun")]
    //[HideInInspector]
    public int hitStunFrames;

    [Header("Enemy Layer")]
    public LayerMask enemyMask;
    public LayerMask parryMask;
    // LayerMask groundMask = 8;
    public string enemyTag = "Player";
    public string ignoreTag;

    [Header("Hit Box")]
    List<HitBoxData> currentHitBoxes;
    List<GameObject> hitBoxBuffer = new List<GameObject>();
    float xOffset = 1;

    float startupTimer;
    [HideInInspector]
    public float attackTimer;
    float recoveryTimer;
    float fullAttackTime;
    bool hitConfirmed = false;
    bool hitWall;
    bool frameOneDone = false;




    [Header("Essential Components")]
    SoundPlayer soundPlayer;
    TimeController timeMgr;
    Controller2D controller;
    HashSet<Transform> currentTargets = new HashSet<Transform>();
    // Start is called before the first frame update

    /***********************************************/
    /*          START FUNCTION                     */
    /***********************************************/
    void Start()
    {
        controller = GetComponent<Controller2D>();
        soundPlayer = GetComponent<SoundPlayer>();
        timeMgr = GetComponent<TimeController>();
    }

    // Update is called once per frame

    /***********************************************/
    /*          UPDATE FUNCTION                    */
    /***********************************************/
    void Update()
    {
        if (fullAttackTime > 0)
        {
            fullAttackTime -= Time.deltaTime * timeMgr.timeScale;
            if (startupTimer < 0)
            {
                if (attackTimer < 0)
                {
                    ClearHitBoxBuffer();
                    attackTime = 0;
                    inputTime = 100;
                    invincible = false;

                    if (recoveryTimer < 0)
                    {

                    }
                    else
                    {

                        recoveryTimer -= Time.deltaTime * timeMgr.timeScale;
                    }
                }
                else
                {
                    attackTimer -= Time.deltaTime * timeMgr.timeScale;
                    if (canAttack)
                    {
                        if (!frameOneDone && !hurt)
                        {
                            attackTime = Time.time;
                            startupTime = Time.time;
                            foreach (HitBoxData hitBox in currentHitBoxes)
                            {

                                Vector2 hitBoxCenter = new Vector2(transform.position.x + hitBox.hitBoxCenter.x * xOffset, transform.position.y + hitBox.hitBoxCenter.y);
                                float hitBoxRadius = hitBox.hitBoxRadius;
                                GameObject hitBoxSample = ObjectPoolManager.instance.GetItem(CombatManager.instance.parryBox);
                                hitBoxSample.GetComponent<ParryBox>().SetHitBox(hitBoxCenter, hitBoxRadius, transform);
                                hitBoxBuffer.Add(hitBoxSample);
                            }
                            frameOneDone = true;
                        }
                        foreach (HitBoxData hitBox in currentHitBoxes)
                            ProcessHitBox(hitBox);
                    }
                }
            }
            else
            {
                startupTimer -= Time.deltaTime * timeMgr.timeScale;
            }
        }
        else
        {
            invincible = false;
            hitConfirmed = false;
            hurt = false;
        }
    }

    /***********************************************/
    /*          ATTACK FUNCTIONS                   */
    /***********************************************/

    public void Reset()
    {
        startupTimer = 0;
        attackTimer = 0;
        recoveryTimer = 0;
        fullAttackTime = 0;
        currentTargets.Clear();
        ClearHitBoxBuffer();
        frameOneDone = false;
        hitConfirmed = false;
        attackTime = 0;
        inputTime = 100;

    }

    public float Attack(bool facingRight)
    {
        return Attack(defaultAttack, facingRight);
    }

    public float Attack(AttackData attack, bool facingRight)
    {
        Reset();
        canAttack = true;
        startupTime = 0;
        flipX = !facingRight;
        inputTime = Time.time;
        currentHitBoxes = attack.hitBoxes;
        currentAttack = attack;
        hitConfirmed = false;
        hitWall = false;
        xOffset = (!facingRight) ? -1 : 1;

        startupTimer = (float)attack.startupFrames / 60f;
        attackTimer = (float)attack.attackFrames / 60f;
        recoveryTimer = (float)attack.recoveryFrames / 60f;
        fullAttackTime = startupTimer + attackTimer + recoveryTimer;
        hitStunFrames = attack.hitStunFrames;

        return fullAttackTime;
    }

    void ProcessHitBox(HitBoxData hitBox)
    {
        Vector2 hitBoxCenter = new Vector2(transform.position.x + hitBox.hitBoxCenter.x * xOffset, transform.position.y + hitBox.hitBoxCenter.y);
        float hitBoxRadius = hitBox.hitBoxRadius;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(hitBoxCenter, hitBoxRadius, Vector2.zero, 0, enemyMask);
        RaycastHit2D[] hits_2 = Physics2D.CircleCastAll(hitBoxCenter, hitBoxRadius, Vector2.zero, 0, parryMask);

        //
        for (int i = 0; i < hits_2.Length; i++)
        {
            Transform parryTransform = hits_2[i].collider.gameObject.GetComponent<ParryBox>().owner;

            if (parryTransform != transform && !currentTargets.Contains(parryTransform))
            {
                Reflect(parryTransform, hits_2[i]);
                onReflect.Invoke();
                hitConfirmed = true;
            }
        }


        for (int i = 0; i < hits.Length; i++)
        {
            //Debug.Log("In Attack Circle: " + hits[i].transform);

            if (hits[i].collider.gameObject.tag == ignoreTag)
                continue;

            if (hits[i].collider.tag == enemyTag && !currentTargets.Contains(hits[i].transform))
            {
                MeleeAttack enemyMelee = hits[i].collider.gameObject.GetComponentInParent<MeleeAttack>();
                Controller2D enemyController = hits[i].collider.gameObject.GetComponentInParent<Controller2D>();
                //Checkfor parry
                bool parried = (enemyMelee.inputTime < attackTime && (attackTime - enemyMelee.inputTime) <= CombatManager.instance.parryWindow);
                if (parried && gameObject.tag != "Player")
                {
                    Debug.Log("Parry");
                    invincible = true;
                    Reflect(hits[i].transform, hits[i]);
                    enemyMelee.attackTimer = 0f;
                    enemyMelee.onParry.Invoke();
                    enemyMelee.canAttack = false;
                    controller.TriggerKnockback(currentAttack.parryRecoil, !controller.collisionInfo.facingRight, ForceMode.manual);
                    if (parryEffect != null)
                    {
                        ObjectPoolManager.instance.GetItem(parryEffect, hits[i].point);
                    }
                    // invincible = true;
                }
                else
                {

                    enemyMelee.hitStunFrames = hitStunFrames;
                    enemyMelee.ClearHitBoxBuffer();
                    enemyMelee.hurt = true;
                    if (enemyMelee.whenHurt != null)
                        enemyMelee.whenHurt.Invoke();
                    Debug.Log("Shmoop");
                    // if (hits[i].collider.tag != "Player")
                    if (enemyController != null)
                        enemyController.TriggerKnockback(currentAttack.knockBack, controller.collisionInfo.facingRight, ForceMode.impulse);

                    if (hitConfirmEffect != null)
                    {
                        ObjectPoolManager.instance.GetItem(hitConfirmEffect, hits[i].point);
                    }
                    hitConfirmed = true;
                    currentTargets.Add(hits[i].transform);
                }
            }
            else
            {
                if (!hitConfirmed && !hitWall)
                {
                    controller.TriggerKnockback(currentAttack.recoil, controller.collisionInfo.facingRight, ForceMode.manual);
                    //hits[i].point;
                    if (hitEffect != null)
                    {
                        GameObject go = ObjectPoolManager.instance.GetItem(hitEffect, hits[i].point);
                        go.GetComponent<SpriteRenderer>().flipX = flipX;
                    }
                    soundPlayer.PlaySound("wallClash");
                    hitWall = true;
                }
            }

        }
    }

    /***********************************************/
    /*          PARRY FUNCTIONS                    */
    /***********************************************/
    void Reflect(Transform parryTransform, RaycastHit2D hit)
    {
        // Debug.Log("Parry!");
        currentTargets.Add(parryTransform);
        Controller2D enemyController = parryTransform.gameObject.GetComponentInParent<Controller2D>();
        enemyController.TriggerKnockback(currentAttack.parryRecoil, controller.collisionInfo.facingRight, ForceMode.manual);
        if (hitConfirmEffect != null)
        {
            ObjectPoolManager.instance.GetItem(hitConfirmEffect, hit.point);
        }
    }

    public float StartParryRecovery()
    {
        attackTimer = 0;
        Debug.Log("Attack Time: " + attackTimer);
        recoveryTimer = (float)currentAttack.parryRecoveryFrames / 60f;
        return (float)currentAttack.parryRecoveryFrames / 60f;
    }

    /***********************************************/
    /*          ATTACK HELPER FUNCTIONS            */
    /***********************************************/
    public void ClearHitBoxBuffer()
    {
        if (hitBoxBuffer.Count > 0)
        {
            foreach (GameObject go in hitBoxBuffer)
            {
                ObjectPoolManager.instance.ClearObject(go);
            }
        }
        hitBoxBuffer.Clear();
    }

    public bool IsHitConfirm()
    {
        return hitConfirmed;
    }

    /***********************************************/
    /***********************************************/
    /*          GIZMOS FUNCTION                    */
    /***********************************************/
    /***********************************************/

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (HitBoxData hitBox in defaultAttack.hitBoxes)
            Gizmos.DrawWireSphere(new Vector3(transform.position.x + hitBox.hitBoxCenter.x * xOffset, transform.position.y + hitBox.hitBoxCenter.y, transform.position.z), hitBox.hitBoxRadius);

        //Knockback
    }
}

/***********************************************/
/*          ESSENTIAL STRUCTS                  */
/***********************************************/
[System.Serializable]
public struct AttackData
{
    [Header("Frame Data")]
    public int startupFrames;
    public int attackFrames;
    public int recoveryFrames;
    public int hitStunFrames;
    public int parryRecoveryFrames;

    [Header("Knockback")]
    public ForceSim knockBack;
    public ForceSim recoil;
    public ForceSim parryRecoil;

    [Header("HitBoxes")]
    public List<HitBoxData> hitBoxes;

}

[System.Serializable]
public struct HitBoxData
{
    public Vector2 hitBoxCenter;
    public float hitBoxRadius;
}