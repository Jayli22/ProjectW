using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    private Timer destoryTimer;
    private Animator animator;
    private Collider2D[] hitObjects;
    private List<Collider2D> hittedObjects;
    public float rotateAngle;
    private Vector3 moveDir;
    public int damage = 20;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //destoryTimer = gameObject.AddComponent<Timer>();
        //hitObjects = new Collider2D[50];
        moveDir = (Player.MyInstance.transform.position - transform.position).normalized;

        //destoryTimer.Duration = 3f;
        //destoryTimer.Run();
        transform.RotateAround(transform.position, Vector3.forward, rotateAngle);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDir * 6f * Time.deltaTime;

    }

    //void HitCheck()
    //{
    //    ContactFilter2D c = new ContactFilter2D();
    //    c.useTriggers = true;
    //    Physics2D.OverlapCollider(this.GetComponent<BoxCollider2D>(), c, hitObjects);
    //    if (hitObjects.Length > 0)
    //    {
    //        foreach (Collider2D hit in hitObjects)
    //        {
    //            if (hit != null)
    //            {
    //                //Debug.Log(hit.name);
    //                if (hit.tag == "Player")
    //                {
    //                    hit.GetComponent<Player>().TakeDamage(10);
    //                    //KnockBack(hit);
    //                    // Debug.Log(hit.name + "受到了攻击");
    //                }
    //            }

    //        }

    //    }
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.GetComponent<Player>().TakeDamage(damage);
            Debug.Log(collision.transform.name + "受到了攻击");
        }
        Destroy(gameObject);

    }

    //protected void KnockBack(Collider2D c)
    //{
    //    if (c.tag == "Enemy")
    //    {
    //        c.GetComponent<Enemy>().KnockBack(Player.MyInstance.transform.position - c.transform.position, knockBackFactor);
    //    }

    //}
}
