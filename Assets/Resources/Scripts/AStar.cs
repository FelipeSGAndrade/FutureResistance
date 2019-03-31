using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class AStar {

	private bool done = false;
	private Object done_lock = new Object();

	private bool working = false;
	private Object working_lock = new Object();

	private List<Vector3> path = null;
	private Object path_lock = new Object();

	private bool cancel = false;
	private Object cancel_lock = new Object();

	private Thread thread;

	private Vector3 origin;
	private Vector3 destination;
	private bool stopBefore;

	public bool isDone {
		get {
			lock (done_lock) {
				return done;
			}
		}
	}

	public void CancelProcess(){

		lock (cancel_lock) {
			lock (working_lock) {
				if (working) {
					cancel = true;
				}
			}
		}
	}

	public List<Vector3> GetResult(){

		lock (path_lock) {
			lock (done_lock) {
				done = false;
			}

			return path;
		}
	}

	public bool FindPathAsync(Vector3 origin, Vector3 destination, bool stopBefore = false)
	{
		lock (working_lock) {
			if (working) {
				CancelProcess();
				return false;
			}
		}

		this.origin = origin;
		this.destination = destination;
		this.stopBefore = stopBefore;

		thread = new Thread(FindPath);
		thread.Start();

		return true;
	}

	private void FindPath()
	{
		lock (working_lock) {
			working = true;
		}

		if (!MapManager.walkableMap[(int)destination.x, (int)destination.y]) {
			if (stopBefore) {
				if (!WalkableNeighbors()) {
					Finish(null);
					return;
				}
			}
			else {
				Finish(null);
				return;
			}
		}

		bool pathFound = false;
		Node[,] grid = CreateGrid();
		List<Node> closedList = new List<Node>();
		List<Node> openList = new List<Node>();

		Node start = grid[(int)origin.x, (int)origin.y];
		Node current = start;
		openList.Add(current);

		while (openList.Count > 0) {

			lock (cancel_lock) {
				if (cancel) {
					Finish(null);
					return;
				}
			}

			current = openList[0];

			openList.Remove(current);
			closedList.Add(current);

			if (current.x == destination.x && current.y == destination.y) {
				pathFound = true;
				break;
			}

			Node[] neighbors = GetNeighbors(grid, current);

			for (int i = 0; i < neighbors.Length; i++) {
				Node neighbor = neighbors[i];
				float cost = 1;

				if (neighbor.x != current.x && neighbor.y != current.y)
					cost = Mathf.Infinity;

				if ((closedList.Contains(neighbor) || !MapManager.walkableMap[neighbor.x, neighbor.y])
					&& (!stopBefore || neighbor.x != destination.x || neighbor.y != destination.y))
					continue;
				else if (openList.Contains(neighbor)) {
					if (neighbor.f <= (neighbor.h + current.g + cost))
						continue;

					neighbor.g = current.g + cost;
					neighbor.f = neighbor.h + neighbor.g;
					neighbor.parent = current;

					openList.Remove(neighbor);						
				} else {
					neighbor.g = current.g + cost;
					neighbor.h = (int)(Mathf.Abs(destination.x - neighbor.x) + Mathf.Abs(destination.y - neighbor.y));
					neighbor.f = neighbor.g + neighbor.h;
					neighbor.parent = current;
				}

				AddToOpenList(neighbor, openList);
			}
		}

		if (!pathFound) {
			Debug.Log("Path not Found");
			Finish(null);
			return;
		}

		List<Vector3> steps = new List<Vector3>();
		do {
			steps.Insert(0, new Vector3(current.x, current.y, 0));

			if(current.parent != null)
				current = current.parent;
		} while (current != start);

		Finish(steps);
	}

	private Node[,] CreateGrid()
	{
		Node[,] grid = new Node[MapManager.height, MapManager.width];

		for (int x = 0; x < MapManager.height; x++) {
			for (int y = 0; y < MapManager.width; y++) {
				grid[x, y] = new Node(x, y);
			}
		}

		return grid;
	}

	private Node[] GetNeighbors(Node[,] grid, Node current)
	{
		List<Node> neighbors = new List<Node>();

		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <= 1; j++) {
				int x = current.x + i;
				int y = current.y + j;

				if ((x == current.x && y == current.y)
					|| x < 0 || y < 0
					|| x >= MapManager.width || y >= MapManager.height
				)
					continue;

				neighbors.Add(grid[x, y]);
			}
		}

		return neighbors.ToArray();
	}

	private void AddToOpenList(Node neighbor, List<Node> openList)
	{
		for (int i = 0; i <= openList.Count; i++) {
			if (i == openList.Count || openList[i].f >= neighbor.f) {
				openList.Insert(i, neighbor);
				break;
			}
		}
	}

	private bool WalkableNeighbors() {
		int x = (int)destination.x;
		int y = (int)destination.y;
		bool[,] walkableMap = MapManager.walkableMap;

		return walkableMap[x - 1, y]
			|| walkableMap[x - 1, y - 1]
			|| walkableMap[x, y - 1]
			|| walkableMap[x + 1, y - 1]
			|| walkableMap[x + 1, y]
			|| walkableMap[x + 1, y + 1]
			|| walkableMap[x, y + 1]
			|| walkableMap[x - 1, y + 1];
	}

	private void Finish(List<Vector3> path)
	{
		lock (path_lock) {
			lock (done_lock) {
				lock (working_lock) {
					this.path = path;
					working = false;
					done = true;
					cancel = false;
				}
			}
		}
	}
}
