using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CultivableUI : MonoBehaviour
{
    public Seed seed;
    public Button button;

    void Start() {
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

        button.onClick.AddListener(() => TaskManager.AddTask(new PlantTask(block.GetComponentInParent<Node>())));
    }
}
