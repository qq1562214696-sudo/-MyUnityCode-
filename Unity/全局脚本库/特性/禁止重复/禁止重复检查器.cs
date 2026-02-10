#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 禁止重复组件的编辑器检查器，在层级结构变化时自动检查并移除重复组件
/// </summary>
[InitializeOnLoad]
public class 禁止重复检查器
{
    static 禁止重复检查器()
    {
        // 监听场景层级变化事件
        EditorApplication.hierarchyChanged += 当场景层级变化时;
    }

    private static void 当场景层级变化时()
    {
        // 获取场景中所有GameObject
        var 所有对象 = GameObject.FindObjectsOfType<GameObject>();

        foreach (var 对象 in 所有对象)
        {
            // 获取当前GameObject上的所有MonoBehaviour组件
            var 组件列表 = 对象.GetComponents<MonoBehaviour>();
            // 记录已存在的唯一标识
            var 已见标识集合 = new HashSet<string>();

            foreach (var 组件 in 组件列表)
            {
                if (组件 == null) continue;

                // 获取组件类型上的禁止重复特性
                var 组件类型 = 组件.GetType();
                var 唯一属性 = (禁止重复)System.Attribute.GetCustomAttribute(组件类型, typeof(禁止重复));

                if (唯一属性 != null)
                {
                    string 唯一标识 = 唯一属性.唯一标识;

                    // 检查是否有重复的唯一标识
                    if (已见标识集合.Contains(唯一标识))
                    {
                        Debug.LogWarning($"检测到重复挂载的脚本: {组件类型.Name} (唯一标识: {唯一标识})，已移除新的脚本。", 对象);
                        UnityEngine.Object.DestroyImmediate(组件);
                    }
                    else
                    {
                        已见标识集合.Add(唯一标识);
                    }
                }
            }
        }
    }
}
#endif