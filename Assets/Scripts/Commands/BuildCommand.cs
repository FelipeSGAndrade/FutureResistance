using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCommandArgs : ICommandArgs {
	public Node node;

	public BuildCommandArgs(Node node) {
		this.node = node;
	}
}

public class BuildCommand : MonoBehaviour, ICommand {

	private bool finished = false;
	private bool successful = false;
	private int buildTicks;
	private Buildable buildable;

	public bool Initialize(ICommandArgs args) {
		BuildCommandArgs commandArgs = args as BuildCommandArgs;
		if (commandArgs == null) {
			Abort();
			throw new UnityException("Wrong type of args");
		}

		buildable = commandArgs.node.GetBlock().GetComponent<Buildable>();
		if (!buildable) {
			Abort();
			Debug.LogError("Tried to build a non buildable object");
			return false;
		}

		TickSystem.Subscribe(Build);

		return true;
	}

	void Build() {
		if (finished) return;

		buildTicks++;
		buildable.BuildTick(buildTicks);
		if (buildTicks >= buildable.neededTicks) {
			TickSystem.Unsubscribe(Build);
			FinishBuilding();
		}
	}

	void FinishBuilding() {
		buildable.FinishBuilding();

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
