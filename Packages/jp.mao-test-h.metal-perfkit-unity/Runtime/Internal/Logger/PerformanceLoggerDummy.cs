using UnityEngine;

namespace MetalPerfKit
{
    /// <summary>
    /// ダミー実装の Performance Logger (Editor / 非対応プラットフォーム用)
    /// </summary>
    internal sealed class PerformanceLoggerDummy : IPerformanceLogger
    {
        public bool EnabledPerformanceLogging()
        {
            Debug.Log($"[Dummy] EnabledPerformanceLogging: Metal Performance HUD is not supported on this platform.");
            return false;
        }

        public void SetPerformanceLogging(bool enabled)
        {
            Debug.Log($"[Dummy] SetPerformanceLogging: {enabled} (Metal Performance HUD is not supported on this platform)");
        }

        public bool FetchPerformanceLogs(int pastSeconds, string savePath)
        {
            Debug.Log($"[Dummy] FetchPerformanceLogs: pastSeconds={pastSeconds}, savePath={savePath} (Metal Performance HUD is not supported on this platform)");
            return false;
        }
    }
}
