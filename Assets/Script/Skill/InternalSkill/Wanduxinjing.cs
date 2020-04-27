using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanduxinjing : BaseInternalSkill, IBaseInternalSkill
{
    public override void SelectTarget()
    {
        if (!targets.Contains(Player.MyInstance.gameObject))
            targets.Add(Player.MyInstance.gameObject);
    }

    public override void ApplyBuff()
    {
        InterSkillEffectHub.SpeedUpByValue(Player.MyInstance.gameObject, 0.5f);
    }
    public override void RemoveBuff()
    {
        InterSkillEffectHub.SpeedDownByValue(Player.MyInstance.gameObject, 0.5f);

    }

    public void Effect_1()
    {

    }
}
