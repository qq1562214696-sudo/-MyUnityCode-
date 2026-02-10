using UnityEngine;

public static partial class 静态//启用状态
{
    public static void 对象隐藏(GameObject 切换隐藏状态对象)//调用方法：静态.切换启用();
    {
        切换隐藏状态对象.SetActive(!切换隐藏状态对象.activeSelf);
        Debug.Log("UI元素 " + (切换隐藏状态对象.activeSelf ? "显示" : "隐藏"));
    }

    public static void 模型隐藏(Renderer 渲染器)//调用方法：静态.切换隐藏();
    {
        渲染器.enabled = !渲染器.enabled;
        Debug.Log("渲染器 " + (渲染器.enabled ? "显示" : "隐藏"));
    }

    public static void 精灵隐藏(SpriteRenderer 精灵渲染器)//调用方法：静态.切换隐藏();
    {
        精灵渲染器.enabled = !精灵渲染器.enabled;
        Debug.Log("渲染器 " + (精灵渲染器.enabled ? "显示" : "隐藏"));
    }

    public static void 碰撞开关(Collider 碰撞体)
    {
        碰撞体.enabled = !碰撞体.enabled;
    }

    public static void 碰撞2D开关(Collider2D 碰撞体)
    {
        碰撞体.enabled = !碰撞体.enabled;
    }
}