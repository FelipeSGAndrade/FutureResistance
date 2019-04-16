using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CultivableUI : MonoBehaviour
{
    public Seed seed;
    public Button button;

    void Start() {
        Debug.Log("SETUP");
        GameObject block = UIController.instance.SelectedBlock;
        if (!block) {
            Debug.LogError("No selected block");
            return;
        }

        Cultivable cultivable = block.GetComponent<Cultivable>();
        if (!cultivable) {
            Debug.LogError("Selected block is not cultivable");
            return;
        }

        Debug.Log("SETUP DONE");
        button.onClick.AddListener(() => cultivable.Plant(seed));
    }
}
