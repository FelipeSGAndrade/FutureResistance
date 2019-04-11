using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MapGenerator))]
public class MapManager : MonoBehaviour
{
	public static MapManager instance;
	public static int width;
	public static int height;
	public TerrainType[,] terrainMap;
	public static bool[,] walkableMap;
	public Node[,] nodeMap;

	private bool built;

	private MapGenerator mapGenerator;

	void Awake() {
		if (instance != null) {
			Debug.LogError("There is more than one Map Manager in Scene");
			return;
		}

		instance = this;
	}

	void Start() {
		mapGenerator = GetComponent<MapGenerator>();
		width = mapGenerator.width;
		height = mapGenerator.height;
	}

	void Update() {
		if (!built) {
			width = mapGenerator.width;
			height = mapGenerator.height;
		}
	}

	public void Build() {
		built = true;
		terrainMap = mapGenerator.Finish();
		width = mapGenerator.width;
		height = mapGenerator.height;

		walkableMap = new bool[width, height];
		nodeMap = new Node[width, height];

		for (int x = 0; x < width; x++) {
			Transform rowHolder = new GameObject("Row " + x).transform;
			rowHolder.SetParent(transform);

			for (int y = 0; y < height; y++) {
				Node node = SceneHelper.InstantiateNode(new Vector2(x, y));
				node.Initialize(x, y);
				node.transform.SetParent(rowHolder);
				nodeMap[x, y] = node;

				switch (terrainMap[x, y])
				{
					case TerrainType.ROCKFLOOR:
					case TerrainType.ROCK:
						node.AddFloor(SceneHelper.instance.RockFloorPrefab);
						break;

					default:
						node.AddFloor(SceneHelper.instance.GrassFloorPrefab);
						break;
				}

				switch(terrainMap[x, y]) {
					case TerrainType.ROCK:
						node.AddBlock(SceneHelper.instance.RockPrefab);
						break;

					case TerrainType.TREE:
						node.AddBlock(SceneHelper.instance.TreePrefab);
						break;
				}
			}
		}
	}

	public void NotificateChangeFrom(int x, int y) {
		NotificateChange(x, y + 1);
		NotificateChange(x - 1, y);
		NotificateChange(x, y - 1);
		NotificateChange(x + 1, y);
	}

	void NotificateChange(int x, int y) {
		if(x < 0 || y < 0 || x >= width || y >= height)
			return;

		Node node = nodeMap[x, y];
		GameObject block = node.GetBlock();
		if(!block)
			return;

		AutoTile autoTile = block.GetComponent<AutoTile>() as AutoTile;
		if(autoTile == null)
			return;

		autoTile.UpdateState();
	}
}
