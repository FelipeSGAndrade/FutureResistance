using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    public Buildable building;
    public GameController gameController;
    public Image buttonImage;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => gameController.SetBuildAction(building));
        buttonImage.sprite = building.GetComponent<SpriteRenderer>().sprite;
    }
}
