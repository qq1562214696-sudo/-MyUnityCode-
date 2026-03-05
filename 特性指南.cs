Unity C# 特性指南

目录
//自定义特性

//运行时 Runtime 特性
//序列化与Inspector显示特性
//编辑器 UnityEditor 特性
//其他 Miscellaneous 特性
//.NET 特性

//Mirror


自定义特性
[一键添加("标识")]：添加在可实例化类前，快速添加所有包含对应标识的可实例化类到对象。当使用相关工具或逻辑处理对象时，会自动查找并添加所有带有该标识的可实例化类到目标对象上，提高添加组件的效率。
[禁止重复("标识")]：添加在可实例化类前，使拥有相同标识的组件无法同时添加到同一对象。这有助于避免同一对象上重复添加具有相同功能或标识的组件，保证对象逻辑的清晰和正确性。

[随机初始值(最小值，最大值)]：添加在变量或者嵌套的变量前，使其如果没有赋值或者值为 0，就在检查器获取随机初始值，最大最小值为 float 或者 int。在编辑对象时，如果该变量未设置值或者值为 0，系统会自动在指定的最小值和最大值范围内生成一个随机值作为初始值。

[图层选择]：添加在变量前，用于在检查器中方便地选择 Unity 中的图层。当变量类型为合适的整数类型（用于存储图层索引）时，在检查器中会显示一个图层选择下拉菜单，方便开发者选择所需的图层，而无需手动输入图层索引。
[标签选择]：添加在变量前，用于在检查器中方便地选择 Unity 中的标签。当变量类型为字符串类型（用于存储标签名称）时，在检查器中会显示一个标签选择下拉菜单，方便开发者选择所需的标签，简化标签设置的操作流程。

[静态方法重命名("命名")]：添加在方法前，用作指令台系统快速调取指令使用。在指令台系统中，可通过此重命名后的名称来快速调用该方法，方便操作和管理。
[方法按钮]：添加在方法前，会在检查器中为该方法生成一个按钮。按钮显示名称为特性中指定的名称，点击该按钮即可在编辑器中直接调用对应的方法，无需在代码中手动调用，方便在开发过程中快速测试和执行方法。

[布尔显示("布尔变量名", bool 条件值)]：添加在变量或者嵌套的变量前，使其只有在指定的条件布尔变量的值等于设定的条件值时，才会显示在检查器中。通过这种条件显示的方式，可以根据布尔逻辑来控制变量在编辑器中的可见性，便于根据不同的状态管理和配置变量。
[枚举显示("枚举变量名", int 枚举值)]：添加在变量或者嵌套的变量前，使其只有在对应枚举变量处于对应枚举值时才显示在检查器（Inspector）中。通过这种方式，可以根据枚举状态灵活控制变量在编辑器中的显示情况，便于管理和配置。


1. 运行时 Runtime 特性
[DisallowMultipleComponent]: 禁止在同一 GameObject 上添加多个相同类型的组件。
[RequireComponent]: 强制添加依赖组件；当脚本被移除时，依赖组件不会自动移除，除非该组件也未被其他脚本依赖。
[SelectionBaseAttribute]: 使挂载此特性的GameObject优先被选中。
[SharedBetweenAnimatorsAttribute]: 确保StateMachineBehaviour只实例化一次并在所有Animator之间共享。
[RuntimeInitializeOnLoadMethodAttribute]: 标记方法在运行时初始化，可以选择在场景加载前或后执行。

2.序列化与Inspector显示特性
[SerializeField]: 序列化字段，可以将私有字段显示到Inspector上。
[HideInInspector]: 隐藏字段，可以将公共字段隐藏不显示在Inspector上。
[Header]: 显示字段的标题，简单描述字段。
[Tooltip]: 显示字段详情，通常用于较长的解释文本。
[Range]: 显示属性范围的进度条，但使用代码仍然可以超范围赋值。
[Space]: 修改与上一个字段间的间隔。
[TextArea]: 设置字符串字段在Inspector中显示为多行文本框。
[Multiline]: 同[TextArea]，指定最小和最大行数。
[ContextMenuItem]: 为变量添加右键菜单项。
[ColorUsage]: 控制颜色选择器的行为（Alpha, HDR）。
[Min]: 设定数值类型的最小值。
[Delayed]: 对于基本数据类型 float, int 和 string，在Inspector被修改时不立即生效，而是在按下回车或失去焦点时。
[NonSerialized]: 排除字段不被序列化。
[SerializeReference]: 序列化引用类型对象。

3.编辑器 UnityEditor 特性
[CanEditMultipleObjects]: 支持同时编辑多个对象。
[CustomEditor]: 为特定组件创建自定义的Inspector界面。
[CustomPropertyDrawer]: 为特定类型的属性创建自定义绘制器。
[InitializeOnEnterPlayMode]: 在UnityEditor进入运行模式时执行标记的方法。
[InitializeOnLoad]: 标记的类的静态构造函数将在UnityEditor首次加载时或重新编译脚本时被调用。
[InitializeOnLoadMethod]: 标记的静态方法将在UnityEditor首次加载时或重新编译脚本时被调用。
[MenuItem]: 添加按钮到菜单栏或上下文菜单。
[DrawGizmo]: 注册gizmo绘制逻辑。
[PostProcessBuild]: 构建完成后调用的方法。
[PostProcessScene]: 场景加载后调用的方法。
[CustomGridBrush]: 定义可作为网格刷使用的类。
[ExcludeFromObjectFactory]: 禁止该类及其子类被 ObjectFactory 的方法创建。
[ExcludeFromPreset]: 禁止该类实例被用于创建 Preset 对象。
[ExecuteAlways]: 使脚本无论在 Edit Mode、 Play Mode 都执行。
[ExecuteInEditMode]: 使脚本仅在 Edit Mode 下执行。

4. 其他 Miscellaneous 特性
[AddComponentMenu]: 将脚本置于UnityEditor顶部菜单栏Component菜单中的任意选项。
[CreateAssetMenu]: 创建自定义资源类型并添加到Assets / Create菜单中。
[HelpURL]: 为组件添加帮助链接。
[BeforeRenderOrder]: 注册Application.onBeforeRender事件回调。
[AssemblyIsEditorAssembly]: 将程序集标记为编辑器专用。
[Preserve]: 告诉编译器不要移除这个类或方法，即使它看起来没有被使用。
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] 和[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]: 标记方法在运行时初始化，可以选择在场景加载前或后执行。
[DefaultExecutionOrder(order)]: 设置脚本执行顺序的优先级。

5. .NET 特性
[System.Serializable]: 标记类或结构体为.NET序列化兼容。
[System.Obsolete]: 标记过时的方法或属性。
[System.NonSerialized]: 排除字段不被.NET序列化。
[System.Diagnostics.Conditional("DEBUG")]: 只在条件为真时编译方法。



6.Unity Mirror 网络同步属性简要说明
方法执行控制
[Command]: 客户端调用但在服务器上执行的方法，适用于需要服务器验证或处理的逻辑。
[ClientRpc]: 服务器调用但在所有客户端上执行的方法，用于广播状态变化或更新。
[TargetRpc]: 服务器调用但在特定客户端上执行的方法，适合私聊或个性化提示。

执行环境限定
[ServerCallback]: 服务器执行后自动在客户端调用同名方法，实现服务器到客户端的回调。
[Client]: 方法仅在客户端执行，确保某些逻辑不会在服务器上运行。
[Server]: 方法仅在服务器执行，确保全局逻辑不会在客户端上运行。

数据同步
[SyncVar]: 自动同步变量值，保持服务器和客户端之间的重要状态一致。
[SyncList]: 同步列表数据，适用于玩家列表或物品列表等动态数据。
[SyncObject]: 同步复杂对象的数据，确保对象状态在网络间一致。

特殊功能
[Observers]: 方法会在观察者列表中的每个客户端上执行，用于向特定组的客户端发送消息。
[ClientAuthority]: 给予客户端对组件的控制权，如允许玩家控制自己的角色。