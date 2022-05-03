using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPerfectZoom : MonoBehaviour
{
    public MeshRenderer zoomBox;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update() {
        Camera.main.orthographicSize = zoomBox.bounds.size.x * Screen.height / Screen.width * 0.5f;
        
    }
}
