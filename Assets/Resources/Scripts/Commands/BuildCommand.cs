using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCommandArgs : ICommandArgs {
	public BluePrint bluePrint;

	public BuildCommandArgs(BluePrint bluePrint) {
		this.bluePrint = bluePrint;
	}
}

public class BuildCommand : MonoBehaviour, ICommand {

	private bool finished = false;

	public bool Initialize(ICommandArgs args) {
		BuildCommandArgs commandArgs = args as BuildCommandArgs;
		if (commandArgs == null)
			throw new UnityException("Wrong type of args");

		BluePrint bluePrint = commandArgs.bluePrint;
		GameObject prefab = bluePrint.finalObjectPrefab;
		MapManager.ReplaceObject(prefab, bluePrint.position);
		GameController.bluePrints.Remove(bluePrint);

		finished = true;

		return true;
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
