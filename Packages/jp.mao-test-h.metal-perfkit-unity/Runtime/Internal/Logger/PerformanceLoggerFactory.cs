namespace MetalPerfKit
{
    internal static class PerformanceLoggerFactory
    {
        public static IPerformanceLogger Create()
        {
#if !UNITY_EDITOR && UNITY_IOS
            return new PerformanceLoggerIOS();
#else
            return new PerformanceLoggerDummy();
#endif
        }
    }
}
