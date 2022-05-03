using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EffectsDefault : MonoBehaviour
{
    Animator anim;
    float timer;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        timer = .5f;
        anim.SetTrigger("Play");
    }
    private void OnEnable()
    {
        timer = .5f;
        if (anim != null)
        {
            anim.SetTrigger("Play");
        }
        else
        {
            anim = GetComponent<Animator>();
            anim.SetTrigger("Play");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0)
        {
            ObjectPoolManager.instance.ClearObject(gameObject);
        }
        else
        {
            timer -= Time.deltaTime * EffectsManager.instance.timeScale;
        }
    }
}
