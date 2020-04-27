using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpUi : MonoBehaviour
{
    public Transform hpBar;
    public Transform qiSprite;
    public Sprite breakSheild;
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("????");
    }

    // Update is called once per frame
    private void Update()
    {

        hpBar.localScale = new Vector3((float)GetComponentInParent<Character>().currentHp / GetComponentInParent<Character>().maxHp, 1f, 1f);

        if (GetComponentInParent<Character>().maxQi < 0)
        {

        }
        else
        {
            if (!GetComponentInParent<Character>().stableTag)
            {
                transform.Find("QiBorder").GetComponent<SpriteRenderer>().sprite = breakSheild;
            }
            if (GetComponentInParent<Character>().maxQi_f == 0)
            {
                qiSprite.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
            }
            else
            {
                qiSprite.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, (float)GetComponentInParent<Character>().currentQi / GetComponentInParent<Character>().maxQi);

            }
        }

    }
}
