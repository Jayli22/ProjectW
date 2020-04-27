using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeSkill_LuoDiMengJi : BaseMartialSkill
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        effectDurationTimer = gameObject.AddComponent<Timer>();
        precastTimer = gameObject.AddComponent<Timer>();
        transform.parent = null;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        effectDurationTimer.Duration = effectDuration;
        effectDurationTimer.Run();
        precastTimer.Duration = precastDuration;
        precastTimer.Run();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(precastTimer.Finished)
        {
            HitCheck(true);
            GetComponent<Collider2D>().enabled = false;
        }
        if (effectDurationTimer.Finished)
        {
            TryDestory();
        }
    }

    public override void Release()
    {
        base.Release();
    }

}
