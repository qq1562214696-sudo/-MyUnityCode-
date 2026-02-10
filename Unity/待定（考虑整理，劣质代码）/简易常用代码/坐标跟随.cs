using UnityEngine;

public class 坐标跟随 : MonoBehaviour
{
    public Transform 跟随目标;
    private Vector3 坐标差值;

    void Awake()
    {
        坐标差值 = transform.position - 跟随目标.position;
    }

    void LateUpdate()
    {
        Vector3 移动目标点 = 跟随目标.position + 坐标差值;
        transform.position = 移动目标点;
    }
}