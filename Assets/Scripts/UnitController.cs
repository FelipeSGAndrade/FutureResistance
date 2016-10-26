﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {

	public float moveTime = 0.2f;

	private Vector3 destination = Vector3.back;
	private List<Vector3> waitingPath = null;
	private float inverseMoveTime;
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	private IEnumerator movingCoroutine = null;
	private IEnumerator smoothMovementCoroutine = null;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();

		inverseMoveTime = 1f / moveTime;
	}
	
	void Update() {

		SetUnitMovement();
	}

	private void SetUnitMovement() {

		if (destination == Vector3.back)
			return;
		
		waitingPath = FindPath(destination);
		destination = Vector3.back;
	}

	void FixedUpdate () {

		MoveUnit();
		spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
	}

	void MoveUnit()
	{
		if (waitingPath == null)
			return;

		if (movingCoroutine != null)
			StopCoroutine(movingCoroutine);

		movingCoroutine = MovingCoordinator(waitingPath.ToArray());
		StartCoroutine(movingCoroutine);
		waitingPath = null;
	}

	private List<Vector3> FindPath(Vector3 destination)
	{
		if (!Walkable((int)destination.x, (int)destination.y)) {
			return new List<Vector3> { transform.position };
		}

		bool pathFound = false;
		Node[,] grid = CreateGrid();
		List<Node> closedList = new List<Node>();
		List<Node> openList = new List<Node>();

		Node start = grid[(int)transform.position.x, (int)transform.position.y];
		Node current = start;
		openList.Add(current);

		while (openList.Count > 0) {
			
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
					cost = 1.5f;

				if (closedList.Contains(neighbor) || !Walkable(neighbor))
					continue;
				else if (openList.Contains(neighbor)) {
					if (neighbor.f <= (neighbor.h + current.g + cost))
						continue;
					
					neighbor.g = current.g + cost;
					neighbor.parent = current;

					openList.Remove(neighbor);						
				} else {
					neighbor.g = current.g + cost;
					neighbor.h = (int)(Mathf.Abs(destination.x - neighbor.x) + Mathf.Abs(destination.y - neighbor.y));
					neighbor.parent = current;
				}

				AddToOpenList(neighbor, openList);
			}
		}

		if (!pathFound) {
			Debug.Log("Path not Found");
			return new List<Vector3> { transform.position };
		}

		List<Vector3> path = new List<Vector3>();
		do {
			path.Insert(0, new Vector3(current.x, current.y, 0));

			if(current.parent != null)
				current = current.parent;
		} while (current != start);

		return path;
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

				if ((x == current.x && y == current.y) || x < 0 || y < 0 || x >= MapManager.width || y >= MapManager.height)
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

	private bool Walkable(Node neighbor)
	{
		return Walkable(neighbor.x, neighbor.y);
	}

	private bool Walkable(int x, int y)
	{
		TerrainEnum terrain = MapManager.map[x, y];
		return terrain != TerrainEnum.ROCK && terrain != TerrainEnum.TREE;
	}

	private IEnumerator MovingCoordinator(Vector3[] steps)
	{
		for(int i = 0; i < steps.Length; i++){

			Vector3 step = steps[i];

			if (smoothMovementCoroutine != null)
				StopCoroutine(smoothMovementCoroutine);

			smoothMovementCoroutine = SmoothMovement(step);
			yield return StartCoroutine(smoothMovementCoroutine);
		}
	}

	private IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		animator.SetBool("walking", true);

		if ((transform.position.x - end.x) > 0 && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Left")) {
			animator.SetTrigger("faceLeft");
		} else if ((transform.position.x - end.x) < 0 && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Right")) {
			animator.SetTrigger("faceRight");
		} else if ((transform.position.y - end.y) > 0 && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Front")) {
			animator.SetTrigger("faceFront");
		} else if ((transform.position.y - end.y) < 0 && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Back")) {
			animator.SetTrigger("faceBack");
		}

		yield return null;

		while (sqrRemainingDistance > float.Epsilon) {
			
			Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
			transform.position = newPosition;

			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			yield return null;
		}

		animator.SetBool("walking", false);
	}

	public void SetMovement(Vector3 destination)
	{
		if (this.destination == Vector3.back)
			this.destination = destination;
	}
}
