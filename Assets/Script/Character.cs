using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NPCCurrentState
{
    Normal,
    RandomIdle,
    BaseAttack,
    MoveToPlayer,
    Hitteed,
    Shocked,
    RandomMove,
    Death,
    Skill_1,
    Skill_2,
    Skill_3,
    Skill_4,
    Skill_5,


    
}
public enum PlayerCurrentState
{
    Normal,
    BaseAttack,
    Skill,
    MoveSkill,
    Hitteed,
    Move,
    Fangun,
}
public class Character : MonoBehaviour
{
    public int maxHp;
    public int currentHp;
    public int maxHp_t;
    public float maxHp_p;
    public int maxHp_f;

    public int baseATK;// 攻击力
    public int atk_t;
    public float atk_p;
    public int atk_f;
    //public int baseDEF;//防御力
    public int maxQi; //内力值
    public int maxQi_t;
    public float maxQi_p;
    public int maxQi_f;
    //public int qiEffect; //内力值转化护体真气效率 
    public int currentQi; //护体真气
    public int skillStars; //技能水平参考值
   // public GameObject[] attackPositions;


    public float moveSpeed;
    public float moveSpeed_t;
    public float moveSpeed_p;
    public float moveSpeed_f;
    public bool isAlive; //存活标记
    protected bool isDizzy = false;  //眩晕标记
    public bool hitableTag;  //无敌标记
    public bool stableTag; //霸体标记
    protected Animator animator; //
    protected AnimatorOverrideController animatorOverrideController;
    protected AnimatorStateInfo stateInfo;
    protected Rigidbody2D rb2d;


    public float randomMoveTendency;
    public float randomIdleTendency;

    protected Vector3 randomDir; //随机移动方向
    protected Timer stiffnessTimer;   //僵直计时器
    protected Timer dizzyTimer; //眩晕计时器
    protected Timer inhitableTimer;   //无敌计时器
    protected Timer randomIdleTimer; //间歇时长计时器
    protected Timer randomIdleCoolDownTimer; //间歇计时器
    protected Timer randomMoveTimer; //随机移动时长计时器
    protected Timer randomMoveCooldownTimer; //随机移动间隔计时器
    

    public bool canMove;  //是否可移动标记
    public bool onFangun; //是否正在翻滚
    public GameObject hittedEffectPrefab; //受击特效

    //protected bool isStiffness = false;//硬直标记
    public float stiffnessDuration; //硬直时间、
    public float inhitableDuration; //无敌持续时间
    protected bool actionCastTri; //动作是否被打断的标记

    public List<int> learnedSkill_id = new List<int>();
    public int activeSkill_id;
    public InventoryObject skillInventory;


    public float dashTime;
    public float dashSpeed;
    public Vector2 dashDir;
    private PolyNavAgent _agent;
    protected PolyNavAgent agent
    {
        get { return _agent != null ? _agent : _agent = GetComponent<PolyNavAgent>(); }
    }
    public virtual void Init()
    {
        moveSpeed_p = 1;
        moveSpeed_t = 0;
        atk_p = 1;
        atk_t = 0;
        maxHp_p = 1;
        maxHp_t = 0;
        maxQi_p = 1;
        maxQi_t = 0;
    }
    protected virtual void Start()
    {
       
    }

    protected virtual void Awake()
    {
        isAlive = true;
        hitableTag = true;

        animator = gameObject.GetComponent<Animator>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        stiffnessTimer = gameObject.AddComponent<Timer>();
        stiffnessTimer.Duration = stiffnessDuration;
        dizzyTimer = gameObject.AddComponent<Timer>();
        inhitableTimer = gameObject.AddComponent<Timer>();
        inhitableTimer.Duration = inhitableDuration;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {

    }


    public virtual void Inhitable()
    {
        hitableTag = false;
        inhitableTimer.Run();
    }
    public virtual void UnInhitable()
    {
        hitableTag = true;
    }

    /// <summary>
    /// 造成硬直
    /// </summary>
    public virtual void DoStiffness(bool shock = false)
    {
        Inhitable();
        if (!stableTag)
        {
            actionCastTri = false;
            animator.SetBool("Hitted", true);
            stiffnessTimer.Run();
        }
        
    }

  
    /// <summary>
    /// 解除硬直
    /// </summary>
    public virtual void UndoStiffness()
    {
        Debug.Log("undostiffness");     
        //animator.SetBool("Hitted", false);
        Inhitable();

    }
    public virtual void Stun(float stunDuration)
    {
        isDizzy = true;
        animator.SetBool("dizzy", true);
        rb2d.velocity = Vector2.zero;
        dizzyTimer.Duration = stunDuration;
        dizzyTimer.Run();
    }

    public virtual void Sober()
    {
        isDizzy = false;
        animator.SetBool("dizzy", false);
       
    }
    /// <summary>
    /// 角色受击击退
    /// </summary>
    /// <param name="backDir">击退方向</param>
    /// <param name="backFactor">击退距离</param>
    public void KnockBack(Vector2 backDir, float backFactor = 0.3f)
    {
        if (backFactor != 0)
        {
            //backDir = backDir.normalized * backFactor;
            backDir = backDir.normalized;

            LineDriveDash(backDir, backFactor, stiffnessDuration);
            //transform.position = new Vector2(transform.position.x - backDir.x, transform.position.y - backDir.y);
            //Debug.Log("击退" + backFactor + " ?" + backDir);
        }
    }
    

    /// <summary>
    /// 开始移动
    /// </summary>
    protected void StartMoving()
    {
        animator.SetBool("Move", true);
        //Debug.Log(gameObject.name);
    }
    /// <summary>
    /// 停止移动
    /// </summary>
    protected void StopMoving()
    {
        agent.Stop();
        animator.SetBool("Move", false);

    }

    /// <summary>
    /// 进入间歇
    /// </summary>
    public void EnterIdle()
    {
        StopMoving();
        randomIdleTimer.Duration = Random.Range(0.5f, 2f); //随机此次间歇时长
        randomIdleTimer.Run();
       //Debug.Log("开始Idle");

    }
    /// <summary>
    /// 停止间歇
    /// </summary>
    protected void EndIdle()
    {

        StartMoving();
        //Debug.Log("结束Idle");

    }
    public void BuffCalculate()
    {
        moveSpeed_f = (int)((moveSpeed + moveSpeed_t) * moveSpeed_p);
        atk_f = (int)((baseATK + atk_t) * atk_p);
        maxHp_f = (int)((maxHp + maxHp_t) * maxHp_p);
        maxQi_f = (int)((maxQi + maxQi_t) * maxQi_p);
        if(maxQi <0)
        {
            stableTag = true;
        }
        else if(currentQi <= 0)
        {
            stableTag = false;
        }
    }
    public void LineDriveDash(Vector2 dir, float distance, float time = 0.1f)
    {
        dashTime = time;
        dashDir = dir;
        dashSpeed = distance;
    }

    public void DashOverTime()
    {
        if (dashTime <= 0)
        {
            rb2d.velocity = Vector2.zero;
        }
        else
        {
            dashTime -= Time.deltaTime;
            rb2d.velocity = dashDir * dashSpeed;
        }
    }
    ///// <summary>
    ///// 直线冲刺
    ///// </summary>
    ///// <param name="distance"></param>
    //public IEnumerator LineDrive(Vector2 dir, float distance, float time = 0.1f)
    //{
    //    //canMove = false;
    //    float duration = 0.0f;
    //    Vector2 targetPos = transform.position + (Vector3)(distance * dir);
    //    Debug.Log(transform.position + "+" + dir);

    //    while (duration <= time)
    //    {
    //        duration += Time.deltaTime;
    //        transform.position = Vector2.Lerp(transform.position, targetPos, duration / time);
    //        yield return null;
    //    }

    //}
    //public void StartLineDrive(Vector2 dir,float distance, float time = 0.1f)
    //{
    //    StartCoroutine(LineDrive(dir,distance, time));
    //}
}
