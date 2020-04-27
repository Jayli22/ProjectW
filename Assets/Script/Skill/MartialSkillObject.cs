using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default", menuName = "Skill/MartialSkill")]
public class MartialSkillObject : ItemObject, ISerializationCallbackReceiver
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
        type = ItemType.MartialSkill;
        // Debug.Log(prefab.GetComponent<BaseInternalSkill>().skillID);

        id = prefab.GetComponent<BaseMartialSkill>().skillId;
        //icon = prefab.GetComponent<BaseMartialSkill>().icon;

        //Debug.Log(prefab.GetComponent<BaseInternalSkill>().skillName);

        objName = prefab.GetComponent<BaseMartialSkill>().skillName;
        // Debug.Log("before" + id);
    }
}