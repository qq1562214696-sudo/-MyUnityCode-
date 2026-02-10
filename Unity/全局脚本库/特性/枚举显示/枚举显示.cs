using System;
using UnityEditor;
using UnityEngine;

// 属性类定义必须放在所有UNITY_EDITOR条件外
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class 枚举显示 : PropertyAttribute
{
    public readonly string 控制枚举名称;
    public readonly int 显示条件值;

    public 枚举显示(string 控制枚举名称, int 显示条件值)
    {
        this.控制枚举名称 = 控制枚举名称;
        this.显示条件值 = 显示条件值;
    }
}