using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Seed", fileName = "newSeed")]
public class Seed : ScriptableObject
{
    public int ticksToGrow;
    public Sprite[] stageSprites;
}
