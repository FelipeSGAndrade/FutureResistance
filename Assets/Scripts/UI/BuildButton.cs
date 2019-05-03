using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    public Buildable building;
    public Image buttonImage;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => GameController.instance.SetBuildAction(building));
        buttonImage.sprite = building.GetComponent<SpriteRenderer>().sprite;
    }
}
