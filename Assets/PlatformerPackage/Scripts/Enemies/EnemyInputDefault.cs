using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInputDefault : MonoBehaviour
{
    public enum EnemyState { idle, patrol, chase, caution, attack, dead }
    public enum PatrolType { normal, auto }

    EnemyDefault enemy;
    string spikeLayer = "Enemy";

    [Header("Basic Movement")]
    public float walkSpeed = 2;
    public float runSpeed = 6;

    [Header("Waypoint System")]
    public PatrolType patrolType = PatrolType.normal;
    EnemyDefault.EnemyDirection autoPatrolDir = EnemyDefault.EnemyDirection.right;
    public float waitTime;
    public Vector3[] localWaypoints;
    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;
    Vector3[] globalWaypoints;

    [Header("Vision System")]
    EnemyVision enemyVision;
    public Transform visionPivot;
    public float timeToLose;
    float loseTimer;
    bool hasSight;
    Vector2 currentTarget;

    [Header("Caution System")]
    public float investigateTime = 3;
    float cautionTimer;
    Vector2 investigateSpot;

    [Header("Attack System")]
    public float attackDistance = 1;
    float attackTimer;
    MeleeAttack meleeAttack;

    [Header("Input Parameters")]
    public bool canPatrol;
    bool hasPatrol;
    bool isDead;
    EnemyState currentState = EnemyState.idle;
    EnemyDefault.EnemyDirection currentDirection = EnemyDefault.EnemyDirection.none;
    EnemyDefault.EnemyDirection bannedDirection = EnemyDefault.EnemyDirection.none;
    float checkTimer;
    LayerMask currentLayer;

    [Header("Audio")]
    SoundPlayer soundPlayer;

    [Header("Collision")]
    public GameObject collision;

    // Start is called before the first frame update

    /***********************************************/
    /***********************************************/
    /*              START FUNCTION                 */
    /***********************************************/
    /***********************************************/
    void Start()
    {
        enemy = GetComponent<EnemyDefault>();
        soundPlayer = GetComponent<EnemyDefault>().soundPlayer;
        currentLayer = gameObject.layer;
        //Set Components
        enemyVision = GetComponentInChildren<EnemyVision>();
        meleeAttack = enemy.meleeAttack;

        StartPatrol();
    }

    /***********************************************/
    /***********************************************/
    /*              UPDATE FUNCTION                */
    /***********************************************/
    /***********************************************/
    // Update is called once per frame
    void Update()
    {
        //currentDirection = EnemyDefault.EnemyDirection.none;
        if (Input.GetKey(KeyCode.J))
        {
            Activate();
        }
        if (Input.GetKey(KeyCode.L))
        {
            Deactivate();
        }
        // if (Input.GetKey(KeyCode.K)) {
        //     currentDirection = EnemyDefault.EnemyDirection.none;
        // }
        //Check for sight
        // enemyVision.SetSightOrigin(enemy.controller.raycastOrigins.topLeft);
        // if (Time.time)
        if (!PauseManager.instance.isPaused)
        {

            visionPivot.rotation = Quaternion.Euler(0, 0, (enemy.facingRight) ? -90 : 90);


            bool hasTarget = enemyVision.hasTarget;

            if (hasTarget && (currentState != EnemyState.attack && currentState != EnemyState.dead && currentState != EnemyState.chase))
            {
                currentState = EnemyState.chase;
                enemy.SetMoveSpeed(runSpeed);
            }


            // if (enemyVision.FindVisibleTargets(direction).Count > 0)
            //     target = enemyVision.FindVisibleTargets(direction)[0].position;
            // if (target != Vector2.zero)
            //     currentState = EnemyState.chase;

            switch (currentState)
            {
                case EnemyState.idle:
                    currentDirection = EnemyDefault.EnemyDirection.none;
                    enemy.SetMoveSpeed(walkSpeed);
                    break;

                case EnemyState.patrol:
                    if (hasPatrol)
                        Patrol();
                    else
                        currentState = EnemyState.idle;
                    enemy.SetMoveSpeed(walkSpeed);
                    break;

                case EnemyState.chase:
                    Chase();
                    break;
                case EnemyState.caution:
                    Investigate();
                    break;
                case EnemyState.attack:
                    Attack();
                    break;
                case EnemyState.dead:
                    Dead();
                    break;

                default:
                    break;
            }
            //Make sure enemy doesnt run over cliff or into walls


            //Debug.Log("Floom");

            // (currentDirection != EnemyDefault.EnemyDirection.none)
            AdjustForCliff(ref currentDirection);

            enemy.SetCurrentDirection(currentDirection);

        }
        //Set Input on Controller
    }

    /***********************************************/
    /***********************************************/
    /*            STATES METHODS                   */
    /***********************************************/
    /***********************************************/

    void Patrol()
    {
        if (nextMoveTime <= 0)
        {
            if (patrolType == PatrolType.normal)
            {
                fromWaypointIndex %= globalWaypoints.Length;
                int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
                bool atWaypoint = (Vector2.Distance(transform.position, globalWaypoints[toWaypointIndex]) < 0.05);

                FollowTarget(globalWaypoints[toWaypointIndex], ref currentDirection);

                if (atWaypoint)
                {
                    fromWaypointIndex++;

                    nextMoveTime = waitTime;
                }
                //Debug.Log("At Waypoint: " + atWaypoint);
            }
            else if (patrolType == PatrolType.auto)
            {
                bool atCliff = CheckForCliff(autoPatrolDir);

                currentDirection = autoPatrolDir;

                //Debug.Log("Patrol Cliff: " + atCliff);

                if (atCliff)
                {
                    FlipDirection(ref autoPatrolDir);
                    nextMoveTime = waitTime;
                }
            }
        }
        else
        {
            nextMoveTime -= Time.deltaTime * enemy.timeMgr.timeScale;
            currentDirection = EnemyDefault.EnemyDirection.none;

        }
    }

    void Chase()
    {
        if (enemyVision.hasTarget)
        {
            loseTimer = timeToLose;
        }
        if (loseTimer <= 0)
        {
            investigateSpot = enemy.targetPlayer.transform.position;
            cautionTimer = investigateTime;
            currentState = EnemyState.caution;
            return;
        }
        else
            loseTimer -= Time.deltaTime * enemy.timeMgr.timeScale;
        //Debug.Log("Chasing");

        //Debug.Log("Distance To Player: " + Vector2.Distance(transform.position, enemy.targetPlayer.position));

        //Attack when close 
        if (Vector2.Distance(transform.position, enemy.targetPlayer.position) < attackDistance)
        {
            Debug.Log("Attack");
            investigateSpot = enemy.targetPlayer.transform.position;
            attackTimer = meleeAttack.Attack(enemy.facingRight);
            enemy.anim.SetTrigger("Attack");
            currentState = EnemyState.attack;
            return;
        }

        enemy.SetMoveSpeed(runSpeed);


        FollowTarget(enemy.targetPlayer.position, ref currentDirection);
    }

    void Investigate()
    {
        Debug.Log("Investigating");
        if (cautionTimer <= 0)
        {
            currentState = EnemyState.patrol;
        }
        else
        {
            cautionTimer -= Time.deltaTime * enemy.timeMgr.timeScale;
        }

        FollowTarget(investigateSpot, ref currentDirection);
    }

    void Attack()
    {
        // Debug.Log("Attacking");
        currentDirection = EnemyDefault.EnemyDirection.none;
        if (attackTimer < 0)
        {
            cautionTimer = investigateTime;
            currentState = EnemyState.caution;
            return;
        }
        else
        {
            attackTimer -= Time.deltaTime * enemy.timeMgr.timeScale;
        }
    }

    void Dead()
    {
        currentDirection = EnemyDefault.EnemyDirection.none;

    }

    /***********************************************/
    /***********************************************/
    /*            STATES HELPER METHODS            */
    /***********************************************/
    /***********************************************/
    void FollowTarget(Vector2 target, ref EnemyDefault.EnemyDirection direction)
    {
        if (target.x > transform.position.x && Mathf.Abs(target.x - transform.position.x) > 0.1)
        {
            //if (bannedDirection != EnemyDefault.EnemyDirection.right)
            direction = EnemyDefault.EnemyDirection.right;
        }
        else if (target.x < transform.position.x && Mathf.Abs(target.x - transform.position.x) > 0.1)
        {
            //if (bannedDirection != EnemyDefault.EnemyDirection.left)
            direction = EnemyDefault.EnemyDirection.left;
        }
        else
            direction = EnemyDefault.EnemyDirection.none;

    }

    protected void FlipDirection(ref EnemyDefault.EnemyDirection direction)
    {
        //Debug.Log("Flop");
        if (direction == EnemyDefault.EnemyDirection.right)
            direction = EnemyDefault.EnemyDirection.left;
        else if (direction == EnemyDefault.EnemyDirection.left)
            direction = EnemyDefault.EnemyDirection.right;

        //checkTimer = .1f;
    }

    public bool CheckForCliff(EnemyDefault.EnemyDirection direction)
    {
        if (enemy.controller != null)
        {
            // //Runs into wall or cliff switch direction
            if ((enemy.controller.collisionInfo.left && direction == EnemyDefault.EnemyDirection.left) || (enemy.controller.collisionInfo.right && direction == EnemyDefault.EnemyDirection.right))
            {
                return true;
            }

            // if ((enemy.controller.HangingOverCliff(enemy.velocity, enemy.GetDirectionalInput(direction), enemy.facingRight)))
            // {

            //     return true;
            // }
            // else
            //     return false;

            //Debug.Log("Hanging Cliff: " + enemy.controller.HangingOverCliff(enemy.controller.collisionInfo.moveAmountOld, enemy.GetDirectionalInput(direction), enemy.facingRight));

            return enemy.controller.HangingOverCliff(enemy.controller.collisionInfo.moveAmountOld, enemy.GetDirectionalInput(direction), enemy.facingRight);
        }
        return false;
    }

    void AdjustForCliff(ref EnemyDefault.EnemyDirection direction)
    {
        //Debug.Log("Check2: " + CheckForCliff(direction));
        if (CheckForCliff(direction))
        {
            //Debug.Log("poo");
            //  bannedDirection = direction;
            direction = EnemyDefault.EnemyDirection.none;
        }
        else
            bannedDirection = EnemyDefault.EnemyDirection.none;
    }

    /***********************************************/
    /***********************************************/
    /*            ACTIVATION AND DEACTIVATION      */
    /***********************************************/
    /***********************************************/
    public void Deactivate()
    {
        if (!isDead)
        {
            enemy.anim.SetTrigger("Hurt");
            enemy.anim.SetBool("Dead", true);
            currentState = EnemyState.dead;
            gameObject.tag = "Untagged";
            isDead = true;
            //enemy.controller.canMove = false;
            // enemy.controller.boxCollider.enabled = false;
            meleeAttack.Reset();
            enemy.controller.collisionMask = enemy.controller.deactivateMask;
            collision.SetActive(false);
            gameObject.layer = 2;

            // EffectsManager.instance.TriggerHitStop();
            // gameObject.layer = 0;
        }
    }
    public void Activate()
    {
        enemy.healthManager.ResetHealth();
        isDead = false;
        enemy.anim.SetTrigger("Start");
        enemy.anim.SetBool("Dead", false);
        gameObject.tag = spikeLayer;
        //gameObject.layer = currentLayer;
        gameObject.layer = enemy.controller.defaultLayer;
        collision.SetActive(true);
        enemy.controller.collisionMask = enemy.controller.defaultMask;
        enemy.controller.appliedVelocity = Vector2.zero;

        enemy.controller.boxCollider.enabled = true;

        enemy.controller.canMove = true;

        StartPatrol();
    }

    void StartPatrol()
    {
        enemy.SetMoveSpeed(walkSpeed);


        //Set patrol route
        if (canPatrol)
        {
            hasPatrol = true;
            currentState = EnemyState.patrol;

            if (patrolType == PatrolType.auto)
            {
                currentDirection = EnemyDefault.EnemyDirection.right;
            }

            globalWaypoints = new Vector3[localWaypoints.Length];
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                globalWaypoints[i] = localWaypoints[i] + transform.position;
            }
        }
        else
            currentState = EnemyState.idle;
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
