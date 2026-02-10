using UnityEngine;

public class 摄像机跟随_暂弃 : MonoBehaviour
{
    public Transform 跟随对象;
    public float 平滑比例 = 0.125f;
    public float 跳转阈值 = 0.01f;
    private Vector3 坐标差值;

    void Awake()
    {
        坐标差值 = transform.position - 跟随对象.position;
    }

    void LateUpdate()
    {
        Vector3 移动目标点 = 跟随对象.position + 坐标差值;
        float 距离 = Vector3.Distance(transform.position, 移动目标点);

        if (距离 < 跳转阈值)
        {
            transform.position = 移动目标点;
        }
        else
        {
            Vector3 平滑后坐标 = Vector3.Lerp(transform.position, 移动目标点, 平滑比例);
            transform.position = 平滑后坐标;
        }
    }
}