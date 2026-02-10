#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(场景Attribute))]
public class 场景特性绘制器 : PropertyDrawer
{
    public override void OnGUI(Rect 位置, SerializedProperty 属性, GUIContent 标签)
    {
        if (属性.propertyType != SerializedPropertyType.Integer &&
            属性.propertyType != SerializedPropertyType.String)
        {
            Debug.LogError("场景特性只能用于int或string字段，请纠正代码！！！");
            EditorGUI.LabelField(位置, "错误", "场景特性只能用于int或string字段");
            return;
        }

        SceneAsset 场景资源 = 获取当前场景资源(属性);

        EditorGUI.BeginChangeCheck();
        场景资源 = (SceneAsset)EditorGUI.ObjectField(
            位置,
            标签,
            场景资源,
            typeof(SceneAsset),
            false
        );

        if (EditorGUI.EndChangeCheck())
        {
            处理场景变更(属性, 场景资源);
        }
    }

    private SceneAsset 获取当前场景资源(SerializedProperty 属性)
    {
        if (属性.propertyType == SerializedPropertyType.Integer)
        {
            int 索引 = 属性.intValue;
            if (索引 >= 0 && 索引 < EditorBuildSettings.scenes.Length)
            {
                string 路径 = EditorBuildSettings.scenes[索引].path;
                return AssetDatabase.LoadAssetAtPath<SceneAsset>(路径);
            }
            return null;
        }
        else
        {
            string 场景名称 = 属性.stringValue;
            if (!string.IsNullOrEmpty(场景名称))
            {
                foreach (var 场景 in EditorBuildSettings.scenes)
                {
                    if (System.IO.Path.GetFileNameWithoutExtension(场景.path) == 场景名称)
                    {
                        return AssetDatabase.LoadAssetAtPath<SceneAsset>(场景.path);
                    }
                }
            }
            return null;
        }
    }

    private void 处理场景变更(SerializedProperty 属性, SceneAsset 场景资源)
    {
        if (场景资源 == null)
        {
            if (属性.propertyType == SerializedPropertyType.Integer)
                属性.intValue = -1;
            else
                属性.stringValue = string.Empty;
            return;
        }

        string 路径 = AssetDatabase.GetAssetPath(场景资源);
        if (string.IsNullOrEmpty(路径))
        {
            Debug.LogError("无效的场景资源");
            return;
        }

        int 构建索引 = 添加场景到构建设置(路径);

        if (属性.propertyType == SerializedPropertyType.Integer)
        {
            属性.intValue = 构建索引;
        }
        else
        {
            属性.stringValue = 场景资源.name;
        }
    }

    private int 添加场景到构建设置(string 路径)
    {
        EditorBuildSettingsScene[] 原始设置 = EditorBuildSettings.scenes;
        for (int i = 0; i < 原始设置.Length; i++)
        {
            if (原始设置[i].path == 路径)
                return i;
        }

        EditorBuildSettingsScene[] 新设置 = new EditorBuildSettingsScene[原始设置.Length + 1];
        System.Array.Copy(原始设置, 新设置, 原始设置.Length);
        新设置[原始设置.Length] = new EditorBuildSettingsScene(路径, true);
        EditorBuildSettings.scenes = 新设置;

        return 原始设置.Length;
    }
}
#endif