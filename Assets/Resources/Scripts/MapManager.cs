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

	public static GameObject[,] objectsMap;
	public GameObject[,] floorTiles;

	[NonSerialized]
	public Transform mapHolder;
	[NonSerialized]
	public AssetsHolder SceneHelper;

	public void InitializeMap(AssetsHolder sceneHelper)
	{
		SceneHelper = sceneHelper;
		InitialiseList();
		Generate();
		Build();
	}

	void InitialiseList()
	{
		terrainMap = new TerrainEnum[width, height];
		walkableMap = new bool[width, height];
		objectsMap = new GameObject[width, height];
		floorTiles = new GameObject[width, height];
	}

	void Generate()
	{
		TerrainEnum lastTile = TerrainEnum.NONE;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				TerrainEnum[] chances = GetChances(x, y, lastTile);
				terrainMap[x, y] = chances[Random.Range(0, chances.Length)];
				lastTile = terrainMap[x, y];
			}
		}

		PostGeneration();
		PostGeneration();

		CreateResources();
	}

	private bool Walkable(TerrainEnum terrain)
	{
		return terrain != TerrainEnum.ROCK && terrain != TerrainEnum.TREE;
	}

	void PostGeneration() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				TerrainEnum t1 = GetTerrainAtPosition(x, y + 1);
				TerrainEnum t2 = GetTerrainAtPosition(x, y - 1);
				TerrainEnum t3 = GetTerrainAtPosition(x + 1, y);
				TerrainEnum t4 = GetTerrainAtPosition(x - 1, y);

				if ((t1 == TerrainEnum.GRASS || t1 == TerrainEnum.NONE) &&
					(t2 == TerrainEnum.GRASS || t2 == TerrainEnum.NONE) &&
					(t3 == TerrainEnum.GRASS || t3 == TerrainEnum.NONE) &&
					(t4 == TerrainEnum.GRASS || t4 == TerrainEnum.NONE))
				{
					terrainMap[x, y] = TerrainEnum.GRASS;
				} 
				else if ((t1 == TerrainEnum.ROCK || t1 == TerrainEnum.ROCKFLOOR || t1 == TerrainEnum.NONE) &&
						(t2 == TerrainEnum.ROCK || t2 == TerrainEnum.ROCKFLOOR || t2 == TerrainEnum.NONE) &&
						(t3 == TerrainEnum.ROCK || t3 == TerrainEnum.ROCKFLOOR || t3 == TerrainEnum.NONE) &&
						(t4 == TerrainEnum.ROCK || t4 == TerrainEnum.ROCKFLOOR || t4 == TerrainEnum.NONE))
				{
					terrainMap[x, y] = TerrainEnum.ROCK;
				} 

				if((terrainMap[x,y] == TerrainEnum.ROCKFLOOR || terrainMap[x,y] == TerrainEnum.ROCK) &&
						t1 != TerrainEnum.ROCK && t2 != TerrainEnum.ROCK && t3 != TerrainEnum.ROCK && t4 != TerrainEnum.ROCK)
				{
					terrainMap[x, y] = TerrainEnum.GRASS;
				}
				else if(terrainMap[x,y] == TerrainEnum.GRASS &&
					(t1 == TerrainEnum.ROCK || t2 == TerrainEnum.ROCK || t3 == TerrainEnum.ROCK || t4 == TerrainEnum.ROCK))
				{
					terrainMap[x, y] = TerrainEnum.ROCKFLOOR;
				}

				if (Walkable(terrainMap[x, y]))
					walkableMap[x, y] = true;
				else
					walkableMap[x, y] = false;
			}
		}
	}

	void CreateResources()
	{
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

	TerrainEnum[] GetChances(int x, int y, TerrainEnum lastTile)
	{
		Dictionary<TerrainEnum, int> tilesCount = new Dictionary<TerrainEnum, int>();

		IncludePosition(x - 1, y - 1, tilesCount);
		IncludePosition(x - 1, y, tilesCount);
		IncludePosition(x - 1, y + 1, tilesCount);
		IncludePosition(x, y + 1, tilesCount);
		IncludePosition(x + 1, y + 1, tilesCount);
		IncludePosition(x + 1, y, tilesCount);
		IncludePosition(x + 1, y - 1, tilesCount);
		IncludePosition(x, y, tilesCount);

		if (lastTile != TerrainEnum.NONE) {
			if (!tilesCount.ContainsKey(lastTile))
				tilesCount.Add(lastTile, 2);
			else
				tilesCount[lastTile] += 2;
		}

		TerrainEnum greaterTile = TerrainEnum.NONE;
		int greaterAmount = 0;

		foreach (TerrainEnum key in tilesCount.Keys) {
			if (key != TerrainEnum.NONE && tilesCount[key] > greaterAmount) {
				greaterAmount = tilesCount[key];
				greaterTile = key;
			}
		}

		return ChancesForGreater(greaterTile);
	}

	void IncludePosition(int x, int y, Dictionary<TerrainEnum, int> tilesCount)
	{
		TerrainEnum tile = GetTerrainAtPosition(x, y);

		if (!tilesCount.ContainsKey(tile)) {
			tilesCount.Add(tile, 1);
		} else {
			tilesCount[tile]++;
		}
	}

	TerrainEnum GetTerrainAtPosition(int x, int y)
	{
		TerrainEnum terrain;
		if (x < 0 || y < 0 || x >= width || y >= height) {
			terrain = TerrainEnum.NONE;
		} else {
			terrain = terrainMap[x, y];
		}

		return terrain;
	}

	TerrainEnum[] ChancesForGreater(TerrainEnum tile) 
	{
		int max = 100;
		List<TerrainEnum> chances = new List<TerrainEnum>();
		int actual = 0;

		switch (tile) {
			case TerrainEnum.NONE:
			case TerrainEnum.GRASS:
				InsertChances(TerrainEnum.GRASS, 90, actual, max, chances);
				actual = 90;
				InsertChances(TerrainEnum.ROCKFLOOR, 10, actual, max, chances);
//				actual = 10;
//				InsertChances(TerrainEnum.ROCK, 10, actual, max, chances);
				break;

			case TerrainEnum.ROCKFLOOR:
				InsertChances(TerrainEnum.GRASS, 30, actual, max, chances);
				actual = 30;
				InsertChances(TerrainEnum.ROCK, 70, actual, max, chances);
				break;

			case TerrainEnum.ROCK:
//				InsertChances(TerrainEnum.GRASS, 10, actual, max, chances);
//				actual = 10;
				InsertChances(TerrainEnum.ROCKFLOOR, 30, actual, max, chances);
				actual = 30;
				InsertChances(TerrainEnum.ROCK, 60, actual, max, chances);
				break;
		}

		return chances.ToArray();
	}

	void InsertChances(TerrainEnum tile, int amount, int actual, int max, List<TerrainEnum> chances)
	{
		int i = actual;
		int end = actual + amount;
		while (i < end && i < max) {
			chances.Add(tile);
			i++;
		}
	}

	void Build()
	{
		mapHolder = new GameObject("Map").transform;

		Transform rowHolder;

		for (int x = 0; x < width; x++) {
			rowHolder = new GameObject("Row " + x).transform;
			rowHolder.SetParent(mapHolder);

			for (int y = 0; y < height; y++) {
				GameObject floor = SceneHelper.InstantiateFloor(terrainMap[x, y], new Vector2(x, y));
				floor.transform.SetParent(rowHolder);
				floorTiles[x, y] = floor;

				GameObject block = SceneHelper.InstantiateObject(terrainMap[x, y], new Vector2(x, y));
				if (block != null) {
					block.transform.SetParent(rowHolder);
				}

				objectsMap[x, y] = block;
			}
		}
	}

	public GameObject CreateObject(GameObject prefab, Vector3 position) {

		return CreateObject(prefab, position, null);
	}

	public GameObject CreateObject(GameObject prefab, Vector3 position, Sprite sprite) {

		if(objectsMap[(int)position.x, (int)position.y] != null)
			return null;

		GameObject newObject = SceneHelper.InstantiationHelper(prefab, (Vector2)position);
		newObject.transform.SetParent(GameObject.Find("Row " + position.x).transform);

		if(sprite != null) {
			SpriteRenderer renderer = (SpriteRenderer)newObject.GetComponent<SpriteRenderer>();
			renderer.sprite = sprite;	
		}

		objectsMap[(int)position.x, (int)position.y] = newObject;
		walkableMap[(int)position.x, (int)position.y] = !(newObject.layer == LayerMask.NameToLayer("Blocking"));

		return newObject;
	}

	public void DeleteObject(Vector3 position) {

		int x = (int)position.x;
		int y = (int)position.y;
		if (objectsMap [x, y] == null)
			return;

		GameObject removingObj = objectsMap[x, y];
		objectsMap[x, y] = null;
		UnityEngine.Object.Destroy(removingObj);

		NotificateChangeFrom(x, y);
	}

	public static void NotificateChangeFrom(int x, int y){

		NotificateChange(x, y + 1);
		NotificateChange(x - 1, y);
		NotificateChange(x, y - 1);
		NotificateChange(x + 1, y);
	}

	static void NotificateChange(int x, int y){

		if(x < 0 || y < 0 || x >= width || y >= height || objectsMap[x, y] == null)
			return;

		BlockUnit controller = objectsMap[x, y].GetComponent<BlockUnit>() as BlockUnit;
		if(controller == null)
			return;

		controller.UpdateState();
	}
}
