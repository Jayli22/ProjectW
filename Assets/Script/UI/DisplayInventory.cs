using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();
    public InventoryObject inventory;
    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEMS;
    public int NUMBER_OF_COLUMN;
    public int Y_SPACE_BETWEEN_ITEMS;
    Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
    public GameObject iconPrefab;
    private void Start()
    {
        CreateSlots();
    }

    private void Update()
    {
        UpdateSlots();
        if(Input.GetMouseButton(0))
       {

        }
    }
    public void CreateSlots()
    {
        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Container.InventorySlots.Length; i ++)
        {
            var obj = Instantiate(iconPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            AddEvent(obj, EventTriggerType.PointerClick, delegate { OnPointerClick(obj); });
            
           // AddEvent(obj, EventTriggerType., delegate { OnCancel(obj); });

            itemsDisplayed.Add(obj, inventory.Container.InventorySlots[i]);
        }
    }
    public void UpdateSlots()
    {
        foreach(KeyValuePair<GameObject,InventorySlot> _slot in itemsDisplayed)
        {
            if(_slot.Value.id >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = inventory.database.GetItem[_slot.Value.item.id].icon;
                _slot.Key.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                _slot.Key.GetComponentInChildren<Text>().text = _slot.Value.amount.ToString("n0");
                if (_slot.Key.transform.childCount < 6 && inventory.database.GetItem[_slot.Value.item.id].type == ItemType.InternalSkill)
                {
                    GameObject sg = Instantiate(inventory.database.GetItem[_slot.Value.item.id].prefab, _slot.Key.transform);
                    Debug.Log("??");
                    sg.SetActive(false);
                }

            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<Text>().text = "";
                if (_slot.Key.transform.childCount >= 6)
                    Destroy(_slot.Key.transform.GetChild(5).gameObject);
            }
        }
    }
    public void UpdateDisplay()
    {
        //for (int i = 0; i < inventory.Container.Items.Length; i++)
        //{

        //    InventorySlot slot = inventory.Container.Items[i];

        //    if (itemDisplayed.ContainsKey(slot))
        //    {
        //        itemDisplayed[slot].GetComponentInChildren<Text>().text = slot.amount.ToString("n0");
        //    }
        //    else
        //    {
        //        var obj = Instantiate(iconPrefab, Vector3.zero, Quaternion.identity, transform);
        //        obj.transform.GetChild(0).GetComponent<Image>().sprite = inventory.database.GetItem[slot.item.id].icon;
        //        obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
        //        obj.GetComponentInChildren<Text>().text = slot.amount.ToString("n0");
        //        itemDisplayed.Add(slot, obj);
        //    }
        //}
    }
    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEMS * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0f);
    }

    private void AddEvent(GameObject obj, EventTriggerType type,UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        mouseItem.hoverObj = obj;
        if (itemsDisplayed.ContainsKey(obj))
        {
            mouseItem.hoverItem = itemsDisplayed[obj];
        }
       if(itemsDisplayed[obj].id  >= 0  && !obj.transform.GetChild(2).gameObject.activeSelf)
        {
            obj.transform.GetChild(4).gameObject.SetActive(true);
            obj.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = (itemsDisplayed[obj].item.name);
            obj.transform.GetChild(4).GetChild(1).GetComponent<Text>().text = (itemsDisplayed[obj].item.description);
            //obj.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = (itemsDisplayed[obj].item.name);

        }
    }
    public void OnExit(GameObject obj)
    {
        mouseItem.hoverObj = null;
        mouseItem.hoverItem = null;
        obj.transform.GetChild(3).gameObject.SetActive(false);
        obj.transform.GetChild(2).gameObject.SetActive(false);
        obj.transform.GetChild(4).gameObject.SetActive(false);

    }
    public void OnDragStart(GameObject obj)
    {
        var mouseObj = new GameObject();
        var rt = mouseObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);
        mouseObj.transform.SetParent(transform.parent);
        if(itemsDisplayed[obj].id >= 0)
        {
            var img = mouseObj.AddComponent<Image>();
            img.sprite = inventory.database.GetItem[itemsDisplayed[obj].id].icon;
            img.raycastTarget = false;
        }
        mouseItem.obj = mouseObj;
        mouseItem.item = itemsDisplayed[obj];
    }
    public void OnDragEnd(GameObject obj)
    {
        if (mouseItem.hoverObj)
        {
            inventory.MoveItem(itemsDisplayed[obj], itemsDisplayed[mouseItem.hoverObj]);
        }
        else
        {
            //inventory.RemoveItem(itemsDisplayed[obj].item);
        }
        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }
    public void OnDrag(GameObject obj)
    {
        if(mouseItem.obj != null)
        {
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
    public void OnPointerClick(GameObject obj)
    {
        if (itemsDisplayed[obj].id >= 0 && itemsDisplayed[obj].item.type == ItemType.InternalSkill)
        {
            obj.transform.GetChild(2).gameObject.SetActive(true);

        }
        if (itemsDisplayed[obj].id >= 0 && itemsDisplayed[obj].item.type == ItemType.MartialSkill)
        {
            obj.transform.GetChild(3).gameObject.SetActive(true);
            obj.transform.GetChild(3).GetComponent<MartialSkillActiveGroup>().id = itemsDisplayed[obj].id;

        }
    }

}
public class MouseItem
{
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}