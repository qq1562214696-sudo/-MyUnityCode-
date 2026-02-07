// 文件名建议： MySidebarPanel.jsx
// 建议保存到：
// Windows: C:\Program Files\Adobe\Adobe Photoshop CS6 (64 Bit)\Presets\Scripts\
// Mac: /Applications/Adobe Photoshop CS6/Presets/Scripts/

#target photoshop
#targetengine "MySidebarSession"   // 非常关键！让引擎持久存在

// 防止重复运行
if (typeof mySidebarPanel !== "undefined" && mySidebarPanel instanceof Window && mySidebarPanel.visible) {
    mySidebarPanel.show();          // 已经存在就只是显示
    // 或者 mySidebarPanel.close(); mySidebarPanel = null; 再重新建
} else {
    buildSidebarPanel();
}

function buildSidebarPanel() {

    // 使用资源字符串方式定义 UI（最稳定）
    var res = 
    "palette { \
        text: '我的侧边栏工具', \
        alignChildren: ['fill', 'top'], \
        properties: {resizable: true}, \
        preferredSize: [220, 300], \
        grp: Group { \
            orientation: 'column', \
            alignChildren: ['fill', 'top'], \
            spacing: 8, \
            margins: 10, \
            btn1: Button { text: '按钮 1', preferredSize: [-1, 32] }, \
            btn2: Button { text: '按钮 2（图层相关）', preferredSize: [-1, 32] }, \
            btn3: Button { text: '按钮 3（颜色）', preferredSize: [-1, 32] }, \
            btn4: Button { text: '清空图层', preferredSize: [-1, 32] }, \
            btn5: Button { text: '关闭面板', preferredSize: [-1, 32] }, \
            spacer: Group { preferredSize: [-1, 20] }, \
            lbl: StaticText { text: '暂时没功能，只是测试', justified: 'center' } \
        } \
    }";

    // 创建窗口
    var win = new Window(res);
    
    // 给按钮加一点点击反馈（目前只是 alert，未来可换成功能）
    win.grp.btn1.onClick = function() {
        alert("你点击了按钮 1", "提示");
    };

    win.grp.btn2.onClick = function() {
        alert("按钮 2 被按下（可以放图层相关的代码）", "提示");
    };

    win.grp.btn3.onClick = function() {
        alert("按钮 3（颜色相关预留）", "提示");
    };

    win.grp.btn4.onClick = function() {
        try {
            alert("假设要清空图层（仅演示）", "提示");
            // 未来可放真正的删除图层代码
        } catch(e) {}
    };

    win.grp.btn5.onClick = function() {
        win.close();
    };

    // 窗口关闭时清理引用（可选）
    win.onClose = function() {
        mySidebarPanel = null; // 允许下次重新创建
    };

    // 存到全局变量，防止被垃圾回收
    mySidebarPanel = win;

    // 显示（非模态，不会阻塞）
    win.show();

    // 可选：尝试让它更像面板（CS6 不一定完全成功）
    win.show();
}