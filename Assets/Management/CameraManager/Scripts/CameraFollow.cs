using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Parameters")]
    public GameObject followObject;
    public Vector2 followOffset;
    private Vector2 threshold;
    public float speed = 5;

    private BoxCollider2D cameraBox;
    private Rigidbody2D followRb;
    private CameraEffects cameraEffects;
    // Start is called before the first frame update
    void Start()
    {
        cameraBox = GetComponent<BoxCollider2D>();
        threshold = CalculateThreshold();
        followRb = followObject.GetComponent<Rigidbody2D>();
        cameraEffects = GetComponent<CameraEffects>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector2 follow = followObject.transform.position;
        float xDifference = Vector2.Distance(Vector2.right * transform.position.x, Vector2.right * follow.x);
        float yDifference = Vector2.Distance(Vector2.up * transform.position.y, Vector2.up * follow.y);

        Vector3 newPosition = transform.position;
        if (Mathf.Abs(xDifference) >= threshold.x)
        {
            newPosition.x = follow.x;
        }
        if (Mathf.Abs(yDifference) >= threshold.y)
        {
            newPosition.y = follow.y;
        }
        float moveSpeed = followRb.velocity.magnitude > speed ? followRb.velocity.magnitude : speed;

        //  if (cameraEffects.IsShaking())
        // newPosition += cameraEffects.ShakeAdditive();

        //Clamp camera to boundary
        if (GameObject.Find("Boundary"))
        {
            BoxCollider2D boundary = GameObject.Find("Boundary").GetComponent<BoxCollider2D>();
            Vector3 playerPosition = newPosition;
            newPosition = new Vector3(Mathf.Clamp(playerPosition.x, boundary.bounds.min.x + cameraBox.size.x / 2, boundary.bounds.max.x - cameraBox.size.x / 2),
                                      Mathf.Clamp(playerPosition.y, boundary.bounds.min.y + cameraBox.size.y / 2, boundary.bounds.max.y - cameraBox.size.y / 2),
                                      transform.position.z);
        }

        transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeed * Time.deltaTime);
    }

    private Vector3 CalculateThreshold()
    {
        Rect aspect = Camera.main.pixelRect;
        Vector2 t = new Vector2(Camera.main.orthographicSize * aspect.width / aspect.height, Camera.main.orthographicSize);
        t.x -= followOffset.x;
        t.y -= followOffset.y;
        return t;
    }

    void AdjustForAspectRatio()
    {
        //16:10 ratio
        if (Camera.main.aspect >= 1.6f && Camera.main.aspect < 1.7f)
        {
            cameraBox.size = new Vector2(23, 14.3f);
        }

        //16:9 ratio
        if (Camera.main.aspect >= 1.7f && Camera.main.aspect < 1.8f)
        {
            cameraBox.size = new Vector2(23.47f, 14.3f);
        }

        //5:4 ratio
        if (Camera.main.aspect >= 1.25f && Camera.main.aspect < 1.3f)
        {
            cameraBox.size = new Vector2(18, 14.3f);
        }

        //4:3 ratio
        if (Camera.main.aspect >= 1.3f && Camera.main.aspect < 1.4f)
        {
            cameraBox.size = new Vector2(19.13f, 14.3f);
        }
        //3:2 ratio
        if (Camera.main.aspect >= 1.5f && Camera.main.aspect < 1.6f)
        {
            cameraBox.size = new Vector2(21.6f, 14.3f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector2 border = CalculateThreshold();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
    }
}
