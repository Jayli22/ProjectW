using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xiongshou : MeleeEnemy
{
    private Timer skillTimer_0;
    private Timer skillTimer_1;
    private Timer skillTimer_2;
    private Timer skillTimer_3;
    private Timer actionCooldownTimer;

    [ShowInInspector, PropertyTooltip("各技能的冷却时间")]
    public float[] skillsCooldown;
    //"当前状态0-未攻击,1-普通攻击,2-技能1，3-技能2..
    private int curState;

    [ShowInInspector, PropertyTooltip("最长动作间隔时间")]
    public float actionCooldown;

    private Timer castingTimer;

    private Vector2 jumpTargetPos;
    private Vector2 jumpStartPos;

    private bool skillTrigger_4;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        skillTimer_0 = gameObject.AddComponent<Timer>();
        skillTimer_1 = gameObject.AddComponent<Timer>();
        skillTimer_2 = gameObject.AddComponent<Timer>();
        skillTimer_3 = gameObject.AddComponent<Timer>();
        actionCooldownTimer = gameObject.AddComponent<Timer>();
        castingTimer = gameObject.AddComponent<Timer>();
        curState = 0;
        skillTimer_0.Duration = skillsCooldown[0];
        skillTimer_1.Duration = skillsCooldown[1];
        skillTimer_2.Duration = skillsCooldown[2];
        skillTimer_3.Duration = skillsCooldown[3];
        actionCooldownTimer.Duration = Random.Range(0, actionCooldown);
        skillTimer_0.Run();
        skillTimer_1.Run();
        skillTimer_2.Run();
        skillTimer_3.Run();
        actionCooldownTimer.Run();
        skillTimer_0.RemainTime = 0.1f;
        skillTimer_1.RemainTime = 0.1f;
        skillTimer_2.RemainTime = 0.1f;
        skillTimer_3.RemainTime = 0.1f;
        skillTrigger_4 = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isAlive)
        {
          
            canMove = false;
            base.Update();
            if(castingTimer.Finished)
            {
                skillTrigger_4 = false;
            }
            if (!isStiffness)
            {
                //AttackDetection(0.5f);
                //AlarmRadiusDetection();
                CombatLogic();
                
            }
        }
        else
        {
            canMove = false;
            hitable = false;
            DeathComing();
        }
    }




    public override void TakeDamage(int damage)
    {
        if (hitable)
        {
            base.TakeDamage(damage);
            //DoStiffness();
            Debug.Log("我受到了攻击");
        }
    }


    /// <summary>
    /// 凶兽攻击逻辑
    /// </summary>
    private void CombatLogic()
    {
        if (actionCooldownTimer.Finished)
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    Swoop();
                    break;
                case 1:
                    Swoop();

                    break;
                case 2:
                    Swoop();

                    break;
                case 3:
                    Swoop();

                    break;
            }

            actionCooldownTimer.Duration = Random.Range(2, actionCooldown);


            if (castingTimer.Finished)
            {
                actionCooldownTimer.Run();

            }

        }

    }
    /// <summary>
    /// 直线冲刺
    /// </summary>
    /// <param name="distance"></param>
    public IEnumerator LineDrive(float distance, float time = 0.1f)
    {
        canMove = false;
        float rate = 1 / time;
        float duration = 0.0f;
        Vector2 startPos = transform.position;
        Vector2 targetPos = transform.position + (Vector3)(distance * ((Vector3)playerCharacterPos - transform.position).normalized);

        while (duration < 1.0)
        {
            duration += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, targetPos, duration);
            duration += rate * Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }

    public void StartLineDrive(float distance, float time = 0.1f)
    {
        StartCoroutine(LineDrive(distance, time));

    }



    /// <summary>
    /// 冲锋
    /// </summary>
    private void Dash()
    {
        castingTimer.Duration = 2f;
        castingTimer.Run();
        animator.SetInteger("CurState", 1);
        //skillScript.GetComponent<BaseSkill>().Release();
        canMove = false;
        curState = 1;
        StartLineDrive(4f, 2f);

    }
    /// <summary>
    /// 怒吼
    /// </summary>
    private void Roar()
    {
        castingTimer.Duration = 1f;
        castingTimer.Run();
        animator.SetInteger("CurState", 2);
        //skillScript.GetComponent<BaseSkill>().Release();
        canMove = false;
        curState = 2;

    }
    /// <summary>
    /// 撕咬
    /// </summary>
    private void Bite()
    {
        castingTimer.Duration = 1f;
        castingTimer.Run();
        animator.SetInteger("CurState", 3);
        //skillScript.GetComponent<BaseSkill>().Release();
        canMove = false;
        curState = 3;

    }
    /// <summary>
    /// 飞扑
    /// </summary>
    private void Swoop()
    {
        castingTimer.Duration = 1f;
        castingTimer.Run();
        animator.SetInteger("CurState", 4);
        //skillScript.GetComponent<BaseSkill>().Release();
        canMove = false;
        curState = 4;
        StartCoroutine(Scale(2f,1.1f,1.3f));

    }

    private void JumpToPostion()
    {
        transform.localScale *= 1.2f;
        transform.localScale *= 1.0f;
        jumpTargetPos = playerCharacterPos;
        skillTrigger_4 = true;
    }

    public float maxSize;
    public float growFactor;
    public float waitTime;
    IEnumerator Scale(float scaleTime , float growFactor , float maxScale)
    {
        float timer = 0;

        while (true) // this could also be a condition indicating "alive or dead"
        {
            // we scale all axis, so they will have the same value, 
            // so we can work with a float instead of comparing vectors
            while (maxScale > transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }
            // reset the timer

            yield return new WaitForSeconds(scaleTime);

            timer = 0;
            while (1 < transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }

            timer = 0;
            yield return new WaitForSeconds(scaleTime);
        }
    }

}

