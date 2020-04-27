using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "InternalSkill", menuName = "Skill/InternalSkill", order = 1), TableList(ShowIndexLabels = true)]
public class BaseInternalSkill : MonoBehaviour, IBaseInternalSkill
{
    [Tooltip("技能ID")]
    public int skillID;
    [Tooltip("技能名称")]
    public string skillName;
    [PreviewField(Alignment = ObjectFieldAlignment.Center)]
    public Sprite icon;

    [TextArea, Tooltip("技能描述")]
    public string description;

    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<BuffEventCfg> buffEventCfgs = new List<BuffEventCfg>();

    public int attackValue;
    public int hpValue;
    public float movespeedValue;
    public float attackPercentage;
    public float hpPercentage;
    public float movespeedPercentage;

    protected List<GameObject> targets = new List<GameObject>();
    public void Print()
    {
        Debug.Log("BuffType:" + buffEventCfgs[0].buffType);
        Debug.Log("triggerTypes:" + buffEventCfgs[0].triggerTypes);
        Debug.Log("triggerPreConditions:" + buffEventCfgs[0].triggerPreConditions);
        Debug.Log("targetTypes:" + buffEventCfgs[0].targetTypes);
        Debug.Log("targetFilterTypes:" + buffEventCfgs[0].targetFilterTypes);
        Debug.Log("setValueTypes:" + buffEventCfgs[0].setValueTypes);
        Debug.Log("SetValueSourceTypes:" + buffEventCfgs[0].SetValueSourceTypes);
        Debug.Log("overlapTypes:" + buffEventCfgs[0].overlapTypes);
    }

    public virtual void SelectTarget()
    {
    }

    public  void Learn()
    {
        SelectTarget();
        foreach (GameObject g in targets)
        {
            Debug.Log(skillName+ "在" +g+"上生效了");
            Character c = g.GetComponent<Character>();
            if (!c.learnedSkill_id.Contains(skillID))
            {
                c.learnedSkill_id.Add(skillID);
                c.atk_t += attackValue;
                c.moveSpeed_t += movespeedValue;
                c.maxHp_t += hpValue;
                c.atk_p += attackPercentage;
                c.moveSpeed_p += movespeedPercentage;
                c.maxHp_p += hpPercentage;
            }

            if (c.activeSkill_id == skillID)
            {

            }
            else
            {
                if (c.activeSkill_id == -1)
                {
                    ApplyBuff();
                    c.activeSkill_id = skillID;
                }
                else
                {
                    c.skillInventory.database.GetItem[c.activeSkill_id].prefab.GetComponent<BaseInternalSkill>().RemoveBuff();
                    ApplyBuff();
                    c.activeSkill_id = skillID;

                }
            }

        }
    }

    public virtual void ApplyBuff()
    {
    }

    public virtual void RemoveBuff()
    {
    }
}

