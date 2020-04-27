using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff触发条件类别，如出生，被命中
/// </summary>
public enum BuffTriggerEventType 
{
    /// <summary>
    /// 创建时
    /// </summary>
   OnCreate = 0,
   /// <summary>
   /// 激活时
   /// </summary>
   OnActive = 1,

   OnHitted = 2,
}
