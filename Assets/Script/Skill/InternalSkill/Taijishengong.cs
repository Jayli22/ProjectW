using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taijishengong : BaseInternalSkill, IBaseInternalSkill
{
    public override void SelectTarget()
    {
        if (!targets.Contains(Player.MyInstance.gameObject))
            targets.Add(Player.MyInstance.gameObject);
    }

    public override void ApplyBuff()
    {
        InterSkillEffectHub.QiUpByValue(Player.MyInstance.gameObject, 5);
    }
    public override void RemoveBuff()
    {
        InterSkillEffectHub.QiDownByValue(Player.MyInstance.gameObject, 5);

    }

}
