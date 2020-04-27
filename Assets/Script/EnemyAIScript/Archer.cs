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
            baseUpdateInfo();
            DashOverTime();

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
        if (currentStatus == NPCCurrentState.Normal || currentStatus == NPCCurrentState.MoveToPlayer)
        {
            if (playerDistance > attackDetectionRadius)
            {
                if(aimTarget)
                {
                    StopAim();
                }
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
                    StatusSwitch(NPCCurrentState.MoveToPlayer);
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
        if (currentStatus == NPCCurrentState.Hitteed || currentStatus == NPCCurrentState.Shocked)
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
                if (aimTarget == false)
                {
                    BaseAttack();
                }
                else
                {
                    AimTarget();
                }
                if (playerDistance > attackDetectionRadius && !actionCastTri)
                {
                    StatusSwitch(NPCCurrentState.Normal);
                }
                break;
            default:
                break;
        }
    }

    protected override void BaseAttack(float t = 0)
    {
       
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
        baseattackCooldownTimer.Run();
        StopAim();
        StatusSwitch(NPCCurrentState.Normal);
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
        Turn();
        if(aimTimer.Finished)
        {
            TriggerAttack();
            aimTimer.Run();
        }
    }

    private void StopAim()
    {
        animator.SetBool("Aim", false);      
        aimLine.SetActive(false);
        aimTarget = false;
    }
 
    public override void DoStiffness(bool shock = false)
    {
        base.DoStiffness();
        if (aimTarget)
            StopAim();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Barrier" && currentStatus == NPCCurrentState.Shocked)
        {
            stiffnessTimer.Duration = 1f;
            TakeDamage(20);
            DoStiffness();
            Debug.Log("推动推动");
        }
    }
}
