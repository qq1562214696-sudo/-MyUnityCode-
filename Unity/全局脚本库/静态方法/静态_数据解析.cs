
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public enum 触发标签
{
    玩家,
    敌人,
    中立,
    玩家攻击,
    敌人攻击,
    中立攻击,
    障碍物,
}

public static partial class 静态//数据解析
{
    public static bool 匹配触发标签(string 触发对象标签, 触发标签[] 触发标签数组)
    {
        if (触发标签数组 == null || 触发标签数组.Length == 0) return false;

        foreach (触发标签 触发标签 in 触发标签数组)
        {
            if (触发对象标签 == 触发标签.ToString())
            {
                return true;
            }
        }
        return false;
    }

    public static Vector2? Vector2解析(string 原始字符串)
    {
        string 处理后的字符串 = Vector2格式化(原始字符串);

        if (Vector2格式验证(处理后的字符串))
        {
            string[] 数值组 = 处理后的字符串.Split(',');
            float x值 = float.Parse(数值组[0]);
            float y值 = float.Parse(数值组[1]);
            return new Vector2(x值, y值);
        }

        return null;
    }

    private static string Vector2格式化(string 输入字符串)
    {
        Regex 规则 = new Regex(@"[^0-9,\-\.]");
        return 规则.Replace(输入字符串, "");
    }

    private static bool Vector2格式验证(string 待验证字符串)
    {
        if (待验证字符串.Count(c => c == ',') != 1) return false;

        string[] 分割结果 = 待验证字符串.Split(',');
        if (分割结果.Length != 2) return false;

        return float.TryParse(分割结果[0], out _) &&
               float.TryParse(分割结果[1], out _);
    }
}