using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Enemy
{
    //public GameObject aimLinePrefabs;
    private bool aimTarget;
    public GameObject aimLine;
    private Timer aimTimer;//瞄准时间计时器
    private float tangle = 0;
    public float aimTime;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        aimTimer = gameObject.AddComponent<Timer>();

    }

    protected override void Update()
    {
        baseUpdateInfo();
        if (isAlive)
        {
            base.Update();
            if (!isStiffness)
            {
                AttackDetection(0.5f);
                //AlarmRadiusDetection();
                //YieldAniFinish("Skill");
                if (aimTarget)
                {
                    AimTarget();
                    Turn();
                
                if (aimTimer.Finished)
                {
                    aimTarget = false;
                    canMove = true;

                    aimLine.SetActive(false);
                    TriggerAttack();
                }
                }
                if(stateInfo.IsTag("Attack") && stateInfo.normalizedTime >= 1.0f)
                {
                    animator.SetBool("Attack", false);

                }
            }
        }
        else
        {
            canMove = false;
            hitable = false;
            DeathComing();
        }
    }


    protected override void BaseAttack(float t)
    {
        StopMoving();
        animator.SetBool("Move", false);
        aimTarget = true;
        animator.SetBool("Aim", true);
        aimTimer.Duration = aimTime;
        aimTimer.Run();

        //StartCoroutine(TriggerAttack(t));


    }

    /// <summary>
    /// 触发攻击(生成箭矢)
    /// </summary>
    protected void TriggerAttack()
    {
        animator.SetBool("Attack", true);
        GameObject a = Instantiate(attacksPrefabs[0],transform.position,transform.rotation);
        //a.transform.Rotate(Vector3.forward, ToolsHub.GetAngleBetweenVectors(Vector2.up, playerCharacterPos));
        a.GetComponent<ArcherArrow>().damage = baseATK;
    }

    /// <summary>
    /// 基础攻击范围检测
    /// </summary>
    /// <param name="t"></param>
    public void AttackDetection(float t)
    {
        if (playerDistance < attackDetectionRadius)
        {
            //Debug.Log("进入范围");
            StopMoving();
            // StartCoroutine(AttackDelay(0.2f));
            if (baseattackCooldownTimer.Finished)
            {
                BaseAttack(t);
                baseattackCooldownTimer.Run();
            }
            else
            {
               // animator.SetBool("Attack", false);
            }
        }
        else
        {
            StopAim();

        }
    }
 
    /// <summary>
    /// 瞄准目标
    /// </summary>
    private void AimTarget()
    {
        float angle = ToolsHub.GetAngleBetweenVectors(new Vector2(0, 1), ((Vector3)playerCharacterPos - transform.position).normalized);
        aimLine.transform.RotateAround(gameObject.transform.position, Vector3.forward, angle - tangle);
        tangle = angle;
        aimLine.SetActive(true);
    }

    private void StopAim()
    {
        animator.SetBool("Aim", false);
        
        aimLine.SetActive(false);
        aimTarget = false;
        StartMoving();
    }
 
    public override void DoStiffness()
    {
        base.DoStiffness();
        if (aimTarget)
            StopAim();
    }
  
}
