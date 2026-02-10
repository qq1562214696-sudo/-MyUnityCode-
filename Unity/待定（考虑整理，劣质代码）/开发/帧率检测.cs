using UnityEngine;
using System.Collections.Generic;

public class 帧率检测 : MonoBehaviour
{
    [SerializeField, Range(0, 240)]
    private float _平均帧率 = 0f;
    [SerializeField]
    private float _时间窗口 = 3f;

    private Queue<float> _帧率样本 = new Queue<float>();
    private float _样本总和 = 0f;
    private int _最大样本数 = 300;
    private WaitForSeconds _采样间隔 = new WaitForSeconds(0.1f);

    public float 平均帧率 => _平均帧率;
    public float 时间窗口
    {
        get => _时间窗口;
        set
        {
            _时间窗口 = Mathf.Max(1f, value);
            更新最大样本数();
        }
    }

    private void Start()
    {
        更新最大样本数();
        StartCoroutine(帧率采样());
    }

    private System.Collections.IEnumerator 帧率采样()
    {
        while (true)
        {
            yield return _采样间隔;

            float 当前帧率 = 1f / Time.unscaledDeltaTime;
            _样本总和 += 当前帧率;
            _帧率样本.Enqueue(当前帧率);

            while (_帧率样本.Count > _最大样本数)
            {
                _样本总和 -= _帧率样本.Dequeue();
            }

            _平均帧率 = _样本总和 / _帧率样本.Count;
        }
    }

    private void 更新最大样本数()
    {
        _最大样本数 = Mathf.FloorToInt(_时间窗口 / 0.1f);
        _最大样本数 = Mathf.Clamp(_最大样本数, 10, 500);
    }

    private void OnGUI()
    {
        GUIStyle 样式 = GUI.skin.label;
        样式.fontSize = 20;
        样式.normal.textColor = Color.white;

        Rect 矩形区域 = new Rect(10, Screen.height - 40, 300, 30);
        GUI.Label(矩形区域, $"FPS: {_平均帧率:0}", 样式);
    }
}