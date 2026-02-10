using UnityEngine;
using UnityEditor;
using System.IO;

public class 从项目中导入包体
{
    public static void 从项目中导入()
    {
        string initialPath = "Assets";
        string fullFolderPath = EditorUtility.OpenFolderPanel("选择包含unitypackage的文件夹", initialPath, "");

        if (!string.IsNullOrEmpty(fullFolderPath))
        {
            string[] unityPackageFiles = Directory.GetFiles(fullFolderPath, "*.unitypackage", SearchOption.TopDirectoryOnly);

            foreach (string packageFilePath in unityPackageFiles)
            {
                // 使用Unity的AssetDatabase接口来导入Unity Package，这会弹出一个界面供用户选择导入哪些资源
                AssetDatabase.ImportPackage(packageFilePath, false);
                
                Debug.LogFormat("Import process triggered for: {0}", packageFilePath.Replace(Application.dataPath, "Assets"));
            }

            Debug.Log("UnityPackage import process completed.");
        }
        else
        {
            Debug.LogWarning("No folder selected.");
        }
    }
}