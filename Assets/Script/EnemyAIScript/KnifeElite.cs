using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeElite : MeleeEnemy
{
  

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (isAlive)
        {
            
           
        }
        else
        {
            canMove = false;
            hitableTag = false;
            DeathComing();
        }
    }




    public override void TakeDamage(int damage)
    {
        if (hitableTag)
        {
            base.TakeDamage(damage);
            //KnockBack(Player.MyInstance.transform.position - transform.position);
            Debug.Log("普通刀兵受到了攻击");
        }
    }
}
