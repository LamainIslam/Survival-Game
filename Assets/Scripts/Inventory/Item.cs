using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class
[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public ActionType actionType;
    public bool stackable = true;
    public Sprite image;
    public GameObject prefab;

    public float damagePoints;
    public float defencePoints;
    public float healthRestored;
    public float hungerRestored;
}

public enum ItemType
{
    Weapon,
    Pickaxe,
    Axe,
    Torch,
    Shield,
    Helmet,
    Chestplate,
    Leggings,
    Boots,
    Food,
    Resource,
    Other
}

public enum ActionType
{
    Attack,
    Equip,
    Eat,
    Nothing
}
