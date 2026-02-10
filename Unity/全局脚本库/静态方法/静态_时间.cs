using UnityEngine;

public static partial class 静态//时间
{
    [静态方法重命名("暂停")]
    public static void 切换暂停()//调用方法：静态.切换暂停();
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        Debug.Log("游戏"+ (Time.timeScale == 0 ?"暂停":"开始"));
    }

    [静态方法重命名("时间")]
    public static void 时间缩放(float 缩放时间)//调用方法：静态.时间缩放();
    {
        Time.timeScale = 缩放时间;
        Debug.Log("游戏速度设置为:"+ 缩放时间 +"倍。");
    }

    public static float 游戏时长(float 缩放时间)
    {
        Debug.Log(Time.realtimeSinceStartup);
        return Time.realtimeSinceStartup;
    }

    public static string 时间格式A(float 时间, 格式 格式类型)
    {
        switch (格式类型)
        {
            case 格式.秒格式:
                return 时间 +"秒";
            case 格式.分秒格式:
                int 分钟 = Mathf.FloorToInt(时间 / 60);
                float 剩余秒数 = 时间 % 60;
                return 分钟 +"分"+ 剩余秒数 +"秒";
            case 格式.时分秒格式:
                int 小时 = Mathf.FloorToInt(时间 / 3600);
                int 剩余分钟 = Mathf.FloorToInt((时间 % 3600) / 60);
                float 剩余秒 = (时间 % 3600) % 60;
                return 小时 +"时"+ 剩余分钟 +"分"+ 剩余秒 +"秒";
            default:
                return"错误时间";
        }
    }

    public static string 时间格式B(float 时间, 格式 格式类型)
    {
        switch (格式类型)
        {
            case 格式.秒格式:
                return 时间.ToString("F0");
            case 格式.分秒格式:
                int 分钟 = Mathf.FloorToInt(时间 / 60);
                float 剩余秒数 = 时间 % 60;
                return 分钟 +":" + 剩余秒数.ToString("F0");
            case 格式.时分秒格式:
                int 小时 = Mathf.FloorToInt(时间 / 3600);
                int 剩余分钟 = Mathf.FloorToInt((时间 % 3600) / 60);
                float 剩余秒 = (时间 % 3600) % 60;
                return 小时 +":" + 剩余分钟 +":" + 剩余秒.ToString("F0");
            default:
                return"错误时间";
        }
    }

    public static string 时间格式C(float 时间, 格式 格式类型)
    {
        switch (格式类型)
        {
            case 格式.秒格式:
                return 时间.ToString("F2");
            case 格式.分秒格式:
                int 分钟 = Mathf.FloorToInt(时间 / 60);
                float 剩余秒数 = 时间 % 60;
                return 分钟 + ":" + 剩余秒数.ToString("F2");
            case 格式.时分秒格式:
                int 小时 = Mathf.FloorToInt(时间 / 3600);
                int 剩余分钟 = Mathf.FloorToInt((时间 % 3600) / 60);
                float 剩余秒 = (时间 % 3600) % 60;
                return 小时 + ":" + 剩余分钟 + ":" + 剩余秒.ToString("F2");
            default:
                return "错误时间";
        }
    }
}