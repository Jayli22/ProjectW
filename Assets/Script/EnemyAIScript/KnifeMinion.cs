using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeMinion : MeleeEnemy
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

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


    //void CheckCollision()
    //{
    //    RaycastHit2D hit = new RaycastHit2D();

    //    if (targetDirection.x != 0)
    //    {
    //        hit = Physics2D.Raycast(rb2d.position, Vector2.right * targetDirection.x , 1, ~(1<<9|1<<8));


    //        Debug.DrawRay(rb2d.position, Vector2.right * targetDirection.x, Color.yellow);

    //    }

    //    else if (targetDirection.y != 0)
    //    {
    //        hit = Physics2D.Raycast(rb2d.position, Vector2.up * targetDirection.y ,~( 1 << 9 | 1 << 8));


    //        Debug.DrawRay(rb2d.position, Vector2.up * targetDirection.y, Color.yellow);
    //    }

    //    if (hit.collider != null && hit.distance <= 0.5)
    //    {
    //        rb2d.velocity = Vector2.zero;
    //        Debug.Log(hit.collider);
    //    }
    //    else
    //    {
    //        rb2d.velocity = targetDirection * 2;
    //    }

    //}

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
                if(baseattackCooldownTimer.Finished)
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
                if(randomIdleTimer.Finished)
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
