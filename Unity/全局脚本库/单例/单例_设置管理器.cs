using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace 设置系统
{
    public class 单例_设置管理器 : MonoBehaviour
    {
        public static 单例_设置管理器 _实例;

        [Header("音效设置")]
        public 是否枚举 是否静音 = 是否枚举.否;
        public Button 静音开关按钮;
        [Range(0, 100)]
        public int 音量 = 100;
        public Slider 音量滑动条;

        [Header("画面设置")]
        public 是否枚举 是否全屏 = 是否枚举.是;
        public Button 全屏开关按钮;
        public 分辨率 分辨率 = 分辨率.分辨率1080P;
        public Dropdown 分辨率选择下拉列表;

        public List<设置项> 设置项列表 = new();

        [Header("帧率设置")]
        public Dropdown 帧率选择下拉列表;
        public 帧率选项[] 帧率选项列表;
        public 是否枚举 帧率显示 = 是否枚举.是;
        public Button 帧率显示按钮;
        public Text 帧率文本;

        private readonly float 帧时间差;
        private float 帧刷新间隔 = 0;
        private int 帧率上限 = -1;
        private int 帧率下拉列表索引 = 0;

        void Awake()
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

            加载数据();
            初始化设置();
        }

        void Update()
        {
            if (帧率显示 == 是否枚举.是)
            {
                帧刷新间隔 += Time.deltaTime;

                if (帧刷新间隔 > 1f)
                {
                    帧率文本.text = "FPS: " + 静态.帧率计算(帧时间差);
                    帧刷新间隔 = 0.0f;
                }
            }
        }

        private void OnApplicationQuit()
        {
            储存数据();
        }

        public void 加载数据()
        {
            string 文件路径 = 静态.获取文件路径(常量.设置管理文件名);

            if (!File.Exists(文件路径))
            {
                Debug.Log("数据文件不存在, 正在创建默认数据...");
                储存数据();
            }

            if (File.Exists(文件路径))
            {
                Debug.Log("加载设置管理配置文件自: " + 文件路径);
                XmlDocument 设置管理文件XML = new();
                设置管理文件XML.Load(文件路径);

                XmlNode 设置节点 = 设置管理文件XML.SelectSingleNode("//设置管理数据/设置管理数据");

                if (设置节点 != null)
                {
                    是否静音 = (是否枚举)System.Enum.Parse(typeof(是否枚举), 设置节点.Attributes["是否静音"].Value);
                    音量 = (int)(float.Parse(设置节点.Attributes["音量"].Value)); // 直接将音量值转换为整数
                    音量滑动条.value = 音量 / 100f; // 将音量转换为 0-1 范围
                    是否全屏 = (是否枚举)System.Enum.Parse(typeof(是否枚举), 设置节点.Attributes["是否全屏"].Value);
                    分辨率 = (分辨率)System.Enum.Parse(typeof(分辨率), 设置节点.Attributes["分辨率"].Value);
                    帧率显示 = (是否枚举)System.Enum.Parse(typeof(是否枚举), 设置节点.Attributes["帧率显示"].Value);

                    // 加载每个设置项的数据
                    foreach (var 设置项 in 设置项列表)
                    {
                        var 设定节点 = 设置管理文件XML.SelectSingleNode($"//设置管理数据/{设置项.设置对象.name}");
                        if (设定节点 != null)
                        {
                            设置项.是否开启 = (是否枚举)System.Enum.Parse(typeof(是否枚举), 设定节点.Attributes["是否开启"].Value);
                        }
                    }

                    if (设置节点.Attributes["帧率下拉列表指数"] != null)
                    {
                        int.TryParse(设置节点.Attributes["帧率下拉列表指数"].Value, out 帧率下拉列表索引);
                        设置帧率(帧率下拉列表索引); // 设置帧率下拉列表
                    }
                }
                else
                {
                    Debug.LogError("未找到设置节点");
                }
            }
        }

        public void 储存数据()
        {
            string 文件路径 = 静态.获取文件路径(常量.设置管理文件名);
            Debug.Log("保存设置管理配置文件至: " + 文件路径);

            XmlDocument 设置管理文件 = new();
            XmlElement root = 设置管理文件.CreateElement("设置管理数据");
            设置管理文件.AppendChild(root);

            XmlElement 设置元素 = 设置管理文件.CreateElement("设置管理数据");
            设置元素.SetAttribute("是否静音", 是否静音.ToString());
            设置元素.SetAttribute("音量", 音量.ToString());
            设置元素.SetAttribute("是否全屏", 是否全屏.ToString());
            设置元素.SetAttribute("分辨率", 分辨率.ToString());
            设置元素.SetAttribute("帧率上限", 帧率上限.ToString());
            设置元素.SetAttribute("帧率下拉列表指数", 帧率下拉列表索引.ToString());
            设置元素.SetAttribute("帧率显示", 帧率显示.ToString());

            foreach (var 设置项 in 设置项列表)
            {
                XmlElement 设定元素 = 设置管理文件.CreateElement(设置项.设置对象.name);
                设定元素.SetAttribute("是否开启", 设置项.是否开启.ToString());
                设置元素.AppendChild(设定元素);
            }

            root.AppendChild(设置元素);
            设置管理文件.Save(文件路径);
        }



        private void 初始化设置()
        {
            静音开关按钮.GetComponentInChildren<Text>().text = 是否静音 == 是否枚举.是 ? "取消静音" : "静音";
            帧率显示按钮.GetComponentInChildren<Text>().text = 帧率显示 == 是否枚举.是 ? "隐藏帧率" : "显示帧率";
            全屏开关按钮.GetComponentInChildren<Text>().text = 是否全屏 == 是否枚举.是 ? "窗口化" : "全屏";

            GameObject 文本对象 = 帧率文本.gameObject;
            文本对象.SetActive(帧率显示 == 是否枚举.是);
            设置分辨率();

            // 遍历设置项列表并初始化每个设置项的按钮
            foreach (var 设置项 in 设置项列表)
            {
                // 先初始化按钮的状态
                初始化设置按钮(设置项);

                // 添加按钮的点击事件
                设置项.设置按钮.onClick.AddListener(delegate { 初始化设置项按钮(设置项); });
                设置项.设置按钮.GetComponentInChildren<Text>().text = 设置项.是否开启 == 是否枚举.是 ? "关闭" : "启用";
            }

            // 添加静音按钮事件
            静音开关按钮.onClick.AddListener(设置静音按钮);
            // 添加全屏按钮事件
            全屏开关按钮.onClick.AddListener(设置全屏按钮);
            // 添加帧率按钮事件
            帧率显示按钮.onClick.AddListener(设置帧率文本);
            音量滑动条.onValueChanged.AddListener(设置音量滑动条);

            // 初始化分辨率下拉列表
            分辨率选择下拉列表.ClearOptions();
            List<string> 分辨率选项 = new() { "720P", "1080P", "2K", "4K" };
            分辨率选择下拉列表.AddOptions(分辨率选项);
            分辨率选择下拉列表.value = (int)分辨率; // 设置当前选项
            分辨率选择下拉列表.onValueChanged.AddListener(设置分辨率下拉列表);

            // 初始化帧率下拉列表
            帧率选择下拉列表.ClearOptions();
            List<string> 下拉列表按钮选项 = new();

            foreach (var 帧率选项 in 帧率选项列表)
            {
                if (帧率选项.帧率上限 == -1)
                {
                    下拉列表按钮选项.Add("帧率无限制");
                }
                else if (帧率选项.帧率上限 == 0)
                {
                    下拉列表按钮选项.Add("垂直同步");
                }
                else
                {
                    下拉列表按钮选项.Add("帧率上限：" + 帧率选项.帧率上限.ToString());
                }
            }
            帧率选择下拉列表.AddOptions(下拉列表按钮选项);
            帧率选择下拉列表.onValueChanged.AddListener(设置帧率);
            帧率选择下拉列表.value = 帧率下拉列表索引;
        }

        private void 初始化设置按钮(设置项 设置项)
        {
            if (设置项.设置对象 != null)
            {
                设置项.设置对象.SetActive(设置项.是否开启 == 是否枚举.是);
            }
        }

        public void 初始化设置项按钮(设置项 设置项)
        {
            设置项.是否开启 = 设置项.是否开启 == 是否枚举.是 ? 是否枚举.否 : 是否枚举.是;
            设置项.设置按钮.GetComponentInChildren<Text>().text = 设置项.是否开启 == 是否枚举.是 ? "关闭" : "启用";
            初始化设置按钮(设置项); // 应用设置
            储存数据(); // 保存设置到文件
        }

        private void 设置分辨率下拉列表(int 索引)
        {
            分辨率 = (分辨率)索引;
            设置分辨率();
            储存数据();
        }

        public void 设置静音按钮()
        {
            是否静音 = 是否静音 == 是否枚举.是 ? 是否枚举.否 : 是否枚举.是;
            设置音效();
            静音开关按钮.GetComponentInChildren<Text>().text = 是否静音 == 是否枚举.是 ? "取消静音" : "静音";
            储存数据();
        }

        public void 设置全屏按钮()
        {
            是否全屏 = 是否全屏 == 是否枚举.是 ? 是否枚举.否 : 是否枚举.是;

            Screen.fullScreen = 是否全屏 == 是否枚举.是; // 切换全屏模式
            if (是否全屏 == 是否枚举.否)
            {
                设置分辨率(); // 设置窗口尺寸
            }

            全屏开关按钮.GetComponentInChildren<Text>().text = 是否全屏 == 是否枚举.是 ? "窗口化" : "全屏";
            储存数据();
        }

        private void 设置帧率文本()
        {
            帧率显示 = 帧率显示 == 是否枚举.是 ? 是否枚举.否 : 是否枚举.是;
            帧率显示按钮.GetComponentInChildren<Text>().text = 帧率显示 == 是否枚举.是 ? "隐藏帧率" : "显示帧率";
            GameObject 文本对象 = 帧率文本.gameObject;
            文本对象.SetActive(帧率显示 == 是否枚举.是);
            储存数据();
        }

        private void 设置帧率(int 索引)
        {
            if (索引 >= 0 && 索引 <= 帧率选项列表.Length)
            {
                帧率下拉列表索引 = 索引;

                帧率上限 = 索引 == 帧率选项列表.Length ? -1 : 帧率选项列表[索引].帧率上限;

                QualitySettings.vSyncCount = 帧率上限 == 0 ? 1 : 0;
                Application.targetFrameRate = 帧率上限 == -1 ? -1 : 帧率上限;

                帧率选择下拉列表.value = 索引;

                储存数据();
            }
            else
            {
                Debug.LogError($"无效的下拉菜单索引: {索引}");
            }
        }

        public void 设置分辨率()
        {
            switch (分辨率)
            {
                case 分辨率.分辨率720P:
                    Screen.SetResolution(1280, 720, 是否全屏 == 是否枚举.是);
                    break;
                case 分辨率.分辨率1080P:
                    Screen.SetResolution(1920, 1080, 是否全屏 == 是否枚举.是);
                    break;
                case 分辨率.分辨率2K:
                    Screen.SetResolution(2560, 1440, 是否全屏 == 是否枚举.是);
                    break;
                case 分辨率.分辨率4K:
                    Screen.SetResolution(3840, 2160, 是否全屏 == 是否枚举.是);
                    break;
            }
        }

        private void 设置音量滑动条(float 音量滑动条值)
        {
            音量 = (int)(音量滑动条值 * 100); // 将滑动条值转换为整数音量
            设置音效();
        }

        private void 设置音效()
        {
            if (是否静音 == 是否枚举.是)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = 音量 / 100f; // 将音量转换为 0-1 范围
            }
        }
    }
}