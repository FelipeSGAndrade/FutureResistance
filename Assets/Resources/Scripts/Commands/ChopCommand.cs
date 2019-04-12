using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopCommandArgs : ICommandArgs {

	public Node node;
	public ChopCommandArgs(Node node) {
		this.node = node;
	}
}

public class ChopCommand : MonoBehaviour, ICommand {

	private bool finished = false;
	private bool successful = false;
	private int neededTicks = 5;
	private int ticks;
	Node node;
	SpriteRenderer spriteRenderer;

	public bool Initialize(ICommandArgs args) {
		ChopCommandArgs commandArgs = args as ChopCommandArgs;
		if (commandArgs == null) {
			successful = false;
			finished = true;
			throw new UnityException("Wrong type of args");
		}

		node = commandArgs.node;

		GameObject tree = node.GetBlock();
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
		node.RemoveBlock();
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
