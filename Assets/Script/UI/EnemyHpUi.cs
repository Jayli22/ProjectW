using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpUi : MonoBehaviour
{
    private Transform hpBar;
    // Start is called before the first frame update
    void Start()
    {
        hpBar = transform.Find("HpBar");
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.localScale = new Vector3((float)GetComponentInParent<Character>().currentHp / GetComponentInParent<Character>().maxHp, 1, 1);
    }
}
