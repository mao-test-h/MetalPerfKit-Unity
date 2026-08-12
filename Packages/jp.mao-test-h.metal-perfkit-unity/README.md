# MetalPerfKit-Unity

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?logo=unity)
![iOS](https://img.shields.io/badge/iOS-16.0%2B-000000?logo=apple)
![License](https://img.shields.io/badge/License-MIT-blue.svg)

A Unity package for utilizing Metal Performance HUD in Unity for iOS environments.

<img width="1280" src="https://github.com/user-attachments/assets/a513b2ce-71b7-4921-8ddc-f423a4337e00" />

[日本語版 README はこちら](./Packages/jp.mao-test-h.metal-perfkit-unity/README-JP.md)

## Overview

This package enables you to leverage Metal's performance analysis features when building iOS apps with Unity.

Key features:
- Control visibility and position of Metal Performance HUD
- Capture and save performance logs
- Report application state through StateReporting
- Configure Metal environment variables during Xcode build

## Requirements

- Unity 2022.3+
- iOS 16+
- Xcode 27+ / iOS 27+ when using StateReporter

## Installation (WIP)

Install via Unity Package Manager.

1. Open your project in Unity Editor
2. Select Window > Package Manager
3. Click the "+" button
4. Select "Add package from git URL..." and enter the following URL

```
https://github.com/mao-test-h/MetalPerfKit-Unity.git?path=Packages/jp.mao-test-h.metal-perfkit-unity
```

Or add the following to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "jp.mao-test-h.metal-perfkit-unity": "https://github.com/mao-test-h/MetalPerfKit-Unity.git?path=Packages/jp.mao-test-h.metal-perfkit-unity",
  }
}
```

## Features

### 1. PerformanceHUDSwitcher

Controls the visibility and position of the Metal Performance HUD.

#### API

```csharp
// Get the visibility state of the HUD
bool GetPerformanceHUDVisible()

// Get the HUD position (relative position from 0.0 to 1.0)
Vector2 GetPerformanceHUDPosition()

// Set the HUD visibility
void SetPerformanceHUDVisible(bool visible)

// Set both HUD visibility and position
void SetPerformanceHUDVisible(bool visible, float x, float y)
```

#### Example Usage

```csharp
using MetalPerfKit;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        // Show the HUD
        PerformanceHUDSwitcher.SetPerformanceHUDVisible(true);

        // Show the HUD at the top-right corner (x=1.0, y=0.0)
        PerformanceHUDSwitcher.SetPerformanceHUDVisible(true, 1.0f, 0.0f);

        // Check current state
        bool isVisible = PerformanceHUDSwitcher.GetPerformanceHUDVisible();
        Vector2 position = PerformanceHUDSwitcher.GetPerformanceHUDPosition();
        Debug.Log($"HUD: {isVisible}, Position: {position}");
    }
}
```

### 2. PerformanceLogger

Captures Metal Performance HUD logs and saves them to a file.

#### API

```csharp
// Get logging status
bool EnabledPerformanceLogging()

// Enable/disable logging
void SetPerformanceLogging(bool enabled)

// Fetch logs and save to file
bool FetchPerformanceLogs(int pastSeconds, string savePath)
```

#### Example Usage

```csharp
using MetalPerfKit;
using System.IO;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        // Enable logging
        PerformanceLogger.SetPerformanceLogging(true);
    }

    void FetchPerformanceLogs()
    {
        // Fetch logs from the past 60 seconds and save
        // Use FileUtility to generate a filename with device information
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

Reports application state transitions and metadata through StateReporting.
StateReporter operates in apps built with Xcode 27 or later and running on iOS 27 or later. Calls are ignored when the SDK doesn't contain StateReporting or the device is running an earlier iOS version.

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

`StateReporterMetadataValue` accepts strings, Boolean values, signed and unsigned integers, `float`, `double`, and `DateTimeOffset`.

#### Example Usage

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

`ReporterForDomain` returns the same instance for a given domain. Keep the reporter in a field or another long-lived object for as long as the domain is relevant. Use a stable reverse-DNS string that represents one functional area. Changing the domain name starts a new data series that isn't correlated with data collected under the previous name.

At most one state can be active in each domain. A state is identified by the combination of its label and stable metadata. Reporting the same label and stable metadata as the current state doesn't record a new transition.

- Use a small, fixed set of short labels such as `Playing`, `High`, or `Low`.
- Stable metadata participates in per-state aggregation. Use it for information with few distinct values, such as graphics quality or engine version.
- Use volatile metadata for health, progress, position, and other values that change within a state. Volatile metadata doesn't affect per-state aggregation.

`ReportTransitionToStateLabel(null)` ends the current state. If the state label is `null`, any stable and volatile metadata passed with it is ignored. `ReportVolatileMetadataUpdate(null)` clears volatile metadata without ending the current state and has no effect when no state is active.

State transitions and volatile metadata updates are rate-limited. Don't call them every frame or in a tight loop. Call them at human-interaction timescales, such as a button press, screen navigation, or activity start, or less frequently. StateReporting drops data when these methods are called too often.

After instrumenting the app, use Metal Performance HUD or Instruments to verify that domains, transitions, and metadata appear as intended.

### 4. LaunchEnvironment (Editor Extension)

Sets Metal-related environment variables during Xcode build.

#### Supported Environment Variables

- `MTL_HUD_OPACITY`: HUD opacity (0.0 to 1.0)
- `MTL_HUD_SCALE`: HUD scale (0.0 to 1.0)
- `MTL_HUD_ALIGNMENT`: HUD alignment position
  - topleft, topcenter, topright
  - centerleft, centered, centerright
  - bottomleft, bottomcenter, bottomright
- `MTL_HUD_INSIGHTS_ENABLED`: Enable Metal Insights
- `MTL_HUD_INSIGHT_TIMEOUT`: Insights timeout duration
- `MTL_HUD_INSIGHT_REPORT_INTERVAL`: Insights report interval

#### Usage

1. Create a settings file in `Assets/Settings` or similar location
   - Right-click > Create > MetalPerfKit > Launch Environment
2. Configure environment variables in the Inspector
3. Variables are automatically applied to the Xcode project during build

## Samples

Sample scenes are included in `Assets/_Example/`.

- `ExampleApplication.cs`: Implementation examples for HUD control and log capture
- UI operation examples

## License

MIT License

## Documentation

- [Monitoring your Metal app’s graphics performance](https://developer.apple.com/documentation/xcode/monitoring-your-metal-apps-graphics-performance)
- [Customizing the Metal Performance HUD](https://developer.apple.com/documentation/xcode/customizing-metal-performance-hud)
- [Understanding the Metal Performance HUD metrics](https://developer.apple.com/documentation/xcode/understanding-metal-performance-hud-metrics)
- [Gaining performance insights with the Metal Performance HUD](https://developer.apple.com/documentation/xcode/gaining-performance-insights-with-metal-performance-hud)
- [Generating performance reports with the Metal Performance HUD](https://developer.apple.com/documentation/xcode/generating-performance-reports-with-metal-performance-hud)
- [Getting started with StateReporting](https://developer.apple.com/documentation/statereporting/getting-started-with-statereporting)
- [Find and fix performance issues in Metal games](https://developer.apple.com/videos/play/wwdc2026/388/)
