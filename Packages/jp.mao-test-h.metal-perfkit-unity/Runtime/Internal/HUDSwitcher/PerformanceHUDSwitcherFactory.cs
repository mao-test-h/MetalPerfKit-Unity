namespace MetalPerfKit
{
    internal static class PerformanceHUDSwitcherFactory
    {
        public static IPerformanceHUDSwitcher Create()
        {
#if !UNITY_EDITOR && UNITY_IOS
            return new PerformanceHUDSwitcherIOS();
#else
            return new PerformanceHUDSwitcherDummy();
#endif
        }
    }
}
