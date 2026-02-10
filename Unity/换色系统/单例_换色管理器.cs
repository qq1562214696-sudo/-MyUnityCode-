using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;

public class 单例_换色管理器 : MonoBehaviour
{
    public static 单例_换色管理器 _实例;

    public GameObject _调色盘;
    public GameObject _调色盘实例;
    public List<颜色数据> 材质换色;

    private void Awake()
    {
        if (_实例 == null)
        {
            _实例 = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        加载颜色();
        _调色盘实例 = Instantiate(_调色盘,transform);
        foreach (var materialChange in 材质换色)
        {
            materialChange.换色按钮.onClick.AddListener(() => 改变按钮颜色(materialChange));
            确认颜色(materialChange);
        }
    }

    private void 确认颜色(颜色数据 materialInfo)
    {
        if (!string.IsNullOrEmpty(materialInfo.颜色))
        {
            if (ColorUtility.TryParseHtmlString(materialInfo.颜色, out Color storedColor))
            {
                获取颜色(materialInfo, storedColor, false);
            }
        }
    }

    public void 改变按钮颜色(颜色数据 materialInfo)
    {
        var currentColor = 输出颜色到材质(materialInfo);

        bool created = 调色盘.Create(currentColor,
            "选择颜色",
            color => 获取颜色(materialInfo, color, false),
            color => {
                获取颜色(materialInfo, color, true);
            },
            true
        );

        if (created)
        {
            调取调色盘(materialInfo.换色按钮);
        }
    }

    private void 调取调色盘(Button button)
    {
        if (_调色盘实例 != null)
        {
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            RectTransform colorPickerRect = _调色盘实例.GetComponent<RectTransform>();

            Vector3 buttonWorldPosition = buttonRect.TransformPoint(Vector3.zero);

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, buttonWorldPosition);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(colorPickerRect.parent as RectTransform, screenPoint, Camera.main, out Vector2 localPoint);

            colorPickerRect.localPosition = localPoint;

            调取调色盘(colorPickerRect);
        }
    }

    private void 调取调色盘(RectTransform colorPickerRect)
    {
        Canvas canvas = colorPickerRect.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Vector3[] corners = new Vector3[4];
            colorPickerRect.GetWorldCorners(corners);

            Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
            Vector2 adjustment = Vector2.zero;

            if (corners[0].x < 0)
                adjustment.x = -corners[0].x;
            else if (corners[2].x > canvasSize.x)
                adjustment.x = canvasSize.x - corners[2].x;

            if (corners[0].y < 0)
                adjustment.y = -corners[0].y;
            else if (corners[1].y > canvasSize.y)
                adjustment.y = canvasSize.y - corners[1].y;

            colorPickerRect.localPosition += (Vector3)adjustment;
        }
    }

    private Color 输出颜色到材质(颜色数据 materialInfo)
    {
        if (materialInfo.材质球 != null)
        {
            return materialInfo.材质球.GetColor(materialInfo.颜色代号);
        }
        return Color.white;
    }

    private void 获取颜色(颜色数据 materialInfo, Color currentColor, bool saveToXml)
    {
        if (materialInfo.材质球 != null)
        {
            materialInfo.材质球.SetColor(materialInfo.颜色代号, currentColor);
        }

        if (materialInfo.精灵渲染器 != null)
        {
            materialInfo.精灵渲染器.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        }

        if (materialInfo.目标图片 != null)
        {
            materialInfo.目标图片.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        }

        materialInfo.颜色 = "#" + ColorUtility.ToHtmlStringRGBA(currentColor);

        if (saveToXml)
        {
            储存颜色();
        }
    }

    private void 储存颜色()
    {
        string 文件路径 = 静态.获取文件路径(常量.换色系统文件名);
        XmlSerializer serializer = new(typeof(List<颜色数据>));
        using StringWriter stringWriter = new();
        serializer.Serialize(stringWriter, 材质换色);
        File.WriteAllText(文件路径, stringWriter.ToString());
        Debug.Log($"保存到: {文件路径}");
    }

    private void 加载颜色()
    {
        string 文件路径 = 静态.获取文件路径(常量.换色系统文件名);
        if (File.Exists(文件路径))
        {
            XmlSerializer serializer = new(typeof(List<颜色数据>));
            List<颜色数据> loadedColors;
            using (StringReader stringReader = new(File.ReadAllText(文件路径)))
            {
                loadedColors = (List<颜色数据>)serializer.Deserialize(stringReader);
            }

            foreach (var loadedColor in loadedColors)
            {
                var existingColor = 材质换色.Find(c => c.ID == loadedColor.ID);
                if (existingColor != null)
                {
                    existingColor.颜色 = loadedColor.颜色;
                }
            }
        }
    }
}