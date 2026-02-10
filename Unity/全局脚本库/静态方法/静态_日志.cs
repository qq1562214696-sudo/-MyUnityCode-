using UnityEngine;

public static partial class 静态//日志
{
    public static void 日志(string 内容)
    {
        Debug.Log(内容);
    }

    public static void 警告(string 内容)
    {
        Debug.LogWarning(内容);
    }

    public static void 错误(string 内容)
    {
        Debug.LogError(内容);
    }
}