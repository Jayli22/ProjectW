using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_1 : MeleeEnemy
{
    private Vector3 baseAttackStartPos;
    private Vector3 baseAttackTargetPos;
    private float duration;
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

    protected override void BaseAttack(float t)
    {
        if (!actionCastTri)
        {
            duration = 0;
            baseAttackStartPos = transform.position;
            baseAttackTargetPos = transform.position + (Vector3)(0.7f * (playerCharacterPos - transform.position));
            animator.SetTrigger("Attack");
            actionCastTri = true;
            StartCoroutine(TriggerAttack(t));
        }

    }
    /// <summary>
    /// 触发攻击(生成攻击碰撞检测盒子等）
    /// </summary>
    protected override IEnumerator TriggerAttack(float t)
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
    /// 判定角色现应处于的状态
    /// </summary>
    public void StatusDetermination()
    {
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
                BaseAttack(precastTime[0]);
                duration += Time.deltaTime;
                transform.position = Vector2.Lerp(baseAttackStartPos,baseAttackTargetPos,duration/ precastTime[0]);
                if (playerDistance > attackDetectionRadius && !actionCastTri)
                {
                    StatusSwitch(NPCCurrentState.Normal);
                }
                break;
            default:
                break;
        }
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
