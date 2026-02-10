using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public static class 编辑器协程工具
{
    private static readonly List<IEnumerator> 协程列表 = new List<IEnumerator>();

    static 编辑器协程工具()
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

[System.Serializable]
public class 摄像机配置
{
    public Camera 参考摄像机;
    public Vector3 位置偏移;
    [System.NonSerialized] public Camera 实例缓存;
}

public class 截图工具 : EditorWindow
{
    private enum 功能模式 { 截图功能, 缩放功能 }
    private 功能模式 当前模式;

    [SerializeField] private 摄像机配置[] 摄像机列表 = new 摄像机配置[1]; // 初始化数组
    [SerializeField] private List<GameObject> 目标物体列表 = new List<GameObject>();
    [SerializeField] private string 保存路径 = @"D:\360MoveData\Users\s\Desktop\截图";
    [SerializeField] private int 截图宽度 = 2000;
    [SerializeField] private int 截图高度 = 2000;
    [SerializeField] private bool 透明背景 = true;
    [SerializeField] private float 目标大小 = 6.0f;

    private Vector2 滚动位置;
    private bool 运行中;
    private List<bool> 物体原始状态 = new List<bool>();
    private Dictionary<Camera, 摄像机状态> 原始摄像机状态 = new Dictionary<Camera, 摄像机状态>();
    private List<GameObject> 临时摄像机实例 = new List<GameObject>();

    private Vector3 中心点 = new Vector3(0, 0, 0);
    private Vector3 偏移量 = new Vector3(8, 0, 8);
    private GameObject[] 选中物体数组;
    private bool 动画开关状态 = false;

    [MenuItem("❤多功能工具集❤/截图工具_待修改")]
    public static void 打开窗口()
    {
        GetWindow<截图工具>("截图工具");
    }

    void OnGUI()
    {
        滚动位置 = EditorGUILayout.BeginScrollView(滚动位置);
        当前模式 = (功能模式)GUILayout.Toolbar((int)当前模式, new[] { "截图功能", "对象缩放和分布调整" });

        switch (当前模式)
        {
            case 功能模式.截图功能:
                绘制截图界面();
                break;
            case 功能模式.缩放功能:
                绘制缩放界面();
                break;
        }

        EditorGUILayout.EndScrollView();
    }

    void 绘制截图界面()
    {
        SerializedObject 序列化对象 = new SerializedObject(this);

        if (摄像机列表 != null)
        {
            for (int i = 0; i < 摄像机列表.Length; i++)
            {
                if (摄像机列表[i] == null)
                {
                    摄像机列表[i] = new 摄像机配置();
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"摄像机 {i + 1}", EditorStyles.boldLabel);

                摄像机列表[i].参考摄像机 = (Camera)EditorGUILayout.ObjectField("摄像机", 摄像机列表[i].参考摄像机, typeof(Camera), true);

                摄像机列表[i].位置偏移 = EditorGUILayout.Vector3Field("位置偏移", 摄像机列表[i].位置偏移);
            }
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("增加摄像机", GUILayout.Height(40)))
        {
            添加摄像机();
        }

        if (GUILayout.Button("减少摄像机", GUILayout.Height(40)))
        {
            移除摄像机();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(30);

        EditorGUILayout.PropertyField(序列化对象.FindProperty("目标物体列表"), true);
        序列化对象.ApplyModifiedProperties();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("增加 选中对象", GUILayout.Height(40)))
        {
            追加选中物体();
        }

        if (GUILayout.Button("替换 选中对象", GUILayout.Height(40)))
        {
            添加选中物体();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(30);

        保存路径 = EditorGUILayout.TextField("保存路径", 保存路径);

        EditorGUILayout.BeginHorizontal();
        截图宽度 = EditorGUILayout.IntField("截图宽度", 截图宽度);
        截图高度 = EditorGUILayout.IntField("截图高度", 截图高度);
        EditorGUILayout.EndHorizontal();

        透明背景 = EditorGUILayout.Toggle("透明背景", 透明背景);

        GUI.enabled = !运行中;
        if (GUILayout.Button("开始截图", GUILayout.Height(50)))
        {
            编辑器协程工具.启动协程(截图流程());
        }
        GUI.enabled = true;
    }

    void 绘制缩放界面()
    {
        目标大小 = EditorGUILayout.FloatField("目标大小", 目标大小);

        if (GUILayout.Button("执行缩放", GUILayout.Height(50)))
        {
            缩放选中模型();
        }

        EditorGUILayout.Space(30);

        中心点 = EditorGUILayout.Vector3Field("中心点", 中心点);
        偏移量 = EditorGUILayout.Vector3Field("偏移量", 偏移量);

        选中物体数组 = Selection.gameObjects;

        if (GUILayout.Button("分布物体", GUILayout.Height(50)))
        {
            分布物体();
        }

        EditorGUILayout.Space(30);

        EditorGUILayout.BeginHorizontal();
        动画开关状态 = GUILayout.Toggle(动画开关状态, "开启动画器", GUILayout.Height(50));
        if (GUILayout.Button("<=是否开关          开关全部动画器", GUILayout.Height(50)))
        {
            开启动画器();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void 添加选中物体()
    {
        目标物体列表.Clear();
        foreach (GameObject 物体 in Selection.gameObjects)
        {
            if (物体 != null && !目标物体列表.Contains(物体))
            {
                目标物体列表.Add(物体);
            }
        }
    }

    private IEnumerator 截图流程()
    {
        try
        {
            运行中 = true;
            保存原始状态();
            float 总进度 = 目标物体列表.Count * 摄像机列表.Length;
            float 当前进度 = 0;
            bool 用户取消 = false;

            foreach (var 物体 in 目标物体列表)
            {
                if (物体 == null) continue;

                foreach (var obj in 目标物体列表)
                {
                    if (obj != null) obj.SetActive(obj == 物体);
                }

                foreach (var 配置 in 摄像机列表)
                {
                    // 显示带取消按钮的进度条
                    用户取消 = EditorUtility.DisplayCancelableProgressBar(
                        "截图进度",
                        $"正在处理 {物体.name}",
                        当前进度 / 总进度
                    );

                    if (用户取消) break;

                    当前进度++;

                    Camera 当前摄像机 = 获取可用摄像机(配置);
                    if (当前摄像机 == null) continue;

                    定位摄像机(当前摄像机, 配置.位置偏移, 物体.transform);
                    yield return null;

                    var 渲染格式 = 透明背景 ? TextureFormat.RGBA32 : TextureFormat.RGB24;
                    var 渲染纹理 = new RenderTexture(截图宽度, 截图高度, 24);
                    var 截图纹理 = new Texture2D(渲染纹理.width, 渲染纹理.height, 渲染格式, false);

                    当前摄像机.targetTexture = 渲染纹理;
                    当前摄像机.clearFlags = 透明背景 ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
                    if (透明背景) 当前摄像机.backgroundColor = new Color(0, 0, 0, 0);

                    当前摄像机.Render();
                    RenderTexture.active = 渲染纹理;
                    截图纹理.ReadPixels(new Rect(0, 0, 渲染纹理.width, 渲染纹理.height), 0, 0);
                    截图纹理.Apply();

                    // 自动创建目录
                    string 文件路径 = 生成文件名(物体, 当前摄像机);
                    string 保存目录 = Path.GetDirectoryName(文件路径);
                    if (!Directory.Exists(保存目录))
                    {
                        try
                        {
                            Directory.CreateDirectory(保存目录);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"创建目录失败: {e.Message}");
                            continue;
                        }
                    }

                    File.WriteAllBytes(文件路径, 截图纹理.EncodeToPNG());

                    RenderTexture.active = null;
                    当前摄像机.targetTexture = null;
                    DestroyImmediate(渲染纹理);
                    DestroyImmediate(截图纹理);
                }

                if (用户取消) break;
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            恢复原始状态();
            运行中 = false;
        }
    }

    string 生成文件名(GameObject 物体, Camera 摄像机)
    {
        return Path.Combine(保存路径, $"{物体.name}_{摄像机.name}_{DateTime.Now:yyyyMMddHHmmssfff}.png");
    }

    Camera 获取可用摄像机(摄像机配置 配置)
    {
        if (配置.参考摄像机 == null) return null;

        if (配置.实例缓存 != null) return 配置.实例缓存;

        if (!配置.参考摄像机.gameObject.scene.IsValid())
        {
            GameObject 实例化对象 = PrefabUtility.InstantiatePrefab(配置.参考摄像机.gameObject) as GameObject;
            临时摄像机实例.Add(实例化对象);
            配置.实例缓存 = 实例化对象.GetComponent<Camera>();
            return 配置.实例缓存;
        }

        if (!原始摄像机状态.ContainsKey(配置.参考摄像机))
        {
            原始摄像机状态[配置.参考摄像机] = new 摄像机状态
            {
                位置 = 配置.参考摄像机.transform.position,
                旋转 = 配置.参考摄像机.transform.rotation,
                激活状态 = 配置.参考摄像机.gameObject.activeSelf
            };
        }
        return 配置.参考摄像机;
    }

    private Vector3 计算中心坐标(GameObject 物体)
    {
        Renderer[] 所有渲染器 = 物体.GetComponentsInChildren<Renderer>();
        if (所有渲染器.Length == 0) return 物体.transform.position;

        Bounds 总包围盒 = 所有渲染器[0].bounds;
        foreach (Renderer 渲染器 in 所有渲染器)
        {
            总包围盒.Encapsulate(渲染器.bounds);
        }

        return 总包围盒.center;
    }

    void 定位摄像机(Camera 摄像机, Vector3 偏移量, Transform 目标变换)
    {
        Vector3 中心坐标 = 计算中心坐标(目标变换.gameObject);

        Vector3 最终位置 = 中心坐标 + 偏移量;

        摄像机.transform.position = 最终位置;
        摄像机.transform.LookAt(中心坐标);
    }

    private void CollectAllVerticesRecursive(GameObject obj, ref List<Vector3> 所有顶点)
    {
        MeshFilter 网格过滤器 = obj.GetComponent<MeshFilter>();
        if (网格过滤器 != null && 网格过滤器.sharedMesh != null)
        {
            foreach (Vector3 顶点 in 网格过滤器.sharedMesh.vertices)
            {
                所有顶点.Add(obj.transform.TransformPoint(顶点));
            }
        }

        SkinnedMeshRenderer 蒙皮网格渲染器 = obj.GetComponent<SkinnedMeshRenderer>();
        if (蒙皮网格渲染器 != null && 蒙皮网格渲染器.sharedMesh != null)
        {
            foreach (Vector3 顶点 in 蒙皮网格渲染器.sharedMesh.vertices)
            {
                所有顶点.Add(obj.transform.TransformPoint(顶点));
            }
        }

        foreach (Transform 子对象 in obj.transform)
        {
            CollectAllVerticesRecursive(子对象.gameObject, ref 所有顶点);
        }
    }

    void 保存原始状态()
    {
        物体原始状态.Clear();
        foreach (var 物体 in 目标物体列表)
        {
            if (物体 != null) 物体原始状态.Add(物体.activeSelf);
        }
    }

    void 恢复原始状态()
    {
        for (int i = 0; i < 目标物体列表.Count; i++)
        {
            if (目标物体列表[i] != null)
            {
                目标物体列表[i].SetActive(物体原始状态[i]);
            }
        }

        foreach (var 临时实例 in 临时摄像机实例)
        {
            DestroyImmediate(临时实例);
        }
        临时摄像机实例.Clear();

        foreach (var 状态记录 in 原始摄像机状态)
        {
            状态记录.Key.transform.SetPositionAndRotation(状态记录.Value.位置, 状态记录.Value.旋转);
            状态记录.Key.gameObject.SetActive(状态记录.Value.激活状态);
        }
        原始摄像机状态.Clear();

        for (int i = 0; i < 目标物体列表.Count; i++)
        {
            if (目标物体列表[i] != null)
            {
                目标物体列表[i].SetActive(物体原始状态[i]);
            }
        }
    }

    class 摄像机状态
    {
        public Vector3 位置;
        public Quaternion 旋转;
        public bool 激活状态;
    }

    private void 缩放选中模型()
    {
        GameObject[] 选中对象 = Selection.gameObjects;

        if (选中对象.Length == 0)
        {
            Debug.LogWarning("未选中任何模型。");
            return;
        }

        foreach (GameObject 对象 in 选中对象)
        {
            处理对象(对象);
        }
    }

    private void 处理对象(GameObject 对象)
    {
        MeshFilter 网格过滤器 = 对象.GetComponent<MeshFilter>();
        SkinnedMeshRenderer 蒙皮网格渲染器 = 对象.GetComponent<SkinnedMeshRenderer>();

        if (网格过滤器 != null || 蒙皮网格渲染器 != null)
        {
            缩放对象(对象);
            return;
        }

        List<GameObject> 子对象列表 = new List<GameObject>();
        foreach (Transform 子对象 in 对象.transform)
        {
            MeshFilter 子网格过滤器 = 子对象.GetComponent<MeshFilter>();
            SkinnedMeshRenderer 子蒙皮网格渲染器 = 子对象.GetComponent<SkinnedMeshRenderer>();
            if (子网格过滤器 != null || 子蒙皮网格渲染器 != null)
            {
                子对象列表.Add(子对象.gameObject);
            }
        }

        if (子对象列表.Count > 0)
        {
            缩放对象(对象, 子对象列表);
        }
        else
        {
            Debug.LogError($"对象 {对象.name} 及其一级子对象中没有模型或蒙皮。");
        }
    }

    private void 缩放对象(GameObject 对象, List<GameObject> 子对象列表 = null)
    {
        List<Vector3> 所有顶点 = new List<Vector3>();

        if (子对象列表 == null)
        {
            CollectObjectVertices(对象, 所有顶点);
        }
        else
        {
            foreach (GameObject 子对象 in 子对象列表)
            {
                CollectObjectVertices(子对象, 所有顶点);
            }
        }

        if (所有顶点.Count == 0)
        {
            Debug.LogError($"对象 {对象.name} 没有有效的顶点数据。");
            return;
        }

        Bounds 包围盒 = CalculateBounds(所有顶点);
        float 最大尺寸 = 包围盒.size.magnitude;

        if (最大尺寸 <= 0)
        {
            Debug.LogError($"对象 {对象.name} 的尺寸无效。");
            return;
        }

        float 缩放比例 = 目标大小 / 最大尺寸;
        对象.transform.localScale *= 缩放比例;
    }

    private void CollectObjectVertices(GameObject 对象, List<Vector3> 顶点列表)
    {
        MeshFilter 网格过滤器 = 对象.GetComponent<MeshFilter>();
        if (网格过滤器 != null)
        {
            foreach (Vector3 顶点 in 网格过滤器.sharedMesh.vertices)
            {
                顶点列表.Add(对象.transform.TransformPoint(顶点));
            }
        }

        SkinnedMeshRenderer 蒙皮网格渲染器 = 对象.GetComponent<SkinnedMeshRenderer>();
        if (蒙皮网格渲染器 != null)
        {
            foreach (Vector3 顶点 in 蒙皮网格渲染器.sharedMesh.vertices)
            {
                顶点列表.Add(对象.transform.TransformPoint(顶点));
            }
        }
    }

    private Bounds CalculateBounds(List<Vector3> 顶点列表)
    {
        Bounds 包围盒 = new Bounds(顶点列表[0], Vector3.zero);
        foreach (Vector3 顶点 in 顶点列表)
        {
            包围盒.Encapsulate(顶点);
        }
        return 包围盒;
    }

    private void 分布物体()
    {
        if (选中物体数组.Length == 0)
        {
            Debug.Log("未选中任何物体。");
            return;
        }

        int 行数 = Mathf.CeilToInt(Mathf.Sqrt(选中物体数组.Length));
        int 列数 = Mathf.CeilToInt((float)选中物体数组.Length / 行数);

        float 起始X偏移 = -((列数 - 1) * 偏移量.x) / 2f;
        float 起始Z偏移 = -((行数 - 1) * 偏移量.z) / 2f;

        for (int i = 0; i < 选中物体数组.Length; i++)
        {
            Undo.RecordObject(选中物体数组[i].transform, "物体自动分布");
            EditorUtility.SetDirty(选中物体数组[i].transform);

            int 行索引 = i / 列数;
            int 列索引 = i % 列数;

            Vector3 新位置 = 中心点;
            新位置.x += 起始X偏移 + 列索引 * 偏移量.x;
            新位置.z += 起始Z偏移 + 行索引 * 偏移量.z;

            选中物体数组[i].transform.position = 新位置;
        }
    }

    private void 开启动画器()
    {
        Animator[] allAnimators = FindObjectsOfType<Animator>();
        foreach (Animator animator in allAnimators)
        {
            animator.enabled = 动画开关状态;
            Undo.RecordObject(animator, "开关Animator");
            EditorUtility.SetDirty(animator);
        }

        Animation[] allAnimations = FindObjectsOfType<Animation>();
        foreach (Animation animation in allAnimations)
        {
            animation.enabled = 动画开关状态;
            Undo.RecordObject(animation, "开关animation");
            EditorUtility.SetDirty(animation);
        }
    }

    private void 添加摄像机()
    {
        Array.Resize(ref 摄像机列表, 摄像机列表.Length + 1);
        摄像机列表[摄像机列表.Length - 1] = new 摄像机配置();
    }

    private void 移除摄像机()
    {
        if (摄像机列表.Length == 0) return;

        Array.Resize(ref 摄像机列表, 摄像机列表.Length - 1);
    }

    private void 追加选中物体()
    {
        foreach (GameObject 物体 in Selection.gameObjects)
        {
            if (物体 != null && !目标物体列表.Contains(物体))
            {
                目标物体列表.Add(物体);
            }
        }
    }
}