using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public ItemDatabaseObject database;
    public Inventory Container;
    public void AddItem(Item _item, int _amount)
    {
        for (int i = 0; i < Container.InventorySlots.Length; i++)
        {

            if (Container.InventorySlots[i].id == (_item.id))
            {
                Container.InventorySlots[i].AddAmount(_amount);
                return;
            }
        }

        SetEmptySlot(_item, _amount);

    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.id, item2.item, item2.amount);
        item2.UpdateSlot(item1.id, item1.item, item1.amount);
        item1.UpdateSlot(temp.id, temp.item, temp.amount);
    }

    public void RemoveItem(Item _item)
    {
        for(int i = 0; i < Container.InventorySlots.Length; i++)
        {
            if (Container.InventorySlots[i].item == _item)
            {
                Container.InventorySlots[i].UpdateSlot(-1, null, 0);
            }
        }
    }
    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for ( int i = 0; i < Container.InventorySlots.Length;i++)
        {
            if(Container.InventorySlots[i].id <= -1)
            {
                Container.InventorySlots[i].UpdateSlot(_item.id,_item,_amount);
                return Container.InventorySlots[i];
            }
        }
        //for full inventory
        return null;
    }


    [ContextMenu("Save")]
    public void Save()
    {
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file,saveData);
        //file.Close();
        //Debug.Log(Application.persistentDataPath);
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        if(File.Exists(string.Concat(Application.persistentDataPath,savePath)))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //file.Close();
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for(int i = 0; i < Container.InventorySlots.Length; i++)
            {
                Container.InventorySlots[i].UpdateSlot(newContainer.InventorySlots[i].id, newContainer.InventorySlots[i].item, newContainer.InventorySlots[i].amount);
            }
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }
}
[System.Serializable]
public class Inventory
{
    public InventorySlot[] InventorySlots = new InventorySlot[24];
}

[System.Serializable]
public class InventorySlot
{
    public int id;
    public Item item;
    public int amount;
    public InventorySlot(int _id, Item _item, int _amount)
    {
        id = _id;
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(int _id, Item _item, int _amount)
    {
        id = _id;
        item = _item;
        amount = _amount;
    }
    public InventorySlot()
    {
        id = -1;
        item = null ;
        amount = 0;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}