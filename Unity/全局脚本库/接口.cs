using UnityEngine;
public interface I操作系统接口
{
    void 移动(float 移动速度);
}

// 创建一个实现了I操作系统接口的类
public class 玩家 : MonoBehaviour, I操作系统接口
{
    public void 移动(float 移动速度)
    {
        // 根据输入方向移动玩家
        Vector3 方向 = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(Time.deltaTime * 移动速度 * 方向);
    }
}

public class 怪物 : MonoBehaviour, I操作系统接口
{
    public void 移动(float 移动速度)
    {
        // 示例：怪物移动逻辑
        // 这里可以添加怪物的移动逻辑
        transform.Translate(Time.deltaTime * 移动速度 * Vector3.forward);
    }
}

public class 控制脚本<T> : MonoBehaviour where T : I操作系统接口
{
    public float 移动速度;
    private T 可移动对象;

    private void Awake()
    {
        可移动对象 = GetComponent<T>();
    }

    private void Update()
    {
        可移动对象?.移动(移动速度);
    }
}