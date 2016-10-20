using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private Camera actualCamera;

	// Use this for initialization
	void Start () {
		actualCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

		bool zoomIn = Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus);
		bool zoomOut = Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus);

		bool up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
		bool down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
		bool left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
		bool right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

		if (zoomIn) {
			actualCamera.orthographicSize--;
		} else if (zoomOut) {
			actualCamera.orthographicSize++;
		}

		if(up || down || left || right){
			
			Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

			if (up) {
				newPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
			} else if (down) {
				newPosition = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
			}

			if (left) {
				newPosition = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
			} else if (right) {
				newPosition = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
			}

			transform.position = newPosition;
		}
	}
}
