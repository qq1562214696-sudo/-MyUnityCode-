using UnityEngine;
using System.Collections;

public class ObjectMovement : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public AnimationCurve movementCurve;
    public float 移动时长 = 0.3f;
    public float 偏移距离 = 1f;

    void Start()
    {
        transform.position = startPoint.position;
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 移动时长)
        {
            float progress = Mathf.Clamp01(elapsedTime / 移动时长);

            Vector3 mainMovement = Vector3.Lerp(startPoint.position, endPoint.position, progress);

            float curveValue = movementCurve.Evaluate(progress);
            Vector3 upwardOffset = Vector3.up * curveValue * 偏移距离;

            transform.position = mainMovement + upwardOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPoint.position;
    }
}