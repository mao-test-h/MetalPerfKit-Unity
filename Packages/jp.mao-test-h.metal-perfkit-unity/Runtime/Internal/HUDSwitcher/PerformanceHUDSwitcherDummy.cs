using UnityEngine;

namespace MetalPerfKit
{
    /// <summary>
    /// ダミー実装の Performance HUD Switcher (Editor / 非対応プラットフォーム用)
    /// </summary>
    internal sealed class PerformanceHUDSwitcherDummy : IPerformanceHUDSwitcher
    {
        public bool GetPerformanceHUDVisible()
        {
            Debug.Log($"[Dummy] GetPerformanceHUDVisible: Metal Performance HUD is not supported on this platform.");
            return false;
        }

        public Vector2 GetPerformanceHUDPosition()
        {
            Debug.Log($"[Dummy] GetPerformanceHUDPosition: Metal Performance HUD is not supported on this platform.");
            return Vector2.zero;
        }

        public void SetPerformanceHUDVisible(bool visible)
        {
            Debug.Log($"[Dummy] SetPerformanceHUDVisible: {visible} (Metal Performance HUD is not supported on this platform)");
        }

        public void SetPerformanceHUDVisible(bool visible, float x, float y)
        {
            Debug.Log($"[Dummy] SetPerformanceHUDVisible: {visible}, Position: ({x}, {y}) (Metal Performance HUD is not supported on this platform)");
        }
    }
}
