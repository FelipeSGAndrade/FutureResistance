using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopTask : Task {

	private ChopTask(Vector2 targetPosition) {
		GameObject tree = MapManager.instance.nodeMap[(int)targetPosition.x, (int)targetPosition.y].GetBlock();
		if (!tree) {
			return;
		}

		type = TaskType.CHOP;
		AddMoveCommand(targetPosition);
		AddChopCommand(targetPosition);
	}

	public static ChopTask Create(Vector2 targetPosition) {
		GameObject tree = MapManager.instance.nodeMap[(int)targetPosition.x, (int)targetPosition.y].GetBlock();
		if (!tree || !tree.name.Contains("Tree")) {
			return null;
		}

		return new ChopTask(targetPosition);
	}

	private void AddMoveCommand(Vector2 targetPosition) {
		MoveCommandArgs args = new MoveCommandArgs(targetPosition, true);
		Step step = new Step(typeof(MoveCommand), args);
		stepList.Add(step);
	}

	private void AddChopCommand(Vector2 targetPosition) {
		ChopCommandArgs args = new ChopCommandArgs(targetPosition);
		Step step = new Step(typeof(ChopCommand), args);
		stepList.Add(step);
	}
}
