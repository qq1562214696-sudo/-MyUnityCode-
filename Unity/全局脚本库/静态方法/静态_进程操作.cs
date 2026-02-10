using UnityEngine;

public static partial class 静态//进程操作
{

    public static void 暂停()
    {
        Time.timeScale = 0;
    }

    public static void 进入游戏()
    {
        全部加载();
    }

    public static void 退出游戏()//调用方法：静态.退出游戏();
    {
        全部保存();
        Application.Quit();
    }
}