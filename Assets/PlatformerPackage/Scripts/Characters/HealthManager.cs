using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [Header("When Health Zero")]
    public UnityEvent whenHealthZero;

    [Header("Health Variables")]
    public float maxHealth;
    float currentHealth;

    [Header("Effects")]
    public HitStopData hitStopNormal;
    public HitStopData hitStopHealthZero;
    public Material whiteOut;
    Material defaultMaterial;
    float hitStunTimer;
    bool hitStuned;

    [Header("Essential Components")]
    SoundPlayer soundPlayer;
    MeleeAttack meleeAttack;
    SpriteRenderer sprite;

    private void Start()
    {
        ResetHealth();
        soundPlayer = GetComponent<SoundPlayer>();
        meleeAttack = GetComponent<MeleeAttack>();
        sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>();

        defaultMaterial = sprite.material;
    }

    private void Update()
    {
        if (hitStunTimer > 0)
        {
            sprite.material = whiteOut;
            hitStunTimer -= Time.deltaTime * EffectsManager.instance.timeScale;
        }
        else
        {
            if (hitStuned)
            {
                sprite.material = defaultMaterial;
                hitStuned = false;
            }
        }
    }

    // [Header("Hitbox Variables")]
    // public LayerMask dangerMask;
    // public string enemyTag;
    // public string ignoreTag;
    // public CircleCollider2D[] collider2Ds;

    // Update is called once per frame
    // void Update()
    // {
    //     //Run Check for bumping into
    //     for (int i = 0; i < collider2Ds.Length; i++)
    //     {
    //         Collider2D[] hits = Physics2D.OverlapCircleAll(collider2Ds[i].bounds.center, collider2Ds[i].radius, dangerMask);

    //         for (int j = 0; j < hits.Length; j++)
    //         {
    //             if (hits[j].tag == ignoreTag)
    //                 continue;
    //             if (hits[j].tag == enemyTag || hits[j].tag == "SpikeLayer")
    //             {
    //                // whenHit.Invoke();
    //                 break;
    //             }
    //         }
    //     }
    // }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void SubtractHealth(float health)
    {
        if (!meleeAttack.invincible)
        {
            Debug.Log("Lose Health");
            currentHealth -= health;
            hitStunTimer = (float)meleeAttack.hitStunFrames / 60f;
            hitStuned = true;
            if (currentHealth > 0)
            {
                soundPlayer.PlaySound("hurt");
                EffectsManager.instance.TriggerHitStop(hitStopNormal);
            }
            if (currentHealth <= 0)
            {
                EffectsManager.instance.TriggerHitStop(hitStopHealthZero);
                soundPlayer.PlaySound("Dead");


                whenHealthZero.Invoke();
            }
        }
    }
}
