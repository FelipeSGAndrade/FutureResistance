using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
	public int width = 100;
	public int height = 100;
	public float scale = 20f;
	private float initialOffsetX;
	private float initialOffsetY;
	public float offsetX;
	public float offsetY;
	public int octaves = 1;
	public float persistance = 0.5f;
	public float lacunarity = 1;
    public TerrainEnum[,] terrainMap;

	private bool built;
	private new SpriteRenderer renderer;

	void Start() {
		terrainMap = new TerrainEnum[width, height];

		initialOffsetX = Random.Range(0f, 999999f);
		initialOffsetY = Random.Range(0f, 999999f);

		offsetX = initialOffsetX;
		offsetY = initialOffsetY;

		renderer = GetComponent<SpriteRenderer>();
		renderer.transform.localScale = new Vector3(width, height, 1);
	}

	void Update() {
		if (!built) {
			Texture2D texture = Generate();
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
			renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0f, 0f));
		}
	}

	public TerrainEnum[,] Finish() {
		built = true;
		renderer.enabled = false;
		return terrainMap;
	}

	Texture2D Generate() {
		Texture2D texture = new Texture2D(width, height);
		Vector2 offset = new Vector2(offsetX, offsetY);

		float[,] noiseMap = Noise.CalculateNoise(width, height, scale, octaves, persistance, lacunarity, offset);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Vector2 position = new Vector2(x, y);

				TerrainEnum terrain = GetTerrainFromNoise(noiseMap[x, y]);
				terrainMap[x, y] = terrain;

				switch(terrain) {
					case TerrainEnum.ROCK:
						texture.SetPixel(x, y, Color.black);
						break;
					case TerrainEnum.ROCKFLOOR:
						texture.SetPixel(x, y, new Color(0.64f, 0.16f, 0.16f));
						break;
					case TerrainEnum.GRASS:
						texture.SetPixel(x, y, Color.green);
						break;
					case TerrainEnum.TREE:
						texture.SetPixel(x, y, new Color(0, 0.5f, 0));
						break;
					case TerrainEnum.WATER:
						texture.SetPixel(x, y, new Color(0, 0, 0.5f));
						break;
				}
			}
		}

		CreateResources();
		texture.Apply();
		return texture;
	}

	TerrainEnum GetTerrainFromNoise(float noise) {
		if (noise > 0.6) 
			return TerrainEnum.ROCK;

		if (noise > 0.55) 
			return TerrainEnum.ROCKFLOOR;

		if (noise < 0.1)
			return TerrainEnum.WATER;

		return TerrainEnum.GRASS;
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
						}
						break;
				}
			}
		}
	}

	TerrainEnum GetTerrainAtPosition(int x, int y) {
		if (x < 0 || y < 0 || x >= width || y >= height)
			return TerrainEnum.NONE;

		return terrainMap[x, y];
	}

	public void ChangeOffsetX(Single amount) {
		offsetX = initialOffsetX + amount;
	}

	public void ChangeOffsetY(Single amount) {
		offsetY = initialOffsetY + amount;
	}

	public void ChangeScale(Single amount) {
		scale = amount;
	}

	public void ChangeOctaves(Single amount) {
		octaves = (int)amount;
	}

	public void ChangeLacunarity(Single amount) {
		lacunarity = amount;
	}

	public void ChangePersistance(Single amount) {
		persistance = amount;
	}
}
