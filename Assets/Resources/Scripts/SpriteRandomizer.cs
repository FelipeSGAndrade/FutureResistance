using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{
    public Sprite[] spriteArray;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (!spriteRenderer) {
            throw new UnityException("Sprite randomizer needs a sprite renderer");
        }

		spriteRenderer.sprite = spriteArray[Random.Range(0, spriteArray.Length)];
    }
}
