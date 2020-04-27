using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PickableItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public ItemObject item;

    public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = item.icon;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
    }
}
