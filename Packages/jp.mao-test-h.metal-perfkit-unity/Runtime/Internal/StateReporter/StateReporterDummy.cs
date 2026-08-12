using System.Collections.Generic;

namespace MetalPerfKit
{
    internal sealed class StateReporterDummy : IStateReporter
    {
        public void ReportTransitionToStateLabel(
            string stateLabel,
            IReadOnlyDictionary<string, StateReporterMetadataValue> stableMetadata,
            IReadOnlyDictionary<string, StateReporterMetadataValue> volatileMetadata)
        {
        }

        public void ReportVolatileMetadataUpdate(
            IReadOnlyDictionary<string, StateReporterMetadataValue> updatedMetadata)
        {
        }
    }
}
