using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType {
	WOOD
}

public static class ResourceManager {

	private static Dictionary<ResourceType, int> amounts = new Dictionary<ResourceType, int>();

	public static void AddResource(ResourceType type, int amount) {
		amounts[type] = GetAmount(type) + amount;
	}

	public static bool RemoveResource(ResourceType type, int amount) {
		if (GetAmount(type) < amount) {
			return false;
		}

		amounts[type] = GetAmount(type) - amount;
		return true;
	}

	public static int GetAmount(ResourceType type) {
		if (!amounts.ContainsKey(type)) {
			amounts[type] = 0;
		}

		return amounts[type];
	}
}
