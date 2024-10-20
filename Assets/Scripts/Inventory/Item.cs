using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public ActionType actionType;
    public bool stackable = true;
    public Sprite image;
    public GameObject prefab;
}

public enum ItemType
{
public enum ItemType {
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
public enum ActionType {
    Attack,
    Equip,
    Eat,
    Nothing
