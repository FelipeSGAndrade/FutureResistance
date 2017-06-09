using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private Camera currentCamera;

	private float minX;
	private float maxX;
	private float minY;
	private float maxY;
	private float minOrtographicSize;
	private float maxOrtographicSize;
	private float screenProportion;

	// Use this for initialization
	void Start () {
		currentCamera = GetComponent<Camera>();

		screenProportion = (float)Screen.width / (float)Screen.height;
		maxOrtographicSize = (float)MapManager.width / (2f * screenProportion);
		minOrtographicSize = 1;

		CalculateCameraBorders();
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
			currentCamera.orthographicSize = Mathf.Clamp(currentCamera.orthographicSize - 1, minOrtographicSize, maxOrtographicSize);
			CalculateCameraBorders();
		} else if (zoomOut) {
			currentCamera.orthographicSize = Mathf.Clamp(currentCamera.orthographicSize + 1, minOrtographicSize, maxOrtographicSize);
			CalculateCameraBorders();
		}

		if(up || down || left || right){
			
			Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

			if (up) {
				newPosition.y = Mathf.Clamp(newPosition.y + 1, minY, maxY);
			} else if (down) {
				newPosition.y = Mathf.Clamp(newPosition.y - 1, minY, maxY);
			}

			if (left) {
				newPosition.x = Mathf.Clamp(newPosition.x - 1, minX, maxX);
			} else if (right) {
				newPosition.x = Mathf.Clamp(newPosition.x + 1, minX, maxX);
			}

			transform.position = newPosition;
		}
	}

	private void CalculateCameraBorders(){

		float vertExtent = currentCamera.orthographicSize;
		float horzExtent = vertExtent * screenProportion;

		minX = horzExtent - 0.5f;
		maxX = MapManager.width - horzExtent + 0.5f;
		minY = vertExtent - 0.5f;
		maxY = MapManager.height - vertExtent + 0.5f;

		Vector3 newPosition;
		newPosition.x = Mathf.Clamp(transform.position.x, minX, maxX);
		newPosition.y = Mathf.Clamp(transform.position.y, minY, maxY);
		newPosition.z = transform.position.z;

		transform.position = newPosition;
	}
}
