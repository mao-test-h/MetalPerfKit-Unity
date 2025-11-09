namespace MetalPerfKit
{
    /// <summary>
    /// Metal Performance HUD の表示位置を指定する列挙型
    /// </summary>
    /// <remarks>
    /// MTL_HUD_ALIGNMENT 環境変数に対応する位置指定。
    /// デフォルト位置は TopRight。
    /// </remarks>
    public enum HudAlignment
    {
        /// <summary>
        /// 左上 (topleft)
        /// </summary>
        TopLeft = 0,

        /// <summary>
        /// 中央上 (topcenter)
        /// </summary>
        TopCenter = 1,

        /// <summary>
        /// 右上 (topright) - デフォルト
        /// </summary>
        TopRight = 2,

        /// <summary>
        /// 左中央 (centerleft)
        /// </summary>
        CenterLeft = 3,

        /// <summary>
        /// 中央 (centered)
        /// </summary>
        Centered = 4,

        /// <summary>
        /// 右中央 (centerright)
        /// </summary>
        CenterRight = 5,

        /// <summary>
        /// 左下 (bottomleft)
        /// </summary>
        BottomLeft = 6,

        /// <summary>
        /// 中央下 (bottomcenter)
        /// </summary>
        BottomCenter = 7,

        /// <summary>
        /// 右下 (bottomright)
        /// </summary>
        BottomRight = 8
    }
}
