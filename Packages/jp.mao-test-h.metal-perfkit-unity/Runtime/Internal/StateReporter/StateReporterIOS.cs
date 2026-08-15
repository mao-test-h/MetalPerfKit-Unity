#if UNITY_IOS
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MetalPerfKit
{
    internal sealed class StateReporterIOS : IStateReporter
    {
        private readonly string _domain;

        public StateReporterIOS(string domain)
        {
            _domain = domain;
        }

        public void ReportTransitionToStateLabel(
            string stateLabel,
            IReadOnlyDictionary<string, StateReporterMetadataValue> stableMetadata,
            IReadOnlyDictionary<string, StateReporterMetadataValue> volatileMetadata)
        {
            var result = MetalPerfKit_StateReporter_ReportTransition(
                _domain,
                stateLabel,
                StateReporterMetadataSerializer.Serialize(stableMetadata),
                StateReporterMetadataSerializer.Serialize(volatileMetadata));
            if (result == (int)Status.Error)
            {
                throw new MetalPerfKitException("StateReporter の状態遷移の報告に失敗しました。");
            }
        }

        public void ReportVolatileMetadataUpdate(
            IReadOnlyDictionary<string, StateReporterMetadataValue> updatedMetadata)
        {
            var result = MetalPerfKit_StateReporter_ReportVolatileMetadataUpdate(
                _domain,
                StateReporterMetadataSerializer.Serialize(updatedMetadata));
            if (result == (int)Status.Error)
            {
                throw new MetalPerfKitException("StateReporter の volatile metadata の更新に失敗しました。");
            }
        }

        [DllImport("__Internal", EntryPoint = "MetalPerfKit_StateReporter_ReportTransition",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern int MetalPerfKit_StateReporter_ReportTransition(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string domain,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string stateLabel,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string stableMetadataJson,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string volatileMetadataJson);

        [DllImport("__Internal", EntryPoint = "MetalPerfKit_StateReporter_ReportVolatileMetadataUpdate",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern int MetalPerfKit_StateReporter_ReportVolatileMetadataUpdate(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string domain,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string updatedMetadataJson);
    }
}
#endif
