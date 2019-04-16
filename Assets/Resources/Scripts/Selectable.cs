using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    private Buildable buildable;
    
    void Start() {
        buildable = GetComponent<Buildable>();
    }

    public void Select() {
        if (buildable && !buildable.complete)
            return;

        UIController.instance.ShowBlockUI(gameObject);
    }
}
