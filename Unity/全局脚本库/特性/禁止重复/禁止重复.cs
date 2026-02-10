using System;

/// <summary>
/// 禁止重复特性，用于标记不允许在同一GameObject上重复挂载的组件
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class 禁止重复 : Attribute
{
    /// <summary>
    /// 组件的唯一标识，用于区分不同类型的唯一性检查
    /// </summary>
    public string 唯一标识 { get; private set; }

    /// <summary>
    /// 构造函数，指定组件的唯一标识
    /// </summary>
    /// <param name="唯一标识">用于区分不同组件的唯一字符串</param>
    public 禁止重复(string 唯一标识)
    {
        this.唯一标识 = 唯一标识;
    }
}