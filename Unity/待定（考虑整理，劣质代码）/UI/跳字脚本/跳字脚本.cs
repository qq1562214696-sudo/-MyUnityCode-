using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum 跳字类型
{
    白字伤害,
    暴击伤害,
    治疗,
}

public class 跳字脚本 : MonoBehaviour
{
    public Text _跳字文本;
    public AnimationCurve _跳字路径;

    private void Awake()/////////////////
    {
        _跳字文本 = gameObject.GetComponent<Text>();/////////////////
        填入文本(跳字类型.白字伤害, 9999);/////////////////
        StartCoroutine(跳字协程());/////////////////
    }

    public void 填入文本(跳字类型 a_跳字类型, float a_跳字数值)
    {
        switch (a_跳字类型)
        {
            case 跳字类型.白字伤害:
                _跳字文本.text = $"A{a_跳字数值}";
                break;
            case 跳字类型.暴击伤害:
                _跳字文本.text = $"B{a_跳字数值}";
                break;
            case 跳字类型.治疗:
                _跳字文本.text = $"C{a_跳字数值}";
                break;
        }
    }

    public IEnumerator 跳字协程()
    {
        float a_跳字偏向方向 = Random.Range(0f, 360f);
        float a_跳字偏移距离 = 2f;
        float a_跳字偏移高度 = 2f;

        Vector3 a_开始坐标 = transform.position;
        float radian = a_跳字偏向方向 * Mathf.Deg2Rad;
        Vector3 a_终点坐标 = new Vector3(Mathf.Cos(radian) * a_跳字偏移距离, 0, Mathf.Sin(radian) * a_跳字偏移距离);

        float a_销毁时间 = 0f;
        float a_运动时间 = 1f;

        while (a_销毁时间 < a_运动时间)
        {
            float t = a_销毁时间 / a_运动时间;
            float a_曲线高度 = _跳字路径.Evaluate(t) * a_跳字偏移高度;
            Vector3 a_当前曲线偏移值 = Vector3.Lerp(Vector3.zero, a_终点坐标, t);
            Vector3 a_当前坐标 = a_开始坐标 + a_当前曲线偏移值 + new Vector3(0, a_曲线高度, 0);
            transform.position = a_当前坐标;

            a_销毁时间 += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
