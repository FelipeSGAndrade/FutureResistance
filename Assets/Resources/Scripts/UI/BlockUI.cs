using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockUI : MonoBehaviour
{
    public Text nameText;
    public Transform blockStatusPanel;

    public void Show(Selectable selectable) {
        CleanUI();

        GameObject block = UIController.instance.SelectedBlock;

        gameObject.SetActive(true);
        nameText.text = block.name;

        selectable
            .GetInteractions()
            .ForEach((interaction) => interaction.AddUI(blockStatusPanel));
    }

    public void Hide() {
        gameObject.SetActive(false);
        CleanUI();
    }

    private void CleanUI() {
        foreach (Transform child in blockStatusPanel) {
            Destroy(child.gameObject);
        }
    }
}
