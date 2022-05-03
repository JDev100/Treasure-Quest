using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Essential Components")]
    Animator anim;
    [HideInInspector]
    public float timeScale;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timeScale = EffectsManager.instance.timeScale;
        if (PauseManager.instance.isPaused)
            timeScale = 0;
        if (anim != null)
            anim.speed = timeScale;
    }

    public void TriggerKnockBack()
    {
        EffectsManager.instance.TriggerHitStop();
    }
}
