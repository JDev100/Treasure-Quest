using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruct : MonoBehaviour
{
    private void OnEnable()
    {
        if (FindObjectOfType<GameManager>())
        {
            Destroy(FindObjectOfType<GameManager>());
        }
    }
}
