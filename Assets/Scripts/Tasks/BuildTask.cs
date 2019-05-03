using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTask : Task {
	private Buildable building;
	private Node node;

	public BuildTask(Node node, Buildable building) : base(TaskType.BUILD) {
		this.building = building;
		this.node = node;

		Validate();

		if (!CreateBuildBlueprint(node, building)) {
			status = TaskStatus.ABORTED;
			return;
		}
	}

	public override TaskStatus Validate() {
		for (int i = 0; i < building.resources.Length; i ++) {
			if (ResourceManager.GetAmount(building.resources[i]) < building.costs[i]) {
				status = TaskStatus.WAITING;
				return status;
			}
		}

		status = TaskStatus.READY;
		return status;
	}

	public override bool Initialize() {
		Validate();

		if (status != TaskStatus.READY) {
			return false;
		}

		SpendResources();
		AddMoveCommand();
		AddBuildCommand();

		return true;
	}

	public void SpendResources() {
		for (int i = 0; i < building.resources.Length; i ++) {
			ResourceManager.RemoveResource(building.resources[i], building.costs[i]);
		}
	}

	private bool CreateBuildBlueprint(Node node, Buildable building) {
		GameObject buildingBlock = node.Build(building);
		if (!buildingBlock) return false;

		return true;
	}

	private void AddMoveCommand() {
		MoveCommandArgs args = new MoveCommandArgs(new Vector2(node.x, node.y), true);
		Step step = new Step(typeof(MoveCommand), args);
		stepList.Add(step);
	}

	private void AddBuildCommand() {
		BuildCommandArgs args = new BuildCommandArgs(node);
		Step step = new Step(typeof(BuildCommand), args);
		stepList.Add(step);
	}
}
