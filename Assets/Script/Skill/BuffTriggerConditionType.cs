using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff触发前置条件，如每隔多少秒CD
/// </summary>
public enum BuffTriggerConditionType 
{
    /// <summary>
    /// 常驻BUFF
    /// </summary>
    None = 0,
    /// <summary>
    /// 持续时长
    /// </summary>
    TimeDuration = 1,
    /// <summary>
    /// 时间间隔
    /// </summary>
   TimeIntervals = 2,

   /// <summary>
   /// 攻击次数间隔
   /// </summary>
   AttackCount = 3,
}
