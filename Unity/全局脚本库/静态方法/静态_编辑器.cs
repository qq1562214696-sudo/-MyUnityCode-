using UnityEditor;
using UnityEngine;

public static partial class 静态//编辑器或开发模式
{
    public static void 编辑器暂停()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        EditorApplication.isPaused = !EditorApplication.isPaused;
#endif
    }

    public static void 编辑器进入游戏()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        全部加载();
#endif
    }

    public static void 编辑器退出游戏()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        全部保存();
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.P))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
}