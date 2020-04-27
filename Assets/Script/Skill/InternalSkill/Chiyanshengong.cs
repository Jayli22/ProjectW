using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chiyanshengong : BaseInternalSkill, IBaseInternalSkill
{
    private bool effect_enable = false;
    private bool effect_1_tag;
    public override void SelectTarget()
    {
        if (!targets.Contains(Player.MyInstance.gameObject))
            targets.Add(Player.MyInstance.gameObject);
    }

    public override void ApplyBuff()
    {
        Debug.Log("激活" + skillName);
        Effect_1();
       // InterSkillEffectHub.SpeedUpByValue(Player.MyInstance.gameObject, 0.5f);
    }
    public override void RemoveBuff()
    {
        Debug.Log("停止激活" + skillName);

        // InterSkillEffectHub.SpeedDownByValue(Player.MyInstance.gameObject, 0.5f);

    }

    private void Update()
    {
         if(effect_enable)
        {
            if(Player.MyInstance.ComboStatus() == 3 && !effect_1_tag)
            {
                Player.MyInstance.atk_t += 15;
                effect_1_tag = true;
            }
            if (Player.MyInstance.ComboStatus() == 1)
            {
                if(effect_1_tag)
                {
                    Player.MyInstance.atk_t -= 15;
                }
                effect_1_tag = false;
            }

        }
    }
    public void Effect_1()
    {
        effect_enable = true;
    }
}
