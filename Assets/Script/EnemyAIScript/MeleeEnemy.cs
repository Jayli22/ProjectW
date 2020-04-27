using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void BaseAttack(float t)
    {
        if (!actionCastTri)
        {
            animator.SetBool("Attack", true);
            actionCastTri = true;
            StartCoroutine(TriggerAttack(t));
        }

    }

    /// <summary>
    /// 触发攻击(生成攻击碰撞检测盒子等）
    /// </summary>
    protected virtual IEnumerator TriggerAttack(float t)
    {
        yield return new WaitForSeconds(t);
        if (currentStatus == NPCCurrentState.BaseAttack)
        {
            GameObject a = Instantiate(attacksPrefabs[0], transform);
            float angle = ToolsHub.GetAngleBetweenVectors(new Vector2(0, 1), Player.MyInstance.transform.position - gameObject.transform.position);
            a.transform.RotateAround(gameObject.transform.position, Vector3.forward, angle);
            //a.transform.Rotate(Vector3.forward, 45f,relativeTo:Space.World);
            a.SetActive(true);
            baseattackCooldownTimer.Run();
            StatusSwitch(NPCCurrentState.Normal);
        }

    }

    /// <summary>
    /// 基础攻击范围检测
    /// </summary>
    /// <param name="t"></param>
    public void AttackDetection(float t)
    {
        if (playerDistance < attackDetectionRadius)
        {
            StatusSwitch(NPCCurrentState.BaseAttack);
            // StartCoroutine(AttackDelay(0.2f));
            if (baseattackCooldownTimer.Finished)
            {
                BaseAttack(t);
                baseattackCooldownTimer.Run();
            }
            else
            {
                animator.SetBool("Attack", false);
            }
        }
        else
        {
            canMove = true;
        }
    }
    /// <summary>
    /// 指定距离范围检测模版
    /// </summary>
    /// <param name="t"></param>
    public void AttackDetection(float t, float distance)
    {
        if (playerDistance < distance)
        {
            StopMoving();
            // StartCoroutine(AttackDelay(0.2f));
            if (baseattackCooldownTimer.Finished)
            {
                //BaseAttack();
                baseattackCooldownTimer.Run();
            }
            else
            {
                animator.SetBool("Attack", false);
            }
        }
        else
        {
            canMove = true;
        }
    }
}
