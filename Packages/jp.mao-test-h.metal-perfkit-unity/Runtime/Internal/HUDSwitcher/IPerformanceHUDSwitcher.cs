using UnityEngine;

namespace MetalPerfKit
{
    /// <summary>
    /// Performance HUD の制御インターフェース
    /// </summary>
    internal interface IPerformanceHUDSwitcher
    {
        bool GetPerformanceHUDVisible();
        Vector2 GetPerformanceHUDPosition();
        void SetPerformanceHUDVisible(bool visible);
        void SetPerformanceHUDVisible(bool visible, float x, float y);
    }
}
