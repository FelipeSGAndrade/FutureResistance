using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private GameObject block;
    private GameObject floor;

    void Start() {
        gameObject.name = "Node" + "(" + transform.position.x + "," + transform.position.y + ")";
    }

    void Update() {
    }

    void OnMouseDown() {
    }

    public GameObject AddBlock(GameObject blockPrefab) {
        block = (GameObject)Instantiate(blockPrefab, transform.position, Quaternion.identity);
        block.transform.SetParent(transform);

        SpriteRenderer spriteRenderer = block.GetComponent<SpriteRenderer>();
        if (spriteRenderer) {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
        }
        return block;
    }

    public void RemoveBlock() {
        Destroy(block);
        block = null;
    }

    public GameObject AddFloor(GameObject floorPrefab) {
        floor = (GameObject)Instantiate(floorPrefab, transform.position, Quaternion.identity);
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
