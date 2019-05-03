using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTask : Task {
	private bool stopBefore;
	private Node node;

	public MoveTask(Node node, bool stopBefore = false) : base(TaskType.MOVE) {
		this.stopBefore = stopBefore;
		this.node = node;
		Validate();
	}

	public override TaskStatus Validate() {
		if (stopBefore) {
			status = TaskStatus.READY;
		}
		else if (!node.IsWalkable()) {
			status = TaskStatus.ABORTED;
		}
		else status = TaskStatus.READY;

		return status;
	}

	public override bool Initialize() {
		Vector2 target = new Vector2(node.x, node.y);
		MoveCommandArgs args = new MoveCommandArgs(target, stopBefore);
		Step step = new Step(typeof(MoveCommand), args);
		stepList.Add(step);
		return true;
	}
}
