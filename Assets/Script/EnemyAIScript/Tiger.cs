using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger : MeleeEnemy
{
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
            base.Update();

            if (!isStiffness)
            {
                AttackDetection(0.4f);
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
            //KnockBack(Player.MyInstance.transform.position - transform.position);
            //UndoStiffness();
            Debug.Log(name + "受到了攻击");
        }
    }

}
