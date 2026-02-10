#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class 标签选择 : PropertyAttribute { }

[CustomPropertyDrawer(typeof(标签选择))]
public class 标签选择绘制器 : PropertyDrawer
{
    public override void OnGUI(Rect 位置, SerializedProperty 属性, GUIContent 标签)
    {
        if (属性.propertyType == SerializedPropertyType.String)
        {
            属性.stringValue = EditorGUI.TagField(位置, 标签, 属性.stringValue);
        }
        else
        {
            EditorGUI.PropertyField(位置, 属性, 标签);
        }
    }
}
#endif