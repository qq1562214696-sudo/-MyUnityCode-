using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//设置管理器
[System.Serializable]
public class 帧率选项
{
    public int 帧率上限;
}

[System.Serializable]
public class 设置项
{
    public 是否枚举 是否开启 = 是否枚举.是;
    public GameObject 设置对象;
    public Button 设置按钮;
}

//对象状态系统
[System.Serializable]
public class 对象状态
{
    public string ID; // 唯一标识符
    [XmlIgnore] // 忽略此字段的序列化
    public GameObject 对象;
    public 是否枚举 是否显示 = 是否枚举.是;
    public Vector3 位置;
    public Vector3 旋转值; // 更改为Vector3
    public Vector3 缩放值;

    // 设置状态
    public void 设置状态(Vector3 位置, Vector3 旋转值, Vector3 缩放值)
    {
        this.位置 = 位置;
        this.旋转值 = 旋转值;
        this.缩放值 = 缩放值;
    }

    // 应用状态到对象
    public void 应用状态()
    {
        if (对象 != null)
        {
            对象.transform.position = 位置;
            对象.transform.eulerAngles = 旋转值; // 使用eulerAngles代替rotation
            对象.transform.localScale = 缩放值;
        }
    }

    // 从对象获取状态
    public void 从对象获取状态()
    {
        if (对象 != null)
        {
            位置 = 对象.transform.position;
            旋转值 = 对象.transform.eulerAngles; // 使用eulerAngles代替rotation
            缩放值 = 对象.transform.localScale;
        }
    }
}

//红外线脚本
[System.Serializable]
public class 红外线源数据
{
    public GameObject 红外线源;
    public Vector3 射线终点 = Vector3.zero; // 直接在这里存储射线终点
    public 红外线事件委托 动画播放委托;
}

//管理器_音效播放切换
[System.Serializable]
public class 音效对象池
{
    [Tooltip("要播放或停止的音效对象")]
    public AudioSource 音效对象;

    [Tooltip("该音效被触发的次数")]
    public int 触发次数;
}

//管理器_显示切换
[System.Serializable]
public class 显示对象池
{
    [Tooltip("要显示或隐藏的游戏对象")]
    public GameObject 显示对象;

    [Tooltip("该对象被触发的次数")]
    public int 触发次数;
}

//永久背包配置文件
[Serializable]
public class 方法与特效设置
{
    public string 方法参数;
    public GameObject 特效预制体;
    public float 特效持续时间 = 5f;
    public bool 特效多人可见 = true; // 新增字段
}

[System.Serializable]
public class 永久背包配置文件数据
{
    public string 道具名称;
    public 道具类型 道具类型;
    public int 道具数量;
    public TextAsset 道具描述;
    public Sprite 道具图标;
    public List<方法与特效设置> 消耗品方法; // 新增：方法与特效设置列表
    public float 消耗品CD; // 新增：冷却时间
}

//材质球换色
[Serializable]
public class 颜色数据
{
    public int ID;
    [XmlIgnore]
    public Material 材质球;
    [XmlIgnore]
    public string 颜色代号;
    [XmlIgnore]
    public Button 换色按钮;
    [XmlIgnore]
    public SpriteRenderer 精灵渲染器; // 用于精灵
    [HideInInspector]
    public string 颜色;

    [XmlIgnore]
    public Image 目标图片; // 新增字段，用于指定要改变颜色的图片组件
}

//消耗品使用脚本
[Serializable]
public class 使用道具项
{
    [HideInInspector] public int 减少道具剩余数量;
    [HideInInspector] public UnityEvent 使用后触发事件;
    [HideInInspector] public GameObject 特效触发的坐标;

    public string 减少道具;
    public Button 使用按钮;
    public Image 冷却进度条;
    public InputAction 使用按键;
}

//装备配置文件
[Serializable]
public class 属性加成类型与数值
{
    public 属性加成类型 属性加成类型;
    public string 增加数值;
}

[System.Serializable]
public class 装备配置文件数据
{
    public string 道具名称;
    public 装备类型 装备类型;
    public List<属性加成类型与数值> 属性加成列表; // 改为列表

    public TextAsset 装备描述;
    public Sprite 装备图标;
}


[System.Serializable]
public class 角色增幅
{
    public 消耗元素收益 消耗元素收益;
    public float 剩余时长; // 剩余时长（秒）
    public float 增幅强度; // 增幅强度
}

[System.Serializable]
public class 元素按键配置
{
    public 元素 元素;
    public 消耗元素收益 消耗元素收益;
    public 元素 生成元素;
    public InputAction 按键;
}

[System.Serializable]
public class 触发事件与对应数据
{
    public 触发事件 触发事件;
    public 触发对象 易相触发对象;
    public 元素 易相改变属性;
}

[System.Serializable]
public class 技能文件数据
{
    public string 技能名称;
    public string ID;
    public 元素[] 消耗元素;
    public float 技能CD;
    public 掌握状态 掌握状态;
    public 触发事件与对应数据[] 触发事件与对应数据;
    public string 技能描述;
}

[System.Serializable]
public class 元素剩余量
{
    public 元素 元素;
    public int 元素量上限;
    public int 当前剩余元素量;
}

