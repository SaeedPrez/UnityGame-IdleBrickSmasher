#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Type = System.Type;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;
using Object = UnityEngine.Object;


namespace VInspector
{
    internal static class VInspectorResettableVariables
    {
        private static readonly Dictionary<Type, object> targetWithDefaulValues_byType = new();

        private static readonly MethodInfo mi_removeLogEntries =
            typeof(Editor).Assembly.GetType("UnityEditor.LogEntry").GetMethod("RemoveLogEntriesByMode", maxBindingFlags);

        public static void ResetButtonGUI(Rect fieldRect, SerializedProperty property, FieldInfo fieldInfo, IEnumerable<object> targets)
        {
            // if (!fieldRect.IsHovered()) return;


            var targetWithDefaultValues = GetTargetWithDefaulValues(targets.First().GetType());

            bool isResetted(object target)
            {
                if (property.isInstantiatedPrefab && !PrefabUtility.IsAddedComponentOverride(property.serializedObject.targetObject)) return !property.prefabOverride;

                if (targetWithDefaultValues == null) return true;


                var currentValue = fieldInfo.GetValue(target);
                var defaultValue = fieldInfo.GetValue(targetWithDefaultValues);

                var isResetted = Equals(currentValue, defaultValue);


                if (typeof(Object).IsAssignableFrom(fieldInfo.FieldType))
                    isResetted |= defaultValue == null && !(bool)(Object)currentValue;

                if (fieldInfo.FieldType == typeof(string))
                    isResetted |= fieldInfo.FieldType == typeof(string) && Equals(currentValue, "") && Equals(defaultValue, null);


                return isResetted;
            }

            if (targets.All(r => isResetted(r))) return;


            var iconSize = 12;
            var colorNormal = Greyscale(.41f);
            var colorHovered = Greyscale(isDarkTheme ? .9f : .0f);
            var colorPressed = Greyscale(isDarkTheme ? .65f : .6f);

            var buttonRect = fieldRect.SetWidthFromRight(20).MoveX(typeof(Object).IsAssignableFrom(fieldInfo.FieldType) ? -18 : 1);

            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.CustomCursor);


            if (!IconButton(buttonRect, "CrossIcon", iconSize, colorNormal, colorHovered, colorPressed)) return;

            if (property.isInstantiatedPrefab && !PrefabUtility.IsAddedComponentOverride(property.serializedObject.targetObject))
            {
                foreach (var target in property.serializedObject.targetObjects)
                    target.RecordUndo();

                PrefabUtility.RevertPropertyOverride(property, InteractionMode.AutomatedAction);
            }
            else
            {
                property.SetBoxedValue(fieldInfo.GetValue(targetWithDefaultValues));
            }

            GUI.changed = true;

            GUI.FocusControl(null);
        }


        private static object GetTargetWithDefaulValues(Type targetType)
        {
            if (targetWithDefaulValues_byType.TryGetValue(targetType, out var cachedResult)) return cachedResult;

            object targetWithDefaultValues = null;

            void scriptOrSO()
            {
                if (!typeof(MonoBehaviour).IsAssignableFrom(targetType) && typeof(ScriptableObject).IsAssignableFrom(targetType)) return;

                targetWithDefaultValues = Activator.CreateInstance(targetType);

                mi_removeLogEntries.Invoke(null, new object[] { 1 << 9 });
            }

            void customClass()
            {
                if (typeof(MonoBehaviour).IsAssignableFrom(targetType)) return;
                if (typeof(ScriptableObject).IsAssignableFrom(targetType)) return;
                if (targetType.GetConstructor(Type.EmptyTypes) == null) return;

                targetWithDefaultValues = Activator.CreateInstance(targetType);
            }

            scriptOrSO();
            customClass();

            return targetWithDefaulValues_byType[targetType] = targetWithDefaultValues;
        }


        public static bool IsResettable(FieldInfo fieldInfo)
        {
            if (!VInspectorMenu.resettableVariablesEnabled) return false;
            if (Application.isPlaying) return false;

            if (Attribute.IsDefined(fieldInfo, typeof(VariantsAttribute))) return false;

            if (typeof(Object).IsAssignableFrom(fieldInfo.FieldType)) return true;
            if (fieldInfo.FieldType == typeof(int)) return true;
            if (fieldInfo.FieldType == typeof(float)) return true;
            if (fieldInfo.FieldType == typeof(double)) return true;
            if (fieldInfo.FieldType == typeof(string)) return true;

            return false;
        }
    }
}
#endif