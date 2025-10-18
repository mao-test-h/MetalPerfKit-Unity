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
- Configure Metal environment variables during Xcode build

## Requirements

- Unity 2022.3+
- iOS 16+

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

    void OnApplicationQuit()
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

### 3. LaunchEnvironment (Editor Extension)

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
