using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class TransformTransferToolEditor
{
    [MenuItem("❤多功能工具集❤/应用父级变换_待修改")]
    static void ApplyParentTransforms()
    {
        Transform parentObject = Selection.activeTransform;
        if (parentObject == null)
        {
            Debug.LogError("No parent object selected.");
            return;
        }

        // 记录父对象的位置、旋转和缩放
        Vector3 parentPosition = parentObject.position;
        Vector3 parentRotation = parentObject.eulerAngles;
        Vector3 parentScale = parentObject.localScale;

        foreach (Transform child in parentObject)
        {
            // 将父对象的位置和旋转应用到子对象
            // 使用世界坐标系下的位置叠加
            child.position = parentObject.TransformPoint(parentObject.localPosition + child.localPosition);

            // 旋转叠加
            child.rotation *= Quaternion.Euler(parentRotation);

            // 使用Vector3.Scale来实现元素级别的乘法
            child.localScale = Vector3.Scale(child.localScale, parentScale);
        }

        // 重置父对象的位置、旋转和缩放
        Undo.RecordObject(parentObject.gameObject, "Reset Parent Transform");
        parentObject.position = Vector3.zero;
        parentObject.rotation = Quaternion.identity;
        parentObject.localScale = Vector3.one;
    }
}