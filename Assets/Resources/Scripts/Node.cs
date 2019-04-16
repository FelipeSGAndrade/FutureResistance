using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public GameObject buildingPrefab;
    public Color HighlightColor;
    public Color SelectColor;

    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;
    [HideInInspector]
    public bool selected;

    private GameObject block;
    private GameObject floor;
    private SpriteRenderer floorRenderer;
    private SpriteRenderer blockRenderer;
    private Selectable blockSelectable;
    private bool hovered;

    void Start() {
    }

    void Update() {
        if (selected) {
            if (blockRenderer)
                blockRenderer.color = SelectColor;
            else
                floorRenderer.color = SelectColor;
        }
        else if (hovered) {
            if (blockRenderer)
                blockRenderer.color = HighlightColor;
            else
                floorRenderer.color = HighlightColor;
        } else {
            floorRenderer.color = Color.white;

            if (blockRenderer && (blockRenderer.color == HighlightColor || blockRenderer.color == SelectColor))
                blockRenderer.color = Color.white;
        }
    }

    void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (blockSelectable) {
            blockSelectable.Select();
        } else {
            UIController.instance.HideBlockUI();
        }
    }

    void OnMouseEnter() {
        hovered = true;
    }

    void OnMouseExit() {
        hovered = false;
    }

    public void Initialize(int x, int y) {
        this.x = x;
        this.y = y;
        gameObject.name = "Node" + "(" + x + "," + y + ")";
    }

    public bool IsWalkable() {
        if (!block) {
            return true;
        }

        return block.layer != LayerMask.NameToLayer("Blocking");
    }

    public void UpdateWalkable() {
        MapManager.walkableMap[x, y] = IsWalkable();
    }

    public GameObject Build(Buildable building) {
        if (block) {
			Debug.Log("Cant build there");
			return null;
        }

        AddBlock(building.gameObject);
        return block;
    }

    public GameObject AddBlock(GameObject blockPrefab) {
        block = Instantiate(blockPrefab, transform);
        block.name = blockPrefab.name;
        blockRenderer = block.GetComponent<SpriteRenderer>();
        blockSelectable = block.GetComponent<Selectable>();

        UpdateWalkable();
        return block;
    }

    public void RemoveBlock() {
        Destroy(block);
        MapManager.walkableMap[x, y] = true;
        block = null;
        blockRenderer = null;
    }

    public GameObject AddFloor(GameObject floorPrefab) {
        floor = Instantiate(floorPrefab, transform);
        floorRenderer = floor.GetComponent<SpriteRenderer>();

        UpdateWalkable();
        return floor;
    }

    public GameObject ReplaceFloor(GameObject floorPrefab) {
        Destroy(floor);
        return AddFloor(floorPrefab);
    }

    public GameObject GetBlock() {
        return block;
    }

    public GameObject GetFloor() {
        return floor;
    }
}
