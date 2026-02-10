using System.IO;
using UnityEngine;

public static partial class 静态//文件处理
{
    public static void 全部保存()
    {

    }

    public static void 全部加载()
    {

    }

    public static void 保存数据<T>(T 数据, string 文件路径) where T : ScriptableObject
    {
        try
        {
            string directoryPath = Path.GetDirectoryName(Application.persistentDataPath + "/" + 文件路径);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonUtility.ToJson(数据, false);
            File.WriteAllText(Application.persistentDataPath + "/" + 文件路径, json);
            Debug.Log($"{数据}数据已保存至: {Application.persistentDataPath + "/" + 文件路径}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{数据}保存失败: {e.Message}");
        }
    }

    public static void 加载数据<T>(T 实例, string 文件路径) where T : ScriptableObject
    {
        if (!File.Exists(Application.persistentDataPath + "/" + 文件路径))
        {
            Debug.LogError($"{实例}文件不存在: {Application.persistentDataPath + "/" + 文件路径}");
            return;
        }

        try
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/" + 文件路径);
            JsonUtility.FromJsonOverwrite(json, 实例);
            Debug.Log($"{实例}数据已从 {Application.persistentDataPath + "/" + 文件路径} 加载");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{实例}加载失败: {e.Message}");
        }
    }

    public static void 截屏(string 自定义文件夹路径 = null)
    {
        string 文件路径;
        string 文件名 = System.DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒") + ".png";

        if (string.IsNullOrEmpty(自定义文件夹路径))
        {
            文件路径 = Path.Combine(Application.persistentDataPath, "截屏", 文件名);
        }
        else
        {
            文件路径 = Path.Combine(自定义文件夹路径, "截屏", 文件名);
        }

        string 目标目录 = Path.GetDirectoryName(文件路径);
        if (!Directory.Exists(目标目录))
        {
            Directory.CreateDirectory(目标目录);
        }

        ScreenCapture.CaptureScreenshot(文件路径);
        Debug.Log("截屏保存到: " + 文件路径);
    }

    public static void 删除文件(string 目标文件)
    {
        if (File.Exists(目标文件))
        {
            File.Delete(目标文件);
        }
    }

    public static void 清空文件夹(string 目标文件夹)
    {
        if (Directory.Exists(目标文件夹))
        {
            string[] 文件列表 = Directory.GetFiles(目标文件夹);
            foreach (string 文件 in 文件列表)
            {
                File.Delete(文件);
            }

            string[] 子文件夹列表 = Directory.GetDirectories(目标文件夹);
            foreach (string 子文件夹 in 子文件夹列表)
            {
                Directory.Delete(子文件夹, true);
            }
        }
    }

    public static string 获取文件路径(string 文件名)
    {
        string 文件夹路径 = Path.Combine(Application.persistentDataPath);
        if (!Directory.Exists(文件夹路径))
        {
            Directory.CreateDirectory(文件夹路径);
        }
        return Path.Combine(文件夹路径, 文件名);
    }
}
