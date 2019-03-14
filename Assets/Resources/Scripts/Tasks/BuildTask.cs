using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTask : Task {

	GameObject bluePrintObject;

	public BuildTask(Vector2 targetPosition, Sprite buildingSprite) {
		type = TaskType.BUILD;
		BluePrint bluePrint = CreateBuildBlueprint(targetPosition, buildingSprite);

		AddMoveCommand(targetPosition);
		AddBuildCommand(bluePrint);
	}

	private BluePrint CreateBuildBlueprint(Vector2 targetPosition, Sprite buildingSprite) {
		bluePrintObject = MapManager.CreateObject((GameObject)Resources.Load("Prefabs/Blueprint"), targetPosition, buildingSprite);

		BluePrint bluePrint = bluePrintObject.GetComponent<BluePrint>();
		bluePrint.finalObjectPrefab = (GameObject)Resources.Load("Prefabs/Wall");
		bluePrint.position = targetPosition;
		return bluePrint;
	}

	private void AddMoveCommand(Vector2 targetPosition) {
		MoveCommandArgs args = new MoveCommandArgs(targetPosition, true);
		Step step = new Step(typeof(MoveCommand), args);
		stepList.Add(step);
	}

	private void AddBuildCommand(BluePrint bluePrint) {
		BuildCommandArgs args = new BuildCommandArgs(bluePrint);
		Step step = new Step(typeof(BuildCommand), args);
		stepList.Add(step);
	}
}
