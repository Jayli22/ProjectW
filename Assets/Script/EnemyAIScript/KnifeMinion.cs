using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeMinion : MeleeEnemy
{
    protected override void Start()
    {
        base.Start();
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
                AlarmRadiusDetection();

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

            Debug.Log("普通刀兵受到了攻击");
        }
    }

}
