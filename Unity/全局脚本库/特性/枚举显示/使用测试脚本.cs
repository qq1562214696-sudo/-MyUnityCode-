using System;
using UnityEditor;
using UnityEngine;

// 外层类
public class 我的组件 : MonoBehaviour
{
    public 我的枚举 模式;  // 控制枚举

    [枚举显示("模式", 1)] // 当模式等于1时显示
    public string 模式1时显示;

    [枚举显示("模式", 2)] // 当模式等于2时显示
    public Vector3 模式2时显示;

    public 嵌套数据 数据Z;

    public 嵌套数据[] 数据;
}

// 嵌套类
[Serializable]
public class 嵌套数据
{
    public 我的枚举 模式;  // 控制枚举

    [枚举显示("模式", 1)] // 当模式等于1时显示
    public string 模式1时显示;

    [枚举显示("模式", 2)] // 当模式等于2时显示
    public Vector3 模式2时显示;
}

public enum 我的枚举 { 模式0, 模式1, 模式2 }