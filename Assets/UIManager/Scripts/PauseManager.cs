using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    [HideInInspector]
    public bool isPaused = false;
    float timeScale;

    public GameObject pauseCanvas;
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
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Start") || Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        isPaused = !isPaused;
        // if (pauseCanvas.activeSelf)
        // {
        //     pauseCanvas.GetComponent<MainMenu>().ToggleScreen("Main Menu");
        // }
        Pause();
    }

    void Pause()
    {
        timeScale = timeScale == 0 ? 1 : 0;
        EffectsManager.instance.LowPass(timeScale);
    }


}
