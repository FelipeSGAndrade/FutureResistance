using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CultivableUI : MonoBehaviour
{
    public Button button;
    public Dropdown seedsDropDown;
    public Seed[] seedOptions;

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

        if (seedOptions.Length == 0) {
            Debug.LogError("No seed available");
            return;
        }

        seedsDropDown.ClearOptions();
        foreach (Seed seed in seedOptions) {
            seedsDropDown.options.Add(new Dropdown.OptionData(seed.name));
        }

        button.onClick.AddListener(() => AddPlantTask(block));
    }

    void AddPlantTask(GameObject block) {
        Seed selectedSeed = seedOptions[seedsDropDown.value];
        Node node = block.GetComponentInParent<Node>();
        TaskManager.AddTask(new PlantTask(node, selectedSeed));
    }
}
