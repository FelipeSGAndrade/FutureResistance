using UnityEngine;
using System.Collections;

public class AssetsHolder : MonoBehaviour{

	public GameObject CharacterPrefab;
	public GameObject GrassFloorPrefab;
	public GameObject RockFloorPrefab;
	public GameObject RockPrefab;
	public GameObject TreePrefab;
	public GameObject Cursor;

	public GameObject InstantiateFloor(TerrainEnum type, Vector2 position)
	{
		GameObject floor = null;

		switch (type)
		{
			case TerrainEnum.ROCKFLOOR:
			case TerrainEnum.ROCK:
				floor = InstantiationHelper (RockFloorPrefab, position);
				break;

			default:
				floor = InstantiationHelper (GrassFloorPrefab, position);
				break;
		}

		return floor;
	}

	public GameObject InstantiateObject(TerrainEnum type, Vector2 position)
	{
		GameObject obj = null;

		switch (type)
		{
			case TerrainEnum.ROCK:
				obj = InstantiationHelper(RockPrefab, position);
				break;

			case TerrainEnum.TREE:
				obj = InstantiationHelper(TreePrefab, position);
				break;
		}

		return obj;
	}

	public GameObject InstantiateChar(Vector2 position)
	{
		GameObject newGameObject = (GameObject)Instantiate(CharacterPrefab, position, Quaternion.identity);
		return newGameObject;
	}

	public GameObject InstantiateCursor(Vector3 mousePosition)
	{
		return (GameObject)Instantiate(Cursor, mousePosition, Quaternion.identity);
	}

	public GameObject InstantiationHelper(GameObject prefab, Vector2 position)
	{
		GameObject newGameObject = (GameObject)Instantiate(prefab, position, Quaternion.identity);

		SpriteRenderer sr = newGameObject.GetComponent<SpriteRenderer>();
		sr.sortingOrder = Mathf.RoundToInt(position.y) * -1;

		newGameObject.name = prefab.name + "(" + position.x + ", " + position.y + ")";

		return newGameObject;
	}
}
