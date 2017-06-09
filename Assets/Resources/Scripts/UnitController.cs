using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {

	public float moveTime = 0.2f;

	private Vector3 destination = Vector3.back;
	private float inverseMoveTime;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private AStar pathFinder;

	private IEnumerator movingCoroutine = null;
	private IEnumerator smoothMovementCoroutine = null;

	private Vector3 mapPosition;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		pathFinder = new AStar();

		mapPosition = transform.position;

		inverseMoveTime = 1f / moveTime;
	}

	void Update() {

		ProcessMovement();
	}

	void ProcessMovement() {

		if (destination == Vector3.back)
			return;

		if (movingCoroutine != null)
			StopCoroutine(movingCoroutine);

		if(pathFinder.FindPathAsync(transform.position, destination))
			destination = Vector3.back;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		MoveUnit();
		spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
	}

	void MoveUnit()
	{
		if (!pathFinder.isDone)
			return;

		List<Vector3> path = pathFinder.GetResult();
		if (path != null) {
			movingCoroutine = MovingCoordinator(path.ToArray());
			StartCoroutine(movingCoroutine);
		}
	}

	private IEnumerator MovingCoordinator(Vector3[] steps)
	{
		for(int i = 0; i < steps.Length; i++){

			Vector3 step = steps[i];

			if (smoothMovementCoroutine != null)
				StopCoroutine(smoothMovementCoroutine);

			smoothMovementCoroutine = SmoothMovement(step);
			yield return StartCoroutine(smoothMovementCoroutine);

			if(MapManager.objectsMap[(int)step.x, (int)step.y] == null) {
				MapManager.objectsMap[(int)mapPosition.x, (int)mapPosition.y] = null;
				MapManager.objectsMap[(int)step.x, (int)step.y] = gameObject;
				mapPosition = step;
			}
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

	public void Build(GameObject blueprint)
	{
		
	}
}
