using UnityEngine;
using UnityEditor;
using System.IO;

public class 预制体批量替换
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
            string 相对路径 = Path.Combine(完整文件夹路径.Substring(Application.dataPath.Length - 6)); // "-6"是为了去掉"Assets/"的长度
            
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
                
                // 如果该预制体已经存在，Unity会询问是否覆盖
                PrefabUtility.SaveAsPrefabAsset(对象, 预制体路径);
                
                Debug.LogFormat("预制体已创建: {0}", 预制体路径);
            }
            
            Debug.Log("批量预制体创建完成。");
        }
        else
        {
            Debug.LogWarning("未选择文件夹。预制体创建终止。");
        }
    }
}