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
    public TerrainType[,] terrainMap;

	private bool built;
	private new SpriteRenderer renderer;

	void Start() {
		terrainMap = new TerrainType[width, height];

		initialOffsetX = Random.Range(0f, 999999f);
		initialOffsetY = Random.Range(0f, 999999f);

		offsetX = initialOffsetX;
		offsetY = initialOffsetY;

		renderer = GetComponent<SpriteRenderer>();
		renderer.transform.localScale = new Vector3(width, height, 1);
	}

	void Update() {
		if (!built) {
			Generate();
		}
	}

	public TerrainType[,] Finish() {
		Generate();
		built = true;
		renderer.enabled = false;
		return terrainMap;
	}

	void Generate() {
		Texture2D texture = CreateTexture();
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0f, 0f));
	}

	Texture2D CreateTexture() {
		Texture2D texture = new Texture2D(width, height);
		Vector2 offset = new Vector2(offsetX, offsetY);

		float[,] noiseMap = Noise.CalculateNoise(width, height, scale, octaves, persistance, lacunarity, offset);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Vector2 position = new Vector2(x, y);

				TerrainType terrain = GetTerrainFromNoise(noiseMap[x, y]);
				terrainMap[x, y] = terrain;

				switch(terrain) {
					case TerrainType.ROCK:
						texture.SetPixel(x, y, Color.black);
						break;
					case TerrainType.ROCKFLOOR:
						texture.SetPixel(x, y, new Color(0.64f, 0.16f, 0.16f));
						break;
					case TerrainType.GRASS:
						texture.SetPixel(x, y, Color.green);
						break;
					case TerrainType.TREE:
						texture.SetPixel(x, y, new Color(0, 0.5f, 0));
						break;
					case TerrainType.WATER:
						texture.SetPixel(x, y, new Color(0, 0, 0.5f));
						break;
				}
			}
		}

		CreateResources();
		texture.Apply();
		return texture;
	}

	TerrainType GetTerrainFromNoise(float noise) {
		if (noise > 0.6) 
			return TerrainType.ROCK;

		if (noise > 0.55) 
			return TerrainType.ROCKFLOOR;

		if (noise < 0.1)
			return TerrainType.WATER;

		return TerrainType.GRASS;
	}


	void CreateResources() {
		float treeChance = 10;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				switch (terrainMap[x, y]) {
					case TerrainType.GRASS:

						TerrainType t1 = GetTerrainAtPosition(x + 1, y);
						TerrainType t2 = GetTerrainAtPosition(x - 1, y);

						if (t1 == TerrainType.TREE || t2 == TerrainType.TREE)
							continue;

						if (Random.Range(0, 100) < treeChance) {
							terrainMap[x, y] = TerrainType.TREE;
						}
						break;
				}
			}
		}
	}

	TerrainType GetTerrainAtPosition(int x, int y) {
		if (x < 0 || y < 0 || x >= width || y >= height)
			return TerrainType.NONE;

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
