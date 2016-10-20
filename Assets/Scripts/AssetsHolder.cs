using UnityEngine;
using System.Collections;

public class AssetsHolder : MonoBehaviour{

	public GameObject Character;
	public GameObject Floor;
	public GameObject Rock;
	public GameObject Tree;

	public Sprite[] grassSprites;
	public Sprite[] rockFloorSprites;
	public Sprite[] rockSprites;
	public Sprite[] treeSprites;

	public RuntimeAnimatorController[] charAnimations;

	public GameObject InstantiateFloor(TerrainEnum type, Vector2 position)
	{
		GameObject floor = null;

		switch (type)
		{
			case TerrainEnum.ROCKFLOOR:
			case TerrainEnum.ROCK:
				floor = InstantiationHelper (Floor, position, rockFloorSprites);
				break;

			default:
				floor = InstantiationHelper (Floor, position, grassSprites);
				break;
		}

		return floor;
	}

	public GameObject InstantiateObject(TerrainEnum type, Vector2 position)
	{
		GameObject obj = null;

		switch (type)
		{
			case TerrainEnum.ROCK:
				obj = InstantiationHelper(Rock, position, rockSprites);
				break;

			case TerrainEnum.TREE:
				obj = InstantiationHelper(Tree, position, treeSprites);
				break;
		}

		return obj;
	}

	public GameObject InstantiateChar(Vector2 position)
	{
		return InstantiateChar(position, charAnimations[Random.Range(0, charAnimations.Length)]);
	}

	public GameObject InstantiateChar(Vector2 position, RuntimeAnimatorController charAnimationController)
	{
		GameObject newGameObject = (GameObject)Instantiate(Character, position, Quaternion.identity);

		Animator animator = newGameObject.GetComponent<Animator>();
		animator.runtimeAnimatorController = charAnimationController;

		return newGameObject;
	}

	public GameObject InstantiationHelper(GameObject prefab, Vector2 position, Sprite[] spriteArray)
	{
		GameObject newGameObject = (GameObject)Instantiate(prefab, position, Quaternion.identity);

		SpriteRenderer sr = newGameObject.GetComponent<SpriteRenderer>();
		sr.sprite = spriteArray[Random.Range (0, spriteArray.Length)];

		sr.sortingOrder = Mathf.RoundToInt(position.y) * -1;

		return newGameObject;
	}
}
