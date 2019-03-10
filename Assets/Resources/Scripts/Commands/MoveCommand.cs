using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommandArgs : ICommandArgs {
	public Vector3 destination;
	public float moveTime;

	public MoveCommandArgs(Vector3 destination, float moveTime) {
		this.destination = destination;
		this.moveTime = moveTime;
	}
}

public class MoveCommand : MonoBehaviour, ICommand {

	private Vector3 destination = Vector3.back;
	private float inverseMoveTime;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private AStar pathFinder;
	private bool execute = false;
	private bool finished = false;

	private IEnumerator movingCoroutine = null;
	private IEnumerator smoothMovementCoroutine = null;

	private Vector3 mapPosition;

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();

		mapPosition = transform.position;

		pathFinder = new AStar();
	}

	public void Execute(ICommandArgs args) {

		MoveCommandArgs commandArgs = args as MoveCommandArgs;
		if (commandArgs == null)
			throw new UnityException("Wrong type of args");
		
		this.destination = commandArgs.destination;
		inverseMoveTime = 1f / commandArgs.moveTime;

		execute = true;
	}

	public void Stop() {
		if (movingCoroutine != null)
			StopCoroutine(movingCoroutine);

		finished = true;
	}

	public bool isFinished() {
		return finished;
	}

	void Update() {

		if (execute) ProcessMovement();
	}

	void ProcessMovement() {
		
		if (movingCoroutine != null)
			StopCoroutine(movingCoroutine);

		if (pathFinder.FindPathAsync(transform.position, destination))
			execute = false;
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (pathFinder.isDone) {
			List<Vector3> path = pathFinder.GetResult();
			if (path != null) {
				movingCoroutine = MovingCoordinator(path.ToArray());
				StartCoroutine(movingCoroutine);
			}
		}

		spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
	}

	private IEnumerator MovingCoordinator(Vector3[] steps)
	{
		for(int i = 0; i < steps.Length; i++){

			Vector3 step = steps[i];

			if (smoothMovementCoroutine != null)
				StopCoroutine(smoothMovementCoroutine);

			if (MapManager.objectsMap[(int)step.x, (int)step.y] == null) {
				MapManager.objectsMap[(int)mapPosition.x, (int)mapPosition.y] = null;
				MapManager.objectsMap[(int)step.x, (int)step.y] = gameObject;
				mapPosition = step;
			} else {
				Stop();
				yield break;
			}

			smoothMovementCoroutine = SmoothMovement(step);
			yield return StartCoroutine(smoothMovementCoroutine);
		}

		finished = true;
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
}
