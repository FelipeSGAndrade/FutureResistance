using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTile : MonoBehaviour {

	private int currentTileValue = 0;
	private bool updatable = true;
	private SpriteRenderer parentsRenderer;
	public Texture2D texture;

	[NonSerialized]
	public Sprite defaultSprite;

	UnityEngine.Object[] sortedTiles;

	// Use this for initialization
	void Start () {
		parentsRenderer = (SpriteRenderer)gameObject.GetComponent<SpriteRenderer>();

		if(!parentsRenderer) {
			updatable = false;
			return;
		}

		defaultSprite = parentsRenderer.sprite;

		UnityEngine.Object[] tiles = Resources.LoadAll("Sprites/" + texture.name);

		sortedTiles = new UnityEngine.Object[16];

		sortedTiles[0] = tiles[16];	
		sortedTiles[1] = tiles[12];
		sortedTiles[2] = tiles[15];
		sortedTiles[3] = tiles[11];
		sortedTiles[4] = tiles[13];
		sortedTiles[5] = tiles[9];
		sortedTiles[6] = tiles[14];
		sortedTiles[7] = tiles[10];
		sortedTiles[8] = tiles[4];
		sortedTiles[9] = tiles[8];
		sortedTiles[10] = tiles[3];
		sortedTiles[11] = tiles[7];
		sortedTiles[12] = tiles[1];
		sortedTiles[13] = tiles[5];
		sortedTiles[14] = tiles[2];
		sortedTiles[15] = tiles[6];

		UpdateState();
	}

	public GameObject getNeighbor(int x, int y) {
		if (x < 0 || y < 0 || x >= MapManager.width || y >= MapManager.height) {
			GameObject border = new GameObject();
			border.tag = tag;
			return border;
		}

		return MapManager.objectsMap[x, y];
	}

	public void UpdateState() {
		if(!parentsRenderer || !updatable)
			return;

		int x = (int)transform.position.x;
		int y = (int)transform.position.y;

		int tileValue = 0;

		GameObject neighbor = getNeighbor(x, y + 1);				
		if (neighbor != null && neighbor.CompareTag(tag)) {
			tileValue += 1;
		}

		neighbor = getNeighbor(x - 1, y);
		if (neighbor != null && neighbor.CompareTag(tag)) {
			tileValue += 2;
		}

		neighbor = getNeighbor(x, y - 1);
		if (neighbor != null && neighbor.CompareTag(tag)) {
			tileValue += 8;
		}

		neighbor = getNeighbor(x + 1, y);
		if (neighbor != null && neighbor.CompareTag(tag)) {
			tileValue += 4;
		}

		if(tileValue == currentTileValue)
			return;

		parentsRenderer.sprite = (Sprite)sortedTiles[tileValue];

		currentTileValue = tileValue;
		MapManager.NotificateChangeFrom(x, y);
	}
}
