using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIState {
    MAP_GENERATION,
    GAME
}

public class UIController : MonoBehaviour
{
    public GameObject mapGenerationUI;
    public GameObject hud;

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
}
