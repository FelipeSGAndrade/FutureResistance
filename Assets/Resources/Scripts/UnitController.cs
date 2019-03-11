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

	bool isIdle() {
		return currentAction == null;
	}

	bool hasActions() {
		return actionQueue.Count > 0;
	}

	bool isDone() {
		return currentAction.IsFinished();
	}

	bool bluePrintsAvailable() {
		return GameController.bluePrints.Count > 0;
	}

	void Update() {

		if (isIdle() && hasActions()) {
			currentAction = actionQueue.Dequeue();	
		}

		if (!isIdle() && isDone()) {
			currentAction = null;
		}

		if (!isIdle()) {
			currentAction.Update();
		}

		if (isIdle() && !hasActions() && bluePrintsAvailable()) {
			BluePrint bluePrint = GameController.bluePrints.Find(nextBluePrint => !nextBluePrint.taken);
			SetBuildAction(bluePrint, true);
		}
	}
	
	public void SetMovement(Vector3 destination, bool clearActions)
	{
		if (clearActions) CleanAllActions();

		Action newAction = new Action(gameObject, ActionEnum.MOVE, new MoveCommandArgs(destination, moveTime));
		actionQueue.Enqueue(newAction);
	}

	public void SetBuildAction(BluePrint bluePrint, bool clearActions)
	{
		if (clearActions) CleanAllActions();

		bluePrint.taken = true;
		Action newAction = new Action(gameObject, ActionEnum.BUILD, new BuildCommandArgs(bluePrint, moveTime));
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
