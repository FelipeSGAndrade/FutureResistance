using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleActionButton : MonoBehaviour
{
    public Image buttonImage;
    public ActionType action;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => GameController.instance.SetAction(action));
    }
}
