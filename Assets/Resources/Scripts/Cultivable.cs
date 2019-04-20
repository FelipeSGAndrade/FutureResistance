using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cultivable : MonoBehaviour
{

    public int stage;
    public int growth;
    public bool grown;
    public Seed seed;

    private SpriteRenderer cropRenderer;
    private int numberOfStages;
    private int ticksPerStage;

    void Start() {
        GameObject cultivableObject = new GameObject("Cultivable");
        cultivableObject.transform.SetParent(transform, false);

        cropRenderer = cultivableObject.AddComponent<SpriteRenderer>();
        cropRenderer.sortingLayerName = "Items";
        cropRenderer.enabled = false;
    }

    public void Plant() {
        Plant(this.seed);
    } 

    public void Plant(Seed seed) {
        this.seed = seed;
        numberOfStages = seed.stageSprites.Length;
        ticksPerStage = seed.ticksToGrow / numberOfStages;

        cropRenderer.sprite = seed.stageSprites[stage];
        cropRenderer.enabled = true;

        TickSystem.Subscribe(Grow);
    }

    public void Harvest() {
        stage = 0;
        seed = null;
        cropRenderer.enabled = false;
    }

    private void Grow() {
        growth++;
        SetStage();

        if (stage == numberOfStages - 1) {
            grown = true;
            TickSystem.Unsubscribe(Grow);
        }
    }

    private void SetStage() {
        int previousStage = stage;
        stage = growth / ticksPerStage;

        if (previousStage != stage) {
            cropRenderer.sprite = seed.stageSprites[stage];
        }
    }
}
