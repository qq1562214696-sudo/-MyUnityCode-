#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 布尔显示特性的编辑器绘制器，控制字段在Inspector中的显示逻辑
/// </summary>
[CustomPropertyDrawer(typeof(布尔显示))]
public class 布尔显示属性绘制器 : PropertyDrawer
{
    /// <summary>
    /// 绘制字段的GUI
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 获取特性实例
        布尔显示 attributeInstance = (布尔显示)attribute;
        // 查找条件字段的SerializedProperty
        SerializedProperty conditionProperty = GetConditionalProperty(property, attributeInstance.条件字段名称);

        // 判断是否需要显示当前字段
        bool shouldDisplay = true;
        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
        {
            // 条件字段存在且为布尔类型时，比较值
            shouldDisplay = conditionProperty.boolValue == attributeInstance.条件值;
        }
        else if (conditionProperty == null)
        {
            // 条件字段不存在时警告
            Debug.LogWarning($"布尔显示特性找不到条件字段: {attributeInstance.条件字段名称}");
        }

        // 满足条件时绘制字段
        if (shouldDisplay)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    /// <summary>
    /// 获取字段的高度（不显示时高度为0）
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        布尔显示 attributeInstance = (布尔显示)attribute;
        SerializedProperty conditionProperty = GetConditionalProperty(property, attributeInstance.条件字段名称);

        // 判断是否需要显示
        bool shouldDisplay = true;
        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
        {
            shouldDisplay = conditionProperty.boolValue == attributeInstance.条件值;
        }

        // 显示时返回正常高度，否则返回0（隐藏字段）
        return shouldDisplay ? EditorGUI.GetPropertyHeight(property, label) : 0;
    }

    /// <summary>
    /// 查找条件字段的SerializedProperty（支持嵌套类和数组）
    /// </summary>
    private SerializedProperty GetConditionalProperty(SerializedProperty property, string conditionalFieldName)
    {
        // 获取当前字段的父路径（处理嵌套结构）
        string parentPath = GetParentPropertyPath(property.propertyPath);
        SerializedProperty parentProperty = null;

        // 查找父属性
        if (!string.IsNullOrEmpty(parentPath))
        {
            parentProperty = property.serializedObject.FindProperty(parentPath);
        }

        // 从父属性中查找条件字段（优先处理嵌套结构）
        if (parentProperty != null)
        {
            SerializedProperty conditionProperty = parentProperty.FindPropertyRelative(conditionalFieldName);
            if (conditionProperty != null)
            {
                return conditionProperty;
            }
        }

        // 直接从序列化对象中查找条件字段（顶级字段）
        return property.serializedObject.FindProperty(conditionalFieldName);
    }

    /// <summary>
    /// 获取当前字段路径的父路径（用于处理嵌套结构）
    /// </summary>
    private string GetParentPropertyPath(string propertyPath)
    {
        int lastDotIndex = propertyPath.LastIndexOf('.');
        return lastDotIndex == -1 ? "" : propertyPath.Substring(0, lastDotIndex);
    }
}
#endif