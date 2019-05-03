using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopTask : Task {
	private Node node;

	public ChopTask(Node node) : base(TaskType.CHOP) {
		this.node = node;
		Validate();
	}

	public override TaskStatus Validate() {
		GameObject tree = node.GetBlock();
		if (!tree || !tree.name.Contains("Tree")) {
			status = TaskStatus.ABORTED;
			return status;
		}

		status = TaskStatus.READY;
		return status;
	}

	public override bool Initialize() {
		AddMoveCommand();
		AddChopCommand();
		return true;
	}

	private void AddMoveCommand() {
		Vector2 targetPosition = new Vector2(node.x, node.y);
		MoveCommandArgs args = new MoveCommandArgs(targetPosition, true);
		Step step = new Step(typeof(MoveCommand), args);
		stepList.Add(step);
	}

	private void AddChopCommand() {
		ChopCommandArgs args = new ChopCommandArgs(node);
		Step step = new Step(typeof(ChopCommand), args);
		stepList.Add(step);
	}
}
