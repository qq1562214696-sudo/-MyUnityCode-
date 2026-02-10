using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class 静态//异步操作
{
    public static void 切换场景(string 场景名称)
    {
        协程加载场景(场景名称);
    }

    private static IEnumerator 协程加载场景(string 场景名称)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(场景名称);

        while (!asyncLoad.isDone)
        {
            Debug.Log($"加载进度: {asyncLoad.progress * 100}%");
            yield return null;

            Debug.Log("场景加载完成！");
        }
    }
}