#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using static VHierarchy.Libs.VUtils;


namespace VHierarchy
{
    internal class VHierarchyMenu
    {
        private const string dir = "Tools/vHierarchy/";

        private const string componentMinimap = dir + "Component minimap";
        private const string hierarchyLines = dir + "Hierarchy lines";
        private const string minimalMode = dir + "Minimal mode";
        private const string zebraStriping = dir + "Zebra striping";
        private const string activationToggle = dir + "Activation toggle";
        private const string collapseAllButton = dir + "Collapse All button";
        private const string editLightingButton = dir + "Edit Lighting button";

        private const string toggleActive = dir + "A to toggle active";
        private const string focus = dir + "F to focus";
        private const string delete = dir + "X to delete";
        private const string toggleExpanded = dir + "E to expand or collapse";
        private const string collapseEverythingElse = dir + "Shift-E to isolate";
        private const string collapseEverything = dir + "Ctrl-Shift-E to collapse all";

        private const string disablePlugin = dir + "Disable vHierarchy";

        public static bool componentMinimapEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-componentMinimapEnabled");
            set => EditorPrefsCached.SetBool("vHierarchy-componentMinimapEnabled", value);
        }

        public static bool hierarchyLinesEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-hierarchyLinesEnabled");
            set => EditorPrefsCached.SetBool("vHierarchy-hierarchyLinesEnabled", value);
        }

        public static bool minimalModeEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-minimalModeEnabled");
            set => EditorPrefsCached.SetBool("vHierarchy-minimalModeEnabled", value);
        }

        public static bool zebraStripingEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-zebraStripingEnabled");
            set => EditorPrefsCached.SetBool("vHierarchy-zebraStripingEnabled", value);
        }

        public static bool activationToggleEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-acctivationToggleEnabled");
            set => EditorPrefsCached.SetBool("vHierarchy-acctivationToggleEnabled", value);
        }

        public static bool collapseAllButtonEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-collapseAllButtonEnabled");
            set => EditorPrefsCached.SetBool("vHierarchy-collapseAllButtonEnabled", value);
        }

        public static bool editLightingButtonEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-editLightingButtonEnabled");
            set => EditorPrefsCached.SetBool("vHierarchy-editLightingButtonEnabled", value);
        }

        public static bool toggleActiveEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-toggleActiveEnabled", true);
            set => EditorPrefsCached.SetBool("vHierarchy-toggleActiveEnabled", value);
        }

        public static bool focusEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-focusEnabled", true);
            set => EditorPrefsCached.SetBool("vHierarchy-focusEnabled", value);
        }

        public static bool deleteEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-deleteEnabled", true);
            set => EditorPrefsCached.SetBool("vHierarchy-deleteEnabled", value);
        }

        public static bool toggleExpandedEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-toggleExpandedEnabled", true);
            set => EditorPrefsCached.SetBool("vHierarchy-toggleExpandedEnabled", value);
        }

        public static bool collapseEverythingElseEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-collapseEverythingElseEnabled", true);
            set => EditorPrefsCached.SetBool("vHierarchy-collapseEverythingElseEnabled", value);
        }

        public static bool collapseEverythingEnabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-collapseEverythingEnabled", true);
            set => EditorPrefsCached.SetBool("vHierarchy-collapseEverythingEnabled", value);
        }

        public static bool pluginDisabled
        {
            get => EditorPrefsCached.GetBool("vHierarchy-pluginDisabled");
            set => EditorPrefsCached.SetBool("vHierarchy-pluginDisabled", value);
        }


        [MenuItem(dir + "Features", false, 1)]
        private static void daasddsas()
        {
        }

        [MenuItem(dir + "Features", true, 1)]
        private static bool dadsdasas123()
        {
            return false;
        }

        [MenuItem(componentMinimap, false, 2)]
        private static void daadsdsadasdadsas()
        {
            componentMinimapEnabled = !componentMinimapEnabled;
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem(componentMinimap, true, 2)]
        private static bool dadsadasddasadsas()
        {
            Menu.SetChecked(componentMinimap, componentMinimapEnabled);
            return !pluginDisabled;
        }

        [MenuItem(hierarchyLines, false, 3)]
        private static void dadsadadsadadasss()
        {
            hierarchyLinesEnabled = !hierarchyLinesEnabled;
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem(hierarchyLines, true, 3)]
        private static bool dadsaddasdasaasddsas()
        {
            Menu.SetChecked(hierarchyLines, hierarchyLinesEnabled);
            return !pluginDisabled;
        }

        [MenuItem(minimalMode, false, 4)]
        private static void dadsadadasdsdasadadasss()
        {
            minimalModeEnabled = !minimalModeEnabled;
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem(minimalMode, true, 4)]
        private static bool dadsaddadsasdadsasaasddsas()
        {
            Menu.SetChecked(minimalMode, minimalModeEnabled);
            return !pluginDisabled;
        }

        [MenuItem(zebraStriping, false, 5)]
        private static void dadsadadadssadsadass()
        {
            zebraStripingEnabled = !zebraStripingEnabled;
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem(zebraStriping, true, 5)]
        private static bool dadsaddadaadsssadsaasddsas()
        {
            Menu.SetChecked(zebraStriping, zebraStripingEnabled);
            return !pluginDisabled;
        }

        [MenuItem(activationToggle, false, 6)]
        private static void daadsdsadadsasdadsas()
        {
            activationToggleEnabled = !activationToggleEnabled;
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem(activationToggle, true, 6)]
        private static bool dadsadasdsaddasadsas()
        {
            Menu.SetChecked(activationToggle, activationToggleEnabled);
            return !pluginDisabled;
        }

        [MenuItem(collapseAllButton, false, 7)]
        private static void daadsdsadadsadadsas()
        {
            collapseAllButtonEnabled = !collapseAllButtonEnabled;
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem(collapseAllButton, true, 7)]
        private static bool dadsadasdsaddasdsas()
        {
            Menu.SetChecked(collapseAllButton, collapseAllButtonEnabled);
            return !pluginDisabled;
        }

        [MenuItem(editLightingButton, false, 8)]
        private static void daadsdsasdadadsadadsas()
        {
            editLightingButtonEnabled = !editLightingButtonEnabled;
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem(editLightingButton, true, 8)]
        private static bool dadsadasdsadsaddasdsas()
        {
            Menu.SetChecked(editLightingButton, editLightingButtonEnabled);
            return !pluginDisabled;
        }


        [MenuItem(dir + "Shortcuts", false, 101)]
        private static void dadsas()
        {
        }

        [MenuItem(dir + "Shortcuts", true, 101)]
        private static bool dadsas123()
        {
            return false;
        }

        [MenuItem(toggleActive, false, 102)]
        private static void dadsadadsas()
        {
            toggleActiveEnabled = !toggleActiveEnabled;
        }

        [MenuItem(toggleActive, true, 102)]
        private static bool dadsaddasadsas()
        {
            Menu.SetChecked(toggleActive, toggleActiveEnabled);
            return !pluginDisabled;
        }

        [MenuItem(focus, false, 103)]
        private static void dadsadasdadsas()
        {
            focusEnabled = !focusEnabled;
        }

        [MenuItem(focus, true, 103)]
        private static bool dadsadsaddasadsas()
        {
            Menu.SetChecked(focus, focusEnabled);
            return !pluginDisabled;
        }

        [MenuItem(delete, false, 104)]
        private static void dadsadsadasdadsas()
        {
            deleteEnabled = !deleteEnabled;
        }

        [MenuItem(delete, true, 104)]
        private static bool dadsaddsasaddasadsas()
        {
            Menu.SetChecked(delete, deleteEnabled);
            return !pluginDisabled;
        }

        [MenuItem(toggleExpanded, false, 105)]
        private static void dadsadsadasdsadadsas()
        {
            toggleExpandedEnabled = !toggleExpandedEnabled;
        }

        [MenuItem(toggleExpanded, true, 105)]
        private static bool dadsaddsasadadsdasadsas()
        {
            Menu.SetChecked(toggleExpanded, toggleExpandedEnabled);
            return !pluginDisabled;
        }

        [MenuItem(collapseEverythingElse, false, 106)]
        private static void dadsadsasdadasdsadadsas()
        {
            collapseEverythingElseEnabled = !collapseEverythingElseEnabled;
        }

        [MenuItem(collapseEverythingElse, true, 106)]
        private static bool dadsaddsdasasadadsdasadsas()
        {
            Menu.SetChecked(collapseEverythingElse, collapseEverythingElseEnabled);
            return !pluginDisabled;
        }

        [MenuItem(collapseEverything, false, 107)]
        private static void dadsadsdasadasdsadadsas()
        {
            collapseEverythingEnabled = !collapseEverythingEnabled;
        }

        [MenuItem(collapseEverything, true, 107)]
        private static bool dadsaddssdaasadadsdasadsas()
        {
            Menu.SetChecked(collapseEverything, collapseEverythingEnabled);
            return !pluginDisabled;
        }


        [MenuItem(dir + "More", false, 1001)]
        private static void daasadsddsas()
        {
        }

        [MenuItem(dir + "More", true, 1001)]
        private static bool dadsadsdasas123()
        {
            return false;
        }


        [MenuItem(dir + "Open manual", false, 1002)]
        private static void dadadsasdsadsas()
        {
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(GetScriptPath("VHierarchy").GetParentPath().CombinePath("Manual.pdf")));
        }


        [MenuItem(dir + "Join our Discord", false, 1003)]
        private static void dadasdsas()
        {
            Application.OpenURL("https://discord.gg/4dG9KsbspG");
        }


        [MenuItem(dir + "Deals ending soon/Get vFolders 2 at 50% off", false, 1004)]
        private static void dadadssadasdsas()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/255470?aid=1100lGLBn&pubref=deal50menu");
        }

        [MenuItem(dir + "Deals ending soon/Get vInspector 2 at 50% off", false, 1005)]
        private static void dadadssadsas()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/252297?aid=1100lGLBn&pubref=deal50menu");
        }

        [MenuItem(dir + "Deals ending soon/Get vTabs 2 at 50% off", false, 1006)]
        private static void dadadadsssadsas()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/263645?aid=1100lGLBn&pubref=deal50menu");
        }

        [MenuItem(dir + "Deals ending soon/Get vFavorites 2 at 50% off", false, 1007)]
        private static void dadadadsssadsadsas()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/263643?aid=1100lGLBn&pubref=deal50menu");
        }


        [MenuItem(disablePlugin, false, 10001)]
        private static void dadsadsdasadasdasdsadadsas()
        {
            pluginDisabled = !pluginDisabled;
            CompilationPipeline.RequestScriptCompilation();
        }

        [MenuItem(disablePlugin, true, 10001)]
        private static bool dadsaddssdaasadsadadsdasadsas()
        {
            Menu.SetChecked(disablePlugin, pluginDisabled);
            return true;
        }


        // [MenuItem(dir + "Clear cache", false, 10001)]
        // static void dassaadsdc() => VHierarchyCache.Clear();
    }
}
#endif