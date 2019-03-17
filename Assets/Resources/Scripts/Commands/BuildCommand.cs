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
	private bool successful = false;
	private int buildNeededTicks = 25;
	private int buildTicks;
	BluePrint bluePrint;
	Vector2 position;
	SpriteRenderer spriteRenderer;

	public bool Initialize(ICommandArgs args) {
		BuildCommandArgs commandArgs = args as BuildCommandArgs;
		if (commandArgs == null) {
			successful = false;
			finished = true;
			throw new UnityException("Wrong type of args");
		}

		bluePrint = commandArgs.bluePrint;
		position = bluePrint.position;
		spriteRenderer = bluePrint.GetComponent<SpriteRenderer>();

		TickSystem.Subscribe(Build);

		return true;
	}

	void UpdateSprite() {
		if (!spriteRenderer) return;

		float newAlpha = (1f / buildNeededTicks) * buildTicks;
		spriteRenderer.color = new Color(1, 1, 1, newAlpha);

		Debug.Log("Build " + buildTicks + " alpha " + newAlpha);
	}

	void Build() {
		if (finished) return;

		buildTicks++;
		UpdateSprite();
		if (buildTicks >= buildNeededTicks) {
			FinishBuilding();
			TickSystem.Unsubscribe(Build);
		}
	}

	void FinishBuilding() {
		MapManager.ReplaceObject(bluePrint.finalObjectPrefab, bluePrint.position);

		finished = true;
		successful = true;
	}

	public void Stop() {
		finished = true;
	}

	public bool isFinished() {
		return finished;
	}

	public bool isSuccessful() {
		return successful;
	}
}
