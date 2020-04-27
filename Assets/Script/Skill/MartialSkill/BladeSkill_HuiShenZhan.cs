using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeSkill_HuiShenZhan : BaseMartialSkill
{
    public float rushDistance;
    public float damageCheckCoolDown;
    protected bool damageCheckTrigger;
    protected Timer damageCheckTimer;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        effectDurationTimer = gameObject.AddComponent<Timer>();
        damageCheckTimer = gameObject.AddComponent<Timer>();
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
        Phase_1();
    }
    public void Phase_1()
    {
        Player.MyInstance.LineDriveDash(Player.MyInstance.mouseDir, rushDistance, 0.1f);

    }

}
