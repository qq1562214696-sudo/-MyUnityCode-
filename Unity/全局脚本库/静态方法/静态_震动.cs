using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public static partial class 静态//震动
{
    public static Coroutine 当前震动协程;
    public static Camera 主摄像机;
    public static Vector3 原始位置;

    public static void 对象震动(float 新震动强度)
    {
        if (当前震动协程 != null)
        {
            单例_震动管理器._实例.StopCoroutine(当前震动协程);
            当前震动协程 = null;
        }

        if (新震动强度 == 0)
        {
            主摄像机.transform.localPosition = 原始位置;
            return;
        }

        当前震动协程 = 单例_震动管理器._实例.StartCoroutine(对象震动协程(新震动强度));
    }

    public static IEnumerator 对象震动协程(float 震动强度)
    {
        while (true)
        {
            Vector3 随机偏移 = UnityEngine.Random.onUnitSphere * 震动强度;

            主摄像机.transform.localPosition = 原始位置 + 随机偏移;
            yield return null;
        }
    }

    public static void 手柄震动(string 强度字符串)
    {
        Vector2? 强度 = Vector2解析(强度字符串);

        if (强度.HasValue)
        {
            float x = Mathf.Clamp(强度.Value.x, 0f, 1f);
            float y = Mathf.Clamp(强度.Value.y, 0f, 1f);

            Gamepad.current.SetMotorSpeeds(x, y);
        }
        else
        {
            Debug.LogError("未能成功解析输入字符串或输入不在有效范围内");
        }
    }

    public static void 手机震动()
    {
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}