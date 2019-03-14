using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTask : Task {

	public MoveTask(Vector2 targetPosition, bool stopBefore = false) {
		type = TaskType.MOVE;

		MoveCommandArgs args = new MoveCommandArgs(targetPosition, stopBefore);
		Step step = new Step(typeof(MoveCommand), args);
		stepList.Add(step);
	}
}
