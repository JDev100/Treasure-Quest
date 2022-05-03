using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class EnemyController2D : Controller2D
{
   [HideInInspector]
    public LayerMask defaultLayer;
   // public LayerMask deactivateLayer;
    public LayerMask defaultMask;
    public LayerMask deactivateMask;
    float checkTime;

    [HideInInspector]
    public bool canMove = true;

    Vector2 oldInput;

    EnemyInputDefault enemyInput;

    public override void Start()
    {
        base.Start();
        enemyInput = GetComponent<EnemyDefault>().enemyInput;
        defaultLayer = gameObject.layer;
        defaultMask = collisionMask;
    }

    // public new void Move(Vector2 moveAmount, Vector2 input, bool facingRight)
    // {
    //     if (canMove)
    //         moveAmount = base.Move(moveAmount);

    //     // HangingOverCliff(moveAmount, input, facingRight);

    //     // Debug.Log("Over Cliff: " + HangingOverCliff(moveAmount, input, facingRight));
    // }

    // protected override RaycastHit2D HorizontalCollisions(ref Vector2 moveAmount)
    // {
    //     RaycastHit2D hit = base.HorizontalCollisions(ref moveAmount);

    //     if (hit)
    //     {
    //         if (hit.collider.tag == "SwordLayer")
    //         {
    //             enemyInput.Deactivate();
    //             return hit;
    //         }
    //     }
    //     return new RaycastHit2D();
    // }

    // protected override RaycastHit2D VerticalCollisions(ref Vector2 moveAmount)
    // {
    //    RaycastHit2D hit = base.VerticalCollisions(ref moveAmount);

    //     if (hit)
    //     {
    //         if (hit.collider.tag == "SwordLayer")
    //         {
    //             enemyInput.Deactivate();
    //             return hit;
    //         }
    //     }
    //     return new RaycastHit2D();
    // }

    /***********************************************/
    /*          COLLISION FUNCTIONS                */
    /***********************************************/


    /***********************************************/
    /*          HELPER  METHODS                    */
    /***********************************************/
    public bool HangingOverCliff(Vector2 velocity, Vector2 input, bool facingRight)
    {

        if (velocity.x != 0)
        {
              if (velocity.y  == 0) 
        velocity.y = -skinWidth;
            oldInput = input;

            //Debug.Log("Fllop");
            float directionX;
            if (input != Vector2.zero)
                directionX = Mathf.Sign(input.x);
            else
                directionX = (facingRight) ? 1 : -1;
            float raylength = Mathf.Abs(velocity.y) + skinWidth;
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.right * velocity.x;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, raylength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.down, Color.blue);

            if (hit)
            {
                collisionInfo.overCliff = collisionInfo.overCliffOld = false;
                return false;
            }
            //if (Mathf.Sign(directionX) == Mathf.Sign(velocity.x))
            collisionInfo.overCliff = collisionInfo.overCliffOld = true;
            return true;

        }
        else
        {
            // Debug.Log("Flep");
            if (input != oldInput)
            {
                collisionInfo.overCliff = false;
                return false;
            }
            return true;
        }
    }

    //public bool OverCliff();
}
