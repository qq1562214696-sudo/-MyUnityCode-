using System;

/// <summary>
/// 方法按钮特性，用于将方法转换为Inspector中的可点击按钮
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class 方法按钮 : Attribute
{
    // 无需构造函数参数，直接使用方法名作为按钮文本
}