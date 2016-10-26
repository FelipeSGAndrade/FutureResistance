using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MapManager
{
	public static int width = 100;
	public static int height = 100;
	public static TerrainEnum[,] map;

	public GameObject[,] mapObjects;
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
		map = new TerrainEnum[width, height];
		mapObjects = new GameObject[width, height];
		floorTiles = new GameObject[width, height];
	}

	void Generate()
	{
		TerrainEnum lastTile = TerrainEnum.NONE;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				TerrainEnum[] chances = GetChances(x, y, lastTile);
				map[x, y] = chances[Random.Range(0, chances.Length)];
				lastTile = map[x, y];
			}
		}

		PostGeneration();
		PostGeneration();

		CreateResources();
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
					map[x, y] = TerrainEnum.GRASS;
				} 
				else if ((t1 == TerrainEnum.ROCK || t1 == TerrainEnum.ROCKFLOOR || t1 == TerrainEnum.NONE) &&
						(t2 == TerrainEnum.ROCK || t2 == TerrainEnum.ROCKFLOOR || t2 == TerrainEnum.NONE) &&
						(t3 == TerrainEnum.ROCK || t3 == TerrainEnum.ROCKFLOOR || t3 == TerrainEnum.NONE) &&
						(t4 == TerrainEnum.ROCK || t4 == TerrainEnum.ROCKFLOOR || t4 == TerrainEnum.NONE))
				{
					map[x, y] = TerrainEnum.ROCK;
				} 

				if((map[x,y] == TerrainEnum.ROCKFLOOR || map[x,y] == TerrainEnum.ROCK) &&
						t1 != TerrainEnum.ROCK && t2 != TerrainEnum.ROCK && t3 != TerrainEnum.ROCK && t4 != TerrainEnum.ROCK)
				{
					map[x, y] = TerrainEnum.GRASS;
				}
				else if(map[x,y] == TerrainEnum.GRASS &&
					(t1 == TerrainEnum.ROCK || t2 == TerrainEnum.ROCK || t3 == TerrainEnum.ROCK || t4 == TerrainEnum.ROCK))
				{
					map[x, y] = TerrainEnum.ROCKFLOOR;
				}
			}
		}
	}

	void CreateResources()
	{
		float treeChance = 10;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				switch (map[x, y]) {
					case TerrainEnum.GRASS:

						TerrainEnum t1 = GetTerrainAtPosition(x + 1, y);
						TerrainEnum t2 = GetTerrainAtPosition(x - 1, y);

						if (t1 == TerrainEnum.TREE || t2 == TerrainEnum.TREE)
							continue;

						if (Random.Range(0, 100) < treeChance) {
							map[x, y] = TerrainEnum.TREE;
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
			terrain = map[x, y];
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
				GameObject floor = SceneHelper.InstantiateFloor(map[x, y], new Vector2(x, y));
				floor.transform.SetParent(rowHolder);
				floorTiles[x, y] = floor;

				GameObject block = SceneHelper.InstantiateObject(map[x, y], new Vector2(x, y));
				if (block != null) {
					block.transform.SetParent(rowHolder);
				}

				mapObjects[x, y] = block;
			}
		}
	}
}
