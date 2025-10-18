namespace MetalPerfKit
{
    /// <summary>
    /// Performance HUD のログ取得クラス
    /// </summary>
    public static class PerformanceLogger
    {
        private static readonly IPerformanceLogger Instance = PerformanceLoggerFactory.Create();

        /// <summary>
        /// ロギング状態を取得する
        /// </summary>
        public static bool EnabledPerformanceLogging()
        {
            return Instance.EnabledPerformanceLogging();
        }

        /// <summary>
        /// ロギングを有効/無効にする
        /// </summary>
        public static void SetPerformanceLogging(bool enabled)
        {
            Instance.SetPerformanceLogging(enabled);
        }

        /// <summary>
        /// ログを取得してファイルに保存する
        /// </summary>
        /// <param name="pastSeconds">過去何秒分のログを取得するか</param>
        /// <param name="savePath">保存先のファイルパス</param>
        /// <returns>成功した場合は true、失敗した場合は false</returns>
        public static bool FetchPerformanceLogs(int pastSeconds, string savePath)
        {
            return Instance.FetchPerformanceLogs(pastSeconds, savePath);
        }
    }
}
