using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCommandArgs : ICommandArgs {
	public BluePrint bluePrint;
	public float moveTime;

	public BuildCommandArgs(BluePrint bluePrint, float moveTime) {
		this.bluePrint = bluePrint;
		this.moveTime = moveTime;
	}
}

public class BuildCommand : MonoBehaviour, ICommand {

	private Vector3 position = Vector3.back;
	private bool finished = false;

	private Vector3 mapPosition;

	void Start() {
		mapPosition = transform.position;
	}

	public void Execute(ICommandArgs args) {
		
		BuildCommandArgs commandArgs = args as BuildCommandArgs;
		if (commandArgs == null)
			throw new UnityException("Wrong type of args");

		BluePrint bluePrint = commandArgs.bluePrint;

		GameObject prefab = bluePrint.finalObjectPrefab;
		MapManager.ReplaceObject(prefab, bluePrint.position);
		GameController.bluePrints.Remove(bluePrint);

		finished = true;
	}

	public void Stop() {
		finished = true;
	}

	public bool isFinished() {
		return finished;
	}

	public bool isSuccessful() {
		return true;
	}
}
