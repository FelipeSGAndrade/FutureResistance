using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {

	private Task currentTask = null;

	bool IsIdle() {
		return currentTask == null;
	}

	void Update() {
		if (!IsIdle() && currentTask.IsDone()) {
			TaskManager.FinishTask(currentTask);
			currentTask = null;
		}

		if (!IsIdle()) {
			bool taskOk = currentTask.Update(gameObject);

			if (!taskOk) {
				Abort();
			}
		}

		if (IsIdle()) {
			currentTask = TaskManager.GetNextTask();
		}
	}

	private void Abort() {
		TaskManager.ReturnTask(currentTask);
		currentTask = null;
	}
}
