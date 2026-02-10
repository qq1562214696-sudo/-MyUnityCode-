using UnityEngine;
using 设置系统;

public static partial class 静态//设置
{
    [静态方法重命名("全屏")]
    public static void 全屏()
    {
        单例_设置管理器._实例.设置全屏按钮();
    }

    [静态方法重命名("静音")]
    public static void 静音()
    {
        单例_设置管理器._实例.设置静音按钮();
    }

    public static void 帧率(byte 帧率)
    {
        Application.targetFrameRate = (帧率 == 0) ? -1 : 帧率;
    }

    public static void 分辨率(Vector2 分辨率, bool 是否全屏)
    {
        Screen.SetResolution((int)分辨率.x, (int)分辨率.y, 是否全屏);
    }

    public static void 垂直同步()
    {
        QualitySettings.vSyncCount = QualitySettings.vSyncCount == 0 ? 1 : 0;
    }

    public static int 显示FPS()
    {
        if (Time.deltaTime <= 0.0f)
        {
            return 0;
        }

        int fps = (int)(1.0f / Time.deltaTime);
        Debug.Log("当前FPS: " + fps);
        return fps;
    }

    public static int 帧率计算(float 帧时间差)// 调用方法：静态.帧率计算(帧时间差);
    {
        帧时间差 = Mathf.Lerp(帧时间差, Time.deltaTime, 0.1f);
        int 帧率 = (int)(0.1f / 帧时间差);
        return 帧率;
    }
}