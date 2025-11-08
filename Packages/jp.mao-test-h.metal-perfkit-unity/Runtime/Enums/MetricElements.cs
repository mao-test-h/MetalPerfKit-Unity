using System;

namespace MetalPerfKit
{
    /// <summary>
    /// Metal Performance HUD に表示するメトリクスを指定する Flags 列挙型
    /// </summary>
    /// <remarks>
    /// MTL_HUD_ELEMENTS 環境変数に対応するメトリクス指定。
    /// ビット演算で複数のメトリクスを組み合わせて指定可能。
    /// </remarks>
    [Flags]
    public enum MetricElements
    {
        /// <summary>
        /// メトリクスなし
        /// </summary>
        None = 0,

        /// <summary>
        /// デバイス情報 (device)
        /// </summary>
        Device = 1 << 0,

        /// <summary>
        /// Rosetta 情報 (rosetta)
        /// </summary>
        Rosetta = 1 << 1,

        /// <summary>
        /// レイヤーサイズ (layersize)
        /// </summary>
        LayerSize = 1 << 2,

        /// <summary>
        /// レイヤースケール (layerscale)
        /// </summary>
        LayerScale = 1 << 3,

        /// <summary>
        /// メモリ使用量 (memory)
        /// </summary>
        Memory = 1 << 4,

        /// <summary>
        /// フレームレート (fps)
        /// </summary>
        Fps = 1 << 5,

        /// <summary>
        /// フレーム間隔 (frameinterval)
        /// </summary>
        FrameInterval = 1 << 6,

        /// <summary>
        /// GPU 時間 (gputime)
        /// </summary>
        GpuTime = 1 << 7,

        /// <summary>
        /// サーマル状態 (thermal)
        /// </summary>
        Thermal = 1 << 8,

        /// <summary>
        /// フレーム間隔グラフ (frameintervalgraph)
        /// </summary>
        FrameIntervalGraph = 1 << 9,

        /// <summary>
        /// プレゼント遅延 (presentdelay)
        /// </summary>
        PresentDelay = 1 << 10,

        /// <summary>
        /// フレーム間隔ヒストグラム (frameintervalhistogram)
        /// </summary>
        FrameIntervalHistogram = 1 << 11,

        /// <summary>
        /// Metal CPU 時間 (metalcpu)
        /// </summary>
        MetalCpu = 1 << 12,

        /// <summary>
        /// GPU タイムライン (gputimeline)
        /// </summary>
        GpuTimeline = 1 << 13,

        /// <summary>
        /// シェーダー情報 (shaders)
        /// </summary>
        Shaders = 1 << 14,

        /// <summary>
        /// フレーム番号 (framenumber)
        /// </summary>
        FrameNumber = 1 << 15,

        /// <summary>
        /// ディスク使用量 (disk)
        /// </summary>
        Disk = 1 << 16,

        /// <summary>
        /// FPS グラフ (fpsgraph)
        /// </summary>
        FpsGraph = 1 << 17,

        /// <summary>
        /// トップラベル付きコマンドバッファ (toplabeledcommandbuffers)
        /// </summary>
        TopLabeledCommandBuffers = 1 << 18,

        /// <summary>
        /// トップラベル付きエンコーダ (toplabeledencoders)
        /// </summary>
        TopLabeledEncoders = 1 << 19,

        /// <summary>
        /// すべてのメトリクス
        /// </summary>
        All = ~0
    }
}
