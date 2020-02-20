using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackEffect : MonoBehaviour
{
    private Timer destoryTimer;
    private Animator animator;
    private Collider2D[] hitObjects;
    private List<Collider2D> hittedObjects;
    public float rotateAngle;
    public float knockBackFactor;
    public float duration;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        destoryTimer = gameObject.AddComponent<Timer>();
        hitObjects = new Collider2D[50];

        destoryTimer.Duration = duration;
        destoryTimer.Run();
        transform.RotateAround(Player.MyInstance.transform.position, Vector3.forward, rotateAngle);
        //transform.Rotate(Player.MyInstance.transform.position,  rotateAngle);

    }
    void Start()
    {
        
        HitCheck();

    }

    // Update is called once per frame
    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime>= 1.0f )
        {
            Destroy(gameObject);
        }
    }

    void HitCheck()
    {
        ContactFilter2D c = new ContactFilter2D();
        c.useTriggers = true;
        Physics2D.OverlapCollider(GetComponent<PolygonCollider2D>(), c, hitObjects);
        if (hitObjects.Length > 0)
        {
            foreach (Collider2D hit in hitObjects)
            {
                if (hit != null)
                {
                    //Debug.Log(hit.name);
                    if (hit.tag == "Enemy")
                    {
                        hit.GetComponent<Enemy>().TakeDamage(Player.MyInstance.transform.position, knockBackFactor * hit.GetComponent<Enemy>().backFactor, 10 );
                        //Debug.Log(hit.name + "受到了攻击");
                    }
                }

            }

        }
    }


}
