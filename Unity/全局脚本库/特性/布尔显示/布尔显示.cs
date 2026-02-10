using System;
using UnityEngine;

/// <summary>
/// 布尔条件显示特性，用于控制字段在编辑器中的显示逻辑
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class 布尔显示 : PropertyAttribute
{
    // 条件字段的名称（需要在同一作用域内存在的布尔字段）
    public readonly string 条件字段名称;
    // 条件字段需要满足的值（true/false）
    public readonly bool 条件值;

    /// <summary>
    /// 构造函数，指定条件字段和目标值
    /// </summary>
    /// <param name="条件字段名称">用于判断的布尔字段名称</param>
    /// <param name="条件值">字段需要满足的布尔值</param>
    public 布尔显示(string 条件字段名称, bool 条件值)
    {
        this.条件字段名称 = 条件字段名称;
        this.条件值 = 条件值;
    }
}