using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {

	private Vector3 mapPosition;

	private Task currentTask = null;

	// Use this for initialization
	void Start () {
		mapPosition = transform.position;
	}

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
