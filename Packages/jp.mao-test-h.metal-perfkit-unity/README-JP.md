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
  - `StateReporter` を使用する場合: Xcode 27+ / iOS 27+

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

### 3. LaunchEnvironment (Editor 拡張)

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

### 4. StateReporter (Preview)

> [!WARNING]
> `StateReporter` はプレビュー機能です。  
>
> Xcode 27 以降でビルドし、iOS 27 以降で実行した場合に動作します。  
> `StateReporting` フレームワークを含まない SDK でビルドした場合や、iOS 27 未満で実行した場合、呼び出しは無視されます。

このパッケージの `StateReporter` は、Apple の StateReporting API を利用して、アプリケーションの状態遷移とメタデータを記録します。  
API の概要や呼び出し頻度に関するレート制限など、詳しい仕様は公式ドキュメントを参照してください。  

- [Getting started with StateReporting](https://developer.apple.com/documentation/statereporting/getting-started-with-statereporting)
- [StateReporter](https://developer.apple.com/documentation/statereporting/statereporter)

#### API

```csharp
// 指定したドメインの StateReporter を取得する
StateReporter StateReporter.ReporterForDomain(string domain)

// 新しい状態への遷移を記録する
void ReportTransitionToStateLabel(
    string stateLabel,
    IReadOnlyDictionary<string, StateReporterMetadataValue> stableMetadata = null,
    IReadOnlyDictionary<string, StateReporterMetadataValue> volatileMetadata = null)

// 現在の状態を維持したまま volatileMetadata を更新する
void ReportVolatileMetadataUpdate(
    IReadOnlyDictionary<string, StateReporterMetadataValue> updatedMetadata)
```

`StateReporterMetadataValue` には、以下の型を指定できます。

- `string`
- `bool`
- `sbyte`, `short`, `int`, `long`
- `byte`,`ushort`, `uint`, `ulong`
- `float`,`double`
- `DateTimeOffset`

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
