using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public partial class 多功能工具集 : EditorWindow
{
    private bool 是否折叠导入按钮 = true;
    private bool 是否折叠预制体按钮 = true;
    private bool 是否折叠材质球通道 = true;
    private bool 是否折叠对象列表 = true;
    private bool 是否折叠排序按钮 = true;

    public static List<贴图分配> 赋值数据 = new List<贴图分配>();

    [Serializable]
    public class 贴图分配
    {
        public bool 是否选中 = true;
        [Tooltip("贴图后缀，限一个字符")]
        public char 后缀;
        public string 属性名称;
    }

    [MenuItem("❤多功能工具集❤/❤多功能工具集❤")]
    static void 显示窗口()
    {
        GetWindow<多功能工具集>("❤多功能工具集❤");
    }

    private Vector2 工具整体滚动位置;
    private Vector2 材质球通道滚动位置;

    private void OnGUI()
    {
        工具整体滚动位置 = EditorGUILayout.BeginScrollView(工具整体滚动位置);

        排序按钮();
        对象列表按钮();
        预制体按钮();
        导入按钮();
        材质球通道按钮();

        EditorGUILayout.EndScrollView();
    }

    private void 导入按钮()
    {
        是否折叠导入按钮 = EditorGUILayout.Foldout(是否折叠导入按钮, "选取文件夹，批量导入文件夹下的包体");
        if (是否折叠导入按钮)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("从项目外导入")) { 从项目外导入包体.从项目外导入(); }
            if (GUILayout.Button("快速从项目内导入")) { 从项目中导入包体.从项目中导入(); }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void 预制体按钮()
    {
        是否折叠预制体按钮 = EditorGUILayout.Foldout(是否折叠预制体按钮, "选中对象，选择文件夹批量制作预制体");
        if (是否折叠预制体按钮)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("创建预制体")) { 预制体批量替换.创建预制体(); }
            if (GUILayout.Button("批量覆盖")) { 预制体批量生成.创建预制体(); }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void 材质球通道按钮()
    {
        是否折叠材质球通道 = EditorGUILayout.Foldout(是否折叠材质球通道, "选中材质球与贴图，批量给材质球贴图");
        if (是否折叠材质球通道)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            材质球通道滚动位置 = EditorGUILayout.BeginScrollView(材质球通道滚动位置, GUILayout.Height(150)); // 固定高度为150

            for (int i = 0; i < 赋值数据.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                // 选择复选框
                赋值数据[i].是否选中 = EditorGUILayout.Toggle(赋值数据[i].是否选中, GUILayout.Width(20));

                // 后缀
                EditorGUILayout.LabelField("后缀", GUILayout.Width(30));
                string 后缀输入 = EditorGUILayout.TextField(赋值数据[i].后缀.ToString(), GUILayout.Width(20));
                if (!string.IsNullOrEmpty(后缀输入))
                {
                    赋值数据[i].后缀 = 后缀输入[0];
                }

                // 对应通道
                EditorGUILayout.LabelField("对应通道", GUILayout.Width(60));
                赋值数据[i].属性名称 = EditorGUILayout.TextField(赋值数据[i].属性名称, GUILayout.ExpandWidth(true));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // 按钮行
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("为选取材质-贴图分配通道", GUILayout.Height(30))) // 固定高度为30
            {
                选中材质球批量贴图.材质球批量贴图();
            }
            if (GUILayout.Button("打印贴图通道", GUILayout.Height(30))) // 固定高度为30
            {
                选中材质球批量贴图.将选中材质球的贴图通道添加到列表();
            }
            if (GUILayout.Button("增加通道", GUILayout.Height(30))) // 固定高度为30
            {
                赋值数据.Add(new 贴图分配());
            }
            if (GUILayout.Button("移除所选", GUILayout.Height(30)) && 赋值数据.Count > 0) // 固定高度为30
            {
                List<int> 移除索引 = new List<int>();
                for (int i = 0; i < 赋值数据.Count; i++)
                {
                    if (赋值数据[i].是否选中)
                    {
                        移除索引.Add(i);
                    }
                }
                移除索引.Reverse();
                foreach (int index in 移除索引)
                {
                    赋值数据.RemoveAt(index);
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }

    private void 对象列表按钮() // 新增对象列表功能
    {
        是否折叠对象列表 = EditorGUILayout.Foldout(是否折叠对象列表, "将选中对象填入列表数组");
        if (是否折叠对象列表)
        {
            if (GUILayout.Button("将选中对象填入列表数组容器"))
            {
                选中对象填入列表数组.将选择对象填充至列表数组();
            }
        }
    }

    private void 排序按钮()
    {
        是否折叠排序按钮 = EditorGUILayout.Foldout(是否折叠排序按钮, "按照命名排序选中对象");
        if (是否折叠排序按钮)
        {
            if (GUILayout.Button("按照命名排序选中对象"))
            {
                排序选中对象.排序();
            }
        }
    }
}