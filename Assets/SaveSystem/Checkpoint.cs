using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    
      public void SetCheckpoint() {
          GameplayData.instance.playerStartLocation = transform.position;
        //playerStartLocation = transform.position;
    }

    public void EndGame() {
        SceneManager.LoadScene(2);
    }
}
