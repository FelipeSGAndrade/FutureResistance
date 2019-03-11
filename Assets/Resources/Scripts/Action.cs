using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

	private ICommand currentCommand = null;
	private Queue<ICommandPrefab> commandQueue = new Queue<ICommandPrefab>();
	private GameObject actingObject;
	private ICommandArgs args;
	private bool finished = false;

	public Action(GameObject actingObject, ActionEnum action, ICommandArgs args) {

		this.actingObject = actingObject;
		this.args = args;

		switch (action) {
			case ActionEnum.MOVE:
				CommandPrefab<MoveCommand> newCommand = new CommandPrefab<MoveCommand>(actingObject, args);
				commandQueue.Enqueue((ICommandPrefab)newCommand);
				break;

			case ActionEnum.BUILD:
				BuildCommandArgs buildArgs = args as BuildCommandArgs;
				if (buildArgs == null)
					throw new UnityException("Wrong type of args");

				Vector3 position = buildArgs.bluePrint.position;
				MoveCommandArgs moveArgs = new MoveCommandArgs(position, buildArgs.moveTime, true);

				CommandPrefab<MoveCommand> moveCommand = new CommandPrefab<MoveCommand>(actingObject, moveArgs);
				commandQueue.Enqueue((ICommandPrefab)moveCommand);

				CommandPrefab<BuildCommand> buildCommand = new CommandPrefab<BuildCommand>(actingObject, buildArgs);
				commandQueue.Enqueue((ICommandPrefab)buildCommand);
				break;
		}
	}

	public void Update() {
		if (currentCommand == null && commandQueue.Count > 0) {
			ICommandPrefab commandPrefab = commandQueue.Dequeue();
			actingObject.AddComponent(commandPrefab.GetComponentType());
			currentCommand = commandPrefab.Execute();
		}

		if (currentCommand != null && currentCommand.isFinished()) {
			if (!currentCommand.isSuccessful()){
				Abort();
			}

			CleanCommand();
		}

		if (currentCommand == null && commandQueue.Count == 0) {
			finished = true;
		}
	}

	public void Abort() {
		CleanAllCommands();

		BuildCommandArgs buildArgs = args as BuildCommandArgs;
		if (buildArgs != null) {
			buildArgs.bluePrint.taken = false;
		}
	}

	public bool IsFinished() {
		return finished;
	}

	private void CleanCommand() {
		UnityEngine.Object.Destroy((MonoBehaviour)currentCommand);
		currentCommand = null;
	}

	public void CleanAllCommands()
	{
		commandQueue.Clear();
		if (currentCommand != null) {
			currentCommand.Stop();
		}
	}
}
