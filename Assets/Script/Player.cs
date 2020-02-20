using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public Image HealthBar;
    public Image uiiSkill_1_CoolDown;
    public Image uiiSkill_2_CoolDown;
    public Image uiiSkill_3_CoolDown;
    public Image uiiSkill_4_CoolDown;
    public Text uitSkill_1_CoolDown;
    public Text uitSkill_2_CoolDown;
    public Text uitSkill_3_CoolDown;
    public Text uitSkill_4_CoolDown;
    /// <summary>
    /// 当前关卡标记
    /// </summary>
    public int levelTag; 
    public enum CurrentState
    {
        Normal,
        BaseAttack,
        Skill,
        MoveSkill,
        Hitteed,
        Move,
    }
    protected Vector3 movement;
    private static Player instance;

    public GameObject[] skillPrefabs;
    public GameObject[] baseAttackPrefabs;
    public BaseSkill[] skills;
    public GameObject[] releasedSkills;
    AnimatorClipInfo[] m_CurrentClipInfo;
    public int baseAttackIndex = 0;
    public int skillIndex = 0;

    public bool canChangeMouseDir;
    private Vector2 mouseDir;
    public float mouseAngle;
    public float tmouseAngle;

    private Collider2D[] hitObjects;

    public GameObject[] attackEffect;
    //private GameObject curAttackEffect;
    //public Vector2 attackPosition;
    public GameObject guideArrow;
  
    public int clickCount; //combo次数判断标记
    public float maxComboDelayTime; //最大combo攻击间隔时间
    private float lastClickTime; //最后一次点击时间
    protected AnimationClipOverrides clipOverrides;

    private Timer skillCoolDownTimer_1;
    private Timer skillCoolDownTimer_2;
    private Timer skillCoolDownTimer_3;
    private Timer skillCoolDownTimer_4;
    private Timer castingTimer;

    private Buffer buffers;

    //public bool comboEffectFirstMark;
    //combo攻击判断标记
    public bool comboMark;//当前combo区间内是否有按下攻击键
    public bool comboEffectMark;
    public float[] baseAttackPreCastTime = { 0.01f, 0.01f, 0.1f };


    public bool isCastingSkill; //释放技能
    public bool isBaseAttack; //基础三连击
    public bool canBaseAttack; //是否可以进行三连击
    public bool canSkill; //是否可以释放技能
    public CurrentState curStatus; //当前状态


    private Timer preCastTimer_1;
    private Timer preCastTimer_2;
    private Timer preCastTimer_3;

    public GameObject attackGuidePrefab;
    public static Player MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player>();
            }
            return instance;
        }
    }


    protected override void Awake()
    {
        base.Awake();
        UIBinding();
        DontDestroyOnLoad(gameObject);
    }
    //角色生成后初始化
    public void Init()
    {
        rb2d = GetComponent<Rigidbody2D>();
        levelTag = 0;
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);

        comboMark = false;
        clickCount = 0;
        comboEffectMark = false;
        //comboEffectFirstMark = false;

        curStatus = CurrentState.Normal;
        StatusSwitch(curStatus);

        buffers = gameObject.AddComponent<Buffer>();
        skillCoolDownTimer_1 = gameObject.AddComponent<Timer>();
        skillCoolDownTimer_2 = gameObject.AddComponent<Timer>();
        skillCoolDownTimer_3 = gameObject.AddComponent<Timer>();
        skillCoolDownTimer_4 = gameObject.AddComponent<Timer>();
        preCastTimer_1 = gameObject.AddComponent<Timer>();
        preCastTimer_2 = gameObject.AddComponent<Timer>();
        preCastTimer_3 = gameObject.AddComponent<Timer>();

        castingTimer = gameObject.AddComponent<Timer>();
        castingTimer.Duration = 0.1f;
        castingTimer.Run();
        skills = new BaseSkill[4];
        releasedSkills = new GameObject[4];
        isBaseAttack = false;
        BaseAttackSwitch();
        SkillSwitch();
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        Init();
       
    }
    protected override void FixedUpdate()
    {
        
    }


    /// <summary>
    /// 设置玩家朝向
    /// </summary>
    private void SetPlayerDirection()
    {
        animator.SetFloat("AttackHorizontal", mouseDir.x);
        animator.SetFloat("AttackVertical", mouseDir.y);
        mouseAngle = ToolsHub.GetAngleBetweenVectors(new Vector2(0, 1), mouseDir);
    }

    protected override void Update()
    {
        base.Update();
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (isAlive)
        {
          
            GetInput();
            AttackGuide();
            if (canMove)
            {
                Move();
            }

            ComboCheck();
            if (stateInfo.IsName("Idle") || stateInfo.IsName("Movement"))
            {
                SetPlayerDirection();
            }
            
            
            
            //if (stateInfo.IsTag("BaseAttack"))
            //{
            //    isBaseAttack = true;
            //    canMove = false;
            //    if (stateInfo.normalizedTime >= 0.9f)
            //    {
            //        canMove = true;
                    
            //        if (canChangeMouseDir)
            //        {
            //            SetPlayerDirection();
            //        }
            //    }
            //}
            //else
            //{
            //    isBaseAttack = false;
            //}

          
            if (castingTimer.Finished && (curStatus == CurrentState.Skill || curStatus == CurrentState.MoveSkill))
            {
                StatusSwitch(CurrentState.Normal);               
            }

            
            mouseDir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            mouseDir = mouseDir.normalized;
            if (canChangeMouseDir)
            {
                SetPlayerDirection();
            }
            UIUpdate();
        }
        else
        {
            UIManager.MyInstance.deadHint.SetActive(true);

        }
    }


    /// <summary>
    /// 获取输入
    /// </summary>
    protected void GetInput()
    {
        movement = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            movement.y += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x -= 1;

        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y -= 1;

        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x += 1;

        }
        movement = movement.normalized;
        if (canBaseAttack)
            {
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J)))
            {
                lastClickTime = Time.time;
                comboMark = true;

                clickCount++;
                // if (clickCount == 0)
                //  {
                //    clickCount = 1;
                // comboMark = true;
                // comboEffectFirstMark = true;
                // }
                if (clickCount == 1)
                {
                    animator.SetBool("Attack", true);
                    animator.SetInteger("AttackCMD", 1);
                    StatusSwitch(CurrentState.BaseAttack);
                }

                clickCount = Mathf.Clamp(clickCount, 0, 3);
            }
        }
        if (canSkill)
        {
            if (Input.GetMouseButtonDown(1)) //Skill1
            {
                if (skillCoolDownTimer_1.Finished)
                {
                    releasedSkills[0] = InstantiateEffectRotate(skillPrefabs[0], mouseAngle);
                    animator.SetTrigger("Skill_1");
                    skillCoolDownTimer_1.Run();
                    DetermineSkillType(releasedSkills[0]);

                }

            }
            if (Input.GetKeyDown(KeyCode.Q)) //Skill2
            {
                if (skillCoolDownTimer_2.Finished)
                {
                    releasedSkills[1] = InstantiateEffectRotate(skillPrefabs[1], mouseAngle); 
                    skillCoolDownTimer_2.Run();
                    animator.SetTrigger("Skill_2");

                    Debug.Log(releasedSkills[1].GetComponent<BaseSkill>().skillType);             
                    DetermineSkillType(releasedSkills[1]);

                }
            }
            if (Input.GetKeyDown(KeyCode.E)) //Skill3
            {
                if (skillCoolDownTimer_3.Finished)
                {
                    releasedSkills[2] = InstantiateEffectRotate(skillPrefabs[2], mouseAngle);
                    animator.SetTrigger("Skill_3");
                    skillCoolDownTimer_3.Run();         
                    DetermineSkillType(releasedSkills[2]);
                }
            }
            if (Input.GetKeyDown(KeyCode.R)) //Skill4
            {
                if (skillCoolDownTimer_4.Finished)
                {
                    releasedSkills[3] = InstantiateEffectRotate(skillPrefabs[3]); 
                    skillCoolDownTimer_4.Run();
                    animator.SetTrigger("Skill_4");

                    DetermineSkillType(releasedSkills[3]);



                }
            }
        }
    }


    /// <summary>
    /// 移动
    /// </summary>
    public void Move()
    {
       
        gameObject.transform.position = gameObject.transform.position + movement * Time.deltaTime * moveSpeed;
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);
        if(movement.magnitude > 0.1f)
        {
            animator.SetFloat("IdleDirectionX", movement.x);
            animator.SetFloat("IdleDirectionY", movement.y);
        }
    }


    /// <summary>
    /// 设置玩家Idel朝向
    /// </summary>
    void SetIdleDir()
    {
        if (stateInfo.IsName("Movement") && movement.magnitude > 0.1)
        {
            animator.SetFloat("IdleDirectionX", movement.x);
            animator.SetFloat("IdleDirectionY", movement.y);
        }
        else
        {
            animator.SetFloat("IdleDirectionX", mouseDir.x);
            animator.SetFloat("IdleDirectionY", mouseDir.y);
        }

    }

    public GameObject InstantiateEffectRotate(GameObject g, float mouseAngle)
    {

        //return Instantiate(g, SwitchAttackPosition(mouseAngle).transform.position, transform.rotation * Quaternion.Euler(0, 0, mouseAngle), transform);
        GameObject n = Instantiate(g, transform);
        //n.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, mouseAngle);
        n.transform.RotateAround(gameObject.transform.position, Vector3.forward, mouseAngle);
        return n;

    }


    public GameObject InstantiateEffectRotate(GameObject g)
    {
        //g.transform.RotateAround(transform.position, Vector3.forward, mouseAngle);
        //g.transform.Rotate(g.transform.position, Vector3.forward, mouseAngle);
        return Instantiate(g, transform);   


    }


    /// <summary>
    /// 三连击系统
    /// </summary>
    void ComboCheck()
    {
              
        if (Time.time - lastClickTime > maxComboDelayTime)
        {
            clickCount = 0;
            //comboEffectMark = false;
            comboMark = false;
            animator.SetInteger("AttackCMD", 0);
            animator.SetBool("Attack", false);
        }
        //noOfClicks = Mathf.Clamp(noOfClicks, 0, 4);
    }


    /// <summary>
    /// 获取玩家属性
    /// </summary>
    /// <returns></returns>
    public int[] GetPlayerStats()
    {
        int[] playerStats = new int[10];
        playerStats[0] = maxHp;
        return playerStats;
    }

    /// <summary>
    /// 动画片段替换
    /// </summary>
    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }


    public void BaseAttackSwitch()
    {
        switch (baseAttackIndex)
        {
            case 0:
                {
                    clipOverrides["BaseAttackUp_1"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_1[0];
                    clipOverrides["BaseAttackUp_2"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_2[0];
                    clipOverrides["BaseAttackUp_3"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_3[0];
                    clipOverrides["BaseAttackUpRight_1"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_1[1];
                    clipOverrides["BaseAttackUpRight_2"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_2[1];
                    clipOverrides["BaseAttackUpRight_3"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_3[1];
                    clipOverrides["BaseAttackRight_1"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_1[2];
                    clipOverrides["BaseAttackRight_2"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_2[2];
                    clipOverrides["BaseAttackRight_3"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_3[2];
                    clipOverrides["BaseAttackDownRight_1"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_1[3];
                    clipOverrides["BaseAttackDownRight_2"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_2[3];
                    clipOverrides["BaseAttackDownRight_3"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_3[3];
                    clipOverrides["BaseAttackDown_1"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_1[4];
                    clipOverrides["BaseAttackDown_2"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_2[4];
                    clipOverrides["BaseAttackDown_3"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_3[4];
                    clipOverrides["BaseAttackDownLeft_1"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_1[5];
                    clipOverrides["BaseAttackDownLeft_2"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_2[5];
                    clipOverrides["BaseAttackDownLeft_3"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_3[5];
                    clipOverrides["BaseAttackLeft_1"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_1[6];
                    clipOverrides["BaseAttackLeft_2"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_2[6];
                    clipOverrides["BaseAttackLeft_3"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_3[6];
                    clipOverrides["BaseAttackUpLeft_1"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_1[7];
                    clipOverrides["BaseAttackUpLeft_2"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_2[7];
                    clipOverrides["BaseAttackUpLeft_3"] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().attack_3[7];
                    animatorOverrideController.ApplyOverrides(clipOverrides);
                    attackEffect = new GameObject[3];
                    attackEffect[0] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().effectPrefabs[0];
                    attackEffect[1] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().effectPrefabs[1];
                    attackEffect[2] = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().effectPrefabs[2];

                    baseAttackPreCastTime = baseAttackPrefabs[0].GetComponent<BaseAttackPrefab>().preCastTime;
                }
                break;
        }
    }
    public void SkillSwitch()
    {
        // switch (skillIndex)
        // {
        //     case 0:
        //         {\

        skills[0] = skillPrefabs[0].GetComponent<BaseSkill>();
        clipOverrides["Skill_1Up"] = skillPrefabs[0].GetComponent<BaseSkill>().dirPrefabs[0];
        clipOverrides["Skill_1UpRight"] = skillPrefabs[0].GetComponent<BaseSkill>().dirPrefabs[1];
        clipOverrides["Skill_1Right"] = skillPrefabs[0].GetComponent<BaseSkill>().dirPrefabs[2];
        clipOverrides["Skill_1DownRight"] = skillPrefabs[0].GetComponent<BaseSkill>().dirPrefabs[3];
        clipOverrides["Skill_1Down"] = skillPrefabs[0].GetComponent<BaseSkill>().dirPrefabs[4];
        clipOverrides["Skill_1DownLeft"] = skillPrefabs[0].GetComponent<BaseSkill>().dirPrefabs[5];
        clipOverrides["Skill_1Left"] = skillPrefabs[0].GetComponent<BaseSkill>().dirPrefabs[6];
        clipOverrides["Skill_1UpLeft"] = skillPrefabs[0].GetComponent<BaseSkill>().dirPrefabs[7];
        skillCoolDownTimer_1.Duration = skillPrefabs[0].GetComponent<BaseSkill>().cooldown;
        skillCoolDownTimer_1.Run();
        skillCoolDownTimer_1.RemainTime = 0.1f;

        skills[1] = skillPrefabs[1].GetComponent<BaseSkill>();
        clipOverrides["Skill_2Up"] = skillPrefabs[1].GetComponent<BaseSkill>().dirPrefabs[0];
        clipOverrides["Skill_2UpRight"] = skillPrefabs[1].GetComponent<BaseSkill>().dirPrefabs[1];
        clipOverrides["Skill_2Right"] = skillPrefabs[1].GetComponent<BaseSkill>().dirPrefabs[2];
        clipOverrides["Skill_2DownRight"] = skillPrefabs[1].GetComponent<BaseSkill>().dirPrefabs[3];
        clipOverrides["Skill_2Down"] = skillPrefabs[1].GetComponent<BaseSkill>().dirPrefabs[4];
        clipOverrides["Skill_2DownLeft"] = skillPrefabs[1].GetComponent<BaseSkill>().dirPrefabs[5];
        clipOverrides["Skill_2Left"] = skillPrefabs[1].GetComponent<BaseSkill>().dirPrefabs[6];
        clipOverrides["Skill_2UpLeft"] = skillPrefabs[1].GetComponent<BaseSkill>().dirPrefabs[7];
        skillCoolDownTimer_2.Duration = skillPrefabs[1].GetComponent<BaseSkill>().cooldown;

        skillCoolDownTimer_2.Run();
        skillCoolDownTimer_2.RemainTime = 0.1f;

        skills[2] = skillPrefabs[2].GetComponent<BaseSkill>();
        clipOverrides["Skill_3Up"] = skills[2].dirPrefabs[0];
        clipOverrides["Skill_3UpRight"] = skillPrefabs[2].GetComponent<BaseSkill>().dirPrefabs[1];
        clipOverrides["Skill_3Right"] = skillPrefabs[2].GetComponent<BaseSkill>().dirPrefabs[2];
        clipOverrides["Skill_3DownRight"] = skillPrefabs[2].GetComponent<BaseSkill>().dirPrefabs[3];
        clipOverrides["Skill_3Down"] = skillPrefabs[2].GetComponent<BaseSkill>().dirPrefabs[4];
        clipOverrides["Skill_3DownLeft"] = skillPrefabs[2].GetComponent<BaseSkill>().dirPrefabs[5];
        clipOverrides["Skill_3Left"] = skillPrefabs[2].GetComponent<BaseSkill>().dirPrefabs[6];
        clipOverrides["Skill_3UpLeft"] = skillPrefabs[2].GetComponent<BaseSkill>().dirPrefabs[7];
        skillCoolDownTimer_3.Duration = skillPrefabs[2].GetComponent<BaseSkill>().cooldown;

        skillCoolDownTimer_3.Run();
        skillCoolDownTimer_3.RemainTime = 0.1f;

        skills[3] = skillPrefabs[3].GetComponent<BaseSkill>();
        clipOverrides["Skill_4Up"] = skills[3].dirPrefabs[0];
        clipOverrides["Skill_4UpRight"] = skillPrefabs[3].GetComponent<BaseSkill>().dirPrefabs[1];
        clipOverrides["Skill_4Right"] = skillPrefabs[3].GetComponent<BaseSkill>().dirPrefabs[2];
        clipOverrides["Skill_4DownRight"] = skillPrefabs[3].GetComponent<BaseSkill>().dirPrefabs[3];
        clipOverrides["Skill_4Down"] = skillPrefabs[3].GetComponent<BaseSkill>().dirPrefabs[4];
        clipOverrides["Skill_4DownLeft"] = skillPrefabs[3].GetComponent<BaseSkill>().dirPrefabs[5];
        clipOverrides["Skill_4Left"] = skillPrefabs[3].GetComponent<BaseSkill>().dirPrefabs[6];
        clipOverrides["Skill_4UpLeft"] = skillPrefabs[3].GetComponent<BaseSkill>().dirPrefabs[7];
        skillCoolDownTimer_4.Duration = skillPrefabs[3].GetComponent<BaseSkill>().cooldown;
        skillCoolDownTimer_4.Run();
        skillCoolDownTimer_4.RemainTime = 0.1f;

        animatorOverrideController.ApplyOverrides(clipOverrides);
        //          }
        //          break;
        //  }
    }
   

    /// <summary>
    /// 直线冲刺
    /// </summary>
    /// <param name="distance"></param>
    public IEnumerator LineDrive(float distance, float time = 0.1f)
    {
        //canMove = false;
        float duration = 0.0f;
        Vector2 targetPos = transform.position + (Vector3)(distance * mouseDir);

        while (duration <= time)
        {
            duration += Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, targetPos, duration/ time);

            yield return null;
        }

    }
    public void StartLineDrive(float distance, float time = 0.1f)
    {
        StartCoroutine(LineDrive(distance, time));
    }


    public void AbleToSetMousePos(bool b)
    {
        canChangeMouseDir = b;
    }

    /// <summary>
    /// 选择攻击位置
    /// </summary>
    /// <param name="attackAngle"></param>
    /// <returns></returns>
    public GameObject SwitchAttackPosition(float attackAngle)
    {
       // Debug.Log(attackAngle);
        GameObject g = attackPositions[0];
        if (337.5 >= attackAngle || attackAngle < 22.5)
        {
            g = attackPositions[0];
        }
        if (292.5 <= attackAngle && attackAngle < 337.5)
        {
            g = attackPositions[1];
        }
        if (247.5 <= attackAngle && attackAngle < 292.5)
        {
            g = attackPositions[2];
        }
        if (202.5 <= attackAngle && attackAngle < 247.5)
        {
            g = attackPositions[3];
        }
        if (157.5 <= attackAngle && attackAngle < 202.5)
        {
            g = attackPositions[4];
        }
        if (112.5 <= attackAngle && attackAngle < 157.5)
        {
            g = attackPositions[5];
        }
        if (67.5 <= attackAngle && attackAngle < 112.5)
        {
            g = attackPositions[6];
        }
        if (22.5 <= attackAngle && attackAngle < 67.5)
        {
            g = attackPositions[7];
        }
        return g;
    }


    public override void TakeDamage(int damage)
    {
        if (hitable)
        {
            //health reduce 
            currentHp -= damage;
            Debug.Log("玩家受到了攻击"+damage);
            Instantiate(hittedEffectPrefab, transform.position, transform.rotation);
            if (currentHp <= 0)
            {
                isAlive = false;
            }
            else
            {
                //DoStiffness();//造成硬直
                Inhitable();
            }
        }
    }
    public override void TakeDamage(int damage, Transform t)
    {
        if (hitable)
        {
            //health reduce 
            currentHp -= damage;
            Instantiate(hittedEffectPrefab, transform.position, transform.rotation);
            if (currentHp <= 0)
            {
                isAlive = false;
                //m_animator.SetBool("death", false);    
            }
            //DoStiffness();//造成硬直
            KnockBack(t.position - transform.position, 0.1f);
            Inhitable();
        }
    }
    public override void TakeDamage(Vector3 pos, float backFactor, int damage)
    {
        if (hitable)
        {
            //health reduce 
            currentHp -= damage;
            Instantiate(hittedEffectPrefab, transform.position, transform.rotation);

            if (currentHp <= 0)
            {
                isAlive = false;
            }
            else
            {
                KnockBack(pos - transform.position, backFactor);
                Inhitable();
            }

        }
    }
    public override void DoStiffness()
    {
        actionCastTri = false;
        animator.SetBool("Hitted", true);
        stiffnessTimer.Run();
        // animator.speed = 0;
        canMove = false;
        isStiffness = true;
    }

    public override void Inhitable()
    {
        hitable = false;
        inhitableTimer.Run();
    }
    public override void UnInhitable()
    {
        hitable = true;
    }

    /// <summary>
    /// 解除硬直
    /// </summary>
    public override void UndoStiffness()
    {
        //stiffnessTime.Run();
        //animator.speed = 1;
        canMove = true;
        isStiffness = false;
        animator.SetBool("Hitted", false);
    }
    private void UIUpdate()
    {
        UIBinding();
        
        HealthBar.fillAmount = (float)currentHp / (float)maxHp;

        uiiSkill_1_CoolDown.fillAmount = 1 - (skills[0].cooldown - skillCoolDownTimer_1.RemainTime) / skills[0].cooldown;
        uiiSkill_2_CoolDown.fillAmount = 1 - (skills[1].cooldown - skillCoolDownTimer_2.RemainTime) / skills[1].cooldown;
        uiiSkill_3_CoolDown.fillAmount = 1 - (skills[2].cooldown - skillCoolDownTimer_3.RemainTime) / skills[2].cooldown;
        uiiSkill_4_CoolDown.fillAmount = 1 - (skills[3].cooldown - skillCoolDownTimer_4.RemainTime) / skills[3].cooldown;
        uitSkill_1_CoolDown.text = ((int)skillCoolDownTimer_1.RemainTime).ToString();
        uitSkill_2_CoolDown.text = ((int)skillCoolDownTimer_2.RemainTime).ToString();
        uitSkill_3_CoolDown.text = ((int)skillCoolDownTimer_3.RemainTime).ToString();
        uitSkill_4_CoolDown.text = ((int)skillCoolDownTimer_4.RemainTime).ToString();
    }

    public void DetermineSkillType(GameObject tSkill)
    {
        BaseSkill skillScript = tSkill.GetComponent<BaseSkill>();

        switch (skillScript.skillType)
        {
            case SkillType.FIXCASTING:
                castingTimer.Duration = skillScript.castingDuration;
                castingTimer.Run();
                animator.SetBool("IsCasting", true);
                skillScript.GetComponent<BaseSkill>().Release();
                StatusSwitch(CurrentState.Skill);
                
                break;
            case SkillType.LINEDRIVECASTING:
                castingTimer.Duration = skillScript.castingDuration;
                castingTimer.Run();
                animator.SetBool("IsCasting", true);
                skillScript.GetComponent<BaseSkill>().Release();
                StatusSwitch(CurrentState.Skill);
                break;
            case SkillType.MOVECASTING:
                castingTimer.Duration = skillScript.castingDuration;
                castingTimer.Run();
                animator.SetBool("IsCasting", true);
                skillScript.GetComponent<BaseSkill>().Release();

                buffers.Active(skillScript.castingDuration);

                StatusSwitch(CurrentState.MoveSkill);

                break;
            case SkillType.NORMALCASTING:
                castingTimer.Duration = skillScript.castingDuration;
                castingTimer.Run();
                animator.SetBool("IsCasting", true);
                skillScript.Release();
                StatusSwitch(CurrentState.Skill);
                break;
        }
    }
    public void UIBinding()
    {
        HealthBar = GameObject.Find("HealthBar").GetComponent<Image>() ;
        uiiSkill_1_CoolDown = GameObject.Find("Skill_1_CoolDownImage").GetComponent<Image>();
        uiiSkill_2_CoolDown = GameObject.Find("Skill_2_CoolDownImage").GetComponent<Image>();
        uiiSkill_3_CoolDown = GameObject.Find("Skill_3_CoolDownImage").GetComponent<Image>();
        uiiSkill_4_CoolDown = GameObject.Find("Skill_4_CoolDownImage").GetComponent<Image>();
        uitSkill_1_CoolDown = GameObject.Find("Skill_1_CoolDownText").GetComponent<Text>();
        uitSkill_2_CoolDown = GameObject.Find("Skill_2_CoolDownText").GetComponent<Text>();
        uitSkill_3_CoolDown = GameObject.Find("Skill_3_CoolDownText").GetComponent<Text>();
        uitSkill_4_CoolDown = GameObject.Find("Skill_4_CoolDownText").GetComponent<Text>();
    }



    public void StatusSwitch(CurrentState currentState)
    {
        switch (currentState)
        {
            case CurrentState.Normal:
                canMove = true;
                isCastingSkill = false;
                isBaseAttack = false;
                curStatus = CurrentState.Normal;
                canChangeMouseDir = true;
                canBaseAttack = true;
                //animator.SetBool("Move", false);
                animator.SetBool("IsCasting", false);
                canSkill = true;
                break;
            case CurrentState.Move:
                isCastingSkill = false;
                canMove = true;
                curStatus = CurrentState.Move;
                canChangeMouseDir = true;
                canBaseAttack = true;
                //animator.SetBool("Move", true);
                canSkill = true;

                break;
            case CurrentState.MoveSkill:
                isCastingSkill = true;
                canMove = true;
                curStatus = CurrentState.MoveSkill;
                canChangeMouseDir = true;
                canBaseAttack = false;
                animator.SetBool("IsCasting", true);
                canSkill = false;

                break;

            case CurrentState.Skill:
                isCastingSkill = true;
                canMove = false;
                curStatus = CurrentState.Skill;
                canChangeMouseDir = false;
                canBaseAttack = false;
                animator.SetBool("IsCasting", true);
                canSkill = false;

                break;
            
            case CurrentState.Hitteed:
                canMove = false;
                curStatus = CurrentState.Hitteed;
                canChangeMouseDir = false;
                canBaseAttack = false;
                canSkill = false;

                break;

            case CurrentState.BaseAttack:
                canMove = false;
                isBaseAttack = true;
                curStatus = CurrentState.BaseAttack;
                canSkill = false;
                canChangeMouseDir = false;

                break;
            default:
                break;
        }
    }
    
    private void AttackGuide()
    {
        //float angle = ToolsHub.GetAngleBetweenVectors(new Vector2(0, 1), ((Vector3)playerCharacterPos - transform.position).normalized);
        //aimLine.transform.RotateAround(gameObject.transform.position, Vector3.forward, angle - tangle);
        //
        attackGuidePrefab.transform.Rotate(Vector3.forward, mouseAngle - tmouseAngle);
        tmouseAngle = mouseAngle;
    }
}
