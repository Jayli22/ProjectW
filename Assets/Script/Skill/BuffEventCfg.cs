using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffEventCfg
{
    /// <summary>
    /// 该BUFF的类型
    /// </summary>
   [EnumPaging, Tooltip("Buff的类型")]
    public BuffType buffType;
    [Tooltip("Buff的持续时间，0即为持续型BUFF")]
    public float duration;
    /// <summary>
    /// 该BUFF的触发情况
    /// </summary>
    [Tooltip("Buff的触发条件")]
    public List<BuffTriggerEventType> triggerTypes = new List<BuffTriggerEventType>();

    /// <summary>
    /// 该BUFF的触发前置条件
    /// </summary>
    [Tooltip("Buff的触发前置条件")]
    public List<BuffTriggerConditionType> triggerPreConditions = new List<BuffTriggerConditionType>();

    /// <summary>
    /// 目标类型
    /// </summary>
    [Tooltip("Buff的针对目标类型")]
    public BuffTargetType targetTypes;

    /// <summary>
    /// 目标过滤
    /// </summary>
    [Tooltip("Buff的目标过滤")]
    public List<BuffTargetFilterType> targetFilterTypes = new List<BuffTargetFilterType>();

    /// <summary>
    /// 何种数值
    /// </summary>
    [Tooltip("数值类BUFF的类型")]
    public List<BuffSetValueType> setValueTypes = new List<BuffSetValueType>();

    /// <summary>
    /// 数值来源
    /// </summary>
    [Tooltip("数值类Buff的数据源")]
    public List<BuffSetValueSourceType> SetValueSourceTypes = new List<BuffSetValueSourceType>();

    /// <summary>
    /// 效果叠加类型
    /// </summary>
    [Tooltip("Buff的叠加类型")]
    public List<BuffOverlapType> overlapTypes = new List<BuffOverlapType>();
}
