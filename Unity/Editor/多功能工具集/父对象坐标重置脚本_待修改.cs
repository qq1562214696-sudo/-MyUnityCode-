using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class 父对象坐标重置脚本
{
    [MenuItem("❤多功能工具集❤/选中父对象坐标重置_待修改")]
    static void 重置父对象坐标()
    {
        Transform 选中父对象坐标 = Selection.activeTransform;
        if (选中父对象坐标 == null)
        {
            Debug.LogError("未选中父对象");
            return;
        }

        // 记录父对象的位置、旋转和缩放
        Vector3 位置 = 选中父对象坐标.position;
        Vector3 旋转值 = 选中父对象坐标.eulerAngles;
        Vector3 缩放值 = 选中父对象坐标.localScale;

        foreach (Transform 子对象坐标 in 选中父对象坐标)
        {
            // 将父对象的位置和旋转应用到子对象
            // 使用世界坐标系下的位置叠加
            子对象坐标.position = 选中父对象坐标.TransformPoint(选中父对象坐标.localPosition + 子对象坐标.localPosition);

            // 旋转叠加
            子对象坐标.rotation *= Quaternion.Euler(旋转值);

            // 使用Vector3.Scale来实现元素级别的乘法
            子对象坐标.localScale = Vector3.Scale(子对象坐标.localScale, 缩放值);
        }

        // 重置父对象的位置、旋转和缩放
        Undo.RecordObject(选中父对象坐标.gameObject, "重置父对象");
        选中父对象坐标.position = Vector3.zero;
        选中父对象坐标.rotation = Quaternion.identity;
        选中父对象坐标.localScale = Vector3.one;
    }
}