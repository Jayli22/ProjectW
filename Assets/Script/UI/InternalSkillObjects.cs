using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default",menuName = "Skill/InternalSkill")]
public  class InternalSkillObjects : ItemObject, ISerializationCallbackReceiver
{
    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
       // type = ItemType.Skill;
       //// Debug.Log(prefab.GetComponent<BaseInternalSkill>().skillID);

       // id = prefab.GetComponent<BaseInternalSkill>().skillID;
       // icon = prefab.GetComponent<BaseInternalSkill>().icon;

       // Debug.Log(prefab.GetComponent<BaseInternalSkill>().skillName);

       // objName = prefab.GetComponent<BaseInternalSkill>().skillName;
       // Debug.Log("before" + id);

    }

     void OnEnable()
    {
        type = ItemType.InternalSkill;
        // Debug.Log(prefab.GetComponent<BaseInternalSkill>().skillID);

        id = prefab.GetComponent<BaseInternalSkill>().skillID;
        //icon = prefab.GetComponent<BaseInternalSkill>().icon;

        //Debug.Log(prefab.GetComponent<BaseInternalSkill>().skillName);

        objName = prefab.GetComponent<BaseInternalSkill>().skillName;
       // Debug.Log("before" + id);
    }
}

