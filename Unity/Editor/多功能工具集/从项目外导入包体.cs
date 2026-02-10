using UnityEngine;
using UnityEditor;
using System.IO;

public class 从项目外导入包体
{
     public static void 从项目外导入()
        {
            string initialPath = "Assets";
            string fullFolderPath = EditorUtility.OpenFolderPanel("选择包含unitypackage的外部文件夹", initialPath, "");

            if (!string.IsNullOrEmpty(fullFolderPath))
            {
                string[] unityPackageFiles = Directory.GetFiles(fullFolderPath, "*.unitypackage", SearchOption.TopDirectoryOnly);

                foreach (string packageFilePath in unityPackageFiles)
                {
                    // 构建项目内目标路径，保持与原文件相同的相对结构（如果需要）
                    string relativePathInProject = packageFilePath.Substring(fullFolderPath.Length + 1); // 去除外部路径部分
                    string targetPathInProject = Path.Combine("Assets", relativePathInProject);

                    // 确保Assets的上级目录存在
                    string directoryInAssets = Path.GetDirectoryName(targetPathInProject);
                    if (!Directory.Exists(directoryInAssets))
                    {
                        Directory.CreateDirectory(directoryInAssets);
                    }

                    // 先复制文件到Assets目录下
                    string tempPackagePathInAssets = Path.Combine(directoryInAssets, Path.GetFileName(packageFilePath));
                    File.Copy(packageFilePath, tempPackagePathInAssets, true);

                    // 使用Unity的AssetDatabase接口导入
                    AssetDatabase.ImportPackage(tempPackagePathInAssets, false);

                    Debug.LogFormat("Import process triggered for: {0}", tempPackagePathInAssets.Replace(Application.dataPath, "Assets"));

                    // 导入后可以考虑删除临时复制的文件，根据需要调整
                    File.Delete(tempPackagePathInAssets);
                }

                Debug.Log("UnityPackage import process completed.");
            }
            else
            {
                Debug.LogWarning("No folder selected.");
            }
        }
}