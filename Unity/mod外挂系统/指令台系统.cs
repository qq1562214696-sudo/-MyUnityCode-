using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[AddComponentMenu("1自定义脚本/指令台系统")]
public class 指令台系统 : MonoBehaviour
{
    public InputField 指令输入框;
    [Tooltip("命名空间.类名")]
    public string[] 允许的类名列表 = new[]
    {
        "静态",
    };
    private List<Type> 可用类型列表 = new List<Type>();

    private void Awake()
    {
        初始化可用类型列表();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            切换输入框状态();
        }
    }


    private void 切换输入框状态()
    {
        if (!指令输入框.gameObject.activeSelf)
        {
            显示输入框();
        }
        else
        {
            处理并隐藏输入框();
        }
    }

    private void 显示输入框()
    {
        指令输入框.gameObject.SetActive(true);
        指令输入框.ActivateInputField();
    }

    private void 处理并隐藏输入框()
    {
        string 方法字符串 = 指令输入框.text.Trim();
        if (!string.IsNullOrEmpty(方法字符串))
        {
            通用指令方法(方法字符串);
        }

        指令输入框.gameObject.SetActive(false);
    }

    public void 通用指令方法(string 方法字符串)
    {
        try
        {
            方法字符串 = 方法字符串.Replace(" ", "");

            string[] 分割结果 = 方法字符串.Split('/');
            if (分割结果.Length == 0 || 分割结果[0] == "")
            {
                Debug.LogError("指令格式错误：无效的方法或参数");
                return;
            }

            string 方法名 = 分割结果[0].Trim();
            string[] 参数数组 = 分割结果.Length > 1 ? 分割结果[1].Split('/') : new string[0];

            MethodInfo 方法信息 = 获取方法信息(方法名, 参数数组);
            if (方法信息 != null)
            {
                调用方法(方法信息, 参数数组);
                指令输入框.text = "";
            }
            else
            {
                Debug.LogError($"未找到方法：{方法名}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"执行指令时发生错误：{ex.Message}");
        }
    }

    private object[] 准备方法参数(MethodInfo 方法信息, string[] 参数数组)
    {
        ParameterInfo[] 参数信息 = 方法信息.GetParameters();
        object[] 转换后参数 = new object[参数信息.Length];

        for (int i = 0; i < 参数信息.Length; i++)
        {
            if (i >= 参数数组.Length)
            {
                throw new ArgumentException($"提供的参数数量不足，需要 {参数信息.Length} 个参数，但只提供了 {参数数组.Length} 个。");
            }

            string 参数值 = 参数数组[i].Trim();
            Type 参数类型 = 参数信息[i].ParameterType;

            if (参数类型.IsEnum)
            {
                转换后参数[i] = Enum.Parse(参数类型, 参数值, true);
            }
            else if (参数类型 == typeof(string))
            {
                转换后参数[i] = 参数值;
            }
            else
            {
                try
                {
                    转换后参数[i] = Convert.ChangeType(参数值, 参数类型);
                }
                catch (Exception e)
                {
                    Debug.LogError($"参数转换失败: {e.Message}");
                    throw;
                }
            }
        }

        if (参数信息.Length > 参数数组.Length)
        {
            throw new ArgumentException($"提供的参数数量不足，需要 {参数信息.Length} 个参数，但只提供了 {参数数组.Length} 个。");
        }

        return 转换后参数;
    }

    private void 初始化可用类型列表()
    {
        foreach (string 类名 in 允许的类名列表)
        {
            Type 类型 = Type.GetType(类名);
            if (类型 != null)
            {
                可用类型列表.Add(类型);
            }
            else
            {
                Debug.LogWarning($"未找到类型: {类名}");
            }
        }
    }

    private MethodInfo 获取方法信息(string 方法名, string[] 参数数组)
    {
        foreach (Type type in 可用类型列表)
        {
            MethodInfo 方法信息 = 获取带特性的匹配方法(type, 方法名, 参数数组);
            if (方法信息 != null)
            {
                return 方法信息;
            }
        }

        return null;
    }

    private MethodInfo 获取带特性的匹配方法(Type 类型, string 方法名, string[] 参数数组)
    {
        foreach (MethodInfo 方法 in 类型.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            var 重命名特性 = 方法.GetCustomAttribute<静态方法重命名>();
            if (重命名特性 != null && 重命名特性.方法名 == 方法名)
            {
                ParameterInfo[] 参数信息 = 方法.GetParameters();
                if (参数信息.Length == 参数数组.Length)
                {
                    return 方法;
                }
            }
        }
        return null;
    }

    private void 调用方法(MethodInfo 方法信息, string[] 参数数组)
    {
        object[] 转换后参数 = 准备方法参数(方法信息, 参数数组);
        方法信息.Invoke(null, 转换后参数);
    }
}