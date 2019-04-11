using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    public GameObject prefabToBuild;
    public GameController gameController;
    public Image buttonImage;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => gameController.SetAction(ActionType.BUILD, prefabToBuild));

        SpriteRenderer prefabSpriteRenderer = prefabToBuild.GetComponent<SpriteRenderer>();

        if (prefabSpriteRenderer) {
            buttonImage.sprite = prefabSpriteRenderer.sprite;
        }
    }
}
