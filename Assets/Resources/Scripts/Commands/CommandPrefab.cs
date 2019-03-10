using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandPrefab {

	System.Type GetComponentType();
	ICommand Execute();
}

public class CommandPrefab<T> : ICommandPrefab where T : ICommand{

	ICommandArgs args;
	GameObject actingObject;

	public CommandPrefab(GameObject actingObject,ICommandArgs args){
		this.actingObject = actingObject;
		this.args = args;
	}

	public System.Type GetComponentType() {
		return typeof(T);
	}

	public ICommand Execute() {

		ICommand command = actingObject.GetComponent<T>();
		command.Execute(args);
		return command;
	}
}
