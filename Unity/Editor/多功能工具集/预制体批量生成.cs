using UnityEngine;
using UnityEditor;
using System.IO;

public class 预制体批量生成
{
    public static void 创建预制体()
    {
        // 弹出文件夹选择对话框让用户选择保存路径
        string 初始路径 = "Assets";
        string 完整文件夹路径 = EditorUtility.OpenFolderPanel("选择预制体保存目标文件夹", 初始路径, "");

        // 确保用户选择了有效路径
        if (!string.IsNullOrEmpty(完整文件夹路径))
        {
            // 转换为相对于Assets的路径
            string 相对路径 = "Assets" + 完整文件夹路径.Substring(Application.dataPath.Length); // 简化处理路径
            
            // 确保目标文件夹在Assets下存在
            if (!Directory.Exists(相对路径))
            {
                Debug.LogError($"所选文件夹 '{相对路径}' 在Assets目录下不存在。");
                return;
            }

            // 获取当前选择的所有对象
            GameObject[] 选定对象 = Selection.gameObjects;
            
            // 检查是否有对象被选中
            if (选定对象.Length == 0)
            {
                Debug.LogWarning("未选中任何对象。预制体创建终止。");
                return;
            }
            
            foreach (GameObject 对象 in 选定对象)
            {
                // 构造预制体的相对路径
                string 预制体路径 = Path.Combine(相对路径, 对象.name + ".prefab");
                
                // 检查预制体是否已存在
                if (File.Exists(预制体路径))
                {
                    // 弹出对话框询问用户
                    bool 用户选择覆盖 = EditorUtility.DisplayDialog("预制体已存在",
                                                                     $"预制体'{对象.name}.prefab'已存在。是否覆盖?",
                                                                     "覆盖", "跳过");
                    
                    if (用户选择覆盖)
                    {
                        // 用户选择覆盖，保存预制体
                        PrefabUtility.SaveAsPrefabAsset(对象, 预制体路径);
                        Debug.LogFormat("预制体已覆盖: {0}", 预制体路径);
                    }
                    else
                    {
                        Debug.LogFormat("跳过已存在的预制体: {0}", 预制体路径);
                        continue; // 跳过当前对象，处理下一个
                    }
                }
                else
                {
                    // 预制体不存在，直接保存
                    PrefabUtility.SaveAsPrefabAsset(对象, 预制体路径);
                    Debug.LogFormat("预制体已创建: {0}", 预制体路径);
                }
            }
            
            Debug.Log("批量预制体创建及处理完成。");
        }
        else
        {
            Debug.LogWarning("未选择文件夹。预制体创建终止。");
        }
    }
}