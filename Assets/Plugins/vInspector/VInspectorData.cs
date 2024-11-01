#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Type = System.Type;
using static VInspector.VInspectorState;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace VInspector
{
    public class VInspectorData : ScriptableObject
    {
        public List<Bookmark> bookmarks = new();

        [Serializable]
        public class Bookmark
        {
            public GlobalID globalId;
            public string _typeString;
            public Object _obj;


            public bool isSceneGameObject;
            public bool isAsset;

            public int id;


            public Bookmark(Object o)
            {
                globalId = o.GetGlobalID();

                id = Random.value.GetHashCode();

                isSceneGameObject = o is GameObject go && go.scene.rootCount != 0;
                isAsset = !isSceneGameObject;

                _typeString = o.GetType().AssemblyQualifiedName;

                _name = o.name;

                _obj = o;
            }


            public Type type => Type.GetType(_typeString) ?? typeof(DefaultAsset);

            public Object obj
            {
                get
                {
                    if (_obj == null && !isSceneGameObject)
                        _obj = globalId.GetObject();

                    return _obj;

                    // updating scene objects here using globalId.GetObject() could cause performance issues on large scenes
                    // so instead they are batch updated in VInspector.LoadBookmarkObjectsForScene()
                }
            }


            public bool isLoadable => obj != null;

            public bool isDeleted
            {
                get
                {
                    if (!isSceneGameObject)
                        return !isLoadable;

                    if (isLoadable)
                        return false;

                    if (!AssetDatabase.LoadAssetAtPath<SceneAsset>(globalId.guid.ToPath()))
                        return true;

                    for (var i = 0; i < SceneManager.sceneCount; i++)
                        if (SceneManager.GetSceneAt(i).path == globalId.guid.ToPath())
                            return true;

                    return false;
                }
            }

            public string assetPath => globalId.guid.ToPath();


            // [System.NonSerialized]
            public float width => VInspectorNavbar.expandedBookmarkWidth;


            public string name
            {
                get
                {
                    if (!isLoadable) return _name;

                    if (assetPath.GetExtension() == ".cs")
                        _name = obj.name.Decamelcase();
                    else
                        _name = obj.name;

                    return _name;
                }
            }

            public string _name
            {
                get => state._name;
                set => state._name = value;
            }

            public string sceneGameObjectIconName
            {
                get => state.sceneGameObjectIconName;
                set => state.sceneGameObjectIconName = value;
            }


            public BookmarkState state
            {
                get
                {
                    if (!VInspectorState.instance.bookmarkStates_byBookmarkId.ContainsKey(id))
                        VInspectorState.instance.bookmarkStates_byBookmarkId[id] = new BookmarkState();

                    return VInspectorState.instance.bookmarkStates_byBookmarkId[id];
                }
            }
        }


        [CustomEditor(typeof(VInspectorData))]
        private class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                var style = new GUIStyle(EditorStyles.label) { wordWrap = true };


                SetGUIEnabled(false);
                BeginIndent(0);

                Space(10);
                EditorGUILayout.LabelField("This file stores bookmarks from vInspector's navigation bar", style);

                EndIndent(10);
                ResetGUIEnabled();

                // Space(15);
                // base.OnInspectorGUI();
            }
        }
    }
}
#endif