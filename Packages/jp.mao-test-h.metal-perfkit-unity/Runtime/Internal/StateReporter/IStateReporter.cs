using System.Collections.Generic;

namespace MetalPerfKit
{
    internal interface IStateReporter
    {
        void ReportTransitionToStateLabel(
            string stateLabel,
            IReadOnlyDictionary<string, StateReporterMetadataValue> stableMetadata,
            IReadOnlyDictionary<string, StateReporterMetadataValue> volatileMetadata);

        void ReportVolatileMetadataUpdate(
            IReadOnlyDictionary<string, StateReporterMetadataValue> updatedMetadata);
    }
}
