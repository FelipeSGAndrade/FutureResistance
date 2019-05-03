using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {
	public static GameController instance;
	public GameObject selectedCharacter;
	public List<GameObject> characters;
	public int charQuantity;
	public bool enableTerrainModifier;
	public Camera mainCamera;
	public Sprite cancelSprite;

	private GameObject cursor;
	private ActionType currentAction = ActionType.NONE;
	private Buildable buildingAction;
	private Node dragBegin;
	private List<Node> selectedNodes = new List<Node>();

	void Awake() {
		if (instance != null) {
			Debug.LogError("There is more than one Game Controller in Scene");
			return;
		}

		instance = this;
	}

	void Start () {
		cursor = SceneHelper.InstantiateCursor(GetMousePosition());

		if (enableTerrainModifier)
			UIController.instance.SetState(UIState.MAP_GENERATION);
		else
			StartGame();
	}

	public void StartGame() {
		UIController.instance.SetState(UIState.GAME);

		MapManager.instance.Build();
		InitializeChars();
	}

	void InitializeChars() {
		for (int i = 0; i < charQuantity; i++) {
			CreateCharacter();
		}

		selectedCharacter = characters[0];

		CameraController cameraController = mainCamera.GetComponent<CameraController>();
		cameraController.Center(selectedCharacter.transform.position);
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

		if (Input.GetKeyDown(KeyCode.X))
			SetAction(ActionType.DELETE);

		if (Input.GetMouseButtonDown(0)) {
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			dragBegin = GetMouseNode();
		}

		if (dragBegin) {
			Node currentMouseNode = GetMouseNode();

			UnselectNodes();
			SelectNodesInside(dragBegin.x, dragBegin.y, currentMouseNode.x, currentMouseNode.y);
		}

		if (Input.GetMouseButtonUp(0)) {
			dragBegin = null;
			ExecuteAction();
			UnselectNodes();
		}
	}

	void SelectNodesInside(int x1, int y1, int x2, int y2) {
		int greaterX = Mathf.Max(x1, x2);
		int lesserX = Mathf.Min(x1, x2);
		int greaterY = Mathf.Max(y1, y2);
		int lesserY = Mathf.Min(y1, y2);

		for (int x = lesserX; x <= greaterX; x++) {
			for (int y = lesserY; y <= greaterY; y++) {
				SelectNode(MapManager.instance.nodeMap[x, y]);
			}
		}
	}
	void SelectNode(Node node) {
		node.selected = true;
		selectedNodes.Add(node);
	}

	void UnselectNodes() {
		selectedNodes.ForEach((node) => node.selected = false);
		selectedNodes.Clear();
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

	public void SetAction(ActionType action) {
		if(action == currentAction) {
			ResetAction();
			return;
		}

		currentAction = action;

		switch(action) {
			case ActionType.CHOP:
			case ActionType.DELETE:
				((SpriteRenderer)cursor.GetComponent<SpriteRenderer> ()).sprite = cancelSprite;
				break;
		}
	}

	void ResetAction() {
		currentAction = 0;
		((SpriteRenderer)cursor.GetComponent<SpriteRenderer>()).sprite = null;
	}

	void ExecuteAction() {
		foreach (Node node in selectedNodes) {
			switch(currentAction) {
				case ActionType.BUILD:
					TaskManager.AddTask(new BuildTask(node, buildingAction));
					break;
				case ActionType.DELETE:
					node.RemoveBlock();
					break;
				case ActionType.CHOP:
					TaskManager.AddTask(new ChopTask(node));
					break;
			}
		}
	}
}
