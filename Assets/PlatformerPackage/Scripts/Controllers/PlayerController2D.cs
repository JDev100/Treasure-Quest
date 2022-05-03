using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerController2D : Controller2D
{
    // Start is called before the first frame update

    // Update is called once per frame

    //[HideInInspector]
    public bool isGameOver = false;

    // public override void Start() {
    //    base.Start(); 
    // }

    protected override RaycastHit2D HorizontalCollisions(ref Vector2 moveAmount)
    {
        RaycastHit2D hit = base.HorizontalCollisions(ref moveAmount);

        if (hit)
        {
            if ((hit.collider.tag == "SpikeLayer" || hit.collider.tag == "Enemy") && !isGameOver)
            {
                FindObjectOfType<LevelManager>().PlayerGameOver();
                isGameOver = true;
                return hit;
            }

            if (hit.collider.tag == "CrumblingPlatform" && !isGameOver)
            {
                // Debug.Log("Crumble!!");
                // Vector3 hitLocation = hit.point;
                //  hitLocation.y -= 0.25f;
                // LevelManager.instance.StartCrumble(hitLocation);
            }
        }

        return new RaycastHit2D();
    }

    protected override RaycastHit2D VerticalCollisions(ref Vector2 moveAmount)
    {
        RaycastHit2D hit = base.VerticalCollisions(ref moveAmount);

        if (hit)
        {
            if ((hit.collider.tag == "SpikeLayer" || hit.collider.tag == "Enemy") && !isGameOver)
            {
                FindObjectOfType<LevelManager>().PlayerGameOver();
                isGameOver = true;
                return hit;
            }

            if (hit.collider.tag == "CrumblingPlatform" && !isGameOver && collisionInfo.below)
            {
                Debug.Log("Crumble!!");
                Vector3 hitLocation = hit.point;
                 hitLocation.y -= 0.25f;
                LevelManager.instance.StartCrumble(hitLocation);
            }
        }
        return new RaycastHit2D();
    }
}