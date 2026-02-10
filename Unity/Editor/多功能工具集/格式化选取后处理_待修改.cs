using UnityEngine;
using UnityEditor;

public class 格式化选取后处理_待修改 : Editor
{
    [MenuItem("❤多功能工具集❤/规范选中的后处理和反射球_待修改", false, 90)]
    static void 一键格式化()
    {
        GameObject[] 选中物体 = Selection.gameObjects;
        if (选中物体.Length == 0)
        {
            Debug.LogWarning("请先选择要格式化的物体");
            return;
        }

        int 处理后处理数量 = 0;
        int 处理反射球数量 = 0;

        foreach (GameObject 物体 in 选中物体)
        {
            bool 处理了后处理 = false;
            bool 处理了反射球 = false;

            BoxCollider 后处理 = 物体.GetComponent<BoxCollider>();
            if (后处理 != null && 后处理.isTrigger)
            {
                处理后处理(物体, 后处理);
                处理了后处理 = true;
                处理后处理数量++;
            }

            ReflectionProbe 反射球 = 物体.GetComponent<ReflectionProbe>();
            if (反射球 != null)
            {
                处理反射球(物体, 反射球);
                处理了反射球 = true;
                处理反射球数量++;
            }

            if (处理了后处理 || 处理了反射球)
            {
                EditorUtility.SetDirty(物体);
            }
        }

        string 结果信息 = $"已处理 {选中物体.Length} 个物体: ";
        if (处理后处理数量 > 0) 结果信息 += $"{处理后处理数量} 个后处理 ";
        if (处理反射球数量 > 0) 结果信息 += $"{处理反射球数量} 个反射球";

        Debug.Log(结果信息);
    }

    static void 处理后处理(GameObject 物体, BoxCollider 后处理)
    {
        Undo.RecordObjects(new Object[] { 物体.transform, 后处理 }, "格式化后处理");

        Vector3 原始位置 = 物体.transform.position;
        Vector3 原始缩放 = 物体.transform.localScale;

        Vector3 中心 = 后处理.center;
        物体.transform.position += 物体.transform.TransformDirection(中心);
        后处理.center = Vector3.zero;

        后处理.size = new Vector3(
            后处理.size.x * 原始缩放.x,
            后处理.size.y * 原始缩放.y,
            后处理.size.z * 原始缩放.z
        );
        物体.transform.localScale = Vector3.one;
    }

    static void 处理反射球(GameObject 物体, ReflectionProbe 反射球)
    {
        Undo.RecordObjects(new Object[] { 物体.transform, 反射球 }, "格式化反射球");

        Vector3 原始位置 = 物体.transform.position;

        Vector3 中心偏移 = 反射球.center;
        物体.transform.position += 物体.transform.TransformDirection(中心偏移);
        反射球.center = Vector3.zero;
        物体.transform.localScale = Vector3.one;
    }
}