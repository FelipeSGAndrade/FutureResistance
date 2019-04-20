using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Buildable : MonoBehaviour
{
	public bool blocking;
    public Sprite baseSprite;
    public ResourceType[] resources;
    public int[] costs;
	public int neededTicks;
    public int health;
	[HideInInspector]
    public bool complete;

    private SpriteRenderer spriteRenderer;
	private Node parentNode;
    private bool initialized;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start() {
		parentNode = GetComponentInParent<Node>();
	}

    void Update() {
        if (!initialized) {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }
    }

	public void BuildTick(int currentTicks) {
        initialized = true;
		float newAlpha = (1f / neededTicks) * currentTicks;
		spriteRenderer.color = new Color(1, 1, 1, newAlpha);
	}

	public void FinishBuilding() {
        complete = true;
        spriteRenderer.color = new Color(1, 1, 1, 1);

		if (blocking)
			gameObject.layer = LayerMask.NameToLayer("Blocking");

		parentNode.UpdateWalkable();
	}
}
