#if UNITY_IOS
using System.Runtime.InteropServices;

namespace MetalPerfKit
{
    /// <summary>
    /// iOS 実装の Performance Logger
    /// </summary>
    internal sealed class PerformanceLoggerIOS : IPerformanceLogger
    {
        public bool EnabledPerformanceLogging()
        {
            var result = MetalPerfKit_EnabledLogging();
            if (result == (int)Status.Error)
            {
                throw new MetalPerfKitException("Failed to get Performance logging.");
            }

            return result == (int)Status.Success;
        }

        public void SetPerformanceLogging(bool enabled)
        {
            var result = MetalPerfKit_SetLogging((byte)(enabled ? 1 : 0));
            if (result == (int)Status.Error)
            {
                throw new MetalPerfKitException("Failed to set Performance logging.");
            }
        }

        public bool FetchPerformanceLogs(int pastSeconds, string savePath)
        {
            var result = MetalPerfKit_FetchLogs(pastSeconds, savePath);
            if (result == (int)Status.Error)
            {
                throw new MetalPerfKitException("Failed to fetch Performance logs. An error occurred during log retrieval.");
            }

            return result == 1;
        }

        [DllImport("__Internal", EntryPoint = "MetalPerfKit_EnabledLogging")]
        private static extern int MetalPerfKit_EnabledLogging();

        [DllImport("__Internal", EntryPoint = "MetalPerfKit_SetLogging")]
        private static extern int MetalPerfKit_SetLogging(byte enabled);

        [DllImport("__Internal", EntryPoint = "MetalPerfKit_FetchLogs")]
        private static extern int MetalPerfKit_FetchLogs(int pastSeconds, string savePath);
    }
}
#endif
