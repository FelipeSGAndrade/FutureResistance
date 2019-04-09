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
	private int buildNeededTicks = 5;
	private int buildTicks;
	private BluePrint bluePrint;

	public bool Initialize(ICommandArgs args) {
		BuildCommandArgs commandArgs = args as BuildCommandArgs;
		if (commandArgs == null) {
			successful = false;
			finished = true;
			throw new UnityException("Wrong type of args");
		}

		bluePrint = commandArgs.node.GetBluePrint();
		TickSystem.Subscribe(Build);

		return true;
	}

	void Build() {
		if (finished) return;

		buildTicks++;
		bluePrint.BuildTick(buildTicks, buildNeededTicks);
		if (buildTicks >= buildNeededTicks) {
			TickSystem.Unsubscribe(Build);
			FinishBuilding();
		}
	}

	void FinishBuilding() {
		bluePrint.FinishBuilding();

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
