using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockUI : MonoBehaviour
{
    public Text nameText;
    public CultivableUI cultivableUIPrefab;
    public Transform blockStatusPanel;

    public void Show() {
        CleanUI();

        GameObject block = UIController.instance.SelectedBlock;

        gameObject.SetActive(true);
        nameText.text = block.name;

        if (block.GetComponent<Cultivable>()) {
            CultivableSetup();
        }
    }

    public void Hide() {
        gameObject.SetActive(false);
        CleanUI();
    }

    private void CleanUI() {
        for (int i = 1; i < blockStatusPanel.childCount; i++) {
            Destroy(blockStatusPanel.GetChild(i).gameObject);
        }
    }

    private void CultivableSetup() {
        CultivableUI.Instantiate(cultivableUIPrefab, blockStatusPanel);
    }
}
