using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePrint : MonoBehaviour {
	private SpriteRenderer spriteRenderer;
	private GameObject blockToBuild;
	private Node parentNode;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void SetBlockToBuild(Node node, GameObject blockPrefabToBuild) {
		blockToBuild = blockPrefabToBuild;
		parentNode = node;

		SpriteRenderer targetSpriteRenderer = blockToBuild.GetComponent<SpriteRenderer>();
		if (targetSpriteRenderer) {
			spriteRenderer.sprite = targetSpriteRenderer.sprite;
		}
	}

	public GameObject GetBlockToBuild() {
		return blockToBuild;
	}

	public void BuildTick(int ticks, int neededTicks) {
		float newAlpha = (1f / neededTicks) * ticks;
		spriteRenderer.color = new Color(1, 1, 1, newAlpha);
	}

	public void FinishBuilding() {
		parentNode.AddBlock(blockToBuild);
	}
}
