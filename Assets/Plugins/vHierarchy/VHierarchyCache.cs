#if UNITY_EDITOR
using System;
using UnityEditor;
using static VHierarchy.Libs.VUtils;


namespace VHierarchy
{
    [FilePath("Library/vHierarchy Cache.asset", FilePathAttribute.Location.ProjectFolder)]
    public class VHierarchyCache : ScriptableSingleton<VHierarchyCache>
    {
        // used for finding SceneData and SceneIdMap for objects that were moved out of their original scene 
        public SerializableDictionary<int, string> originalSceneGuids_byInstanceId = new();

        // used as cache for converting GlobalID to InstanceID and as a way to find GameObjectData for prefabs in playmode (when prefabs produce invalid GlobalIDs)
        public SerializableDictionary<string, SceneIdMap> sceneIdMaps_bySceneGuid = new();

        // used for fetching icons set inside prefab instances in playmode (when prefabs produce invalid GlobalIDs)
        public SerializableDictionary<int, GlobalID> prefabInstanceGlobalIds_byInstanceIds = new();


        public static void Clear()
        {
            instance.originalSceneGuids_byInstanceId.Clear();
            instance.sceneIdMaps_bySceneGuid.Clear();

            instance.Save(true);
        }


        [Serializable]
        public class SceneIdMap
        {
            public SerializableDictionary<int, GlobalID> globalIds_byInstanceId = new();

            public int instanceIdsHash;
            public int globalIdsHash;
        }

        // public static void Save() => instance.Save(true);    // cache is never saved to disk, it just needs to survive domain reloads
    }
}
#endif