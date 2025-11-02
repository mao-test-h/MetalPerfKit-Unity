# MetalPerfKit-Unity 新機能追加要件

## 概要

MetalPerfKit-Unity にランタイム制御機を追加します。
これらの機能は、現在 LaunchEnvironment でビルド時の環境変数としてのみ設定可能な Metal HUD のプロパティを、実行時に動的に制御できるようにするものです。

### 追加機能

以下の公式ドキュメントにある環境変数をランタイムから制御できるようにします。　

- https://developer.apple.com/documentation/xcode/customizing-metal-performance-hud#Customize-the-Metal-Performance-HUD-programmatically

該当箇所を引用すると、以下に示す項目となります。

```
Additionally, you can add various effects by setting environment variables that the HUD supports in the dictionary, including:

MTL_HUD_LOG_ENABLED=1
Turns on logging for per-frame statistics.

MTL_HUD_LOG_SHADER_ENABLED=1
Turns on logging for shader compilation activities.

MTL_HUD_CONFIG_FILE=<path>
Selects a configuration file the Metal HUD loads, which needs to be the file path the app can access.

MTL_HUD_OPACITY=1.0
Configures the opacity of the overlay in the range [0.0, 1.0]. The default value is 1.0.

MTL_HUD_SCALE=0.2
Configures the scale of the overlay as a percentage of the drawable width in the range [0.0, 1.0]. The default scale is 0.2 with a minimum width of 300 pixels.

MTL_HUD_ALIGNMENT=topright
Sets the position of the overlay. The default position is topright. Available options are topleft, topcenter, topright, centerleft, centered, centerright, bottomright, bottomcenter, and bottomleft.

MTL_HUD_POSITION_X, MTL_HUD_POSITION_Y
Sets the absolute position of the overlay in pixels. Overrides MTL_HUD_ALIGNMENT.

MTL_HUD_ELEMENTS
Specifies a comma-separated list of metrics that appears in the overlay. Available metric names are device, rosetta, layersize, layerscale, memory, fps, frameinterval, gputime, thermal, frameintervalgraph, presentdelay, frameintervalhistogram, metalcpu, gputimeline, shaders, framenumber, disk, fpsgraph, toplabeledcommandbuffers, and toplabeledencoders. See Understanding the Metal Performance HUD metrics for more information.

MTL_HUD_ENCODER_TIMING_ENABLED=1
Turns on encoder-based GPU time tracking. See Understand encoder GPU time tracking for more information.

MTL_HUD_ENCODER_GPU_TIMELINE_FRAME_COUNT=6
Sets the maximum number of frames that appear in the GPU timeline.

MTL_HUD_ENCODER_GPU_TIMELINE_SWAP_DELTA=1
Sets the update interval of the GPU timeline in seconds.

MTL_HUD_SHOW_ZERO_METRICS=1
Makes the overlay show that metrics have a value of 0 since the app launched or since the last reset. The Metal Performance HUD enables this variable by default to hide metrics that may not be available or aren’t used in the current context.

MTL_HUD_SHOW_METRICS_RANGE=1
Reports the range of metrics for the last 1200 frames. See Display the value range of metrics for more information.

MTL_HUD_INSIGHTS_ENABLED=1
Turns on the performance insights feature. See Gaining performance insights with the Metal Performance HUD for more information.

MTL_HUD_INSIGHT_TIMEOUT=10
Sets the performance insight timeout before it dissapears.

MTL_HUD_INSIGHT_REPORT_INTERVAL=5
Sets the report interval of the performance insights in seconds. The Metal Performance HUD reports performance insights if half of the frames during this interval show a particular pattern.

MTL_HUD_RUSAGE_UPDATE_INTERVAL=3
Sets the system resource usage update interval in seconds.

MTL_HUD_METRIC_TIMEOUT=5
Sets the timeout of transient metrics in seconds. The Metal Performance HUD automatically hides transient metrics, such as MetalFX metrics, when you disable MetalFX.

MTL_HUD_REPORT_URL=\<path>
Sets an app writable path where the system writes performance reports to. See Generating performance reports with the Metal Performance HUD for more information.

MTL_HUD_DISABLE_MENU_BAR=1
Disables the Metal Performance HUD menu item.


```

# 実装要件

## 機能の分類と実装クラス

環境変数を責務ごとに3つのクラスに分類して実装します。

### 1. PerformanceHUDSwitcher（HUD表示制御）

既存の責務（HUD の表示/非表示と位置制御）を拡張し、以下の機能を追加：

#### 1.1 外観設定
- `MTL_HUD_OPACITY`: HUD の不透明度（0.0～1.0）
  - `SetOpacity(float opacity)` / `GetOpacity()`
- `MTL_HUD_SCALE`: HUD のスケール（0.0～1.0）
  - `SetScale(float scale)` / `GetScale()`
- `MTL_HUD_ALIGNMENT`: HUD の位置（9種類の位置）
  - `SetAlignment(HudAlignment alignment)` / `GetAlignment()`
  - ※ 既に Position X/Y は実装済みだが、Alignment を enum で型安全に扱えるように追加

#### 1.2 表示メトリクス制御
- `MTL_HUD_ELEMENTS`: 表示するメトリクスのリスト
  - `SetElements(MetricElements elements)` / `GetElements()`
  - ※ Flags enum を使用（ビット演算でOR結合可能）
- `MTL_HUD_SHOW_ZERO_METRICS`: 値が0のメトリクスを表示するか
  - `SetShowZeroMetrics(bool show)` / `GetShowZeroMetrics()`
- `MTL_HUD_SHOW_METRICS_RANGE`: メトリクスの範囲（最小/最大）を表示するか
  - `SetShowMetricsRange(bool show)` / `GetShowMetricsRange()`
- `MTL_HUD_METRIC_TIMEOUT`: 一時的なメトリクスのタイムアウト（秒）
  - `SetMetricTimeout(int seconds)` / `GetMetricTimeout()`

#### 1.3 エンコーダ・タイムライン設定
- `MTL_HUD_ENCODER_TIMING_ENABLED`: エンコーダベースの GPU 時間追跡
  - `SetEncoderTimingEnabled(bool enabled)` / `GetEncoderTimingEnabled()`
- `MTL_HUD_ENCODER_GPU_TIMELINE_FRAME_COUNT`: GPU タイムラインのフレーム数
  - `SetEncoderGpuTimelineFrameCount(int count)` / `GetEncoderGpuTimelineFrameCount()`
- `MTL_HUD_ENCODER_GPU_TIMELINE_SWAP_DELTA`: GPU タイムラインの更新間隔（秒）
  - `SetEncoderGpuTimelineSwapDelta(int seconds)` / `GetEncoderGpuTimelineSwapDelta()`

#### 1.4 その他の HUD 設定
- `MTL_HUD_RUSAGE_UPDATE_INTERVAL`: システムリソース使用状況の更新間隔（秒）
  - `SetRusageUpdateInterval(int seconds)` / `GetRusageUpdateInterval()`
- `MTL_HUD_DISABLE_MENU_BAR`: Metal Performance HUD メニューバー項目を無効化
  - `SetDisableMenuBar(bool disable)` / `GetDisableMenuBar()`

---

### 2. PerformanceLogger（ログ制御）

既存の責務（ログ取得機能）を拡張し、以下の機能を追加：

#### 2.1 ログ種別の制御
- `MTL_HUD_LOG_ENABLED`: フレーム統計のログ
  - ※ 既存実装を確認（既に実装済みの可能性あり）
- `MTL_HUD_LOG_SHADER_ENABLED`: シェーダーコンパイルのログ
  - `SetShaderLoggingEnabled(bool enabled)` / `GetShaderLoggingEnabled()`

---

### 3. PerformanceInsights（新規クラス）

パフォーマンス分析とレポート生成に特化した新しいクラス：

#### 3.1 インサイト機能
- `MTL_HUD_INSIGHTS_ENABLED`: パフォーマンスインサイト機能の有効化
  - `SetInsightsEnabled(bool enabled)` / `GetInsightsEnabled()`
- `MTL_HUD_INSIGHT_TIMEOUT`: インサイトの表示タイムアウト（秒）
  - `SetInsightTimeout(int seconds)` / `GetInsightTimeout()`
- `MTL_HUD_INSIGHT_REPORT_INTERVAL`: インサイトのレポート間隔（秒）
  - `SetInsightReportInterval(int seconds)` / `GetInsightReportInterval()`

#### 3.2 レポート・設定ファイル
- `MTL_HUD_REPORT_URL`: パフォーマンスレポートの保存先パス
  - `SetReportPath(string path)` / `GetReportPath()`
- `MTL_HUD_CONFIG_FILE`: Metal HUD が読み込む設定ファイルのパス
  - `SetConfigFilePath(string path)` / `GetConfigFilePath()`

---

## 新規追加する Enum 定義

### HudAlignment（enum）
HUD の位置を型安全に指定するための列挙型：

```csharp
public enum HudAlignment
{
    TopLeft,
    TopCenter,
    TopRight,
    CenterLeft,
    Centered,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}
```

### MetricElements（Flags enum）
表示するメトリクスをビット演算で指定するための Flags 列挙型：

```csharp
[Flags]
public enum MetricElements
{
    None = 0,
    Device = 1 << 0,              // device
    Rosetta = 1 << 1,             // rosetta
    LayerSize = 1 << 2,           // layersize
    LayerScale = 1 << 3,          // layerscale
    Memory = 1 << 4,              // memory
    Fps = 1 << 5,                 // fps
    FrameInterval = 1 << 6,       // frameinterval
    GpuTime = 1 << 7,             // gputime
    Thermal = 1 << 8,             // thermal
    FrameIntervalGraph = 1 << 9,  // frameintervalgraph
    PresentDelay = 1 << 10,       // presentdelay
    FrameIntervalHistogram = 1 << 11, // frameintervalhistogram
    MetalCpu = 1 << 12,           // metalcpu
    GpuTimeline = 1 << 13,        // gputimeline
    Shaders = 1 << 14,            // shaders
    FrameNumber = 1 << 15,        // framenumber
    Disk = 1 << 16,               // disk
    FpsGraph = 1 << 17,           // fpsgraph
    TopLabeledCommandBuffers = 1 << 18, // toplabeledcommandbuffers
    TopLabeledEncoders = 1 << 19, // toplabeledencoders
    All = ~0                      // すべてのメトリクス
}
```

---

## 実装ファイル構成

### Unity C# ファイル

#### 既存ファイルの拡張
1. `Runtime/PerformanceHudSwitcher.cs`
   - 1.1～1.4 の機能を追加
2. `Runtime/PerformanceLogger.cs`
   - 2.1 の機能を追加
3. `Runtime/Internal/HUDSwitcher/PerformanceHUDSwitcherIOS.cs`
   - iOS 実装の拡張（P/Invoke 追加）
4. `Runtime/Internal/Logger/PerformanceLoggerIOS.cs`
   - iOS 実装の拡張（P/Invoke 追加）
5. `Editor/LaunchEnvironment.cs`
   - すべての新環境変数をプロパティとして追加

#### 新規ファイル
1. `Runtime/PerformanceInsights.cs`
   - 3.1～3.2 の機能を持つパブリック API
2. `Runtime/Internal/Insights/IPerformanceInsights.cs`
   - インターフェース定義
3. `Runtime/Internal/Insights/PerformanceInsightsFactory.cs`
   - ファクトリークラス
4. `Runtime/Internal/Insights/PerformanceInsightsIOS.cs`
   - iOS 実装（P/Invoke）
5. `Runtime/Internal/Insights/PerformanceInsightsDummy.cs`
   - ダミー実装（Editor/非対応プラットフォーム用）
6. `Runtime/Enums/HudAlignment.cs`
   - HudAlignment enum の定義
7. `Runtime/Enums/MetricElements.cs`
   - MetricElements Flags enum の定義

### iOS ネイティブプラグイン

#### 既存ファイルの拡張
1. `Plugins/iOS/MetalPerfKitBridge.swift`
   - 約30個の @_cdecl 関数を追加
   - developerHUDProperties への Set/Get 処理実装

---

## Swift ネイティブブリッジの実装詳細

### 追加する @_cdecl 関数の命名規則

既存パターンに従い、以下の命名規則で実装：

```swift
// Getter: MetalPerfKit_Get{機能名}
// Setter: MetalPerfKit_Set{機能名}
// 戻り値: Int32（Status Code: Success=1, Failure=0, Error=-1）

// 例: Opacity
@_cdecl("MetalPerfKit_GetOpacity")
public func MetalPerfKit_GetOpacity(_ outValue: UnsafeMutablePointer<Float>) -> Int32

@_cdecl("MetalPerfKit_SetOpacity")
public func MetalPerfKit_SetOpacity(_ value: Float) -> Int32
```

### developerHUDProperties への設定方法

既存の `updateProperties` メソッドを活用し、各環境変数に対応するキーと値を設定：

```swift
private func updateProperties(_ updates: [String: Any]) {
    guard let metalLayer = getMetalLayer() else {
        return
    }

    var currentProperties = getCurrentProperties()
    for (key, value) in updates {
        currentProperties[key] = value
    }
    metalLayer.developerHUDProperties = currentProperties
}

// 使用例
func MetalPerfKit_SetOpacity(_ value: Float) -> Int32 {
    updateProperties(["MTL_HUD_OPACITY": value])
    return MPHStatus.success.rawValue
}
```

### MetricElements の文字列変換

C# の Flags enum から Swift の文字列配列への変換処理を実装：

```swift
// C# から渡されるビットマスクを文字列配列に変換
private func metricElementsToString(_ elements: UInt32) -> String {
    var metrics: [String] = []

    if elements & (1 << 0) != 0 { metrics.append("device") }
    if elements & (1 << 1) != 0 { metrics.append("rosetta") }
    // ... (全20種類)

    return metrics.joined(separator: ",")
}

@_cdecl("MetalPerfKit_SetElements")
public func MetalPerfKit_SetElements(_ elements: UInt32) -> Int32 {
    let elementsString = metricElementsToString(elements)
    updateProperties(["MTL_HUD_ELEMENTS": elementsString])
    return MPHStatus.success.rawValue
}
```

### HudAlignment の変換

C# の enum から Swift の文字列への変換：

```swift
private func alignmentToString(_ alignment: Int32) -> String? {
    switch alignment {
    case 0: return "topleft"
    case 1: return "topcenter"
    case 2: return "topright"
    case 3: return "centerleft"
    case 4: return "centered"
    case 5: return "centerright"
    case 6: return "bottomleft"
    case 7: return "bottomcenter"
    case 8: return "bottomright"
    default: return nil
    }
}
```

---

## LaunchEnvironment の拡張

### 追加するプロパティ

既存の ScriptableObject に以下のプロパティを追加：

```csharp
[Serializable]
public class EnvironmentVariable
{
    public bool Enabled;
    public string Key;
    public string Value;
}

// HUD 外観
public EnvironmentVariable Opacity;           // MTL_HUD_OPACITY
public EnvironmentVariable Scale;             // MTL_HUD_SCALE
public EnvironmentVariable Alignment;         // MTL_HUD_ALIGNMENT

// メトリクス
public EnvironmentVariable Elements;          // MTL_HUD_ELEMENTS
public EnvironmentVariable ShowZeroMetrics;   // MTL_HUD_SHOW_ZERO_METRICS
public EnvironmentVariable ShowMetricsRange;  // MTL_HUD_SHOW_METRICS_RANGE
public EnvironmentVariable MetricTimeout;     // MTL_HUD_METRIC_TIMEOUT

// エンコーダ・タイムライン
public EnvironmentVariable EncoderTimingEnabled;        // MTL_HUD_ENCODER_TIMING_ENABLED
public EnvironmentVariable EncoderGpuTimelineFrameCount; // MTL_HUD_ENCODER_GPU_TIMELINE_FRAME_COUNT
public EnvironmentVariable EncoderGpuTimelineSwapDelta;  // MTL_HUD_ENCODER_GPU_TIMELINE_SWAP_DELTA

// その他 HUD
public EnvironmentVariable RusageUpdateInterval; // MTL_HUD_RUSAGE_UPDATE_INTERVAL
public EnvironmentVariable DisableMenuBar;       // MTL_HUD_DISABLE_MENU_BAR

// ログ
public EnvironmentVariable LogShaderEnabled;     // MTL_HUD_LOG_SHADER_ENABLED

// インサイト・レポート
public EnvironmentVariable ConfigFile;           // MTL_HUD_CONFIG_FILE
public EnvironmentVariable ReportUrl;            // MTL_HUD_REPORT_URL
```

### XcodePostProcess での自動設定

既存の `XcodePostProcess.cs` を拡張し、新しいプロパティも Xcode Scheme に自動設定されるようにする。

---

## 技術的考慮事項

### 1. 既存パターンの踏襲
- **ファクトリーパターン**: プラットフォームに応じた実装の自動切り替え
- **インターフェース分離**: `IPerformanceHUDSwitcher`、`IPerformanceLogger`、`IPerformanceInsights`
- **ステータスコード**: `Success=1`, `Failure=0`, `Error=-1` の統一
- **エラーハンドリング**: `MetalPerfKitException` での統一

### 2. 型安全性の確保
- HudAlignment: enum で9種類の位置を型安全に
- MetricElements: Flags enum でビット演算可能に
- 不正な値のバリデーション（範囲チェックなど）

### 3. プラットフォーム対応
- 条件コンパイル: `#if UNITY_IOS` の適切な使用
- Dummy 実装: Editor と非対応プラットフォーム用の実装
- UnityFramework の取得: `UnityFramework.getInstance()`

### 4. メモリ管理
- GCHandle: ポインタ操作時の適切な管理（Alloc → Free）
- try-finally: リソースの確実な解放

### 5. 座標系の扱い
- Position X/Y: 既存の正規化座標⇔絶対座標変換を活用
- Alignment: 位置指定の別方式として提供（Position が優先）

### 6. 文字列処理
- MetricElements: ビットマスク ⇔ カンマ区切り文字列の変換
- パス指定: Application.persistentDataPath などの適切な使用

### 7. バージョン互換性
- Apple が将来追加する新メトリクスへの対応
- 既存 API との後方互換性の維持

---

## 実装の優先順位

1. **Enum 定義** - HudAlignment と MetricElements
2. **Swift ネイティブブリッジ** - 基本的な Set/Get 関数
3. **PerformanceHUDSwitcher の拡張** - 最も利用頻度が高い
4. **PerformanceInsights の新規作成** - 独立した機能
5. **PerformanceLogger の拡張** - 小規模な変更
6. **LaunchEnvironment の拡張** - ビルド時設定の強化
7. **Example シーンの更新** - デモ UI の追加

---

## 作業チェックリスト

### Phase 1: Enum 定義

- [ ] `Runtime/Enums/HudAlignment.cs` の作成
  - [ ] 9種類の位置を定義（TopLeft, TopCenter, TopRight, CenterLeft, Centered, CenterRight, BottomLeft, BottomCenter, BottomRight）
  - [ ] namespace と public アクセス修飾子の設定
  - [ ] XML ドキュメントコメントの追加

- [ ] `Runtime/Enums/MetricElements.cs` の作成
  - [ ] [Flags] 属性の追加
  - [ ] 20種類のメトリクスを定義（Device, Rosetta, LayerSize, LayerScale, Memory, Fps, FrameInterval, GpuTime, Thermal, FrameIntervalGraph, PresentDelay, FrameIntervalHistogram, MetalCpu, GpuTimeline, Shaders, FrameNumber, Disk, FpsGraph, TopLabeledCommandBuffers, TopLabeledEncoders）
  - [ ] None = 0 と All = ~0 の定義
  - [ ] namespace と public アクセス修飾子の設定
  - [ ] XML ドキュメントコメントの追加

- [ ] `Runtime/jp.mao-test-h.metal-perfkit-unity.asmdef` への参照追加（必要に応じて）

---

### Phase 2: Swift ネイティブブリッジの拡張

- [ ] `Plugins/iOS/MetalPerfKitBridge.swift` を開く

#### 2.1 ヘルパーメソッドの追加

- [ ] `metricElementsToString(_ elements: UInt32) -> String` の実装
  - [ ] 20種類のビットフラグから文字列配列への変換ロジック
  - [ ] カンマ区切り文字列の生成

- [ ] `stringToMetricElements(_ elementsString: String) -> UInt32` の実装（Getter用）
  - [ ] カンマ区切り文字列からビットマスクへの逆変換

- [ ] `alignmentToString(_ alignment: Int32) -> String?` の実装
  - [ ] 0-8 の整数から文字列（topleft, topcenter など）への変換

- [ ] `stringToAlignment(_ alignmentString: String) -> Int32` の実装（Getter用）
  - [ ] 文字列から整数への逆変換

#### 2.2 外観設定の @_cdecl 関数（6個）

- [ ] `MetalPerfKit_GetOpacity(_ outValue: UnsafeMutablePointer<Float>) -> Int32`
- [ ] `MetalPerfKit_SetOpacity(_ value: Float) -> Int32`
- [ ] `MetalPerfKit_GetScale(_ outValue: UnsafeMutablePointer<Float>) -> Int32`
- [ ] `MetalPerfKit_SetScale(_ value: Float) -> Int32`
- [ ] `MetalPerfKit_GetAlignment(_ outValue: UnsafeMutablePointer<Int32>) -> Int32`
- [ ] `MetalPerfKit_SetAlignment(_ alignment: Int32) -> Int32`

#### 2.3 表示メトリクス制御の @_cdecl 関数（8個）

- [ ] `MetalPerfKit_GetElements(_ outValue: UnsafeMutablePointer<UInt32>) -> Int32`
- [ ] `MetalPerfKit_SetElements(_ elements: UInt32) -> Int32`
- [ ] `MetalPerfKit_GetShowZeroMetrics() -> Int32`
- [ ] `MetalPerfKit_SetShowZeroMetrics(_ enabled: UInt8) -> Int32`
- [ ] `MetalPerfKit_GetShowMetricsRange() -> Int32`
- [ ] `MetalPerfKit_SetShowMetricsRange(_ enabled: UInt8) -> Int32`
- [ ] `MetalPerfKit_GetMetricTimeout(_ outValue: UnsafeMutablePointer<Int32>) -> Int32`
- [ ] `MetalPerfKit_SetMetricTimeout(_ seconds: Int32) -> Int32`

#### 2.4 エンコーダ・タイムライン設定の @_cdecl 関数（6個）

- [ ] `MetalPerfKit_GetEncoderTimingEnabled() -> Int32`
- [ ] `MetalPerfKit_SetEncoderTimingEnabled(_ enabled: UInt8) -> Int32`
- [ ] `MetalPerfKit_GetEncoderGpuTimelineFrameCount(_ outValue: UnsafeMutablePointer<Int32>) -> Int32`
- [ ] `MetalPerfKit_SetEncoderGpuTimelineFrameCount(_ count: Int32) -> Int32`
- [ ] `MetalPerfKit_GetEncoderGpuTimelineSwapDelta(_ outValue: UnsafeMutablePointer<Int32>) -> Int32`
- [ ] `MetalPerfKit_SetEncoderGpuTimelineSwapDelta(_ seconds: Int32) -> Int32`

#### 2.5 その他 HUD 設定の @_cdecl 関数（4個）

- [ ] `MetalPerfKit_GetRusageUpdateInterval(_ outValue: UnsafeMutablePointer<Int32>) -> Int32`
- [ ] `MetalPerfKit_SetRusageUpdateInterval(_ seconds: Int32) -> Int32`
- [ ] `MetalPerfKit_GetDisableMenuBar() -> Int32`
- [ ] `MetalPerfKit_SetDisableMenuBar(_ disable: UInt8) -> Int32`

#### 2.6 ログ制御の @_cdecl 関数（2個）

- [ ] `MetalPerfKit_GetShaderLoggingEnabled() -> Int32`
- [ ] `MetalPerfKit_SetShaderLoggingEnabled(_ enabled: UInt8) -> Int32`

#### 2.7 インサイト・レポート設定の @_cdecl 関数（10個）

- [ ] `MetalPerfKit_GetInsightsEnabled() -> Int32`
- [ ] `MetalPerfKit_SetInsightsEnabled(_ enabled: UInt8) -> Int32`
- [ ] `MetalPerfKit_GetInsightTimeout(_ outValue: UnsafeMutablePointer<Int32>) -> Int32`
- [ ] `MetalPerfKit_SetInsightTimeout(_ seconds: Int32) -> Int32`
- [ ] `MetalPerfKit_GetInsightReportInterval(_ outValue: UnsafeMutablePointer<Int32>) -> Int32`
- [ ] `MetalPerfKit_SetInsightReportInterval(_ seconds: Int32) -> Int32`
- [ ] `MetalPerfKit_GetReportPath(_ outPath: UnsafeMutablePointer<Int8>, _ maxLength: Int32) -> Int32`
- [ ] `MetalPerfKit_SetReportPath(_ path: UnsafePointer<Int8>) -> Int32`
- [ ] `MetalPerfKit_GetConfigFilePath(_ outPath: UnsafeMutablePointer<Int8>, _ maxLength: Int32) -> Int32`
- [ ] `MetalPerfKit_SetConfigFilePath(_ path: UnsafePointer<Int8>) -> Int32`

---

### Phase 3: PerformanceHUDSwitcher の拡張

#### 3.1 内部実装（iOS）

- [ ] `Runtime/Internal/HUDSwitcher/PerformanceHUDSwitcherIOS.cs` を開く

- [ ] P/Invoke 宣言の追加（26個）
  - [ ] 外観設定（6個）
  - [ ] 表示メトリクス制御（8個）
  - [ ] エンコーダ・タイムライン設定（6個）
  - [ ] その他 HUD 設定（4個）
  - [ ] ログ制御（2個）

- [ ] 各メソッドの実装
  - [ ] Getter メソッド（GCHandle を使用）
  - [ ] Setter メソッド（バリデーション含む）
  - [ ] エラーハンドリング（MetalPerfKitException）

#### 3.2 内部実装（Dummy）

- [ ] `Runtime/Internal/HUDSwitcher/PerformanceHUDSwitcherDummy.cs` を開く
- [ ] 新しいメソッドのダミー実装を追加（常に false や 0 を返す）

#### 3.3 インターフェース更新

- [ ] `Runtime/Internal/HUDSwitcher/IPerformanceHUDSwitcher.cs` を開く
- [ ] 新しいメソッドのシグネチャを追加

#### 3.4 パブリック API

- [ ] `Runtime/PerformanceHudSwitcher.cs` を開く
- [ ] 新しい public static メソッドを追加（14個）
  - [ ] XML ドキュメントコメントの追加
  - [ ] バリデーション（範囲チェックなど）
  - [ ] 内部実装への委譲

---

### Phase 4: PerformanceInsights の新規作成

#### 4.1 インターフェース定義

- [ ] `Runtime/Internal/Insights/IPerformanceInsights.cs` の作成
  - [ ] 5個のメソッドシグネチャを定義
  - [ ] namespace と internal アクセス修飾子

#### 4.2 iOS 実装

- [ ] `Runtime/Internal/Insights/PerformanceInsightsIOS.cs` の作成
  - [ ] P/Invoke 宣言（10個）
  - [ ] IPerformanceInsights の実装
  - [ ] GCHandle を使用した Getter 実装
  - [ ] 文字列パス処理（Marshal.StringToHGlobalAnsi / FreeHGlobal）
  - [ ] エラーハンドリング
  - [ ] #if UNITY_IOS 条件コンパイル

#### 4.3 Dummy 実装

- [ ] `Runtime/Internal/Insights/PerformanceInsightsDummy.cs` の作成
  - [ ] IPerformanceInsights の実装
  - [ ] すべてのメソッドでダミー値を返す

#### 4.4 ファクトリークラス

- [ ] `Runtime/Internal/Insights/PerformanceInsightsFactory.cs` の作成
  - [ ] Create() メソッドの実装
  - [ ] プラットフォーム判定ロジック

#### 4.5 パブリック API

- [ ] `Runtime/PerformanceInsights.cs` の作成
  - [ ] 静的ファクトリーパターンの実装
  - [ ] 5個の public static メソッド
  - [ ] XML ドキュメントコメント
  - [ ] バリデーション

---

### Phase 5: PerformanceLogger の拡張

#### 5.1 内部実装（iOS）

- [ ] `Runtime/Internal/Logger/PerformanceLoggerIOS.cs` を開く
- [ ] P/Invoke 宣言の追加（2個）
  - [ ] `MetalPerfKit_GetShaderLoggingEnabled`
  - [ ] `MetalPerfKit_SetShaderLoggingEnabled`
- [ ] GetShaderLoggingEnabled() メソッドの実装
- [ ] SetShaderLoggingEnabled(bool enabled) メソッドの実装

#### 5.2 内部実装（Dummy）

- [ ] `Runtime/Internal/Logger/PerformanceLoggerDummy.cs` を開く
- [ ] ダミーメソッドの追加

#### 5.3 インターフェース更新

- [ ] `Runtime/Internal/Logger/IPerformanceLogger.cs` を開く
- [ ] 新しいメソッドのシグネチャを追加

#### 5.4 パブリック API

- [ ] `Runtime/PerformanceLogger.cs` を開く
- [ ] GetShaderLoggingEnabled() の追加
- [ ] SetShaderLoggingEnabled(bool enabled) の追加
- [ ] XML ドキュメントコメントの追加

---

### Phase 6: LaunchEnvironment の拡張

#### 6.1 ScriptableObject の拡張

- [ ] `Editor/LaunchEnvironment.cs` を開く
- [ ] 新しい EnvironmentVariable プロパティを追加（16個）
  - [ ] Opacity
  - [ ] Scale
  - [ ] Alignment
  - [ ] Elements
  - [ ] ShowZeroMetrics
  - [ ] ShowMetricsRange
  - [ ] MetricTimeout
  - [ ] EncoderTimingEnabled
  - [ ] EncoderGpuTimelineFrameCount
  - [ ] EncoderGpuTimelineSwapDelta
  - [ ] RusageUpdateInterval
  - [ ] DisableMenuBar
  - [ ] LogShaderEnabled
  - [ ] ConfigFile
  - [ ] ReportUrl
  - [ ] InsightsEnabled（既存確認）
  - [ ] InsightTimeout（既存確認）
  - [ ] InsightReportInterval（既存確認）

- [ ] Inspector での表示グループ化（[Header] 属性）

#### 6.2 XcodePostProcess の拡張

- [ ] `Editor/XcodePostProcess.cs` を開く
- [ ] GetEnabledEnvironmentVariables() メソッドの確認
- [ ] 新しいプロパティが自動的に処理されることを確認
- [ ] 必要に応じてロジックの修正

---

### Phase 7: Example シーンの更新（オプション）

- [ ] `Assets/_Example/Scenes/SampleScene.unity` を開く

- [ ] UI の追加
  - [ ] Opacity スライダー
  - [ ] Scale スライダー
  - [ ] Alignment ドロップダウン
  - [ ] MetricElements チェックボックス（主要なもの）
  - [ ] ShowZeroMetrics トグル
  - [ ] ShowMetricsRange トグル
  - [ ] Encoder Timing トグル
  - [ ] Shader Logging トグル
  - [ ] Insights 関連ボタン

- [ ] スクリプトの追加
  - [ ] `Assets/_Example/Runtime/PerformanceHUDControllerUI.cs` の作成または拡張
  - [ ] 各 UI 要素のイベントハンドラー実装
  - [ ] 現在の設定値の表示更新

---

### Phase 8: テストとドキュメント

#### 8.1 動作確認

- [ ] Unity Editor でビルドエラーがないことを確認
- [ ] iOS デバイスでビルド
- [ ] 各機能の動作確認
  - [ ] Opacity の変更
  - [ ] Scale の変更
  - [ ] Alignment の変更
  - [ ] MetricElements の変更
  - [ ] その他すべての新機能

#### 8.2 エラーハンドリングのテスト

- [ ] 不正な値（負の数、範囲外）の入力テスト
- [ ] null や空文字列のテスト
- [ ] Metal HUD が利用できない環境でのテスト

#### 8.3 ドキュメント更新

- [ ] README.md の更新（新機能の説明）
- [ ] CHANGELOG.md の更新
- [ ] API ドキュメントの生成確認

#### 8.4 コードレビュー

- [ ] コーディング規約の確認
- [ ] XML ドキュメントコメントの完全性
- [ ] エラーメッセージの明確性
- [ ] パフォーマンスへの影響確認

---

### Phase 9: リファクタリングと最適化（必要に応じて）

- [ ] 重複コードの削減
- [ ] ヘルパーメソッドの共通化
- [ ] メモリ効率の最適化
- [ ] API の一貫性確認

---

## 進捗管理

- **開始日**: YYYY/MM/DD
- **完了予定日**: YYYY/MM/DD
- **現在のフェーズ**: Phase X
- **完了率**: XX%

### ブロッカー・課題

（実装中に発生した問題をここに記録）

---

## 備考

- 各 Phase は独立しているため、並行作業も可能
- Phase 1-2 を完了してから Phase 3-5 に着手することを推奨
- Phase 7（Example シーン）は時間に余裕がある場合のみ実施
