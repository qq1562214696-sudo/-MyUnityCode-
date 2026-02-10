#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class 随机初始值Attribute : PropertyAttribute
{
    public readonly float 最小值;
    public readonly float 最大值;

    public 随机初始值Attribute(float 最小值, float 最大值)
    {
        this.最小值 = 最小值;
        this.最大值 = 最大值;
    }

    public 随机初始值Attribute(int 最小值, int 最大值)
    {
        this.最小值 = 最小值;
        this.最大值 = 最大值;
    }
}

[CustomPropertyDrawer(typeof(随机初始值Attribute))]
public class 随机初始值Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var 随机初始值Attribute = (随机初始值Attribute)attribute;
        var fieldInfo = fieldInfoFromProperty(property);

        if (fieldInfo != null)
        {
            switch (Type.GetTypeCode(fieldInfo.FieldType))
            {
                case TypeCode.Single: // float
                    if (property.floatValue == 0) // 仅在值为默认值时生成随机值
                    {
                        property.floatValue = GetValue<float>(随机初始值Attribute.最小值, 随机初始值Attribute.最大值);
                    }
                    EditorGUI.PropertyField(position, property, label);
                    break;
                case TypeCode.Int32: // int
                    if (property.intValue == 0)
                    {
                        property.intValue = GetValue<int>((int)随机初始值Attribute.最小值, (int)随机初始值Attribute.最大值);
                    }
                    EditorGUI.PropertyField(position, property, label);
                    break;
                case TypeCode.Int64: // long
                    if (property.longValue == 0)
                    {
                        property.longValue = GetValue<long>((long)随机初始值Attribute.最小值, (long)随机初始值Attribute.最大值);
                    }
                    EditorGUI.PropertyField(position, property, label);
                    break;
                case TypeCode.Double:
                    if (property.doubleValue == 0)
                    {
                        property.doubleValue = GetValue<double>(随机初始值Attribute.最小值, 随机初始值Attribute.最大值);
                    }
                    EditorGUI.PropertyField(position, property, label);
                    break;
                case TypeCode.Byte:
                    if (property.intValue == 0)
                    {
                        property.intValue = GetValue<byte>((byte)随机初始值Attribute.最小值, (byte)随机初始值Attribute.最大值);
                    }
                    EditorGUI.PropertyField(position, property, label);
                    break;
                case TypeCode.SByte:
                    if (property.intValue == 0)
                    {
                        property.intValue = GetValue<sbyte>((sbyte)随机初始值Attribute.最小值, (sbyte)随机初始值Attribute.最大值);
                    }
                    EditorGUI.PropertyField(position, property, label);
                    break;
                case TypeCode.UInt32: // uint
                    if (property.intValue == 0)
                    {
                        property.intValue = (int)GetValue<uint>((uint)随机初始值Attribute.最小值, (uint)随机初始值Attribute.最大值);
                    }
                    EditorGUI.PropertyField(position, property, label);
                    break;
                case TypeCode.UInt64: // ulong
                    if (property.longValue == 0)
                    {
                        property.longValue = (long)GetValue<ulong>((ulong)随机初始值Attribute.最小值, (ulong)随机初始值Attribute.最大值);
                    }
                    EditorGUI.PropertyField(position, property, label);
                    break;
                case TypeCode.UInt16: // ushort
                    if (property.intValue == 0)
                    {
                        property.intValue = (int)GetValue<ushort>((ushort)随机初始值Attribute.最小值, (ushort)随机初始值Attribute.最大值);
                    }
                    EditorGUI.PropertyField(position, property, label);
                    break;
                default:
                    EditorGUI.PropertyField(position, property, label);
                    break;
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    private FieldInfo fieldInfoFromProperty(SerializedProperty prop)
    {
        string[] separatedPaths = prop.propertyPath.Split('.');
        System.Object targetObject = prop.serializedObject.targetObject;
        FieldInfo field = null;

        foreach (var path in separatedPaths)
        {
            field = targetObject.GetType().GetField(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) break;
            targetObject = field.GetValue(targetObject);
        }

        return field;
    }

    private T GetValue<T>(float 最小值, float 最大值) where T : IComparable
    {
        if (typeof(T) == typeof(float))
        {
            return (T)(object)UnityEngine.Random.Range(最小值, 最大值);
        }
        else if (typeof(T) == typeof(int))
        {
            return (T)(object)UnityEngine.Random.Range((int)最小值, (int)最大值 + 1);
        }
        else if (typeof(T) == typeof(long))
        {
            long min = (long)最小值;
            long max = (long)最大值;
            return (T)(object)(min + (long)(UnityEngine.Random.value * (max - min + 1)));
        }
        else if (typeof(T) == typeof(double))
        {
            return (T)(object)((double)最小值 + UnityEngine.Random.value * ((double)最大值 - (double)最小值));
        }
        else if (typeof(T) == typeof(byte))
        {
            return (T)(object)(byte)UnityEngine.Random.Range((byte)最小值, (byte)最大值 + 1);
        }
        else if (typeof(T) == typeof(sbyte))
        {
            return (T)(object)(sbyte)UnityEngine.Random.Range((sbyte)最小值, (sbyte)最大值 + 1);
        }
        else if (typeof(T) == typeof(uint))
        {
            uint min = (uint)最小值;
            uint max = (uint)最大值;
            return (T)(object)(min + (uint)(UnityEngine.Random.value * (max - min + 1)));
        }
        else if (typeof(T) == typeof(ulong))
        {
            ulong min = (ulong)最小值;
            ulong max = (ulong)最大值;
            return (T)(object)(min + (ulong)(UnityEngine.Random.value * (max - min + 1)));
        }
        else if (typeof(T) == typeof(ushort))
        {
            ushort min = (ushort)最小值;
            ushort max = (ushort)最大值;
            return (T)(object)(ushort)(min + (ushort)(UnityEngine.Random.value * (max - min + 1)));
        }
        else
        {
            throw new NotSupportedException($"Type {typeof(T)} is not supported.");
        }
    }
}
#endif