using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public GameObject mapGenerationUI;
    public GameObject hud;
    public BlockUI blockUI;

    private GameObject selectedBlock;
    public GameObject SelectedBlock { get => selectedBlock; }

	void Awake() {
		if (instance) {
			Debug.LogError("There is more than one UI Controller in Scene");
			return;
		}

		instance = this;
	}

    public void SetState(UIState state) {
        switch (state) {
            case UIState.MAP_GENERATION:
                mapGenerationUI.SetActive(true);
                break;

            case UIState.GAME:
                mapGenerationUI.SetActive(false);
                hud.SetActive(true);
                break;
        }
    }

    public void ShowBlockUI(GameObject block) {
        selectedBlock = block;
        blockUI.Show();
    }

    public void HideBlockUI() {
        blockUI.Hide();
    }
}
