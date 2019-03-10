using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {

	public float moveTime = 0.2f;

	private Vector3 mapPosition;

	private Action currentAction = null;
	private Queue<Action> actionQueue = new Queue<Action>();

	// Use this for initialization
	void Start () {

		mapPosition = transform.position;
	}

	void Update() {

		if (currentAction == null && actionQueue.Count > 0) {
			currentAction = actionQueue.Dequeue();	
		}

		if (currentAction != null && currentAction.IsFinished()) {
			currentAction = null;
		}

		if (currentAction != null) {
			currentAction.Update();
		}

		if (currentAction == null && actionQueue.Count == 0 && GameController.buildings.Count > 0) {
			object[] building = GameController.buildings.Dequeue();
			SetBuildAction((Vector3)building[1], true);
		}
	}
	
	public void SetMovement(Vector3 destination, bool clearActions)
	{
		if (clearActions) CleanAllActions();

		Action newAction = new Action(gameObject, ActionEnum.MOVE, new MoveCommandArgs(destination, moveTime));
		actionQueue.Enqueue(newAction);
	}

	public void SetBuildAction(Vector3 position, bool clearActions)
	{
		if (clearActions) CleanAllActions();

		Action newAction = new Action(gameObject, ActionEnum.BUILD, new BuildCommandArgs(position, moveTime));
		actionQueue.Enqueue(newAction);
	}

	private void CleanAllActions()
	{
		actionQueue.Clear();
		if (currentAction != null) {
			currentAction.CleanAllCommands();
		}
	}
}
