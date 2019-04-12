using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {
	public GameObject selectedCharacter;
	public List<GameObject> characters;
	public int charQuantity;
	public UIController uiController;
	public bool enableTerrainModifier;

	private Camera mainCamera;

	private GameObject cursor;
	private ActionType currentAction = ActionType.NONE;
	private Buildable buildingAction;

	void Start () {
		mainCamera = Camera.main;
		cursor = SceneHelper.InstantiateCursor(GetMousePosition());

		if (enableTerrainModifier)
			uiController.SetState(UIState.MAP_GENERATION);
		else
			StartGame();
	}

	public void StartGame() {
		uiController.SetState(UIState.GAME);

		MapManager.instance.Build();
		InitializeChars();
	}

	void InitializeChars() {
		for (int i = 0; i < charQuantity; i++) {
			CreateCharacter();
		}

		selectedCharacter = characters[0];
		Vector2 charPosition = characters[0].transform.position;
		mainCamera.transform.position = new Vector3(charPosition.x, charPosition.y, mainCamera.transform.position.z);
	}

	void CreateCharacter() {
		Vector2 position = GetStartingPosition();
		GameObject character = SceneHelper.InstantiateChar(position);
		characters.Add(character);
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
	
	void Update() {
		if (Input.GetMouseButtonDown(1)){
			if(currentAction != 0)
				ResetAction();
			else
				MoveCharacter(!isHoldingShift());
		}

		// if (Input.GetKeyDown(KeyCode.E))
		// 	SetAction(ActionType.PLACE, SceneHelper.instance.WallPrefab);
		if (Input.GetKeyDown(KeyCode.X))
			SetAction((int)ActionType.DELETE);

		if(Input.GetMouseButtonDown(0)) {
			if(!EventSystem.current.IsPointerOverGameObject())
				ExecuteAction(isHoldingShift());
		}
	}

	void FixedUpdate() {
		Node node = GetMouseNode();
		if (!node) return;

		float currentZ = cursor.transform.position.z;
		cursor.transform.position = new Vector3(node.x, node.y, currentZ);
	}

	void MoveCharacter(bool clearCommands) {
		Node node = GetMouseNode();

		if(node) {
			TaskManager.AddTask(new MoveTask(node));
		}
	}

	Vector3 GetMousePosition()
	{
		Vector3 position = Input.mousePosition;
		position.z = mainCamera.transform.position.z * -1;
		return mainCamera.ScreenToWorldPoint(position);
	}

	Node GetMouseNode()
	{
		Vector3 position = GetMousePosition();
		position += new Vector3(0.5f, 0.5f, 0);

		if (position.x > 0 && position.x < MapManager.width && position.y > 0 && position.y < MapManager.height) {
			return MapManager.instance.nodeMap[(int)position.x , (int)position.y];
		}

		return null;
	}

	public void SetBuildAction(Buildable building) {
		currentAction = ActionType.BUILD;
		buildingAction = building;
		SpriteRenderer cursorSpriteRenderer = (SpriteRenderer)cursor.GetComponent<SpriteRenderer>();
		cursorSpriteRenderer.sprite = building.GetComponent<SpriteRenderer>().sprite;
	}

	public void SetAction(int actionType) {
		ActionType action = (ActionType)actionType;
		if(action == currentAction) {
			ResetAction();
			return;
		}

		currentAction = action;

		switch(action) {
			case ActionType.CHOP:
			case ActionType.DELETE:
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
		Node node = GetMouseNode();
		if (!node) return;

		bool resetWhenDone = keepAction;

		switch(currentAction) {
			case ActionType.BUILD:
				TaskManager.AddTask(new BuildTask(node, buildingAction));
				break;
			// case ActionType.PLACE:
			// 	node.AddBlock(actionPrefab);
			// 	break;
			case ActionType.DELETE:
				node.RemoveBlock();
				resetWhenDone = false;
				break;
			case ActionType.CHOP:
				TaskManager.AddTask(new ChopTask(node));
				break;
		}

		if (resetWhenDone) ResetAction();
	}
}
