using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public ActionType actionType;
    public bool stackable = true;
    public Sprite image;
}

public enum ItemType {
    Tool,
    Armour,
    Food,
    Resource,
    Other
}

public enum ActionType {
    Attack,
    Equip,
    Eat,
    Nothing
}