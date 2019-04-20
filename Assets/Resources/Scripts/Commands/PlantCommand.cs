using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCommandArgs : ICommandArgs {
	public Node node;

	public PlantCommandArgs(Node node) {
		this.node = node;
	}
}

public class PlantCommand : MonoBehaviour, ICommand {
	private bool finished = false;
	private bool successful = false;
	private int plantNeededTicks = 3;
	private int plantTicks;
	private Cultivable cultivable;

	public bool Initialize(ICommandArgs args) {
		PlantCommandArgs commandArgs = args as PlantCommandArgs;
		if (commandArgs == null) {
			Abort();
			throw new UnityException("Wrong type of args");
		}

		cultivable = commandArgs.node.GetBlock().GetComponent<Cultivable>();
		if (!cultivable) {
			Abort();
			Debug.LogError("Tried to plant a non cultivable object");
			return false;
		}

		TickSystem.Subscribe(Plant);

		return true;
	}

	void Plant() {
		if (finished) return;

		plantTicks++;
		if (plantTicks >= plantNeededTicks) {
			TickSystem.Unsubscribe(Plant);
			FinishPlanting();
		}
	}

	void FinishPlanting() {
		cultivable.Plant();

		finished = true;
		successful = true;
	}

	void Abort() {
		successful = false;
		finished = true;
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
