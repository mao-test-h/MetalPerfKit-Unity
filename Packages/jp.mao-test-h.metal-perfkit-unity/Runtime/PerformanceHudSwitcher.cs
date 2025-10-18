using UnityEngine;

namespace MetalPerfKit
{
    /// <summary>
    /// Performance HUD の制御クラス
    /// </summary>
    public static class PerformanceHUDSwitcher
    {
        private static readonly IPerformanceHUDSwitcher Instance = PerformanceHUDSwitcherFactory.Create();

        /// <summary>
        /// HUD の表示状態を取得する
        /// </summary>
        public static bool GetPerformanceHUDVisible()
        {
            return Instance.GetPerformanceHUDVisible();
        }

        /// <summary>
        /// HUD の位置を取得する
        /// </summary>
        /// <returns>HUD の位置（0.0 ～ 1.0 の相対位置）</returns>
        public static Vector2 GetPerformanceHUDPosition()
        {
            return Instance.GetPerformanceHUDPosition();
        }

        /// <summary>
        /// HUD の表示/非表示を設定する
        /// </summary>
        public static void SetPerformanceHUDVisible(bool visible)
        {
            Instance.SetPerformanceHUDVisible(visible);
        }

        /// <summary>
        /// HUD の表示/非表示を設定する
        /// </summary>
        /// <param name="visible">true: 表示, false: 非表示</param>
        /// <param name="x">HUD の X 座標（0.0 ～ 1.0 の相対位置）</param>
        /// <param name="y">HUD の Y 座標（0.0 ～ 1.0 の相対位置）</param>
        public static void SetPerformanceHUDVisible(bool visible, float x, float y)
        {
            Instance.SetPerformanceHUDVisible(visible, x, y);
        }
    }
}
