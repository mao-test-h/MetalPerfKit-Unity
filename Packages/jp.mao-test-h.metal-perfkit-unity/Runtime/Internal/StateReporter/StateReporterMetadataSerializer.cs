using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetalPerfKit
{
    internal static class StateReporterMetadataSerializer
    {
        public static string Serialize(
            IReadOnlyDictionary<string, StateReporterMetadataValue> metadata)
        {
            if (metadata == null)
            {
                return null;
            }

            var entries = new MetadataEntry[metadata.Count];
            var index = 0;
            foreach (var item in metadata)
            {
                if (item.Key == null)
                {
                    throw new ArgumentException("メタデータのキーは null にできません。", nameof(metadata));
                }

                if (item.Value.SerializedValue == null)
                {
                    throw new ArgumentException("メタデータに未初期化の値を指定できません。", nameof(metadata));
                }

                entries[index] = new MetadataEntry
                {
                    key = item.Key,
                    type = (int)item.Value.Type,
                    value = item.Value.SerializedValue
                };
                index++;
            }

            return JsonUtility.ToJson(new MetadataPayload
            {
                entries = entries
            });
        }

        [Serializable]
        private sealed class MetadataPayload
        {
            public MetadataEntry[] entries;
        }

        [Serializable]
        private sealed class MetadataEntry
        {
            public string key;
            public int type;
            public string value;
        }
    }
}
