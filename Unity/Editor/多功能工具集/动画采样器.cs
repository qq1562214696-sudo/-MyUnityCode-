using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;

public class 动画采样窗口 : EditorWindow
{
    [SerializeField] private Animator 播放动画器;
    [SerializeField] private AnimationClip 采样动画;
    [SerializeField] private GameObject 采样对象;
    [SerializeField] private GameObject 轨迹预制体;
    [SerializeField] private List<位置记录> 位置记录列表 = new List<位置记录>();

    [SerializeField] private bool 显示预览 = true;
    [SerializeField] private int 预览帧索引;

    private Vector2 滚动位置;
    private bool 采样中;
    private float 采样进度;
    private bool cinemachine可用 = false;

    private Type cinemachine路径类型;
    private Type 路径点类型;

    [System.Serializable]
    public class 位置记录
    {
        public Vector3 坐标;
        public Quaternion 旋转;
    }

    [MenuItem("❤多功能工具集❤/动画采样工具")]
    public static void 打开窗口()
    {
        GetWindow<动画采样窗口>("动画采样工具");
    }

    private void OnEnable()
    {
        cinemachine可用 = 初始化Cinemachine类型();
    }

    private bool 初始化Cinemachine类型()
    {
        try
        {
            cinemachine路径类型 = Type.GetType("Cinemachine.CinemachineSmoothPath, Cinemachine");
            if (cinemachine路径类型 == null)
            {
                cinemachine路径类型 = Type.GetType("Cinemachine.CinemachinePath, Cinemachine");
            }

            if (cinemachine路径类型 != null)
            {
                路径点类型 = 获取嵌套类型(cinemachine路径类型, "Waypoint");
                if (路径点类型 == null)
                {
                    Type 基础路径类型 = Type.GetType("Cinemachine.CinemachinePathBase, Cinemachine");
                    if (基础路径类型 != null)
                    {
                        路径点类型 = 获取嵌套类型(基础路径类型, "Waypoint");
                    }
                }
            }

            return cinemachine路径类型 != null && 路径点类型 != null;
        }
        catch
        {
            return false;
        }
    }

    private Type 获取嵌套类型(Type 父类型, string 嵌套类型名)
    {
        Type[] 嵌套类型数组 = 父类型.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
        foreach (Type 嵌套类型 in 嵌套类型数组)
        {
            if (嵌套类型.Name == 嵌套类型名)
            {
                return 嵌套类型;
            }
        }
        return null;
    }

    void OnGUI()
    {
        滚动位置 = EditorGUILayout.BeginScrollView(滚动位置);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("采样设置", EditorStyles.boldLabel);
        播放动画器 = (Animator)EditorGUILayout.ObjectField("动画控制器", 播放动画器, typeof(Animator), true);
        采样动画 = (AnimationClip)EditorGUILayout.ObjectField("采样动画", 采样动画, typeof(AnimationClip), false);
        采样对象 = (GameObject)EditorGUILayout.ObjectField("采样对象", 采样对象, typeof(GameObject), true);

        if (采样动画 != null)
        {
            EditorGUILayout.LabelField($"动画帧率: {采样动画.frameRate:F1} FPS");
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("轨迹生成设置", EditorStyles.boldLabel);
        轨迹预制体 = (GameObject)EditorGUILayout.ObjectField("轨迹预制体", 轨迹预制体, typeof(GameObject), false);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField($"已采样帧数: {位置记录列表.Count}", EditorStyles.boldLabel);
        显示预览 = EditorGUILayout.Foldout(显示预览, "轨迹预览", true);
        if (显示预览 && 位置记录列表.Count > 0)
        {
            int 新索引 = EditorGUILayout.IntSlider("预览帧", 预览帧索引, 0, 位置记录列表.Count - 1);
            if (新索引 != 预览帧索引)
            {
                预览帧索引 = 新索引;
                SceneView.RepaintAll();
            }

            if (预览帧索引 >= 0 && 预览帧索引 < 位置记录列表.Count)
            {
                位置记录 记录 = 位置记录列表[预览帧索引];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"位置: {记录.坐标}");
                EditorGUILayout.LabelField($"旋转: {记录.旋转.eulerAngles}");
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space(10);
        GUI.enabled = !采样中 && 播放动画器 != null && 采样动画 != null && 采样对象 != null;
        if (GUILayout.Button("开始采样", GUILayout.Height(40)))
        {
            位置记录列表.Clear();
            编辑器协程工具z.启动协程(采样流程());
        }
        GUI.enabled = true;

        if (采样中)
        {
            Rect 进度条矩形 = EditorGUILayout.GetControlRect(false, 10);
            EditorGUI.ProgressBar(进度条矩形, 采样进度, $"采样中... {Mathf.FloorToInt(采样进度 * 100)}%");
        }

        EditorGUILayout.Space(5);
        GUI.enabled = 位置记录列表.Count > 0 && 轨迹预制体 != null;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("标准生成轨迹", GUILayout.Height(30)))
        {
            标准生成();
        }

        if (GUILayout.Button("依次生成轨迹", GUILayout.Height(30)))
        {
            依次生成();
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;

        if (cinemachine可用 && 位置记录列表.Count > 0)
        {
            EditorGUILayout.Space(10);
            if (GUILayout.Button("生成Cinemachine曲线", GUILayout.Height(30)))
            {
                生成Cinemachine曲线();
            }
        }
        else if (位置记录列表.Count > 0)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("安装Cinemachine包，以支持Cinemachine路径生成。", MessageType.Warning);
        }
        EditorGUILayout.EndScrollView();
    }

    private void 生成Cinemachine曲线()
    {
        if (位置记录列表.Count < 2)
        {
            EditorUtility.DisplayDialog("错误", "需要至少2个点才能生成曲线", "确定");
            return;
        }

        try
        {
            GameObject 路径容器 = new GameObject($"{采样对象.name}_{采样动画.name}_Cinemachine路径");
            路径容器.transform.position = Vector3.zero;

            Component 路径组件 = 路径容器.AddComponent(cinemachine路径类型);

            FieldInfo 路径点字段 = 递归查找字段(cinemachine路径类型, "m_Waypoints");
            if (路径点字段 == null)
            {
                EditorUtility.DisplayDialog("错误", "找不到路径点字段", "确定");
                UnityEngine.Object.DestroyImmediate(路径容器);
                return;
            }

            int 总点数 = Mathf.CeilToInt(位置记录列表.Count);
            总点数 = Mathf.Clamp(总点数, 2, 1000);

            Array 路径点数组 = Array.CreateInstance(路径点类型, 总点数);

            for (int i = 0; i < 总点数; i++)
            {
                float 插值 = i / (float)(总点数 - 1);
                int 索引 = Mathf.FloorToInt(插值 * (位置记录列表.Count - 1));
                索引 = Mathf.Clamp(索引, 0, 位置记录列表.Count - 1);

                object 路径点 = Activator.CreateInstance(路径点类型);

                FieldInfo 位置字段 = 路径点类型.GetField("position", BindingFlags.Public | BindingFlags.Instance);
                if (位置字段 != null)
                {
                    位置字段.SetValue(路径点, 位置记录列表[索引].坐标);
                }

                FieldInfo 旋转字段 = 路径点类型.GetField("rotation", BindingFlags.Public | BindingFlags.Instance);
                if (旋转字段 != null)
                {
                    旋转字段.SetValue(路径点, 位置记录列表[索引].旋转);
                }

                路径点数组.SetValue(路径点, i);
            }

            路径点字段.SetValue(路径组件, 路径点数组);

            Selection.activeGameObject = 路径容器;
            EditorGUIUtility.PingObject(路径容器);

            EditorUtility.DisplayDialog("成功", $"已生成曲线路径: {路径容器.name}", "确定");
        }
        catch
        {
            EditorUtility.DisplayDialog("错误", "生成曲线时发生异常", "确定");
        }
    }

    private FieldInfo 递归查找字段(Type 类型, string 字段名)
    {
        FieldInfo 字段 = 类型.GetField(字段名, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (字段 != null) return 字段;

        if (类型.BaseType != null && 类型.BaseType != typeof(object))
        {
            return 递归查找字段(类型.BaseType, 字段名);
        }

        return null;
    }

    private IEnumerator 采样流程()
    {
        if (!验证组件())
        {
            yield break;
        }

        bool 原播放状态 = EditorApplication.isPlaying;
        EditorApplication.isPlaying = false;

        AnimationMode.StartAnimationMode();
        AnimationMode.BeginSampling();

        try
        {
            float 采样间隔 = 1f / 采样动画.frameRate;
            int 总帧数 = Mathf.CeilToInt(采样动画.length / 采样间隔);

            for (int i = 0; i <= 总帧数; i++)
            {
                float 时间点 = Mathf.Min(i * 采样间隔, 采样动画.length);
                AnimationMode.SampleAnimationClip(播放动画器.gameObject, 采样动画, 时间点);

                位置记录列表.Add(new 位置记录
                {
                    坐标 = 采样对象.transform.position,
                    旋转 = 采样对象.transform.rotation
                });

                采样进度 = (float)i / 总帧数;

                if (i % 10 == 0) yield return null;
            }
        }
        finally
        {
            AnimationMode.EndSampling();
            AnimationMode.StopAnimationMode();
            EditorApplication.isPlaying = 原播放状态;
            采样中 = false;
        }
    }

    private bool 验证组件()
    {
        if (播放动画器 == null)
        {
            EditorUtility.DisplayDialog("错误", "未指定动画控制器", "确定");
            return false;
        }
        if (采样对象 == null)
        {
            EditorUtility.DisplayDialog("错误", "未指定采样对象", "确定");
            return false;
        }
        if (采样动画 == null)
        {
            EditorUtility.DisplayDialog("错误", "未指定采样动画", "确定");
            return false;
        }
        if (播放动画器.runtimeAnimatorController == null)
        {
            EditorUtility.DisplayDialog("错误", "动画器缺少控制器", "确定");
            return false;
        }
        return true;
    }

    private void 标准生成()
    {
        GameObject 容器 = new GameObject($"{采样对象.name}_{采样动画.name}_轨迹_标准");
        容器.transform.position = Vector3.zero;

        for (int i = 0; i < 位置记录列表.Count; i++)
        {
            位置记录 记录 = 位置记录列表[i];
            GameObject 实例 = PrefabUtility.InstantiatePrefab(轨迹预制体) as GameObject;

            if (实例 != null)
            {
                实例.transform.position = 记录.坐标;
                实例.transform.rotation = 记录.旋转;
                实例.name = $"{采样对象.name}_{i:000}";
                实例.transform.SetParent(容器.transform);
            }
        }

        Selection.activeGameObject = 容器;
    }

    private void 依次生成()
    {
        GameObject 容器 = new GameObject($"{采样对象.name}_{采样动画.name}_轨迹_依次");
        容器.transform.position = Vector3.zero;

        for (int i = 0; i < 位置记录列表.Count - 1; i++)
        {
            位置记录 记录 = 位置记录列表[i];
            GameObject 实例 = PrefabUtility.InstantiatePrefab(轨迹预制体) as GameObject;

            if (实例 == null) continue;

            Vector3 当前方向 = (位置记录列表[i + 1].坐标 - 记录.坐标).normalized;

            if (i > 0)
            {
                Vector3 前一个方向 = (记录.坐标 - 位置记录列表[i - 1].坐标).normalized;
                Vector3 平滑方向 = (前一个方向 + 当前方向).normalized;
                实例.transform.rotation = Quaternion.LookRotation(平滑方向);
            }
            else
            {
                实例.transform.rotation = Quaternion.LookRotation(当前方向);
            }

            实例.transform.position = 记录.坐标;
            实例.name = $"{采样对象.name}_{i:000}";
            实例.transform.SetParent(容器.transform);
        }

        Selection.activeGameObject = 容器;
    }

    void OnDestroy()
    {
        if (采样中)
        {
            AnimationMode.EndSampling();
            AnimationMode.StopAnimationMode();
        }
    }
}

public static class 编辑器协程工具z
{
    private static readonly List<IEnumerator> 协程列表 = new List<IEnumerator>();

    static 编辑器协程工具z()
    {
        EditorApplication.update += 更新;
    }

    public static void 启动协程(IEnumerator 协程)
    {
        协程列表.Add(协程);
    }

    private static void 更新()
    {
        for (int i = 协程列表.Count - 1; i >= 0; i--)
        {
            if (!协程列表[i].MoveNext())
            {
                协程列表.RemoveAt(i);
            }
        }
    }
}