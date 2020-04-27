using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkill_JianZhen : BaseMartialSkill
{
    public float damageCheckCoolDown;
    protected bool damageCheckTrigger;
    protected Timer damageCheckTimer;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        effectDurationTimer = gameObject.AddComponent<Timer>();
        damageCheckTimer = gameObject.AddComponent<Timer>();
        transform.parent = null;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        effectDurationTimer.Duration = effectDuration;
        effectDurationTimer.Run();
        damageCheckTimer.Duration = damageCheckCoolDown;
        damageCheckTrigger = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (effectDurationTimer.Finished)
        {
            TryDestory();
        }
        if (damageCheckTrigger)
        {
            hittedObjects = new List<Collider2D>();
            damageCheckTimer.Run();
            HitCheck();
            damageCheckTrigger = false;
        }
        if (damageCheckTimer.Finished)
        {
            damageCheckTrigger = true;
        }
    }

    public override void Release()
    {
        base.Release();
    }
 
}
