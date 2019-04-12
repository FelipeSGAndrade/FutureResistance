using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommandArgs : ICommandArgs {
	public Vector2 destination;
	public float moveTime = 0.2f;

	public bool stopBefore;

	public MoveCommandArgs(Vector2 destination, bool stopBefore = false) {
		this.destination = destination;
		this.stopBefore = stopBefore;
	}
}

public class MoveCommand : MonoBehaviour, ICommand {

	private Vector2 destination = Vector3.back;
	private bool stopBefore;
	private float inverseMoveTime;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private AStar pathFinder;
	private bool execute = false;
	private bool finished = false;
	private bool successful = false;

	private IEnumerator movingCoroutine = null;
	private IEnumerator smoothMovementCoroutine = null;

	public bool Initialize(ICommandArgs args) {
		MoveCommandArgs commandArgs = args as MoveCommandArgs;
		if (commandArgs == null)
			throw new UnityException("Wrong type of args");

		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		pathFinder = new AStar();

		this.destination = commandArgs.destination;
		this.stopBefore = commandArgs.stopBefore;
		inverseMoveTime = 1f / commandArgs.moveTime;

		execute = true;

		return true;
	}

	public void Stop() {
		if (movingCoroutine != null)
			StopCoroutine(movingCoroutine);

		finished = true;
		successful = false;
	}

	public bool isFinished() {
		return finished;
	}

	public bool isSuccessful() {
		return successful;
	}

	void Update() {
		if (execute)
			ProcessMovement();
	}

	void ProcessMovement() {
		if (movingCoroutine != null)
			StopCoroutine(movingCoroutine);

		if (pathFinder.FindPathAsync(transform.position, destination, stopBefore))
			execute = false;
	}

	void FixedUpdate () {
		if (pathFinder.isDone) {
			List<Vector3> path = pathFinder.GetResult();
			if (path != null) {
				movingCoroutine = MovingCoordinator(path.ToArray());
				StartCoroutine(movingCoroutine);
			} else {
				Stop();
				return;
			}
		}

	}

	private IEnumerator MovingCoordinator(Vector3[] steps) {
		int lastStep = steps.Length;
		if (stopBefore) lastStep--;

		for (int i = 0; i < lastStep; i++) {

			Vector3 step = steps[i];

			if (smoothMovementCoroutine != null)
				StopCoroutine(smoothMovementCoroutine);

			if (!MapManager.walkableMap[(int)step.x, (int)step.y]) {
				Stop();
				yield break;
			}

			smoothMovementCoroutine = SmoothMovement(step);
			yield return StartCoroutine(smoothMovementCoroutine);
		}

		successful = true;
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
