#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(枚举显示))]
public class 枚举显示属性绘制器 : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty 属性, GUIContent 标签)
    {
        var 特性 = (枚举显示)base.attribute;
        SerializedProperty 控制枚举属性 = 获取控制枚举属性(属性, 特性);

        if (控制枚举属性 != null && !是否显示属性(控制枚举属性, 特性))
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        return base.GetPropertyHeight(属性, 标签);
    }

    public override void OnGUI(Rect 位置, SerializedProperty 属性, GUIContent 标签)
    {
        var 特性 = (枚举显示)base.attribute;
        SerializedProperty 控制枚举属性 = 获取控制枚举属性(属性, 特性);

        if (控制枚举属性 != null && 是否显示属性(控制枚举属性, 特性))
        {
            EditorGUI.PropertyField(位置, 属性, 标签, true);
        }
    }

    private SerializedProperty 获取控制枚举属性(SerializedProperty 属性, 枚举显示 特性)
    {
        string 父路径 = GetParentPropertyPath(属性.propertyPath);
        SerializedProperty 父属性 = null;

        if (!string.IsNullOrEmpty(父路径))
        {
            父属性 = 属性.serializedObject.FindProperty(父路径);
        }

        if (父属性 != null)
        {
            SerializedProperty 控制属性 = 父属性.FindPropertyRelative(特性.控制枚举名称);
            if (控制属性 != null)
            {
                return 控制属性;
            }
        }

        return 属性.serializedObject.FindProperty(特性.控制枚举名称);
    }

    private string GetParentPropertyPath(string propertyPath)
    {
        int lastDotIndex = propertyPath.LastIndexOf('.');
        return lastDotIndex == -1 ? "" : propertyPath.Substring(0, lastDotIndex);
    }

    private bool 是否显示属性(SerializedProperty 控制枚举属性, 枚举显示 特性)
    {
        int 当前枚举值 = 控制枚举属性.enumValueIndex;
        return 当前枚举值 == 特性.显示条件值;
    }
}
#endif