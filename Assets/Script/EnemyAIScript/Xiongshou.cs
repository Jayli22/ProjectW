using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xiongshou : MeleeEnemy
{
    private Timer skillCooldownTimer_0;
    private Timer skillCooldownTimer_1;
    private Timer skillCooldownTimer_2;
    private Timer skillCooldownTimer_3;
    private Timer actionCooldownTimer;


    [ShowInInspector, PropertyTooltip("各技能的冷却时间")]
    public float[] skillsCooldown;

    [ShowInInspector, PropertyTooltip("最长动作间隔时间")]
    public float actionCooldown;

    private Timer castingTimer;

    public GameObject swoopEffect;
    public GameObject dashEffect;

    private Vector2 jumpTargetPos;
    private Vector2 jumpStartPos;


    protected List<Collider2D> hittedObjects;
    public GameObject[] fellowPrefabs;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        stableTag = true;
        skillCooldownTimer_0 = gameObject.AddComponent<Timer>();
        skillCooldownTimer_1 = gameObject.AddComponent<Timer>();
        skillCooldownTimer_2 = gameObject.AddComponent<Timer>();
        skillCooldownTimer_3 = gameObject.AddComponent<Timer>();
        actionCooldownTimer = gameObject.AddComponent<Timer>();
        castingTimer = gameObject.AddComponent<Timer>();
        castingTimer.Duration = 0.1f;
        castingTimer.Run();
        skillCooldownTimer_0.Duration = skillsCooldown[0];
        skillCooldownTimer_1.Duration = skillsCooldown[1];
        skillCooldownTimer_2.Duration = skillsCooldown[2];
        skillCooldownTimer_3.Duration = skillsCooldown[3];
        actionCooldownTimer.Duration = Random.Range(0, actionCooldown);
        skillCooldownTimer_0.Run();
        skillCooldownTimer_1.Run();
        skillCooldownTimer_2.Run();
        skillCooldownTimer_3.Run();
        actionCooldownTimer.Run();
        skillCooldownTimer_0.RemainTime = 0.1f;
        skillCooldownTimer_1.RemainTime = 0.1f;
        skillCooldownTimer_2.RemainTime = 0.1f;
        skillCooldownTimer_3.RemainTime = 0.1f;
        hittedObjects = new List<Collider2D>();

    }

    // Update is called once per frame
    protected override void Update()
    {
        baseUpdateInfo();
        if (isAlive)
        {
            StatusDetermination();
            StatusBehavior();

        }
        else
        {
            StatusSwitch(NPCCurrentState.Death);
        }
    }


    /// <summary>
    /// 判定角色现应处于的状态
    /// </summary>
    public void StatusDetermination()
    {
        if(castingTimer.Finished)
        {
            StatusSwitch(NPCCurrentState.Normal);
        }
        if (currentStatus == NPCCurrentState.Normal || currentStatus == NPCCurrentState.MoveToPlayer)
        {
            if (playerDistance > attackDetectionRadius)
            {
                if (RandomIdleDetermination())
                {
                    StatusSwitch(NPCCurrentState.RandomIdle);
                }
                else if (RandomMoveDetermination())
                {
                    StatusSwitch(NPCCurrentState.RandomMove);
                }
                else
                {
                    CombatLogic();
                }
            }
            else
            {
                if (baseattackCooldownTimer.Finished)
                {
                    StatusSwitch(NPCCurrentState.BaseAttack);
                }
            }
        }
        if (currentStatus == NPCCurrentState.Hitteed)
        {
            if (stiffnessTimer.Finished)
            {
                StatusSwitch(NPCCurrentState.Normal);
            }
        }
    }
    /// <summary>
    /// 各状态所执行的行为
    /// </summary>
    public void StatusBehavior()
    {
        switch (currentStatus)
        {
            case NPCCurrentState.MoveToPlayer:
                MoveToPlayer();
                break;
            case NPCCurrentState.RandomIdle:
                if (randomIdleTimer.Finished)
                {
                    StatusSwitch(NPCCurrentState.Normal);
                }
                break;
            case NPCCurrentState.RandomMove:
                RandomMove();
                if (randomMoveTimer.Finished)
                {
                    StatusSwitch(NPCCurrentState.Normal);
                }
                break;
            case NPCCurrentState.BaseAttack:
                if (castingTimer.Finished)
                {
                    Bite();
                }
                    if (playerDistance > attackDetectionRadius)
                {
                    StatusSwitch(NPCCurrentState.Normal);
                }
                break;
            case NPCCurrentState.Skill_1:
                if (castingTimer.Finished)
                {
                    Dash();
                }
                break;
            case NPCCurrentState.Skill_2:
                if (castingTimer.Finished)
                {
                    Roar();
                }
                break;
            case NPCCurrentState.Skill_3:
                if (castingTimer.Finished)
                {
                    Swoop();
                }
                break;

            default:
                break;
        }
    }
    public override void StatusSwitch(NPCCurrentState status)
    {
        //Debug.Log("From" + currentStatus + "to" + status);
        switch (status)
        {
            case NPCCurrentState.Normal:
                StopMoving();
                currentStatus = status;
                break;
            case NPCCurrentState.MoveToPlayer:
                currentStatus = status;
                break;
            case NPCCurrentState.RandomIdle:
                StopMoving();
                EnterRandomIdle();
                currentStatus = status;
                break;
            case NPCCurrentState.RandomMove:
                currentStatus = status;
                EnterRandomMove();
                StopMoving();
                break;
            case NPCCurrentState.Hitteed:
                currentStatus = status;
                StopMoving();
                break;
            case NPCCurrentState.BaseAttack:
                StopMoving();
                currentStatus = status;
                actionCastTri = false;
                break;
            case NPCCurrentState.Death:
                currentStatus = status;
                DeathComing();
                break;
            case NPCCurrentState.Skill_1:
                currentStatus = status;
                StopMoving();
                break;
            case NPCCurrentState.Skill_2:
                currentStatus = status;
                StopMoving();
                break;
            case NPCCurrentState.Skill_3:
                currentStatus = status;
                StopMoving();
                break;
            default:
                currentStatus = status;
                break;
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
                    StatusSwitch(NPCCurrentState.Skill_1);
                    break;
                case 1:
                    StatusSwitch(NPCCurrentState.Skill_2);
                    break;
                case 2:
                    StatusSwitch(NPCCurrentState.Skill_3);
                    break;
                case 3:
                    StatusSwitch(NPCCurrentState.MoveToPlayer);

                    break;
            }

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
        StartLineDrive(4f, 2f);
        hittedObjects = new List<Collider2D>();
        DashHitCheck();
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
        GenerateEnemy(fellowPrefabs[Random.Range(0, 2)]);

    }
    /// <summary>
    //在地图随机范围内随机生成一个敌人对象
    /// </summary>
    public void GenerateEnemy(GameObject g)
    {
        Vector2 pos;
        pos.x = Random.Range(-4.5f, 4.5f);
        pos.y = Random.Range(-4.5f, 4.5f);
        GameObject newEnemy = Instantiate(g, pos, Quaternion.identity);

    }
    /// <summary>
    /// 撕咬
    /// </summary>
    private void Bite()
    {
        castingTimer.Duration = 1f;
        castingTimer.Run();
        animator.SetInteger("CurState", 3);
        actionCastTri = true;
        StartCoroutine(TriggerAttack(precastTime[0]));
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
        hittedObjects = new List<Collider2D>();
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
        SwoopHitCheck();
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

    void SwoopHitCheck()
    {
        Debug.Log("swoophitcheck");
        Collider2D[] p = new Collider2D[5];
        ContactFilter2D cf = new ContactFilter2D();
        cf.useTriggers = true;
        Physics2D.OverlapCollider(this.GetComponent<Collider2D>(), cf, p);
        if (p.Length > 0)
        {
            foreach (Collider2D hit in p)
            {
                if (hit != null)
                {
                    if (hit.tag == "Player")
                    {
                        if(!hittedObjects.Contains(hit))
                        {
                            Player.MyInstance.TakeDamage(transform.position, 1f, 35);
                        }
                    }
                }

            }

        }
    }
    void DashHitCheck()
    {
        Debug.Log("Dashhitcheck");
        Collider2D[] p = new Collider2D[5];
        ContactFilter2D cf = new ContactFilter2D();
        cf.useTriggers = true;
        Physics2D.OverlapCollider(this.GetComponent<Collider2D>(), cf, p);
        if (p.Length > 0)
        {
            foreach (Collider2D hit in p)
            {
                if (hit != null)
                {
                    if (hit.tag == "Player")
                    {
                        if (!hittedObjects.Contains(hit))
                        {
                            Player.MyInstance.TakeDamage(transform.position, 1f, 35);
                        }
                    }
                }

            }

        }
    }
}