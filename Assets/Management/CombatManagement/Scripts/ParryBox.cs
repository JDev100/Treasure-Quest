using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryBox : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform owner;
    public CircleCollider2D hitBox;

    private void Awake() {
        hitBox = GetComponent<CircleCollider2D>();
    }

    public void SetHitBox(Vector2 center, float radius,  Transform _owner) {
        hitBox.radius = radius;
        transform.position = center;
        owner = _owner;
    }
}
