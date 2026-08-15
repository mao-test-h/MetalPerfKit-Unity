#if UNITY_EDITOR && UNITY_IOS
using System.Collections.Generic;
using UnityEngine;

namespace MetalPerfKit
{
    internal sealed class StateReporterEditor : IStateReporter
    {
        private readonly string _domain;

        public StateReporterEditor(string domain)
        {
            _domain = domain;
        }

        public void ReportTransitionToStateLabel(
            string stateLabel,
            IReadOnlyDictionary<string, StateReporterMetadataValue> stableMetadata,
            IReadOnlyDictionary<string, StateReporterMetadataValue> volatileMetadata)
        {
            Debug.Log(
                $"[StateReporter] ReportTransitionToStateLabel: domain={_domain}, " +
                $"stateLabel={stateLabel ?? "null"}, " +
                $"stableMetadata={SerializeMetadata(stableMetadata)}, " +
                $"volatileMetadata={SerializeMetadata(volatileMetadata)}");
        }

        public void ReportVolatileMetadataUpdate(
            IReadOnlyDictionary<string, StateReporterMetadataValue> updatedMetadata)
        {
            Debug.Log(
                $"[StateReporter] ReportVolatileMetadataUpdate: domain={_domain}, " +
                $"updatedMetadata={SerializeMetadata(updatedMetadata)}");
        }

        private static string SerializeMetadata(
            IReadOnlyDictionary<string, StateReporterMetadataValue> metadata)
        {
            return StateReporterMetadataSerializer.Serialize(metadata) ?? "null";
        }
    }
}
#endif
