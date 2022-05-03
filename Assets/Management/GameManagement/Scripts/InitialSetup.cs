using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InitialSetup : MonoBehaviour
{
    public UnityEvent[] onSceneStart;

    private void Start() {
        for (int i = 0; i < onSceneStart.Length; i++)
        {
            onSceneStart[i].Invoke();
        }
    }

    public void StartLevel() {
        LevelManager.instance.StartLevel();
    }
}
