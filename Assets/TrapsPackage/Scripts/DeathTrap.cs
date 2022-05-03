using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathTrap : MonoBehaviour
{
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // private void OnTriggerEnter2D(Collider2D other) {
    //     Debug.Log("Hit SOmething");

    //     if (other.gameObject.tag == "Player")
    //     levelManager.PlayerGameOver();
    // }
}
