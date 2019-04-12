using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject buildingPrefab;
    public Color HighlightColor;

    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;

    private GameObject block;
    private GameObject floor;
    private SpriteRenderer floorRenderer;
    private SpriteRenderer blockRenderer;

    void Start() {
    }

    void Update() {
    }

    void OnMouseDown() {
    }

    void OnMouseEnter() {
        if (blockRenderer)
            blockRenderer.color = HighlightColor;
        else
            floorRenderer.color = HighlightColor;
    }

    void OnMouseExit() {
        floorRenderer.color = Color.white;

        if (blockRenderer)
            blockRenderer.color = Color.white;
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
        block = InstantiateChild(blockPrefab, true);
        blockRenderer = block.GetComponent<SpriteRenderer>();

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
        floor = InstantiateChild(floorPrefab, false);
        floorRenderer = floor.GetComponent<SpriteRenderer>();

        MapManager.walkableMap[x, y] = IsWalkable();
        return floor;
    }

    public GameObject ReplaceFloor(GameObject floorPrefab) {
        Destroy(floor);
        return AddFloor(floorPrefab);
    }

    private GameObject InstantiateChild(GameObject prefab, bool sortSprite) {
        GameObject newObject = (GameObject)Instantiate(prefab, new Vector2(x, y), Quaternion.identity);
        newObject.transform.SetParent(transform);

        return newObject;
    }

    public GameObject GetBlock() {
        return block;
    }

    public GameObject GetFloor() {
        return floor;
    }
}
