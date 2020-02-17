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
        baseUpdateInfo();
        if (isAlive)
        {
            base.Update();
            if (castingTimer.Finished)
            {
                skillTrigger_4 = false;
            }
            if (!isStiffness)
            {
                //AttackDetection(0.5f);
                //AlarmRadiusDetection();
                CombatLogic();
                YieldAniFinish("Skill");
            }
        }
        else
        {
            canMove = false;
            hitable = false;
            animator.SetInteger("CurState", 0);

            DeathComing();
        }
    }




    public override void TakeDamage(int damage)
    {
        if (hitable)
        {
            //health reduce 
            Instantiate(hittedEffectPrefab, transform.position, transform.rotation);
            currentHp -= damage;
            if (currentHp <= 0)
            {
                isAlive = false;

            }
            //else
            //{
            //    DoStiffness();//造成硬直
            //}
            Debug.Log("我受到了攻击");
        }
    }


    /// <summary>
    /// 凶兽攻击逻辑
    /// </summary>
    private void CombatLogic()
    {
        if (actionCooldownTimer.Finished && !isRandomIdle && !isRandomMove)
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    Dash();
                    break;
                case 1:
                    Roar();
                    break;
                case 2:
                    Bite();
                    break;
                case 3:
                    Swoop();

                    break;
            }

            //actionCooldownTimer.Duration = Random.Range(2, actionCooldown);
            actionCooldownTimer.Duration = actionCooldown;
            actionCooldownTimer.Run();



        }

    }
    /// <summary>
    /// 直线冲刺
    /// </summary>
    /// <param name="distance"></param>
    public IEnumerator LineDrive(float distance, float time = 0.1f)
    {
        StopMoving();
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
        StopMoving();
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
        StopMoving();
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
        StopMoving();
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
        StopMoving();
        curState = 4;
        JumpToPostion();

    }

    /// <summary>
    /// 跳跃至玩家位置
    /// </summary>
    private void JumpToPostion()
    {
        StartCoroutine(ScaleUp(0.1f, 1.03f, 1.3f));
        StartCoroutine(ToPosition(playerCharacterPos, 0.3f));

        jumpTargetPos = playerCharacterPos;
        skillTrigger_4 = true;
    }

    /// <summary>
    /// 跳跃至指定位置
    /// </summary>
    /// <param name="pos"></param>
    private void JumpToPostion(Vector2 pos)
    {
        StartCoroutine(ScaleUp(0.1f, 1.03f, 2.5f));
        StartCoroutine(ToPosition(playerCharacterPos, 0.3f));
        jumpTargetPos = pos;
        skillTrigger_4 = true;
    }

    IEnumerator ScaleUp(float scaleTime, float growFactor, float maxScale)
    {
        float timer = 0;

        // while (true) // this could also be a condition indicating "alive or dead"
        // {
        // we scale all axis, so they will have the same value, 
        // so we can work with a float instead of comparing vectors
        yield return new WaitForSeconds(0.6f);

        while (maxScale > transform.localScale.x)
        {
            timer += Time.deltaTime;
            transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
            yield return null;
        }
        // reset the timer
        // Debug.Log("扩大");
        yield return new WaitForSeconds(scaleTime);

        StartCoroutine(ScaleDown(0.5f, 1.3f, 2f));
        yield break;
    }
    IEnumerator ScaleDown(float scaleTime, float growFactor, float minScale)
    {
        float timer = 0;



        timer = 0;
        while (minScale < transform.localScale.x)
        {
            timer += Time.deltaTime;
            transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
            yield return null;
        }
        //Debug.Log("缩小");

        timer = 0;
        //yield return new WaitForSeconds(scaleTime);
        yield break;
        //}
    }


    /// <summary>
    /// 监听动画回调
    /// </summary>
    /// <param name="ani"></param>
    /// <param name="aniName"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public void YieldAniFinish(string aniName)
    {
        AnimatorStateInfo stateinfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateinfo.IsTag(aniName) && (stateinfo.normalizedTime >= 1.0f))
        {
            curState = 0;
            animator.SetInteger("CurState", 0);
            StartMoving();
                }

    }
    public IEnumerator ToPosition(Vector2 Pos, float time = 0.1f)
    {
        StopMoving();
        yield return new WaitForSeconds(0.6f);

        float rate = 1 / time;
        float duration = 0.0f;
        Vector2 startPos = transform.position;
        //  Vector2 targetPos = transform.position + (Vector3)playerCharacterPos - transform.position).normalized);
        Vector2 targetPos = playerCharacterPos;

        while (duration < 1.0)
        {
            duration += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, targetPos, duration);
            duration += rate * Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
}