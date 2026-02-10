using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System;

public class 选中对象填入列表数组
{
    public static void 将选择对象填充至列表数组()
    {
        GameObject[] 所选对象 = Selection.gameObjects;

        string 时间戳 = DateTime.Now.ToString("mm分ss秒");

        GameObject 选择选中对象列表数组容器 = new GameObject($"选中对象列表数组容器_{时间戳}");

        GameObject[] 选中对象数组 = new GameObject[所选对象.Length];
        List<GameObject> 选中对象列表 = new List<GameObject>();

        for (int i = 0; i < 所选对象.Length; i++)
        {
            选中对象数组[i] = 所选对象[i];
            选中对象列表.Add(所选对象[i]);
        }

        Undo.RegisterCreatedObjectUndo(选择选中对象列表数组容器, $"创建 {选择选中对象列表数组容器.name}");

        var collectorComponent = 选择选中对象列表数组容器.AddComponent<选择选中对象列表数组容器脚本>();
        Undo.RecordObject(collectorComponent, "初始化容器组件");

        collectorComponent.选中对象数组 = 选中对象数组;
        collectorComponent.选中对象列表 = 选中对象列表;

        Undo.RecordObject(collectorComponent, "更新选中对象数组和列表");

        StringBuilder 对象名字列表 = new StringBuilder();
        for (int i = 0; i < 所选对象.Length; i++)
        {
            if (i > 0) 对象名字列表.Append("、");
            对象名字列表.Append(所选对象[i].name);
        }

        Debug.Log($"成功将: {对象名字列表} 填入到: {选择选中对象列表数组容器.name}");
    }
}

public class 选择选中对象列表数组容器脚本 : MonoBehaviour
{
    public GameObject[] 选中对象数组;
    public List<GameObject> 选中对象列表;
}