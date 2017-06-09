using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public UnitController selectedCharacter;
	public List<UnitController> characters;

	private AssetsHolder SceneHelper;
	private MapManager mapManager;
	private Camera mainCamera;

	private GameObject cursor;
	private int currentAction = 0;

	// Use this for initialization
	void Start () {
		SceneHelper = GetComponent<AssetsHolder>();
		mainCamera = Camera.main;

		mapManager = new MapManager();
		mapManager.InitializeMap(SceneHelper);

		cursor = SceneHelper.InstantiateCursor(GetMousePosition());

		Vector2 charPosition;

		do {
			charPosition = new Vector2(Random.Range(1, MapManager.width), Random.Range(1, MapManager.height));
		} while(!MapManager.walkableMap[(int)charPosition.x, (int)charPosition.y]);

		selectedCharacter = SceneHelper.InstantiateChar(charPosition).GetComponent<UnitController>();
		characters.Add(selectedCharacter);

		mainCamera.transform.position = new Vector3(charPosition.x, charPosition.y, mainCamera.transform.position.z);
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Mouse1)){
			if(currentAction != 0)
				ResetAction();
			else
				MoveCharacter();
		}
		if (Input.GetKeyDown(KeyCode.Q))
			SetAction(1);
		if(Input.GetKeyDown(KeyCode.Mouse0))
			ExecuteAction();
	}

	void FixedUpdate() {

		cursor.transform.position = GetMousePosition();
	}

	void MoveCharacter()
	{
		Vector3 position = GetGridMousePosition();

		if(position != Vector3.back) {
			selectedCharacter.SetMovement(position);
		}
	}

	Vector3 GetMousePosition()
	{
		Vector3 position = Input.mousePosition;
		position.z = mainCamera.transform.position.z * -1;
		return mainCamera.ScreenToWorldPoint(position);
	}

	Vector3 GetGridMousePosition()
	{
		Vector3 position = GetMousePosition();
		position += new Vector3(0.5f, 0.5f, 0);

		if (position.x > 0 && position.x < MapManager.width && position.y > 0 && position.y < MapManager.height) {
			return new Vector3((int)position.x, (int)position.y, 0);
		}

		return Vector3.back;
	}

	void SetAction(int action)
	{
		if(action == currentAction) {
			ResetAction();
			return;
		}

		currentAction = action;

		switch(action) {
			case 1:
				
				GameObject wallPefab = (GameObject)Resources.Load("Prefabs/Wall");
				((SpriteRenderer)cursor.GetComponent<SpriteRenderer>()).sprite = ((SpriteRenderer)wallPefab.GetComponent<SpriteRenderer>()).sprite;
				break;
		}
	}

	void ResetAction() {

		currentAction = 0;
		((SpriteRenderer)cursor.GetComponent<SpriteRenderer>()).sprite = null;
	}

	void ExecuteAction() {

		switch(currentAction) {
			case 1:
				Vector3 position = GetGridMousePosition();
				if(position != Vector3.back) {
					Sprite buildingSprite = ((SpriteRenderer)cursor.GetComponent<SpriteRenderer>()).sprite;
					mapManager.CreateObject((GameObject)Resources.Load("Prefabs/Blueprint"), position, buildingSprite);
				}
				break;
		}

		ResetAction();
	}
}
