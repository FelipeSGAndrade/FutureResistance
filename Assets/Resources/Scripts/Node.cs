using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject bluePrintPrefab;
    public Color HighlightColor;

    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;

    private GameObject block;
    private GameObject floor;
    private BluePrint bluePrint;
    private SpriteRenderer floorRenderer;

    void Start() {
    }

    void Update() {
    }

    void OnMouseDown() {
    }

    void OnMouseEnter() {
        floorRenderer.color = HighlightColor;
    }

    void OnMouseExit() {
        floorRenderer.color = Color.white;
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

    public BluePrint AddBluePrint(GameObject blockPrefabToBuild) {
        if (block) {
			Debug.Log("Cant build there");
			return null;
        }

        GameObject bluePrintObject = InstantiateChild(bluePrintPrefab, true);
        bluePrint = bluePrintObject.GetComponent<BluePrint>();
        bluePrint.SetBlockToBuild(this, blockPrefabToBuild);

        return bluePrint;
    }

    public GameObject AddBlock(GameObject blockPrefab) {
        block = InstantiateChild(blockPrefab, true);
        MapManager.walkableMap[x, y] = IsWalkable();

        if (bluePrint) {
            Destroy(bluePrint.gameObject);
            bluePrint = null;
        }

        return block;
    }

    public void RemoveBlock() {
        Destroy(block);
        MapManager.walkableMap[x, y] = true;
        block = null;
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

        if (sortSprite) {
            SpriteRenderer spriteRenderer = newObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer) {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(y) * -1;
            }
        }

        return newObject;
    }

    public GameObject GetBlock() {
        return block;
    }

    public BluePrint GetBluePrint() {
        return bluePrint;
    }

    public GameObject GetFloor() {
        return floor;
    }
}
