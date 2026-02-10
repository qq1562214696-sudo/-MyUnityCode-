#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

public class 组件分配菜单
{
    [MenuItem("❤多功能工具集❤/一键添加组件_待修改")]
    public static void 根据标识符分配组件()
    {
        string 标识符 = 编辑器输入对话框.显示("一键添加组件", "输入组件对应字符串:");

        if (!string.IsNullOrEmpty(标识符))
        {
            组件分配器.为选中对象分配组件(标识符);
        }
    }
}

public class 编辑器输入对话框 : EditorWindow
{
    private string 输入文本;
    private Action<string> 确认回调;

    public static string 显示(string 标题, string 提示)
    {
        var 窗口 = CreateInstance<编辑器输入对话框>();
        窗口.titleContent = new GUIContent(标题);
        窗口.输入文本 = 提示;
        窗口.ShowModal();
        return 窗口.输入文本;
    }

    private void OnGUI()
    {
        GUILayout.Label(输入文本);
        输入文本 = EditorGUILayout.TextField(输入文本);

        if (GUILayout.Button("一键添加组件"))
        {
            Close();
        }
    }
}
#endif