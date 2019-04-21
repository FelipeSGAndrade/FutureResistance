using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cultivable : BlockBehaviour
{
    public Seed seed;
    public CultivableUI cultivableUIPrefab;

    [HideInInspector] public bool grown;

    private SpriteRenderer cropRenderer;
    private int stage;
    private int growth;
    private int numberOfStages;
    private int ticksPerStage;

    void Start() {
        GameObject cultivableObject = new GameObject("Cultivable");
        cultivableObject.transform.SetParent(transform, false);

        cropRenderer = cultivableObject.AddComponent<SpriteRenderer>();
        cropRenderer.sortingLayerName = "Items";
        cropRenderer.enabled = false;

        Selectable selectable = GetComponent<Selectable>();
        if (selectable) {
            selectable.AddInteraction(this);
        }
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
        grown = false;
        growth = 0;
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

    public override void AddUI(Transform parent) {
        Instantiate(cultivableUIPrefab, parent);
    }
}
