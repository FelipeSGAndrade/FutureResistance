using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickSystem : MonoBehaviour {

	static TickSystem instance;
	static TickSystem Instance { get { return instance; } }

	public void Awake() {
		instance = this;
	}

	private List<Action> onTickActions = new List<Action>();
	private List<Action> removeList = new List<Action>();
	private List<Action> subscribeList = new List<Action>();

	public static void Subscribe(Action subscriberAction) {
		Instance.subscribeList.Add(subscriberAction);
	}

	public static void Unsubscribe(Action subscriberAction) {
		Instance.removeList.Add(subscriberAction);
	}
	
	private float timeToNextTick;
	private float tickFrequency = .2f;

	void Update () {
		timeToNextTick += Time.deltaTime;
		if (timeToNextTick >= tickFrequency) {
			timeToNextTick -= tickFrequency;
			SubscribeActions();
			ExecuteActions();
			UnsubscribeActions();
		}
	}

	void SubscribeActions() {
		subscribeList.ForEach(action => onTickActions.Add(action));
		subscribeList.Clear();
	}

	void ExecuteActions() {
		onTickActions.ForEach(action => action());
	}

	void UnsubscribeActions() {
		removeList.ForEach(action => onTickActions.Remove(action));
		removeList.Clear();
	}
}
