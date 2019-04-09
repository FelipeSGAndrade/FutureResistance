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
	public TerrainEnum[,] terrainMap;
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

	private bool IsWalkable(TerrainEnum terrain) {
		return terrain != TerrainEnum.ROCK && terrain != TerrainEnum.TREE;
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
				walkableMap[x, y] = IsWalkable(terrainMap[x, y]);

				switch (terrainMap[x, y])
				{
					case TerrainEnum.ROCKFLOOR:
					case TerrainEnum.ROCK:
						node.AddFloor(SceneHelper.instance.RockFloorPrefab);
						break;

					default:
						node.AddFloor(SceneHelper.instance.GrassFloorPrefab);
						break;
				}

				switch(terrainMap[x, y]) {
					case TerrainEnum.ROCK:
						node.AddBlock(SceneHelper.instance.RockPrefab);
						break;

					case TerrainEnum.TREE:
						node.AddBlock(SceneHelper.instance.TreePrefab);
						break;
				}
			}
		}
	}

	public GameObject CreateObject(GameObject prefab, Vector3 position) {
		return CreateObject(prefab, position, null, false);
	}

	public GameObject CreateObject(GameObject prefab, Vector3 position, Sprite sprite) {
		return CreateObject(prefab, position, sprite, false);
	}

	public GameObject CreateObject(GameObject prefab, Vector3 position, Sprite sprite, bool UI) {
		Node node = nodeMap[(int)position.x, (int)position.y];
		if(node.GetBlock() != null)
			return null;

		GameObject newObject = node.AddBlock(prefab);

		if (!UI) {
			walkableMap[(int)position.x, (int)position.y] = !(newObject.layer == LayerMask.NameToLayer("Blocking"));
		}

		return newObject;
	}

	public void DeleteObject(Vector3 position) {
		int x = (int)position.x;
		int y = (int)position.y;
		Node node = nodeMap[x, y];
		if (node.GetBlock() == null)
			return;

		node.RemoveBlock();
		walkableMap[x, y] = true;

		NotificateChangeFrom(x, y);
	}

	public GameObject ReplaceObject(GameObject prefab, Vector3 position) {
		DeleteObject(position);
		return CreateObject(prefab, position);
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
