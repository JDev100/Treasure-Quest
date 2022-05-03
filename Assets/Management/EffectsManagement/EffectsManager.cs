using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager instance;

    [Header("HitStop Variables")]
    public AnimationCurve defaultHitStopCurve;
    AnimationCurve hitStopCurve;
    [Range(1, 120)]
    public int defaultHitStopFrames;
    [Range(0, 1)]
    [HideInInspector]
    public float timeScale = 1;
    float timeStartedLerping = 0f;

    bool hitStopActive = false;
    int hitStopFrames;
    float hitStopTimer;

    [Header("Audio Effects")]
    public AudioMixerSnapshot paused;
    public AudioMixerSnapshot unPaused;
    public AudioMixerSnapshot muffledSound;
    public float timeToMuffle;
    public float timeToNormal;

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

    private void Start()
    {
        hitStopTimer = (float)defaultHitStopFrames / 60f;

    }

    // Update is called once per frame
    void Update()
    {
        if (hitStopActive)
        {
            timeStartedLerping += Time.deltaTime;

            if (timeStartedLerping > hitStopTimer)
            {
                timeStartedLerping = hitStopTimer;
                hitStopActive = false;
            }

            float lerpRatio = timeStartedLerping / hitStopTimer;
            // Debug.Log("Lerp Ratio: " + lerpRatio);
            //  Debug.Log("Lerp Evalaluate: " + hitStopCurve.Evaluate(lerpRatio));

            float offset = hitStopCurve.Evaluate(lerpRatio);
            //Debug.Log("Offset: " + offset);

            timeScale = Mathf.Lerp(0.07f, 1, offset);
        }
    }

    public void LowPass(float _timeScale)
    {
        if (_timeScale == 1)
        {
            paused.TransitionTo(.1f);
        }
        else
        {
            unPaused.TransitionTo(.1f);
        }
    }

    public void MuffleSound()
    {
        muffledSound.TransitionTo(timeToMuffle);
        StartCoroutine(ToNormal(timeToMuffle));
    }

    IEnumerator ToNormal(float delay)
    {
        yield return new WaitForSeconds(delay + 0.001f);
        unPaused.TransitionTo(timeToNormal);
    }


    public void TriggerHitStop()
    {
        if (!hitStopActive)
        {
            timeStartedLerping = 0;
            hitStopFrames = defaultHitStopFrames;
            hitStopTimer = (float)hitStopFrames / 60f;
            hitStopCurve = defaultHitStopCurve;
            hitStopActive = true;
        }
    }

    public void TriggerHitStop(HitStopData hitStopData) {
          if (!hitStopActive)
        {
            timeStartedLerping = 0;
            hitStopFrames = hitStopData.hitStopFrames;
            hitStopCurve = hitStopData.hitStopCurve;
            hitStopTimer = (float)hitStopFrames / 60f;
            hitStopActive = true;
        }
    }

    void HitStop(float lerpTime)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        if (percentageComplete >= 1)
            hitStopActive = false;

        timeScale = Mathf.Lerp(0.07f, 1, percentageComplete);
    }
}

/***********************************************/
/*          ESSENTIAL STRUCTS                  */
/***********************************************/
[System.Serializable]
public struct HitStopData {
    public int hitStopFrames;
    public AnimationCurve hitStopCurve;
}