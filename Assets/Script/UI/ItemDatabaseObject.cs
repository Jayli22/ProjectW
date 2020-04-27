using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Inventory System/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] Items;
    public Dictionary<int, ItemObject> GetItem = new Dictionary<int,ItemObject>();

    public void OnAfterDeserialize()
    {
        //GetItem = new Dictionary<int, ItemObject>();

        //for (int i = 0; i < Items.Length; i++)
        //{
        //    //Items[i].id = i;
        //    GetItem.Add(Items[i].id, Items[i]);
        //}
    }

    public void OnBeforeSerialize()
    {
        //GetItem = new Dictionary<int, ItemObject>();
    }

    private void OnEnable()
    {
        GetItem = new Dictionary<int, ItemObject>();

        for (int i = 0; i < Items.Length; i++)
        {
            //Items[i].id = i;
            GetItem.Add(Items[i].id, Items[i]);
        }
    }
}

