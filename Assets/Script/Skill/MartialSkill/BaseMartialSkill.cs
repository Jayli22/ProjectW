using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseMartialSkill : ReleasedPrefab
{
    [Tooltip("武功技能id，唯一")]
    public int skillId;
    [Tooltip("技能类型")]
    public MartialSkillType skillType;
    [Tooltip("技能名称")]
    public string skillName;
    private Timer destoryTimer;
    protected Animator animator;
    protected Collider2D[] hitObjects;
    protected List<Collider2D> hittedObjects;
    protected Timer castingTimer;  //施法硬直时间
    protected Timer effectDurationTimer; //效果持续时间
    protected Timer precastTimer; //施法前摇
    public AnimationClip[] dirPrefabs;
    public float animatorSpeed;

    public int damage;
    public string description;
    [Tooltip("击退倍率")]
    public float knockbackFactor;
    [Tooltip("眩晕目标的时间长度")]
    public float stunDuration;
    public float cooldown;
    [Tooltip("持续施法技能，持续施法时间")]
    public float castingDuration;
    [Tooltip("效果持续时间")]
    public float effectDuration;
    [Tooltip("施法前摇时间")]
    public float precastDuration;
    public float direction; //施法时角度

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        // destoryTimer = gameObject.AddComponent<Timer>();
        // destoryTimer.Duration = 0.5f;
        hitObjects = new Collider2D[50];
        hittedObjects = new List<Collider2D>();
        // destoryTimer.Run();
    }
    protected virtual void Start()
    {
      
    }

    // Update is called once per frame
    protected virtual void Update()
    {
       
    }
    /// <summary>
    /// 命中检测，碰撞体内是否有敌人
    /// 
    /// </summary>
    protected virtual void HitCheck(bool shock = false)
    {
        Collider2D[] p = new Collider2D[500];
        Physics2D.OverlapCollider(this.GetComponent<Collider2D>(), new ContactFilter2D(), p);
        if(p.Length>0)
        {

            foreach (Collider2D op in p)
        {
                if(op != null)
                {
                   // op.gameObject.SetActive(false);

                    if (op.tag == "Enemy")
                    {

                        if (!hittedObjects.Contains(op))
                        {
                            hittedObjects.Add(op);
                            Damage(op , shock);
                        }
                    }
                }
            }          
        }
    }

    protected void Damage(bool shock = false)
    {
        if (hitObjects.Length > 0)
        {
            foreach (Collider2D hit in hitObjects)
            {
                if (hit != null)
                {
                    //Debug.Log(hit.name);
                    if (hit.tag == "Enemy")
                    {
                        hit.GetComponent<Enemy>().TakeDamage(Player.MyInstance.transform.position, knockbackFactor * hit.GetComponent<Enemy>().backFactor,damage , shock);
                        // Debug.Log(hit.name + "受到了攻击");
                    }
                }
            }
        }
    }
    protected void Damage(Collider2D c , bool shock = false)
    {
            if (c.tag == "Enemy")
            {
                c.GetComponent<Enemy>().TakeDamage(transform.position, knockbackFactor * c.GetComponent<Enemy>().backFactor, damage , shock);
        }
    }


    /// <summary>
    /// 眩晕敌人
    /// </summary>
    protected void Stun()
    {
        if (hitObjects.Length > 0)
        {
            foreach (Collider2D hit in hitObjects)
            {
                if (hit != null)
                {
                    //Debug.Log(hit.name);
                    if (hit.tag == "Enemy")
                    {
                        hit.GetComponent<Enemy>().Stun(stunDuration);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 释放技能
    /// </summary>
    public virtual void Release()
    {

    }

    protected virtual void AfterCreate()
    {
       // Player.MyInstance.AbleToSetMousePos(false);
       // Debug.Log("鼠标位置不再改变了");
    }
    
    protected virtual void BeforeDestory()
    {
        Player.MyInstance.AbleToSetMousePos(true);

    }
    
    protected void TryDestory()
    {
       // BeforeDestory();
        Destroy(gameObject);
    }
}
