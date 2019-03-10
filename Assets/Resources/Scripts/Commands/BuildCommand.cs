using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCommandArgs : ICommandArgs {
	public Vector3 position;
	public float moveTime;

	public BuildCommandArgs(Vector3 position, float moveTime) {
		this.position = position;
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

		position = commandArgs.position;

		int x = (int)position.x;
		int y = (int)position.y;

		GameObject bluePrint = MapManager.objectsMap[x, y];
		if (!bluePrint.CompareTag("BluePrint")) {
			finished = true;
			return;
		}

		GameObject prefab = bluePrint.GetComponent<BluePrint>().finalObjectPrefab;
		MapManager.ReplaceObject(prefab, position);

		finished = true;
	}

	public void Stop() {
		finished = true;
	}

	public bool isFinished() {
		return finished;
	}
}
