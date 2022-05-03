using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Specifications")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    [Range(-180, 180)]
    public float viewDirection = 0;
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    Transform targetPlayer;

    Vector2 sightOrigin;
    //public List<Transform> visibleTargets = new List<Transform>();

    [HideInInspector]
    public bool hasTarget;

    void Start()
    {
        StartCoroutine("VisibleTargetsDelay", 0.2f);

        //Enemies will only ever target player so:
    }

    IEnumerator VisibleTargetsDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (!PauseManager.instance.isPaused)
            FindVisibleTargets(viewDirection);
        }
    }
    void FindVisibleTargets(float direction)
    {
        viewDirection = direction;
        hasTarget = false;
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            Debug.Log("In Circle: " + target);

            Debug.DrawRay(transform.position, dirToTarget, Color.green);


            if (Vector2.Angle(transform.up, dirToTarget) < (viewAngle / 2))
            {
                float dstToTarget = Vector2.Distance(transform.position, target.position);
                Debug.DrawRay(transform.position, dirToTarget, Color.yellow);

                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask);
                //
                Debug.DrawRay(transform.position, dirToTarget, Color.green);
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    hasTarget = true;
                    Debug.Log("Target: " + target);
                    return;
                    // return true;
                }
            }
        }

        //return false;;
    }

    public void SetSightOrigin(Vector2 origin)
    {
        sightOrigin = origin;
    }

    public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
