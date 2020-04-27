using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public bool testTag; //测试AI开关
    protected Timer baseattackCooldownTimer; //攻击间隔计时器

    protected Vector3 playerCharacterPos;
    protected float playerDistance;
    public GameObject[] attacksPrefabs;

    public float attackDetectionRadius;//攻击检测半径
    public float alarmRadius;  //预警半径
    public float attackCooldown; //攻击间隔
    public float backFactor; //受击后退系数
    public int type; //怪物类型
    public NPCCurrentState currentStatus;


    private float randomSpeed;// 随机IDLE时候的速度
    public float[] precastTime;

    /// <summary>
    /// 怪物强度系数，用于关卡生成计算
    /// </summary>
    public int strengthValue;
    /// <summary>
    /// 怪物类型1-普通怪物,2-群居怪物,3-精英怪物
    /// </summary>
    [ShowInInspector, Wrap(0,4), PropertyTooltip("怪物类型1-普通怪物,2-群居怪物,3-精英怪物")]

    private Vector2 moveDirection;

    // Start is called before the first frame update
    protected override void Start()
    {
        playerCharacterPos = Player.MyInstance.transform.position;
        //playerDistance = (playerCharacterPos - new Vector2(transform.position.x,transform.position.y)).magnitude;
        playerDistance = (playerCharacterPos - transform.position).magnitude;
        Init();
        //Debug.Log(playerDistance);
        StatusSwitch(NPCCurrentState.Normal);



        baseattackCooldownTimer = gameObject.AddComponent<Timer>();
        baseattackCooldownTimer.Duration = attackCooldown;
        baseattackCooldownTimer.Run();
        randomMoveCooldownTimer.Duration = 1f;
        randomMoveCooldownTimer.Run();
        
        randomIdleCoolDownTimer.Duration = Random.Range(2, 5);
        randomIdleCoolDownTimer.Run();
        if (currentQi > 0 )
        {
            stableTag = true;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        randomIdleTimer = gameObject.AddComponent<Timer>();
        randomIdleCoolDownTimer = gameObject.AddComponent<Timer>();
        randomMoveTimer = gameObject.AddComponent<Timer>();
        randomMoveCooldownTimer = gameObject.AddComponent<Timer>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
                 
       


    }

    protected void baseUpdateInfo()
    {
        playerCharacterPos = Player.MyInstance.transform.position;
        playerDistance = (playerCharacterPos - transform.position).magnitude;
        // Debug.Log(playerDistance+","+attackDetectionRadius);

        stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
        BuffCalculate();
    }


    public virtual void TakeDamage(int damage)
    {
        if (hitableTag)
        {
            Instantiate(hittedEffectPrefab, transform.position, transform.rotation);

            //health reduce 
            if (currentQi > 0)
            {
                if (currentQi < damage)
                {
                    damage -= currentQi;
                    currentQi = 0;
                }
                else
                {
                    currentQi -= damage;
                }
            }
            if (damage > 0)
            {
                currentHp -= damage;
            }
            if (currentHp <= 0)
            {
                isAlive = false;
            }
            else
            {
                DoStiffness();//造成硬直
            }

        }
    }

    public virtual void TakeDamage(Vector3 pos, float backFactor, int damage, bool shock = false)
    {
        if (hitableTag)
        {
            Instantiate(hittedEffectPrefab, transform.position, transform.rotation);

            //health reduce 
            if (currentQi > 0)
            {
                if (currentQi < damage)
                {
                    damage -= currentQi;
                    currentQi = 0;
                }
                else
                {
                    currentQi -= damage;
                }
            }
            if (damage > 0)
            {
                currentHp -= damage;
            }
            if (currentHp <= 0)
            {
                isAlive = false;
                currentHp = 0;
            }
            else
            {
                if(!stableTag)
                {
                    DoStiffness(shock);//造成硬直
                    KnockBack(transform.position - pos, backFactor);
                }
             }

        }
    }
    /// <summary>
    /// 对气盾造成额外伤害
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="backFactor"></param>
    /// <param name="damage"></param>
    /// <param name="extraFactor">倍率系数大于1为增加，小于1即减少</param>
    public virtual void TakeDamage_ExtraQi(Vector3 pos, float backFactor, int damage, int extraFactor , bool shock = false)
    {
        if (hitableTag)
        {
            Instantiate(hittedEffectPrefab, transform.position, transform.rotation);

            //health reduce 
            if (currentQi > 0)
            {
                if (currentQi < damage * extraFactor)
                {
                    damage = (damage * extraFactor - currentQi) / extraFactor;
                    currentQi = 0;
                }
                else
                {
                    currentQi -= damage * extraFactor;
                }
            }
            if (damage > 0)
            {
                currentHp -= damage;
            }
            if (currentHp <= 0)
            {
                isAlive = false;
            }
            if (!stableTag)
            {
                DoStiffness(shock);//造成硬直
                KnockBack(transform.position - pos, backFactor);
            }

        }
    }
    /// <summary>
    /// 无视部分气盾
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="backFactor"></param>
    /// <param name="damage"></param>
    /// <param name="ignoreFactor">该系数应当小于1</param>
    public virtual void TakeDamage_IgnoreQi(Vector3 pos, float backFactor, int damage, int ignoreFactor , bool shock = false)
    {
        if (hitableTag)
        {
            Instantiate(hittedEffectPrefab, transform.position, transform.rotation);

            if (ignoreFactor > 1)
                ignoreFactor = 1;
            //health reduce 
            if (currentQi > 0)
            {
                if (currentQi < damage * (1 - ignoreFactor))
                {
                    damage -= currentQi;
                    currentQi = 0;
                }
                else
                {
                    currentQi -= damage * (1 - ignoreFactor);
                    damage = damage * ignoreFactor;
                }
            }
            if (damage > 0)
            {
                currentHp -= damage;
            }
            if (currentHp <= 0)
            {
                isAlive = false;
            }
            if (!stableTag)
            {
                DoStiffness(shock);//造成硬直
                KnockBack(transform.position - pos, backFactor);
            }

        }
    }

    /// <summary>
    /// 朝着玩家角色移动
    /// </summary>
    protected void MoveToPlayer()
    {
        animator.SetBool("Move", true);
        agent.SetDestination(playerCharacterPos);
        agent.maxSpeed = moveSpeed_f;
        Turn();
    }


    protected virtual void BaseAttack(float t)
    {
             
    }
    /// <summary>
    /// 造成硬直
    /// </summary>
    public override void DoStiffness(bool shock = false)
    {
        if (!stableTag)
        {
            if(shock)
            {
                StatusSwitch(NPCCurrentState.Shocked);
                actionCastTri = false;
                animator.SetTrigger("Hitted");
                stiffnessTimer.Run();
            }
            else
            {
                StatusSwitch(NPCCurrentState.Hitteed);
                actionCastTri = false;
                animator.SetTrigger("Hitted");
                stiffnessTimer.Duration = stiffnessDuration;
                stiffnessTimer.Run();
            }
            
        }
    }
    /// <summary>
    /// 解除硬直
    /// </summary>
    public override void UndoStiffness()
    {
        Debug.Log("undostiffness");
        //animator.SetBool("Hitted", false);
        StatusSwitch(NPCCurrentState.Normal);
    }


    /// <summary>
    /// 判断是否进入怪物AI Idle状态
    /// </summary>
    public bool RandomIdleDetermination()
    {
        if (randomIdleCoolDownTimer.Finished)
        {
            randomIdleCoolDownTimer.Duration = Random.Range(3, 5f); //什么时候开始判断下次间歇时间
            randomIdleCoolDownTimer.Run();
            if (Random.Range(0,1f) < randomIdleTendency)
            {
                return true;
            }

        }

        return false;
       
    }

   

    ///// <summary>
    ///// 选择攻击方向
    ///// </summary>
    ///// <returns></returns>
    //public GameObject ChooseHitBox()
    //{
    //    GameObject g = attackPositions[0];
    //    float attackAngle = ToolsHub.GetAngleBetweenVectors(new Vector2(0, 1), Player.MyInstance.transform.position);
    //    if (337.5 >= attackAngle || attackAngle < 22.5)
    //    {
    //        g = attackPositions[0];
    //    }
    //    if (292.5 <= attackAngle && attackAngle < 337.5)
    //    {
    //        g = attackPositions[1];
    //    }
    //    if (247.5 <= attackAngle && attackAngle < 292.5)
    //    {
    //        g = attackPositions[2];
    //    }
    //    if (202.5 <= attackAngle && attackAngle < 247.5)
    //    {
    //        g = attackPositions[3];
    //    }
    //    if (157.5 <= attackAngle && attackAngle < 202.5)
    //    {
    //        g = attackPositions[4];
    //    }
    //    if (112.5 <= attackAngle && attackAngle < 157.5)
    //    {
    //        g = attackPositions[5];
    //    }
    //    if (67.5 <= attackAngle && attackAngle < 112.5)
    //    {
    //        g = attackPositions[6];
    //    }
    //    if (22.5 <= attackAngle && attackAngle < 67.5)
    //    {
    //        g = attackPositions[7];
    //    }
    //    return g;
    //}


    /// <summary>
    /// 进入死亡阶段
    /// </summary>
    public void DeathComing()
    {
        animator.SetTrigger("Death");
        hitableTag = false;
        UndoStiffness();
        if (stateInfo.IsName("Death") && stateInfo.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }

    }
    
    /// <summary>
    /// 预警半径检测
    /// 玩家处在预警半径中，敌人不会进入Idle状态
    /// </summary>


    /// <summary>
    /// 控制动画转向
    /// </summary>
    protected void Turn()
    {
        animator.SetFloat("DirectionX", playerCharacterPos.x - transform.position.x);
        animator.SetFloat("DirectionY", playerCharacterPos.y - transform.position.y);
    }
    /// <summary>
    /// 向指定方向转向
    /// </summary>
    /// <param name="v"></param>
    protected void Turn(Vector3 v)
    {
        animator.SetFloat("DirectionX", v.x);
        animator.SetFloat("DirectionY", v.y);
    }


    protected void EnterRandomIdle()
    {
        randomIdleTimer.Duration = Random.Range(1, 2f);
        randomIdleTimer.Run();
    }
    protected void EnterRandomMove()
    {
        randomMoveTimer.Duration = Random.Range(1, 2f);
        randomMoveTimer.Run();
        randomDir.x = Random.Range(-1f, 1f);
        randomDir.y = Random.Range(-1f, 1f);
        randomSpeed = Random.Range(moveSpeed_f/2, moveSpeed_f);
    }


    /// <summary>
    /// 随机方向移动
    /// </summary>
    protected void RandomMove()
    {       
        
        gameObject.transform.position = gameObject.transform.position + randomDir * Time.deltaTime * randomSpeed;
        animator.SetBool("Move", true);
        Turn();
        //Debug.Log("正在随机移动");
               
    }
    
    /// <summary>
    /// 随机移动时间判定
    /// </summary>
    protected bool RandomMoveDetermination()
    {
        
            if(randomMoveCooldownTimer.Finished)
            {
                randomMoveCooldownTimer.Duration = Random.Range(3, 6f);
                randomMoveCooldownTimer.Run();
                if(Random.Range(0,1f) <randomMoveTendency)
                {
                    
                    return true;
                }


            }
        return false;
        

    }
    public virtual void StatusSwitch(NPCCurrentState status)
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
            case NPCCurrentState.Shocked:
                currentStatus = status;
                StopMoving();
                break;
            case NPCCurrentState.BaseAttack:
                StopMoving();
                currentStatus = status;
                actionCastTri = false;
                break;
            case NPCCurrentState.Death:
                DeathComing();
                break;
            default:
                currentStatus = status;
                break;
        }
    }

}
