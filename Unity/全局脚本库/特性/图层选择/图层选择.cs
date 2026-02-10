#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class 图层选择 : PropertyAttribute { }

[CustomPropertyDrawer(typeof(图层选择))]
public class 图层选择绘制器 : PropertyDrawer
{
    public override void OnGUI(Rect 位置, SerializedProperty 属性, GUIContent 标签)
    {
        if (属性.propertyType == SerializedPropertyType.Integer)
        {
            属性.intValue = EditorGUI.LayerField(位置, 标签, 属性.intValue);
        }
        else
        {
            EditorGUI.PropertyField(位置, 属性, 标签);
        }
    }
}
#endif