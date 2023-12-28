using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecycleItem", menuName = "RecycleItem")]
public class RecycleItem : ScriptableObject
{
    public GameObject prefab;
    public string name;
    public int scoreValue = 10;
    public int moneyValue = 10;
    public EntityMaterialType type;
}

public enum EntityMaterialType
{
    Plastic,
    Glass,
    Metal,
    EWaste,
    Oher
}
