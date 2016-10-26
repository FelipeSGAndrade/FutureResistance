using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public UnitController selectedCharacter;
	public List<UnitController> characters;

	private AssetsHolder SceneHelper;
	private MapManager mapManager;
	private Camera mainCamera;

	// Use this for initialization
	void Start () {
		SceneHelper = GetComponent<AssetsHolder>();
		mainCamera = Camera.main;

		mapManager = new MapManager();
		mapManager.InitializeMap(SceneHelper);

		Vector2 charPosition;

		do {
			charPosition = new Vector2(Random.Range(1, MapManager.width), Random.Range(1, MapManager.height));
		} while(MapManager.map[(int)charPosition.x, (int)charPosition.y] == TerrainEnum.ROCK || MapManager.map[(int)charPosition.x, (int)charPosition.y] == TerrainEnum.TREE);

		selectedCharacter = SceneHelper.InstantiateChar(charPosition).GetComponent<UnitController>();
		characters.Add(selectedCharacter);

		mainCamera.transform.position = new Vector3(charPosition.x, charPosition.y, mainCamera.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse1))
			MoveCharacter();
	}

	void MoveCharacter()
	{
		Vector3 position = GetMousePosition();
		position += new Vector3(0.5f, 0.5f, 0);

		if (position.x > 0 && position.x < MapManager.width && position.y > 0 && position.y < MapManager.height) {
			selectedCharacter.SetMovement(new Vector3((int)position.x, (int)position.y, 0));
		}
	}

	Vector3 GetMousePosition()
	{
		Vector3 position = Input.mousePosition;
		position.z = mainCamera.transform.position.z * -1;
		return mainCamera.ScreenToWorldPoint(position);
	}
}
