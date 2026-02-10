using UnityEngine;
using System.Collections.Generic;

public class 鼠标交互脚本 : MonoBehaviour
{
    public Material 悬停材质;
    public Material 点击材质;
    public float 点击缩放倍率 = 1.2f;
    public AudioClip 点击音效;
    public AudioSource 音效播放器;

    private Vector3 原始缩放;
    private List<Material> 原始材质列表 = new List<Material>();
    private Renderer 渲染器;

    void Start()
    {
        渲染器 = GetComponent<Renderer>();
        if (渲染器 != null)
        {
            foreach (var 材质 in 渲染器.materials)
            {
                原始材质列表.Add(材质);
            }
        }

        原始缩放 = transform.localScale;
    }

    void OnMouseOver()
    {
        if (!Input.GetMouseButton(0))
        {
            悬停方法();
        }
        else
        {
            还原方法();
            点击方法();
        }
    }

    void OnMouseExit()
    {
        还原方法();
    }

    void OnMouseUp()
    {
        还原方法();
    }

    void 悬停方法()
    {
        if (渲染器 != null && 悬停材质 != null)
        {
            List<Material> 新材质列表 = new List<Material>(原始材质列表);
            新材质列表.Add(悬停材质);
            渲染器.materials = 新材质列表.ToArray();
        }
    }

    void 点击方法()
    {
        if (渲染器 != null && 点击材质 != null)
        {
            List<Material> 新材质列表 = new List<Material>(原始材质列表);
            新材质列表.Add(点击材质);
            渲染器.materials = 新材质列表.ToArray();

            transform.localScale = 原始缩放 * 点击缩放倍率;

            if (音效播放器 != null && 点击音效 != null)
            {
                音效播放器.PlayOneShot(点击音效);
            }
        }
    }

    void 还原方法()
    {
        if (渲染器 != null && 原始材质列表.Count > 0)
        {
            渲染器.materials = 原始材质列表.ToArray();
        }
        transform.localScale = 原始缩放;
    }
}