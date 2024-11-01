#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using Type = System.Type;
using static VInspector.VInspector;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;


namespace VInspector
{
    public class VInspectorComponentWindow : EditorWindow
    {
        private static readonly Dictionary<Type, Texture> componentIcons_byType = new();

        public static List<VInspectorComponentWindow> instances = new();

        public static VInspectorComponentWindow draggedInstance;
        public Vector2 dragStartMousePos;
        public Vector2 dragStartWindowPos;

        public bool isResizingHorizontally;
        public bool isResizingVertically;
        public Vector2 resizeStartMousePos;
        public Vector2 resizeStartWindowSize;

        public float scrollPosition;

        public float targetHeight;
        public float maxHeight;
        public float prevHeight;

        public Component component;
        public Editor editor;
        private bool hasCustomUITKEditor;
        public InspectorElement inspectorElement;

        private bool skipHeightUpdate;

        public bool isDragged => draggedInstance == this;

        private bool useUITK => editor.target is MonoBehaviour && (HasUITKOnlyDrawers(editor.serializedObject) || hasCustomUITKEditor);

        private void OnDestroy()
        {
            editor?.DestroyImmediate();

            if (instances.Contains(this))
                instances.Remove(this);
        }

        private void OnGUI()
        {
            if (!component)
            {
                Close();
                return;
            }

            if (!editor)
            {
                Init(component);
                skipHeightUpdate = true;
            }


            void background()
            {
                position.SetPos(0, 0).Draw(GUIColors.windowBackground);
            }

            void outline()
            {
                if (Application.platform == RuntimePlatform.OSXEditor) return;

                position.SetPos(0, 0).DrawOutline(Greyscale(.1f));
            }

            void header()
            {
                var headerRect = ExpandWidthLabelRect(18).Resize(-1).AddWidthFromMid(6);
                var closeButtonRect = headerRect.SetWidthFromRight(16).SetHeightFromMid(16).Move(-4, 0);

                var backgroundColor = isDarkTheme ? Greyscale(.25f) : GUIColors.windowBackground;

                void startDragging()
                {
                    if (isResizingVertically) return;
                    if (isResizingHorizontally) return;
                    if (isDragged) return;
                    if (!curEvent.isMouseDrag) return;
                    if (!headerRect.IsHovered()) return;


                    draggedInstance = this;

                    dragStartMousePos = curEvent.mousePosition_screenSpace;
                    dragStartWindowPos = position.position;
                }

                void updateDragging()
                {
                    if (!isDragged) return;


                    var draggedPosition = dragStartWindowPos + curEvent.mousePosition_screenSpace - dragStartMousePos;

                    if (!curEvent.isRepaint)
                        position = position.SetPos(draggedPosition);


                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                }

                void stopDragging()
                {
                    if (!isDragged) return;
                    if (!curEvent.isMouseMove && !curEvent.isMouseUp) return;


                    draggedInstance = null;

                    GUIUtility.hotControl = 0;
                }

                void background()
                {
                    headerRect.Draw(backgroundColor);

                    headerRect.SetHeightFromBottom(1).Draw(isDarkTheme ? Greyscale(.2f) : Greyscale(.7f));
                }

                void icon()
                {
                    var iconRect = headerRect.SetWidth(20).MoveX(14).MoveY(-1);

                    if (!componentIcons_byType.ContainsKey(component.GetType()))
                        componentIcons_byType[component.GetType()] = EditorGUIUtility.ObjectContent(component, component.GetType()).image;

                    GUI.Label(iconRect, componentIcons_byType[component.GetType()]);
                }

                void toggle()
                {
                    var toggleRect = headerRect.MoveX(36).SetSize(20, 20);


                    var pi_enabled = component.GetType().GetProperty("enabled") ??
                                     component.GetType().BaseType?.GetProperty("enabled") ??
                                     component.GetType().BaseType?.BaseType?.GetProperty("enabled") ??
                                     component.GetType().BaseType?.BaseType?.BaseType?.GetProperty("enabled");


                    if (pi_enabled == null) return;

                    var enabled = (bool)pi_enabled.GetValue(component);


                    if (GUI.Toggle(toggleRect, enabled, "") == enabled) return;

                    component.RecordUndo();
                    pi_enabled.SetValue(component, !enabled);
                }

                void name()
                {
                    var nameRect = headerRect.MoveX(54).MoveY(-1);


                    var s = new GUIContent(EditorGUIUtility.ObjectContent(component, component.GetType())).text;
                    s = s.Substring(s.LastIndexOf('(') + 1);
                    s = s.Substring(0, s.Length - 1);

                    if (instances.Any(r => r.component.GetType() == component.GetType() && r.component != component))
                        s += " - " + component.gameObject.name;


                    SetLabelBold();

                    GUI.Label(nameRect, s);

                    ResetLabelStyle();
                }

                void nameCurtain()
                {
                    var flatColorRect = headerRect.SetX(closeButtonRect.x + 3).SetXMax(headerRect.xMax);
                    var gradientRect = headerRect.SetXMax(flatColorRect.x).SetWidthFromRight(30);

                    flatColorRect.Draw(backgroundColor);
                    gradientRect.DrawCurtainLeft(backgroundColor);
                }

                void closeButton()
                {
                    var iconName = "CrossIcon";
                    var iconSize = 14;
                    var color = isDarkTheme ? Greyscale(.65f) : Greyscale(.35f);
                    var colorHovered = isDarkTheme ? Greyscale(.9f) : color;
                    var colorPressed = color;


                    if (!IconButton(closeButtonRect, iconName, iconSize, color, colorHovered, colorPressed)) return;

                    Close();

                    GUIUtility.ExitGUI();
                }

                void escHint()
                {
                    if (!closeButtonRect.IsHovered()) return;
                    if (focusedWindow != this) return;

                    var textRect = headerRect.SetWidthFromRight(42).MoveY(-.5f);
                    var fontSize = 11;
                    var color = Greyscale(.65f);


                    SetLabelFontSize(fontSize);
                    SetGUIColor(color);

                    GUI.Label(textRect, "Esc");

                    ResetGUIColor();
                    ResetLabelStyle();
                }

                startDragging();
                updateDragging();
                stopDragging();

                background();
                icon();
                toggle();
                name();
                nameCurtain();
                closeButton();
                escHint();
            }

            void body_imgui()
            {
                if (useUITK) return;


                EditorGUIUtility.labelWidth = (position.width * .4f).Max(120);


                scrollPosition = EditorGUILayout.BeginScrollView(Vector2.up * scrollPosition).y;
                BeginIndent(17);


                editor?.OnInspectorGUI();

                updateHeight_imgui();


                EndIndent(1);
                EditorGUILayout.EndScrollView();


                EditorGUIUtility.labelWidth = 0;
            }

            void updateHeight_imgui()
            {
                if (useUITK) return;


                ExpandWidthLabelRect(-5);

                if (!curEvent.isRepaint) return;
                if (isResizingVertically) return;


                targetHeight = lastRect.y + 30;

                position = position.SetHeight(targetHeight.Min(maxHeight));


                prevHeight = position.height;
            }

            void updateHeight_uitk()
            {
                if (!useUITK) return;
                if (!curEvent.isRepaint) return;
                if (skipHeightUpdate)
                {
                    skipHeightUpdate = false;
                    return;
                } // crashses otherwise


                var lastElement = inspectorElement[inspectorElement.childCount - 1];

                targetHeight = lastElement.contentRect.yMax + 33;

                position = position.SetHeight(targetHeight);
            }

            void closeOnEscape()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.keyCode != KeyCode.Escape) return;

                Close();

                GUIUtility.ExitGUI();
            }

            void horizontalResize()
            {
                var showingScrollbar = targetHeight > maxHeight;

                var resizeArea = position.SetPos(0, 0).SetWidthFromRight(showingScrollbar ? 3 : 5).AddHeightFromBottom(-20);

                void startResize()
                {
                    if (isDragged) return;
                    if (isResizingHorizontally) return;
                    if (!curEvent.isMouseDown && !curEvent.isMouseDrag) return;
                    if (!resizeArea.IsHovered()) return;

                    isResizingHorizontally = true;

                    resizeStartMousePos = curEvent.mousePosition_screenSpace;
                    resizeStartWindowSize = position.size;
                }

                void updateResize()
                {
                    if (!isResizingHorizontally) return;


                    var resizedWidth = resizeStartWindowSize.x + curEvent.mousePosition_screenSpace.x - resizeStartMousePos.x;

                    var width = resizedWidth.Max(300);

                    if (!curEvent.isRepaint)
                        position = position.SetWidth(width);


                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    // GUI.focused
                }

                void stopResize()
                {
                    if (!isResizingHorizontally) return;
                    if (!curEvent.isMouseUp) return;

                    isResizingHorizontally = false;

                    GUIUtility.hotControl = 0;
                }


                EditorGUIUtility.AddCursorRect(resizeArea, MouseCursor.ResizeHorizontal);

                startResize();
                updateResize();
                stopResize();
            }

            void verticalResize()
            {
                var resizeArea = position.SetPos(0, 0).SetHeightFromBottom(5);

                void startResize()
                {
                    if (isDragged) return;
                    if (isResizingVertically) return;
                    if (!curEvent.isMouseDown && !curEvent.isMouseDrag) return;
                    if (!resizeArea.IsHovered()) return;

                    isResizingVertically = true;

                    resizeStartMousePos = curEvent.mousePosition_screenSpace;
                    resizeStartWindowSize = position.size;
                }

                void updateResize()
                {
                    if (!isResizingVertically) return;


                    var resizedHeight = resizeStartWindowSize.y + curEvent.mousePosition_screenSpace.y - resizeStartMousePos.y;

                    var height = resizedHeight.Min(targetHeight).Max(50);

                    if (!curEvent.isRepaint)
                        position = position.SetHeight(height);

                    maxHeight = height;


                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                }

                void stopResize()
                {
                    if (!isResizingVertically) return;
                    if (!curEvent.isMouseUp) return;

                    isResizingVertically = false;

                    GUIUtility.hotControl = 0;
                }


                EditorGUIUtility.AddCursorRect(resizeArea, MouseCursor.ResizeVertical);

                startResize();
                updateResize();
                stopResize();
            }


            background();
            header();
            outline();


            horizontalResize();
            verticalResize();


            Space(3);
            body_imgui();

            Space(7);

            updateHeight_uitk();
            closeOnEscape();

            if (isDragged)
                Repaint();
        }


        public void Init(Component component)
        {
            if (editor)
                editor.DestroyImmediate();

            this.component = component;
            editor = Editor.CreateEditor(component);

            hasCustomUITKEditor = editor.GetType().GetMethod("CreateInspectorGUI", maxBindingFlags) != null;

            if (!instances.Contains(this))
                instances.Add(this);


            if (!useUITK) return;

            inspectorElement = new InspectorElement(editor.serializedObject);

            inspectorElement.style.marginTop = 23;

            rootVisualElement.Add(inspectorElement);
        }


        public static void CreateDraggedInstance(Component component, Vector2 windowPosition, float windowWidth)
        {
            draggedInstance = CreateInstance<VInspectorComponentWindow>();

            draggedInstance.ShowPopup();
            draggedInstance.Init(component);
            draggedInstance.Focus();


            draggedInstance.wantsMouseMove = true;

            // draggedInstance.minSize = new Vector2(300, 50); // will make window resizeable on mac, but not on windows
            draggedInstance.maxHeight = EditorGUIUtility.GetMainWindowPosition().height * .7f;


            draggedInstance.position = Rect.zero.SetPos(windowPosition).SetWidth(windowWidth).SetHeight(200);
            draggedInstance.prevHeight = draggedInstance.position.height;

            draggedInstance.dragStartMousePos = curEvent.mousePosition_screenSpace;
            draggedInstance.dragStartWindowPos = windowPosition;
        }
    }
}
#endif