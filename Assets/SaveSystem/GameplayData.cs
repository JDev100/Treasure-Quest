using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayData : MonoBehaviour
{
    public static GameplayData instance;

    public AudioClip currentSong;
    public float globalMusicVolume;
    public float globalSFXVolume;
    public Vector2 playerStartLocation;
    // Start is called before the first frame update
    void Awake()
    {
        //Singleton stuff
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Start() {
        
        DontDestroyOnLoad(this.gameObject);
    }

    // private void Update() {
    // }

    public void SetSong(AudioClip audioClip)
    {
        currentSong = audioClip;
    }

  
}
