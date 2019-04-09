using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int x;
    public int y;
    private GameObject block;
    private GameObject floor;

    void Start() {
    }

    void Update() {
    }

    void OnMouseDown() {
    }

    public void Initialize(int x, int y) {
        this.x = x;
        this.y = y;
        gameObject.name = "Node" + "(" + x + "," + y + ")";
    }

    public GameObject AddBlock(GameObject blockPrefab) {
        block = (GameObject)Instantiate(blockPrefab, new Vector2(x, y), Quaternion.identity);
        block.transform.SetParent(transform);

        SpriteRenderer spriteRenderer = block.GetComponent<SpriteRenderer>();
        if (spriteRenderer) {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(y) * -1;
        }
        return block;
    }

    public void RemoveBlock() {
        Destroy(block);
        block = null;
    }

    public GameObject AddFloor(GameObject floorPrefab) {
        floor = (GameObject)Instantiate(floorPrefab, new Vector2(x, y), Quaternion.identity);
        floor.transform.SetParent(transform);
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
