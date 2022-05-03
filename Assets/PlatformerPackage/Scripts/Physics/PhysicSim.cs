using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhysicSim
{
    public const float defaultGravity = -50;
    public float simGravity;
    public float simFallGravityModifier;
    public bool retainVertical;
    [HideInInspector]
    public Vector2 initialVelocity;
    float friction;
    float simTime;
    float timer;

    //[HideInInspector]
    public bool active = false;
    public ForceMode forceMode;

    // public void PrimePhysicsSim(ForceSim force, ref Vector2 velocity, Vector2 initialVelocity, bool facingRight, ForceMode _forceMode) {
    //     simGravity = -Mathf.Abs(force.gravity);
    //     velocity
    // }

    public void PrimePhysicsSim(ForceSim force, ref Vector2 velocity, bool facingRight, ForceMode _forceMode)
    {
        //controller.gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        // simGravity = -(2 * force.height) / Mathf.Pow(force.timeToHeight, 2);

        if (!force.retainVertical)
        {
            simGravity = -Mathf.Abs(force.gravity);
            initialVelocity.y = force.riseForce;
        }
        float xOffset = (!facingRight) ? -1 : 1;
        initialVelocity.x = force.pushForce * xOffset;
        velocity = initialVelocity;
        friction = force.friction;
        retainVertical = force.retainVertical;
        forceMode = _forceMode;
        simTime = force.duration;
        timer = 0;
        active = true;
    }
    public void RunPhysicsSim(ref Vector2 velocity, bool grounded, bool hittingWall)
    {
        //Debug.Log("Wall Hittin: " + hittingWall);
        if (active)
        {
            float gravity = (velocity.y >= 0) ? simGravity : simGravity * simFallGravityModifier;
            velocity.y += -Mathf.Abs(simGravity) * Time.deltaTime * EffectsManager.instance.timeScale;
            if (forceMode == ForceMode.impulse)
            {

                if (grounded)
                {
                   // velocity.y = 0;

                    if (!hittingWall)
                    {
                        float threshold = 0.05f;

                        if (velocity.x > threshold)
                        {
                            velocity.x -= Mathf.Abs(friction) * Time.deltaTime * EffectsManager.instance.timeScale;
                        }
                        else if (velocity.x < -threshold)
                        {
                            velocity.x += Mathf.Abs(friction) * Time.deltaTime * EffectsManager.instance.timeScale;
                        }
                        else
                        {
                            velocity.x = 0;
                            active = false;
                        }
                    }
                    else
                    {
                        Debug.Log("Bloom");
                        active = false;
                    }
                }
            }
            else if (forceMode == ForceMode.manual)
            {
                if (timer < simTime)
                {
                    timer += Time.deltaTime * EffectsManager.instance.timeScale;
                    if (!retainVertical)
                        velocity.y = initialVelocity.y;
                    velocity.x = initialVelocity.x;
                }
                else
                {
                    if (!retainVertical)
                        velocity.y = 0;
                    velocity.x = 0;
                    active = false;
                }
            }
        }
        else
            velocity = Vector2.zero;

        //return Vector2.zero;
    }

    IEnumerator StopSimulation()
    {
        yield return new WaitForEndOfFrame();
        active = false;
    }
}
public enum ForceMode { manual, impulse }

[System.Serializable]
public class ForceSim
{
    public bool retainVertical;
    //public ForceMode forceMode = ForceMode.manual;
    public float pushForce;
    public float riseForce;
    public float duration;
    public float gravity;

    [Range(0.1f, 1000)]
    public float friction;
    //public float angle;
    //[HideInInspector]
    //public float height;
    // public float timeToHeight;
    //public AnimationCurve forceCurve;
    //public float duration;
}