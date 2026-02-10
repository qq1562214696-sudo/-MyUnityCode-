#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

public static class 组件分配器
{
    public static void 为选中对象分配组件(string 标识符)
    {
        GameObject[] 选中对象数组 = Selection.gameObjects;

        if (选中对象数组.Length == 0)
        {
            Debug.LogWarning("没有选中任何对象。");
            return;
        }

        var 可分配类型 = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(程序集 => 程序集.GetTypes())
            .Where(类型 => 类型.IsDefined(typeof(一键添加), false))
            .ToList();

        foreach (var 选中对象 in 选中对象数组)
        {
            foreach (var 类型 in 可分配类型)
            {
                var 属性 = (一键添加)Attribute.GetCustomAttribute(类型, typeof(一键添加));

                if (属性.标识符 == 标识符)
                {
                    if (选中对象.GetComponent(类型) == null)
                    {
                        选中对象.AddComponent(类型);
                        Debug.Log($"已为 {选中对象.name} 添加脚本: {类型.Name}");
                    }
                    else
                    {
                        Debug.LogWarning($"{选中对象.name} 已经挂载了脚本: {类型.Name}");
                    }
                }
            }
        }
    }
}
#endif