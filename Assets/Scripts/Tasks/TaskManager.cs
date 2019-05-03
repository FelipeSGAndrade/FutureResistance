using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {

	public static TaskManager instance;
	private static List<Task> taskList = new List<Task>();
	private static Queue<Task> readyQueue = new Queue<Task>();

	void Awake() {
		if (instance) {
			Debug.LogError("More than one TaskManager in the scene");
			return;
		}

		instance = this;
	}

	void Start() {
		InvokeRepeating("ValidateTasks", 0, 0.5f);
	}

	void ValidateTasks() {
		for (int i = taskList.Count - 1; i >= 0; i--)
		{
			Task task = taskList[i];
			TaskStatus status = task.Validate();

			if (status == TaskStatus.ABORTED) {
				taskList.Remove(task);
				continue;
			}

			if (status == TaskStatus.READY) {
				readyQueue.Enqueue(task);
				taskList.Remove(task);
			}
		}
	}

	public static Task GetNextTask() {
		if (readyQueue.Count == 0)
			return null;

		Task next = readyQueue.Dequeue();
		return next;
	}

	public static void AddTask(Task task) {
		if (task.Status != TaskStatus.ABORTED)
			taskList.Add(task);
	}

	public static void ReturnTask(Task task) {
		AddTask(task);
	}
}
