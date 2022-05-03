using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Movement
{
    public static bool AccelerateToTarget(ref float velocity, float targetVelocity, float accelerationRate)
    {

        if (velocity < targetVelocity)
        {
            velocity += Mathf.Abs(accelerationRate) * Time.deltaTime * EffectsManager.instance.timeScale;
            return false;
        }
        return true;
    }

    public static bool DecelerateToTarget(ref float velocity, float targetVelocity, float decelerationRate)
    {
        if (velocity > targetVelocity)
        {
            velocity -= Mathf.Abs(decelerationRate) * Time.deltaTime * EffectsManager.instance.timeScale;
            return false;
        }
        return true;
    }

    public static Vector2 AccelerateToDirection(Vector2 currentVelocity, float maxVelocity, float accelerationRate, Direction direction)
    {
        if (direction == Direction.right)
        {
            float velocityX = currentVelocity.x;
            velocityX += Mathf.Abs(accelerationRate) * Time.deltaTime * EffectsManager.instance.timeScale;

            if (velocityX > maxVelocity)
                velocityX = maxVelocity;
            return new Vector2(velocityX, 0);
        }
        if (direction == Direction.down)
        {
            float velocityY = currentVelocity.y;
            velocityY -= Mathf.Abs(accelerationRate) * Time.deltaTime * EffectsManager.instance.timeScale;

            if (velocityY < -maxVelocity)
                velocityY = -maxVelocity;
            return new Vector2(0, velocityY);
        }
        if (direction == Direction.left)
        {
            float velocityX = currentVelocity.x;
            velocityX -= Mathf.Abs(accelerationRate) * Time.deltaTime * EffectsManager.instance.timeScale;

            if (velocityX < -maxVelocity)
                velocityX = -maxVelocity;
            return new Vector2(velocityX, 0);
        }
        if (direction == Direction.up)
        {
            float velocityY = currentVelocity.y;
            velocityY += Mathf.Abs(accelerationRate) * Time.deltaTime * EffectsManager.instance.timeScale;

            if (velocityY > maxVelocity)
                velocityY = maxVelocity;
            return new Vector2(0, velocityY);
        }
        return Vector2.zero;
    }
}
