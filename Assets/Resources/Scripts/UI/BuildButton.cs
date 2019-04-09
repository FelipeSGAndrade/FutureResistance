using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    public GameObject prefabToBuild;
    public GameManager gameManager;
    public Image buttonImage;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => gameManager.SetAction(ActionType.BUILD, prefabToBuild));

        SpriteRenderer prefabSpriteRenderer = prefabToBuild.GetComponent<SpriteRenderer>();

        if (prefabSpriteRenderer) {
            buttonImage.sprite = prefabSpriteRenderer.sprite;
        }
    }
}
