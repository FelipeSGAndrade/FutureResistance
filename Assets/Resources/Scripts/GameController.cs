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

	public static List<BluePrint> bluePrints = new List<BluePrint>();

	// Use this for initialization
	void Start () {
		SceneHelper = GetComponent<AssetsHolder>();
		mainCamera = Camera.main;

		mapManager = new MapManager();
		mapManager.InitializeMap(SceneHelper);

		cursor = SceneHelper.InstantiateCursor(GetMousePosition());

		Vector2 firstCharPosition = GetStartingPosition();
		UnitController firstChar = SceneHelper.InstantiateChar(firstCharPosition).GetComponent<UnitController>();

		Vector2 secondCharPosition = GetStartingPosition();
		UnitController secondChar = SceneHelper.InstantiateChar(secondCharPosition).GetComponent<UnitController>();

		characters.Add(firstChar);
		characters.Add(secondChar);
		selectedCharacter = firstChar;

		mainCamera.transform.position = new Vector3(firstCharPosition.x, firstCharPosition.y, mainCamera.transform.position.z);
	}

	private Vector2 GetStartingPosition() {
		Vector2 position = new Vector2(0, 0);

		int maxTries = 100;
		for (int i = 0; i < maxTries; i++) {
			position = new Vector2(Random.Range(1, MapManager.width), Random.Range(1, MapManager.height));

			if (MapManager.walkableMap[(int)position.x, (int)position.y]) {
				return position;
			}
		}

		return position;
	}

	bool isHoldingShift() {
		return Input.GetKey(KeyCode.LeftShift);
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButtonDown(1)){
			if(currentAction != 0)
				ResetAction();
			else
				MoveCharacter(!isHoldingShift());
		}

		if (Input.GetKeyDown(KeyCode.Q))
			SetAction(1);
		if (Input.GetKeyDown(KeyCode.E))
			SetAction(2);
		if (Input.GetKeyDown(KeyCode.X))
			SetAction(3);

		if(Input.GetMouseButtonDown(0))
			ExecuteAction(isHoldingShift());
	}

	void FixedUpdate() {
		cursor.transform.position = GetMousePosition();
	}

	void MoveCharacter(bool clearCommands) {
		Vector3 position = GetGridMousePosition();

		if(position != Vector3.back) {
			TaskManager.AddTask(new MoveTask(position));
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

	void SetAction(int action) {
		if(action == currentAction) {
			ResetAction();
			return;
		}

		currentAction = action;

		switch(action) {
			case 1:
			case 2:
				GameObject wallPefab = (GameObject)Resources.Load ("Prefabs/Wall");
				((SpriteRenderer)cursor.GetComponent<SpriteRenderer> ()).sprite = ((SpriteRenderer)wallPefab.GetComponent<SpriteRenderer> ()).sprite;
				break;

			case 3:
				Sprite cancelSprite = (Sprite)(Resources.LoadAll ("Sprites/Z18-TileA5"))[10];
				((SpriteRenderer)cursor.GetComponent<SpriteRenderer> ()).sprite = cancelSprite;
				break;
		}
	}

	void ResetAction() {
		currentAction = 0;
		((SpriteRenderer)cursor.GetComponent<SpriteRenderer>()).sprite = null;
	}

	void ExecuteAction(bool keepAction) {
		Vector3 position = GetGridMousePosition();
		bool resetWhenDone = keepAction;
		switch(currentAction) {
			case 1:
				if(position != Vector3.back) {
					Sprite buildingSprite = ((SpriteRenderer)cursor.GetComponent<SpriteRenderer>()).sprite;
					TaskManager.AddTask(new BuildTask(position, buildingSprite));
				}
				break;
			case 2:
				if(position != Vector3.back) {
					MapManager.CreateObject((GameObject)Resources.Load("Prefabs/Wall"), position);
				}
				break;
			case 3:
				if (position != Vector3.back) {
					MapManager.DeleteObject(position);
					resetWhenDone = false;
				}
				break;
		}

		if (resetWhenDone) ResetAction();
	}
}
