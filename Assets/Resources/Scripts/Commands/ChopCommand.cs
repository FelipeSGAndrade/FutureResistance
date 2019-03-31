using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopCommandArgs : ICommandArgs {

	public Vector2 targetPosition;
	public ChopCommandArgs(Vector2 targetPosition) {
		this.targetPosition = targetPosition;
	}
}

public class ChopCommand : MonoBehaviour, ICommand {

	private bool finished = false;
	private bool successful = false;
	private int neededTicks = 5;
	private int ticks;
	Vector2 position;
	GameObject tree;
	SpriteRenderer spriteRenderer;

	public bool Initialize(ICommandArgs args) {
		ChopCommandArgs commandArgs = args as ChopCommandArgs;
		if (commandArgs == null) {
			successful = false;
			finished = true;
			throw new UnityException("Wrong type of args");
		}

		position = commandArgs.targetPosition;
		tree = MapManager.objectsMap[(int)position.x, (int)position.y];
		if (!tree) {
			Stop();
			Debug.Log("No tree to chop");
			return false;
		}

		spriteRenderer = tree.GetComponent<SpriteRenderer>();
		TickSystem.Subscribe(Chop);

		return true;
	}

	void UpdateSprite() {
		if (!spriteRenderer) return;

		float newAlpha = (1f / ticks);
		spriteRenderer.color = new Color(1, 1, 1, newAlpha);
	}

	void Chop() {
		if (finished) return;

		ticks++;
		UpdateSprite();
		if (ticks >= neededTicks) {
			TickSystem.Unsubscribe(Chop);
			Finish();
		}
	}

	void Finish() {
		MapManager.DeleteObject(tree.transform.position);
		ResourceManager.AddResource(ResourceType.WOOD, 5);

		finished = true;
		successful = true;
	}

	public void Stop() {
		finished = true;
		successful = false;
	}

	public bool isFinished() {
		return finished;
	}

	public bool isSuccessful() {
		return successful;
	}
}
