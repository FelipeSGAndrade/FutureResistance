using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantTask : Task {
	private Seed seed;
	private Node node;

	public PlantTask(Node node, Seed seed) : base(TaskType.PLANT) {
		this.node = node;
		this.seed = seed;

		Validate();
	}

	public override TaskStatus Validate() {
		status = TaskStatus.READY;
		return status;
	}

	public override bool Initialize() {
		Validate();

		if (status != TaskStatus.READY) {
			return false;
		}

		AddMoveCommand();
		AddPlantCommand();

		return true;
	}

	private void AddMoveCommand() {
		MoveCommandArgs args = new MoveCommandArgs(new Vector2(node.x, node.y), true);
		Step step = new Step(typeof(MoveCommand), args);
		stepList.Add(step);
	}

	private void AddPlantCommand() {
		PlantCommandArgs args = new PlantCommandArgs(node, seed);
		Step step = new Step(typeof(PlantCommand), args);
		stepList.Add(step);
	}
}
