using UnityEngine;

public static partial class 静态//鼠标
{
    public static void 隐藏光标()
    {
        Cursor.visible = !Cursor.visible;
    }

    public static bool 锁定隐藏光标()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        return Cursor.lockState != CursorLockMode.Locked;
    }

    public static void 限制光标()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Confined ? CursorLockMode.None : CursorLockMode.Confined;
    }

    public static void 切换光标(Texture2D 图标 ,Vector2 热点)
    {
        Cursor.SetCursor(图标, 热点, CursorMode.Auto);
    }
}