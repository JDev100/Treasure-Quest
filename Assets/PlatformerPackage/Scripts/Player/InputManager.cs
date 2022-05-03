using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;
    private bool canInput = true;

    private float disableTimer;
    bool timerStart = false;
    // Start is called before the first frame update

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);    
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (disableTimer > 0 && timerStart) {
            disableTimer -= Time.deltaTime;
            SetInput(false);
        }
        else {
            if (timerStart) {
            SetInput(true);
            timerStart = false;
            }
        }
    }

    void SetInput(bool input) {
        canInput = input;
    }

    public bool CanInput() {
        return canInput;
    }

    public void DisableInputForTime(float time) {
        disableTimer = time;
        timerStart = true;
    }

    public void DisableInput() {
        if (!timerStart)
        SetInput(false);
    }

    public void EnableInput() {
        if (!timerStart)
        SetInput(true);
    }
}
