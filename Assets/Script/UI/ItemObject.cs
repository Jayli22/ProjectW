using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    InternalSkill,
    MartialSkill,
    Items
}


public abstract class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public int id;
    public Sprite icon;
    public ItemType type;
    public string objName;
    
}

[System.Serializable]
public class Item
{
    public string name;
    public int id;
    public string description;
    public ItemType type;
    public Item(ItemObject item)
    {
        name = item.objName;
        id = item.id;
        type = item.type;
        if (item.type == ItemType.InternalSkill)
        {
            description = item.prefab.GetComponent<BaseInternalSkill>().description;
        }
        if (item.type == ItemType.MartialSkill)
        {
            description = item.prefab.GetComponent<BaseMartialSkill>().description;
        }
    }
}