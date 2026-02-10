#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;



namespace VHierarchy
{
    class VHierarchyMenu
    {

        public static bool componentMinimapEnabled { get => EditorPrefs.GetBool("vHierarchy-componentMinimapEnabled", true); set => EditorPrefs.SetBool("vHierarchy-componentMinimapEnabled", value); }
        public static bool hierarchyLinesEnabled { get => EditorPrefs.GetBool("vHierarchy-hierarchyLinesEnabled", true); set => EditorPrefs.SetBool("vHierarchy-hierarchyLinesEnabled", value); }
        public static bool minimalModeEnabled { get => EditorPrefs.GetBool("vHierarchy-minimalModeEnabled", true); set => EditorPrefs.SetBool("vHierarchy-minimalModeEnabled", value); }
        public static bool zebraStripingEnabled { get => EditorPrefs.GetBool("vHierarchy-zebraStripingEnabled", true); set => EditorPrefs.SetBool("vHierarchy-zebraStripingEnabled", value); }
        public static bool activationToggleEnabled { get => EditorPrefs.GetBool("vHierarchy-acctivationToggleEnabled", true); set => EditorPrefs.SetBool("vHierarchy-acctivationToggleEnabled", value); }
        public static bool collapseAllButtonEnabled { get => EditorPrefs.GetBool("vHierarchy-collapseAllButtonEnabled", true); set => EditorPrefs.SetBool("vHierarchy-collapseAllButtonEnabled", value); }
        public static bool editLightingButtonEnabled { get => EditorPrefs.GetBool("vHierarchy-editLightingButtonEnabled", true); set => EditorPrefs.SetBool("vHierarchy-editLightingButtonEnabled", value); }

        public static bool toggleActiveEnabled { get => EditorPrefs.GetBool("vHierarchy-toggleActiveEnabled", true); set => EditorPrefs.SetBool("vHierarchy-toggleActiveEnabled", value); }
        public static bool focusEnabled { get => EditorPrefs.GetBool("vHierarchy-focusEnabled", true); set => EditorPrefs.SetBool("vHierarchy-focusEnabled", value); }
        public static bool deleteEnabled { get => EditorPrefs.GetBool("vHierarchy-deleteEnabled", true); set => EditorPrefs.SetBool("vHierarchy-deleteEnabled", value); }
        public static bool toggleExpandedEnabled { get => EditorPrefs.GetBool("vHierarchy-toggleExpandedEnabled", true); set => EditorPrefs.SetBool("vHierarchy-toggleExpandedEnabled", value); }
        public static bool collapseEverythingElseEnabled { get => EditorPrefs.GetBool("vHierarchy-collapseEverythingElseEnabled", true); set => EditorPrefs.SetBool("vHierarchy-collapseEverythingElseEnabled", value); }
        public static bool collapseEverythingEnabled { get => EditorPrefs.GetBool("vHierarchy-collapseEverythingEnabled", true); set => EditorPrefs.SetBool("vHierarchy-collapseEverythingEnabled", value); }

        public static bool pluginDisabled { get => EditorPrefs.GetBool("vHierarchy-pluginDisabled", false); set => EditorPrefs.SetBool("vHierarchy-pluginDisabled", value); }




        const string dir = "❤多功能工具集❤/vHierarchy(汉化版)/";

        const string componentMinimap = dir + "右侧显示包含组件";
        const string hierarchyLines = dir + "显示层级结构线";
        const string minimalMode = dir + "简约模式";
        const string zebraStriping = dir + "相邻对象斑马格分割";
        const string activationToggle = dir + "对象启用勾选框";

        const string delete = dir + "X 键删除对象";
        const string collapseEverythingElse = dir + "Shift+E 展开指针位置关闭其他展开";
        const string collapseEverything = dir + "Ctrl+Shift+E 关闭所有已展开";

        const string disablePlugin = dir + "禁用以上所有功能";



        [MenuItem(componentMinimap, false, 2)] static void daadsdsadasdadsas() { componentMinimapEnabled = !componentMinimapEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(componentMinimap, true, 2)] static bool dadsadasddasadsas() { Menu.SetChecked(componentMinimap, componentMinimapEnabled); return !pluginDisabled; }

        [MenuItem(hierarchyLines, false, 3)] static void dadsadadsadadasss() { hierarchyLinesEnabled = !hierarchyLinesEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(hierarchyLines, true, 3)] static bool dadsaddasdasaasddsas() { Menu.SetChecked(hierarchyLines, hierarchyLinesEnabled); return !pluginDisabled; }

        [MenuItem(minimalMode, false, 4)] static void dadsadadasdsdasadadasss() { minimalModeEnabled = !minimalModeEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(minimalMode, true, 4)] static bool dadsaddadsasdadsasaasddsas() { Menu.SetChecked(minimalMode, minimalModeEnabled); return !pluginDisabled; }

        [MenuItem(zebraStriping, false, 5)] static void dadsadadadssadsadass() { zebraStripingEnabled = !zebraStripingEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(zebraStriping, true, 5)] static bool dadsaddadaadsssadsaasddsas() { Menu.SetChecked(zebraStriping, zebraStripingEnabled); return !pluginDisabled; }

        [MenuItem(activationToggle, false, 6)] static void daadsdsadadsasdadsas() { activationToggleEnabled = !activationToggleEnabled; EditorApplication.RepaintHierarchyWindow(); }
        [MenuItem(activationToggle, true, 6)] static bool dadsadasdsaddasadsas() { Menu.SetChecked(activationToggle, activationToggleEnabled); return !pluginDisabled; }



        [MenuItem(delete, false, 104)] static void dadsadsadasdadsas() => deleteEnabled = !deleteEnabled;
        [MenuItem(delete, true, 104)] static bool dadsaddsasaddasadsas() { Menu.SetChecked(delete, deleteEnabled); return !pluginDisabled; }



        [MenuItem(collapseEverythingElse, false, 106)] static void dadsadsasdadasdsadadsas() => collapseEverythingElseEnabled = !collapseEverythingElseEnabled;
        [MenuItem(collapseEverythingElse, true, 106)] static bool dadsaddsdasasadadsdasadsas() { Menu.SetChecked(collapseEverythingElse, collapseEverythingElseEnabled); return !pluginDisabled; }

        [MenuItem(collapseEverything, false, 107)] static void dadsadsdasadasdsadadsas() => collapseEverythingEnabled = !collapseEverythingEnabled;
        [MenuItem(collapseEverything, true, 107)] static bool dadsaddssdaasadadsdasadsas() { Menu.SetChecked(collapseEverything, collapseEverythingEnabled); return !pluginDisabled; }


        [MenuItem(disablePlugin, false, 10001)] static void dadsadsdasadasdasdsadadsas() { pluginDisabled = !pluginDisabled; UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(); }
        [MenuItem(disablePlugin, true, 10001)] static bool dadsaddssdaasadsadadsdasadsas() { Menu.SetChecked(disablePlugin, pluginDisabled); return true; }

    }
}
#endif