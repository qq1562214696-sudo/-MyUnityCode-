#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

/// <summary>
/// MonoBehaviour的方法按钮编辑器，在Inspector中显示标记了[方法按钮]的方法
/// </summary>
[CustomEditor(typeof(MonoBehaviour), true)]
public class 方法按钮编辑器 : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // 获取目标对象的类型
        var 类型 = target.GetType();
        // 获取所有实例方法（包括公共和非公共）
        var 方法集合 = 类型.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // 遍历所有方法，查找标记了[方法按钮]的方法
        foreach (var 方法 in 方法集合)
        {
            var 按钮特性 = 方法.GetCustomAttribute<方法按钮>();
            if (按钮特性 != null)
            {
                // 创建按钮，点击时调用对应的方法
                if (GUILayout.Button(方法.Name))
                {
                    try
                    {
                        方法.Invoke(target, null);
                    }
                    catch (TargetInvocationException e)
                    {
                        Debug.LogError($"调用方法 {方法.Name} 时发生错误: {e.InnerException?.Message ?? e.Message}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"调用方法 {方法.Name} 时发生异常: {e.Message}");
                    }
                }
            }
        }
    }
}

/// <summary>
/// ScriptableObject的方法按钮编辑器，在Inspector中显示标记了[方法按钮]的方法
/// </summary>
[CustomEditor(typeof(ScriptableObject), true)]
public class 通用ScriptableObject编辑器 : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // 获取目标对象的类型
        var 类型 = target.GetType();
        // 获取所有实例方法（包括公共和非公共）
        var 方法集合 = 类型.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // 遍历所有方法，查找标记了[方法按钮]的方法
        foreach (var 方法 in 方法集合)
        {
            var 按钮特性 = 方法.GetCustomAttribute<方法按钮>();
            if (按钮特性 != null)
            {
                // 创建按钮，点击时调用对应的方法
                if (GUILayout.Button(方法.Name))
                {
                    try
                    {
                        方法.Invoke(target, null);
                    }
                    catch (TargetInvocationException e)
                    {
                        Debug.LogError($"调用方法 {方法.Name} 时发生错误: {e.InnerException?.Message ?? e.Message}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"调用方法 {方法.Name} 时发生异常: {e.Message}");
                    }
                }
            }
        }
    }
}
#endif