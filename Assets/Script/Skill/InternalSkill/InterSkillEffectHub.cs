using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 内功效果脚本仓库，当匹配某个内功标签时，将调用此处的方法
/// </summary>
public  class InterSkillEffectHub 
{
   /// <summary>
   /// 根据数值增加某个单位的移动速度
   /// </summary>
   /// <param name="gameObjects"></param>
    public static void SpeedUpByValue(GameObject[] gameObjects, float value)
    {
        foreach(GameObject g in gameObjects)
        {
            g.GetComponent<Character>().moveSpeed_t += value;
        }
    }
    public static void SpeedUpByValue(GameObject gameObject, float value)
    {
            gameObject.GetComponent<Character>().moveSpeed_t += value;
       
    }
    public static void SpeedDownByValue(GameObject[] gameObjects, float value)
    {
        foreach (GameObject g in gameObjects)
        {
            g.GetComponent<Character>().moveSpeed_t -= value;
        }
    }

    public static void SpeedDownByValue(GameObject gameObject, float value)
    {
        gameObject.GetComponent<Character>().moveSpeed_t -= value;

    }

    public static void QiUpByValue(GameObject gameObject, int value)
    {
        gameObject.GetComponent<Character>().maxQi_t += value;

    }
    public static void QiDownByValue(GameObject gameObject, int value)
    {
        gameObject.GetComponent<Character>().maxQi_t -= value;

    }
}

