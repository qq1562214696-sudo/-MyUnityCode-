using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class 一键清除丢失脚本 : EditorWindow
{
    private static List<string> scenePaths = new List<string>();
    private static bool isProcessing = false;
    private static int totalCleaned = 0;
    private static int processedScenes = 0;

    [MenuItem("❤多功能工具集❤/清除当前场景丢失脚本_待修改")]
    public static void 清除当前场景丢失脚本()
    {
        int cleanedCount = PerformCleanInCurrentScene();
        Debug.Log(cleanedCount > 0 ?
            $"成功清除 {cleanedCount} 个丢失脚本" :
            "当前场景未找到丢失脚本");
    }

    [MenuItem("❤多功能工具集❤/清除所有场景丢失脚本_待修改")]
    public static void 清除所有场景丢失脚本()
    {
        scenePaths = GetAllScenePaths();
        if (scenePaths.Count == 0)
        {
            Debug.Log("项目中未找到场景文件");
            return;
        }

        totalCleaned = 0;
        processedScenes = 0;
        isProcessing = true;

        // 保存当前场景修改
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorApplication.update += ProcessScenesAsync;
        }
        else
        {
            isProcessing = false;
        }
    }

    private static List<string> GetAllScenePaths()
    {
        List<string> paths = new List<string>();
        foreach (string guid in AssetDatabase.FindAssets("t:Scene"))
        {
            paths.Add(AssetDatabase.GUIDToAssetPath(guid));
        }
        return paths;
    }

    private static void ProcessScenesAsync()
    {
        if (!isProcessing || processedScenes >= scenePaths.Count)
        {
            FinishProcessing();
            return;
        }

        string currentScenePath = scenePaths[processedScenes];
        float progress = (float)processedScenes / scenePaths.Count;

        if (EditorUtility.DisplayCancelableProgressBar(
            "正在清理场景...",
            $"[{processedScenes + 1}/{scenePaths.Count}] {System.IO.Path.GetFileNameWithoutExtension(currentScenePath)}",
            progress))
        {
            FinishProcessing();
            return;
        }

        try
        {
            Scene scene = EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
            int cleaned = PerformCleanInCurrentScene();
            totalCleaned += cleaned;

            if (cleaned > 0)
            {
                EditorSceneManager.SaveScene(scene);
            }

            processedScenes++;
            EditorApplication.update += ProcessScenesAsync;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"处理场景失败: {currentScenePath}\n{e}");
            processedScenes++;
            EditorApplication.update += ProcessScenesAsync;
        }
    }

    private static int PerformCleanInCurrentScene()
    {
        int cleanCount = 0;
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(obj);
            if (missingCount > 0)
            {
                Undo.RegisterCompleteObjectUndo(obj, "Remove missing scripts");
                cleanCount += missingCount;
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
            }
        }

        return cleanCount;
    }

    private static void FinishProcessing()
    {
        EditorUtility.ClearProgressBar();
        EditorApplication.update -= ProcessScenesAsync;

        Debug.Log($"成功处理 {processedScenes} 个场景，共清除 {totalCleaned} 个丢失脚本");
        isProcessing = false;
    }
}