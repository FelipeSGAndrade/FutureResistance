using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUnit : MonoBehaviour {

	private BlockStateEnum currentState = BlockStateEnum.DEFAULT;
	private bool updatable = true;
	private SpriteRenderer parentsRenderer;

	[NonSerialized]
	public Sprite defaultSprite;

	public Sprite upSprite;
	public Sprite leftSprite;
	public Sprite downSprite;
	public Sprite rightSprite;
	public Sprite upLeftSprite;
	public Sprite upRightSprite;
	public Sprite downLeftSprite;
	public Sprite downRightSprite;
	public Sprite verticalCenterSprite;
	public Sprite horizontalCenterSprite;
	public Sprite upCenterSprite;
	public Sprite leftCenterSprite;
	public Sprite downCenterSprite;
	public Sprite rightCenterSprite;
	public Sprite centerSprite;

	// Use this for initialization
	void Start () {

		parentsRenderer = (SpriteRenderer)gameObject.GetComponent<SpriteRenderer>();

		if(!parentsRenderer) {
			updatable = false;
			return;
		}

		defaultSprite = parentsRenderer.sprite;

		UpdateState();
	}

	public GameObject getNeighbor(int x, int y) {
		if (x < 0 || y < 0 || x >= MapManager.width || y >= MapManager.height) {
			GameObject border = new GameObject();
			border.tag = tag;
			return border;
		}

		return MapManager.objectsMap[x, y];
	}

	public void UpdateState() {

		if(!parentsRenderer || !updatable)
			return;

		int x = (int)transform.position.x;
		int y = (int)transform.position.y;

		GameObject neighbor = getNeighbor(x, y + 1);				
		bool up = (neighbor != null && neighbor.CompareTag(gameObject.tag));

		neighbor = getNeighbor(x - 1, y);
		bool left = (neighbor != null && neighbor.CompareTag(gameObject.tag));

		neighbor = getNeighbor(x, y - 1);
		bool down = (neighbor != null && neighbor.CompareTag(gameObject.tag));

		neighbor = getNeighbor(x + 1, y);
		bool right = (neighbor != null && neighbor.CompareTag(gameObject.tag));

		BlockStateEnum newState = BlockStateEnum.DEFAULT;

		if(up && left && down && right)
			newState = BlockStateEnum.CENTER;
		//----
		else if(left && down && right)
			newState = BlockStateEnum.UP_CENTER;
		else if(up && down && right)
			newState = BlockStateEnum.LEFT_CENTER;
		else if(up && left && right)
			newState = BlockStateEnum.DOWN_CENTER;
		else if(up && left && down)
			newState = BlockStateEnum.RIGHT_CENTER;
		//----
		else if(up && down)
			newState = BlockStateEnum.VERTICAL_CENTER;
		else if(left && right)
			newState = BlockStateEnum.HORIZONTAL_CENTER;
		//----
		else if(down && right)
			newState = BlockStateEnum.UP_LEFT;
		else if(down && left)
			newState = BlockStateEnum.UP_RIGHT;
		else if(up && right)
			newState = BlockStateEnum.DOWN_LEFT;
		else if(up && left)
			newState = BlockStateEnum.DOWN_RIGHT;
		//----
		else if(down)
			newState = BlockStateEnum.UP;
		else if(right)
			newState = BlockStateEnum.LEFT;
		else if(up)
			newState = BlockStateEnum.DOWN;
		else if(left)
			newState = BlockStateEnum.RIGHT;
		else
			newState = BlockStateEnum.DEFAULT;

		if(newState == currentState)
			return;

		switch(newState) {
			case BlockStateEnum.UP:
				parentsRenderer.sprite = upSprite;
				break;
			case BlockStateEnum.LEFT:
				parentsRenderer.sprite = leftSprite;
				break;
			case BlockStateEnum.DOWN:
				parentsRenderer.sprite = downSprite;
				break;
			case BlockStateEnum.RIGHT:
				parentsRenderer.sprite = rightSprite;
				break;
				//--
			case BlockStateEnum.UP_LEFT:
				parentsRenderer.sprite = upLeftSprite;
				break;
			case BlockStateEnum.UP_RIGHT:
				parentsRenderer.sprite = upRightSprite;
				break;
			case BlockStateEnum.DOWN_LEFT:
				parentsRenderer.sprite = downLeftSprite;
				break;
			case BlockStateEnum.DOWN_RIGHT:
				parentsRenderer.sprite = downRightSprite;
				break;
				//--
			case BlockStateEnum.VERTICAL_CENTER:
				parentsRenderer.sprite = verticalCenterSprite;
				break;
			case BlockStateEnum.HORIZONTAL_CENTER:
				parentsRenderer.sprite = horizontalCenterSprite;
				break;
				//--
			case BlockStateEnum.UP_CENTER:
				parentsRenderer.sprite = upCenterSprite;
				break;
			case BlockStateEnum.LEFT_CENTER:
				parentsRenderer.sprite = leftCenterSprite;
				break;
			case BlockStateEnum.DOWN_CENTER:
				parentsRenderer.sprite = downCenterSprite;
				break;
			case BlockStateEnum.RIGHT_CENTER:
				parentsRenderer.sprite = rightCenterSprite;
				break;
				//--
			case BlockStateEnum.CENTER:
				parentsRenderer.sprite = centerSprite;
				break;
			default:
				parentsRenderer.sprite = defaultSprite;
				break;
		}

		currentState = newState;
		MapManager.NotificateChangeFrom(x, y);
	}
}
