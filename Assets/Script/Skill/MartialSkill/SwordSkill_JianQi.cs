using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkill_JianQi : BaseMartialSkill
{
    public float speed;
    private bool hittable ;
    private bool hitted;
    protected override void Awake()
    {
        base.Awake();
        effectDurationTimer = gameObject.AddComponent<Timer>();
        precastTimer = gameObject.AddComponent<Timer>();
        transform.parent = null;
        hittable = false;
        hitted = false;
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
        if (precastTimer.Finished)
        {
            Vector3 movement = Quaternion.AngleAxis(direction, Vector3.forward) * new Vector3(0, 1, 0);
            gameObject.transform.position = gameObject.transform.position + movement * Time.deltaTime * speed;
            hittable = true;
        }
        if (effectDurationTimer.Finished)
        {
            TryDestory();
        }
        if(hittable)
        {
            HitCheck();
        }
    }

    public override void Release()
    {
        base.Release();
    }

    protected override void HitCheck(bool shock = false)
    {
        Collider2D[] p = new Collider2D[500];
        Physics2D.OverlapCollider(this.GetComponent<Collider2D>(), new ContactFilter2D(), p);
        if (p.Length > 0)
        {

            foreach (Collider2D op in p)
            {
                if (op != null)
                {
                    // op.gameObject.SetActive(false);

                    if (op.tag == "Enemy" || op.tag == "Barrier")
                    {
                        hitted = true;
                        Debug.Log("hitted");
                        if (op.tag == "Enemy")
                        {
                            if (!hittedObjects.Contains(op))
                            {
                                hittedObjects.Add(op);
                                Damage(op,shock);
                            }
                        }
                    }
                }
            }
            if(hitted)
            {
                TryDestory();
            }
        }
    }
}
