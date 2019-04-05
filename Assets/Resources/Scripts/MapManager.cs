using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MapManager
{
	public static int width = 100;
	public static int height = 100;
	public static TerrainEnum[,] terrainMap;
	public static bool[,] walkableMap;

	public static Node[,] nodeMap;

	private Transform mapHolder;

	private float scale = 5f;

	public void InitializeMap() {
		InitialiseList();
		Generate();
		Build();
	}

	void InitialiseList() {
		terrainMap = new TerrainEnum[width, height];
		walkableMap = new bool[width, height];
		nodeMap = new Node[width, height];
	}

	void Generate() {
		float offsetX = Random.Range(0f, 999999f);
		float offsetY = Random.Range(0f, 999999f);
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				terrainMap[x, y] = CalculateTerrain(x, y, offsetX, offsetY);
				walkableMap[x, y] = Walkable(terrainMap[x, y]);
			}
		}

		CreateResources();
	}

	TerrainEnum CalculateTerrain(int x, int y, float offsetX, float offsetY) {
		float xCoord = (float)x / width * scale + offsetX;
		float yCoord = (float)y / height * scale + offsetY;

		float sample = Mathf.PerlinNoise(xCoord, yCoord);
		if (sample > 0.6) {
			return TerrainEnum.ROCK;
		}

		if (sample > 0.55) {
			return TerrainEnum.ROCKFLOOR;
		}

		return TerrainEnum.GRASS;
	}

	private bool Walkable(TerrainEnum terrain) {
		return terrain != TerrainEnum.ROCK && terrain != TerrainEnum.TREE;
	}

	void CreateResources() {
		float treeChance = 10;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				switch (terrainMap[x, y]) {
					case TerrainEnum.GRASS:

						TerrainEnum t1 = GetTerrainAtPosition(x + 1, y);
						TerrainEnum t2 = GetTerrainAtPosition(x - 1, y);

						if (t1 == TerrainEnum.TREE || t2 == TerrainEnum.TREE)
							continue;

						if (Random.Range(0, 100) < treeChance) {
							terrainMap[x, y] = TerrainEnum.TREE;
							walkableMap[x, y] = false;
						}
						break;
				}
			}
		}
	}

	TerrainEnum GetTerrainAtPosition(int x, int y) {
		TerrainEnum terrain;
		if (x < 0 || y < 0 || x >= width || y >= height) {
			terrain = TerrainEnum.NONE;
		} else {
			terrain = terrainMap[x, y];
		}

		return terrain;
	}


	void Build() {
		mapHolder = new GameObject("Map").transform;

		Transform rowHolder;

		for (int x = 0; x < width; x++) {
			rowHolder = new GameObject("Row " + x).transform;
			rowHolder.SetParent(mapHolder);

			for (int y = 0; y < height; y++) {
				Node node = SceneHelper.InstantiateNode(new Vector2(x, y));
				node.transform.SetParent(rowHolder);
				nodeMap[x, y] = node;

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

	public static GameObject CreateObject(GameObject prefab, Vector3 position) {
		return CreateObject(prefab, position, null, false);
	}

	public static GameObject CreateObject(GameObject prefab, Vector3 position, Sprite sprite) {
		return CreateObject(prefab, position, sprite, false);
	}

	public static GameObject CreateObject(GameObject prefab, Vector3 position, Sprite sprite, bool UI) {
		Node node = nodeMap[(int)position.x, (int)position.y];
		if(node.GetBlock() != null)
			return null;

		GameObject newObject = node.AddBlock(prefab);

		if (!UI) {
			walkableMap[(int)position.x, (int)position.y] = !(newObject.layer == LayerMask.NameToLayer("Blocking"));
		}

		return newObject;
	}

	public static void DeleteObject(Vector3 position) {
		int x = (int)position.x;
		int y = (int)position.y;
		Node node = nodeMap[x, y];
		if (node.GetBlock() == null)
			return;

		node.RemoveBlock();
		walkableMap[x, y] = true;

		NotificateChangeFrom(x, y);
	}

	public static GameObject ReplaceObject(GameObject prefab, Vector3 position) {
		DeleteObject(position);
		return CreateObject(prefab, position);
	}

	public static void NotificateChangeFrom(int x, int y) {
		NotificateChange(x, y + 1);
		NotificateChange(x - 1, y);
		NotificateChange(x, y - 1);
		NotificateChange(x + 1, y);
	}

	static void NotificateChange(int x, int y) {
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
