using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff目标，如自己，周围敌人
/// </summary>
public enum BuffTargetType 
{
    /// <summary>
    /// 自身
    /// </summary>
   Self = 0,
   
   /// <summary>
   /// 目标敌人
   /// </summary>
   TargetEnemy = 1,


   /// <summary>
   /// 半径内敌人
   /// </summary>
   RadiusEnemy = 2,
}
