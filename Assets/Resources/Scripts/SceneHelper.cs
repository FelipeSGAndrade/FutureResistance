using UnityEngine;
using System.Collections;

public class SceneHelper : MonoBehaviour{

	public static SceneHelper instance;
	public GameObject CharacterPrefab;
	public GameObject NodePrefab;
	public GameObject GrassFloorPrefab;
	public GameObject RockFloorPrefab;
	public GameObject RockPrefab;
	public GameObject TreePrefab;
	public GameObject WallPrefab;
	public GameObject Cursor;

	void Awake() {
		if (instance != null) {
			Debug.LogError("There is more than one Scene Helper in Scene");
			return;
		}

		instance = this;
	}

	public static GameObject InstantiateChar(Vector2 position)
	{
		GameObject newGameObject = (GameObject)Instantiate(instance.CharacterPrefab, position, Quaternion.identity);
		return newGameObject;
	}

	public static GameObject InstantiateCursor(Vector3 mousePosition)
	{
		return (GameObject)Instantiate(instance.Cursor, mousePosition, Quaternion.identity);
	}

	public static Node InstantiateNode(Vector2 position) {
		GameObject newGameObject = (GameObject)Instantiate(instance.NodePrefab, position, Quaternion.identity);
		return newGameObject.GetComponent<Node>();
	}
}
