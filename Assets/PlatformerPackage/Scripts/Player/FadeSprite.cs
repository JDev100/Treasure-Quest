using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    [Header("Color Gradient")]
    public Color colorStart;
    public Color colorEnd;

    [Header("Time")]
    public float fadeTime = 1f;
    private float fadeTimer;
    // Start is called before the first frame update
    void Start()
    {
        fadeTimer = fadeTime;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = colorStart;
        //Destroy(gameObject, 2f);

    }

    private void OnEnable() {
        if (spriteRenderer != null) {
        fadeTimer = fadeTime;
        spriteRenderer.color = colorStart;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer.color != colorEnd)
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, colorEnd, fadeTime * Time.deltaTime);

        
    }

    public void SetSprite(Sprite spriteToChange) {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteToChange;
    }
    public void SetSprite(Sprite spriteToChange, bool flipX) {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteToChange;
        spriteRenderer.flipX = flipX;

    }
}
