# MetalPerfKit-Unity

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?logo=unity)
![iOS](https://img.shields.io/badge/iOS-16.0%2B-000000?logo=apple)
![License](https://img.shields.io/badge/License-MIT-blue.svg)

Unity for iOS 環境で Metal Performance HUD を利用するためのパッケージです。

<img width="1280" src="https://github.com/user-attachments/assets/a513b2ce-71b7-4921-8ddc-f423a4337e00" />

## 概要

このパッケージは、Unity で iOS アプリをビルドする際に Metal のパフォーマンス分析機能を活用できるようにします。

主な機能:
- Metal Performance HUD の表示制御
- パフォーマンスログの取得と保存
- StateReporting によるアプリケーション状態の報告
- Xcode ビルド時の Metal 環境変数の設定

## 動作環境

- Unity 2022.3+
- iOS 16+
- StateReporter を使用する場合は Xcode 27+ / iOS 27+

## インストール (WIP)

Unity Package Manager からインストールしてください。

1. Unity エディタでプロジェクトを開く
2. Window > Package Manager を選択
3. "+" ボタンをクリック
4. "Add package from git URL..." から以下の URL を入力

```
https://github.com/mao-test-h/MetalPerfKit-Unity.git?path=Packages/jp.mao-test-h.metal-perfkit-unity
```

または、`Packages/manifest.json` に以下を追加:

```json
{
  "dependencies": {
    "jp.mao-test-h.metal-perfkit-unity": "https://github.com/mao-test-h/MetalPerfKit-Unity.git?path=Packages/jp.mao-test-h.metal-perfkit-unity",
  }
}
```

## 機能

### 1. PerformanceHUDSwitcher

Metal Performance HUD の表示/非表示や位置を制御します。

#### API

```csharp
// HUD の表示状態を取得
bool GetPerformanceHUDVisible()

// HUD の位置を取得（0.0 ～ 1.0 の相対位置）
Vector2 GetPerformanceHUDPosition()

// HUD の表示/非表示を設定
void SetPerformanceHUDVisible(bool visible)

// HUD の表示/非表示と位置を同時に設定
void SetPerformanceHUDVisible(bool visible, float x, float y)
```

#### 使用例

```csharp
using MetalPerfKit;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        // HUD を表示
        PerformanceHUDSwitcher.SetPerformanceHUDVisible(true);

        // HUD を画面の右上 (x=1.0, y=0.0) に表示
        PerformanceHUDSwitcher.SetPerformanceHUDVisible(true, 1.0f, 0.0f);

        // 現在の状態を確認
        bool isVisible = PerformanceHUDSwitcher.GetPerformanceHUDVisible();
        Vector2 position = PerformanceHUDSwitcher.GetPerformanceHUDPosition();
        Debug.Log($"HUD: {isVisible}, Position: {position}");
    }
}
```

### 2. PerformanceLogger

Metal Performance HUD のログを取得してファイルに保存します。

#### API

```csharp
// ロギング状態を取得
bool EnabledPerformanceLogging()

// ロギングを有効/無効にする
void SetPerformanceLogging(bool enabled)

// ログを取得してファイルに保存
bool FetchPerformanceLogs(int pastSeconds, string savePath)
```

#### 使用例

```csharp
using MetalPerfKit;
using System.IO;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        // ロギングを有効化
        PerformanceLogger.SetPerformanceLogging(true);
    }

    void FetchPerformanceLogs()
    {
        // 過去60秒分のログを取得して保存
        // FileUtility を使用してデバイス情報を含むファイル名を生成
        string filePath = FileUtility.GenerateFetchLoggingFilePath();
        bool success = PerformanceLogger.FetchPerformanceLogs(60, filePath);

        if (success)
        {
            Debug.Log($"Performance log saved to: {filePath}");
        }
    }
}
```

### 3. StateReporter

StateReporting を使用して、アプリケーションの状態遷移とメタデータを報告します。
StateReporter は Xcode 27 以降でビルドした iOS 27 以降のアプリで動作します。StateReporting を含まない SDK や iOS 27 未満では呼び出しが無視されます。

#### API

```csharp
StateReporter StateReporter.ReporterForDomain(string domain)

void ReportTransitionToStateLabel(
    string stateLabel,
    IReadOnlyDictionary<string, StateReporterMetadataValue> stableMetadata = null,
    IReadOnlyDictionary<string, StateReporterMetadataValue> volatileMetadata = null)

void ReportVolatileMetadataUpdate(
    IReadOnlyDictionary<string, StateReporterMetadataValue> updatedMetadata)
```

`StateReporterMetadataValue` には、文字列、Bool、符号付き・符号なし整数、`float`、`double`、`DateTimeOffset` を指定できます。

#### 使用例

```csharp
using System.Collections.Generic;
using MetalPerfKit;
using UnityEngine;

public class StateReporterExample : MonoBehaviour
{
    private StateReporter _gameplayReporter;

    void Start()
    {
        _gameplayReporter = StateReporter.ReporterForDomain("com.mygame.gameplay");
        _gameplayReporter.ReportTransitionToStateLabel(
            "Playing",
            new Dictionary<string, StateReporterMetadataValue>
            {
                ["graphicsQuality"] = "High",
                ["engineVersion"] = "1.2.3"
            },
            new Dictionary<string, StateReporterMetadataValue>
            {
                ["health"] = 100
            });
    }

    void UpdateHealth(int health)
    {
        _gameplayReporter.ReportVolatileMetadataUpdate(
            new Dictionary<string, StateReporterMetadataValue>
            {
                ["health"] = health
            });
    }

    void EndGameplay()
    {
        _gameplayReporter.ReportTransitionToStateLabel(null);
    }
}
```

`ReporterForDomain` は同じドメインに対して同じインスタンスを返します。レポーターはフィールドなどに保持し、ドメインが必要な期間中存続させてください。ドメインには機能領域を表す安定した逆 DNS 形式の文字列を使用します。ドメイン名を変更すると、以前のドメインで収集したデータとは別の系列として扱われます。

各ドメインで同時にアクティブにできる状態は 1 つです。状態は状態ラベルと stable metadata の組み合わせで識別され、両方が現在の状態と同じ場合は新しい遷移として記録されません。

- 状態ラベルには `Playing`、`High`、`Low` のような短く固定された少数の値を使用してください。
- stable metadata は状態別の集計に使われます。グラフィックス品質やエンジンバージョンなど、値の種類が少ない情報に使用してください。
- volatile metadata は状態内で変化する体力、進捗、位置などに使用してください。volatile metadata は状態別の集計単位にはなりません。

`ReportTransitionToStateLabel(null)` は現在の状態を終了します。状態ラベルが `null` の場合、stable metadata と volatile metadata を同時に指定しても無視されます。`ReportVolatileMetadataUpdate(null)` は現在の状態を維持したまま volatile metadata を消去し、アクティブな状態がない場合は何も行いません。

状態遷移と volatile metadata の更新にはレート制限があります。毎フレームや短いループ内では呼び出さず、ボタン操作、画面遷移、アクティビティ開始など、ユーザー操作と同程度か、それより低い頻度で呼び出してください。高頻度に呼び出すと StateReporting がデータを破棄します。

実装後は Metal Performance HUD または Instruments でドメイン、状態遷移、メタデータが意図どおりに表示されることを確認してください。

### 4. LaunchEnvironment (Editor 拡張)

Xcode ビルド時に Metal 関連の環境変数を設定します。

#### 対応している環境変数

- `MTL_HUD_OPACITY`: HUD の不透明度 (0.0 ～ 1.0)
- `MTL_HUD_SCALE`: HUD のスケール (0.0 ～ 1.0)
- `MTL_HUD_ALIGNMENT`: HUD の配置位置
  - topleft, topcenter, topright
  - centerleft, centered, centerright
  - bottomleft, bottomcenter, bottomright
- `MTL_HUD_INSIGHTS_ENABLED`: Metal Insights の有効化
- `MTL_HUD_INSIGHT_TIMEOUT`: Insights のタイムアウト時間
- `MTL_HUD_INSIGHT_REPORT_INTERVAL`: Insights のレポート間隔

#### 使用方法

1. `Assets/Settings` などに設定ファイルを作成
   - 右クリック > Create > MetalPerfKit > Launch Environment
2. Inspector で環境変数を設定
3. ビルド時に自動的に Xcode プロジェクトに反映されます

## サンプル

サンプルシーンは `Assets/_Example/` に含まれています。

- `ExampleApplication.cs`: HUD の制御とログ取得の実装例
- UI からの操作例

## ライセンス

MIT License

## ドキュメント

- [Monitoring your Metal app’s graphics performance](https://developer.apple.com/documentation/xcode/monitoring-your-metal-apps-graphics-performance)
- [Customizing the Metal Performance HUD](https://developer.apple.com/documentation/xcode/customizing-metal-performance-hud)
- [Understanding the Metal Performance HUD metrics](https://developer.apple.com/documentation/xcode/understanding-metal-performance-hud-metrics)
- [Gaining performance insights with the Metal Performance HUD](https://developer.apple.com/documentation/xcode/gaining-performance-insights-with-metal-performance-hud)
- [Generating performance reports with the Metal Performance HUD](https://developer.apple.com/documentation/xcode/generating-performance-reports-with-metal-performance-hud)
- [Getting started with StateReporting](https://developer.apple.com/documentation/statereporting/getting-started-with-statereporting)
- [Metal ゲームのパフォーマンス問題の検出と修正](https://developer.apple.com/jp/videos/play/wwdc2026/388/)
