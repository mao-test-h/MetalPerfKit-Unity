namespace MetalPerfKit
{
    internal static class StateReporterFactory
    {
        public static IStateReporter Create(string domain)
        {
#if !UNITY_EDITOR && UNITY_IOS
            return new StateReporterIOS(domain);
#else
            return new StateReporterDummy();
#endif
        }
    }
}
