﻿using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace ES3Internal
{
    public class ES3GlobalReferences : ScriptableObject
    {
#if !UNITY_EDITOR || ES3GLOBAL_DISABLED
        public static ES3GlobalReferences Instance{ get{ return null; } }
        public UnityEngine.Object Get(long id){return null;}
        public long GetOrAdd(UnityEngine.Object obj){return -1;}
        public void RemoveInvalidKeys(){}
#else

#if ES3GLOBAL_DISABLED
        private static bool useGlobalReferences = false;
#else
        private static readonly bool useGlobalReferences = true;
#endif

        private const string globalReferencesPath = "ES3/ES3GlobalReferences";

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ES3RefIdDictionary refId = new();

        private static ES3GlobalReferences _globalReferences;

        public static ES3GlobalReferences Instance
        {
            get
            {
                if (Application.isPlaying)
                    return null;

                if (!useGlobalReferences)
                    return null;

                if (_globalReferences == null)
                {
                    _globalReferences = Resources.Load<ES3GlobalReferences>(globalReferencesPath);

                    if (_globalReferences == null)
                    {
                        _globalReferences = CreateInstance<ES3GlobalReferences>();

                        // If this is the version being submitted to the Asset Store, don't include ES3Defaults.
                        if (Application.productName.Contains("ES3 Release"))
                        {
                            Debug.Log("This has been identified as a release build as the title contains 'ES3 Release', so ES3GlobalReferences will not be created.");
                            return _globalReferences;
                        }

                        ES3Settings.CreateDefaultSettingsFolder();
                        AssetDatabase.CreateAsset(_globalReferences, PathToGlobalReferences());
                        AssetDatabase.SaveAssets();
                    }
                }

                return _globalReferences;
            }
        }

        private long Get(Object obj)
        {
            if (obj == null)
                return -1;

            long id;
            if (!refId.TryGetValue(obj, out id))
                return -1;
            return id;
        }

        public Object Get(long id)
        {
            foreach (var kvp in refId)
                if (kvp.Value == id)
                    return kvp.Key;
            return null;
        }

        public void RemoveInvalidKeys()
        {
            var newRefId = new ES3RefIdDictionary();
            foreach (var kvp in refId)
            {
                var obj = kvp.Key;
                if (obj == null)
                    continue;

                if ((obj.hideFlags & HideFlags.DontSave) == HideFlags.DontSave ||
                    (obj.hideFlags & HideFlags.DontSaveInBuild) == HideFlags.DontSaveInBuild ||
                    (obj.hideFlags & HideFlags.DontSaveInEditor) == HideFlags.DontSaveInEditor ||
                    (obj.hideFlags & HideFlags.HideAndDontSave) == HideFlags.HideAndDontSave)
                {
                    var type = obj.GetType();
                    // Meshes are marked with HideAndDontSave, but shouldn't be ignored.
                    if (type != typeof(Mesh) && type != typeof(Material))
                        continue;
                }

                newRefId[obj] = kvp.Value;
            }

            refId = newRefId;
        }

        public long GetOrAdd(Object obj)
        {
            var id = Get(obj);

            // Only add items to global references when it's not playing.
            if (!Application.isPlaying && id == -1 && AssetDatabase.Contains(obj) && ES3ReferenceMgrBase.CanBeSaved(obj))
            {
                id = ES3ReferenceMgrBase.GetNewRefID();
                refId.Add(obj, id);

                EditorUtility.SetDirty(this);
            }

            return id;
        }

        private static string PathToGlobalReferences()
        {
            return ES3Settings.PathToEasySaveFolder() + "Resources/" + globalReferencesPath + ".asset";
        }
#endif
    }
}