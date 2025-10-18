namespace MetalPerfKit
{
    /// <summary>
    /// Performance HUD のログ取得インターフェース
    /// </summary>
    internal interface IPerformanceLogger
    {
        bool EnabledPerformanceLogging();
        void SetPerformanceLogging(bool enabled);
        bool FetchPerformanceLogs(int pastSeconds, string savePath);
    }
}
