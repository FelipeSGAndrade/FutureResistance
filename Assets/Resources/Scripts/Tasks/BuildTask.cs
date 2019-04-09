using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTask : Task {
	public BuildTask(Node node, GameObject prefabToBuild) {
		type = TaskType.BUILD;

		if (!CreateBuildBlueprint(node, prefabToBuild)) {
			aborted = true;
			return;
		}

		AddMoveCommand(node);
		AddBuildCommand(node);
	}

	private bool CreateBuildBlueprint(Node node, GameObject prefabToBuild) {
		BluePrint bluePrint = node.AddBluePrint(prefabToBuild);
		if (!bluePrint) return false;

		return true;
	}

	private void AddMoveCommand(Node node) {
		MoveCommandArgs args = new MoveCommandArgs(new Vector2(node.x, node.y), true);
		Step step = new Step(typeof(MoveCommand), args);
		stepList.Add(step);
	}

	private void AddBuildCommand(Node node) {
		BuildCommandArgs args = new BuildCommandArgs(node);
		Step step = new Step(typeof(BuildCommand), args);
		stepList.Add(step);
	}
}
