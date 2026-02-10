using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;

public class 排序选中对象
{
    public static void 排序()
    {
        // 获取选中的对象
        GameObject[] 选中对象 = Selection.gameObjects;

        // 如果没有选中任何对象，直接返回
        if (选中对象.Length == 0)
        {
            Debug.Log("没有选中的对象。");
            return;
        }

        // 开始一个新的操作组，这样可以将所有移动作为一次操作来撤销
        Undo.RegisterCompleteObjectUndo(选中对象, "排序对象");

        // 对选中的对象进行排序
        var 排序后的对象 = 选中对象
            .OrderBy(obj => 提取排序值(obj.name)) // 按排序值排序
            .ToArray();

        // 将排序后的对象移动到原来的位置，并标记为脏以支持撤销
        for (int i = 0; i < 排序后的对象.Length; i++)
        {
            排序后的对象[i].transform.SetAsLastSibling(); // 移动到最后，准备重新设置索引
            排序后的对象[i].transform.SetSiblingIndex(i);

            // 添加脏标记
            EditorUtility.SetDirty(排序后的对象[i]);
        }

        Debug.Log("对象已排序。");
    }

    private static string 提取排序值(string 名称)
    {
        // 检查名称是否包含数字
        int 数字;
        var 数字匹配 = Regex.Match(名称, @"\d+"); // 匹配连续的数字
        if (数字匹配.Success && int.TryParse(数字匹配.Value, out 数字))
        {
            return "0" + 数字.ToString(); // 数字前加'0'以确保在其他类型之前
        }

        // 检查名称是否包含非数字字符，并判断是否为首字母
        var 首字母 = 名称.FirstOrDefault(c => !char.IsDigit(c));
        if (首字母 != default(char))
        {
            if (char.IsLetter(首字母))
            {
                return "1" + 首字母; // 字母前加'1'以确保在汉字之前
            }
            else if (Regex.IsMatch(首字母.ToString(), @"[\u4e00-\u9fa5]")) // 检查是否为汉字
            {
                return "2" + 首字母; // 汉字前加'2'以确保在字母之后
            }
        }

        // 如果没有匹配到任何东西，默认放最后
        return "3" + "99999"; // 默认值
    }
}