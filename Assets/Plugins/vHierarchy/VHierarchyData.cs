#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;


namespace VHierarchy
{
    public class VHierarchyData : ScriptableObject, ISerializationCallbackReceiver
    {
        public SerializableDictionary<string, SceneData> sceneDatas_byGuid = new();

        public void OnBeforeSerialize()
        {
            VHierarchy.firstDataCacheLayer.Clear();
        }

        public void OnAfterDeserialize()
        {
            VHierarchy.firstDataCacheLayer.Clear();
        }


        [Serializable]
        public class SceneData
        {
            public SerializableDictionary<GlobalID, GameObjectData> goDatas_byGlobalId = new();
        }


        [Serializable]
        public class GameObjectData
        {
            public int colorIndex;
            public string iconNameOrGuid = ""; // name for buildin icons, guid for custom ones

            [NonSerialized] // set in GetGameObjectData
            public SceneData sceneData;
        }


        [CustomEditor(typeof(VHierarchyData))]
        private class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                var style = new GUIStyle(EditorStyles.label) { wordWrap = true };


                SetGUIEnabled(false);
                BeginIndent(0);

                Space(10);
                EditorGUILayout.LabelField("This file contains data about which icons and colors are assigned to objects", style);

                Space();
                GUILayout.Label(
                    "If there are multiple people working on the project, you might want to store this data in scenes to avoid merge conflicts. To do that, create a script that inherits from VHierarchy.VHierarchyDataComponent and add it to any object in the scene",
                    style);

                EndIndent(10);
                ResetGUIEnabled();
            }
        }
    }
}
#endif