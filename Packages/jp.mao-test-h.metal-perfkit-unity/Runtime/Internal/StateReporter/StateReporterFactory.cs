namespace MetalPerfKit
{
    internal static class StateReporterFactory
    {
        public static IStateReporter Create(string domain)
        {
#if !UNITY_EDITOR && UNITY_IOS
            return new StateReporterIOS(domain);
#elif UNITY_EDITOR && UNITY_IOS
            return new StateReporterEditor(domain);
#else
            return new StateReporterDummy();
#endif
        }
    }
}
