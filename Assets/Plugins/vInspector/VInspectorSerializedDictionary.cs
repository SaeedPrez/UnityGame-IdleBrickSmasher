using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VInspector
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public List<SerializedKeyValuePair<TKey, TValue>> serializedKvps = new();

        public float dividerPos = .33f;

        public void OnBeforeSerialize()
        {
            foreach (var kvp in this)
                if (serializedKvps.FirstOrDefault(r => Comparer.Equals(r.Key, kvp.Key)) is SerializedKeyValuePair<TKey, TValue> serializedKvp)
                    serializedKvp.Value = kvp.Value;
                else
                    serializedKvps.Add(kvp);

            serializedKvps.RemoveAll(r => r.Key is not null && !ContainsKey(r.Key));

            for (var i = 0; i < serializedKvps.Count; i++)
                serializedKvps[i].index = i;
        }

        public void OnAfterDeserialize()
        {
            Clear();

            foreach (var serializedKvp in serializedKvps)
            {
                serializedKvp.isKeyNull = serializedKvp.Key is null;
                serializedKvp.isKeyRepeated = serializedKvp.Key is not null && ContainsKey(serializedKvp.Key);

                if (serializedKvp.isKeyNull) continue;
                if (serializedKvp.isKeyRepeated) continue;


                Add(serializedKvp.Key, serializedKvp.Value);
            }
        }


        [Serializable]
        public class SerializedKeyValuePair<TKey_, TValue_>
        {
            public TKey_ Key;
            public TValue_ Value;

            public int index;

            public bool isKeyRepeated;
            public bool isKeyNull;


            public SerializedKeyValuePair(TKey_ key, TValue_ value)
            {
                Key = key;
                Value = value;
            }

            public static implicit operator SerializedKeyValuePair<TKey_, TValue_>(KeyValuePair<TKey_, TValue_> kvp)
            {
                return new SerializedKeyValuePair<TKey_, TValue_>(kvp.Key, kvp.Value);
            }

            public static implicit operator KeyValuePair<TKey_, TValue_>(SerializedKeyValuePair<TKey_, TValue_> kvp)
            {
                return new KeyValuePair<TKey_, TValue_>(kvp.Key, kvp.Value);
            }
        }
    }
}