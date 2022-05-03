using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpriteTest : MonoBehaviour
{
    public Sprite spriteToCheck;
    public Sprite tileToCheck;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spriteToCheck != null && tileToCheck != null)
        {
            // if (spriteToCheck == tileToCheck.sprite) {
            //     Debug.Log("Same Sprite");

            // }
            // else {
            //     Debug.Log("Nope");
            // }
            if (CompareTexture(spriteToCheck.texture, tileToCheck.texture))
            {
                Debug.Log("Same Sprite");
            }
            else
            {
                Debug.Log("Nope");
            }
        }
    }

    private bool CompareTexture(Texture2D first, Texture2D second)
    {
        Color[] firstPix = first.GetPixels();
        Color[] secondPix = second.GetPixels();
        if (firstPix.Length != secondPix.Length)
        {
            return false;
        }
        for (int i = 0; i < firstPix.Length; i++)
        {
            if (firstPix[i] != secondPix[i])
            {
                return false;
            }
        }

        return true;
    }
}
