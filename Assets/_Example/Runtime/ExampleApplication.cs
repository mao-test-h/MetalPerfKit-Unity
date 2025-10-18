using System;
using System.IO;
using iOSUtility.NativeShare;
using MetalPerfKit;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.UI;

namespace _Example
{
    internal sealed class ExampleApplication : MonoBehaviour
    {
        [SerializeField] private Toggle performanceHUDToggle;
        [SerializeField] private Toggle performanceHUDWithPositionToggle;
        [SerializeField] private Toggle switch60FPSToggle;

        [SerializeField] private Slider performanceHUDPositionXSlider;
        [SerializeField] private Slider performanceHUDPositionYSlider;

        [SerializeField] private Toggle performanceLoggingToggle;
        [SerializeField] private Button fetchLogsButton;
        [SerializeField] private InputField pastSecondsInputField;
        [SerializeField] private Button shareFetchLogsButton;

        private readonly INativeShare _nativeShare = NativeShareFactory.Create();
        private string _latestFetchLogFilePath;

        private void Start()
        {
            SetupCommonEvents();
            SetupPerformanceHUDEvents();
            SetupPerformanceLoggingEvents();
            InitializePerformanceHUD();
        }

        private void InitializePerformanceHUD()
        {
            performanceHUDToggle.SetIsOnWithoutNotify(true);
            performanceLoggingToggle.SetIsOnWithoutNotify(false);

            try
            {
                var isVisible = PerformanceHUDSwitcher.GetPerformanceHUDVisible();
                var isLoggingEnabled = PerformanceLogger.EnabledPerformanceLogging();
                var position = PerformanceHUDSwitcher.GetPerformanceHUDPosition();

                Debug.Log(
                    $"Performance HUD is {(isVisible ? "visible" : "hidden")}, logging is {(isLoggingEnabled ? "enabled" : "disabled")}, position=({position.x}, {position.y})");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

                performanceHUDToggle.SetIsOnWithoutNotify(false);
                performanceHUDWithPositionToggle.SetIsOnWithoutNotify(false);
                performanceLoggingToggle.SetIsOnWithoutNotify(false);
                performanceHUDPositionXSlider.SetValueWithoutNotify(0f);
                performanceHUDPositionYSlider.SetValueWithoutNotify(0f);
            }
        }

        private void SetupCommonEvents()
        {
            switch60FPSToggle.onValueChanged.AddListener(isOn =>
            {
                try
                {
                    Application.targetFrameRate = isOn ? 60 : 30;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });
        }

        private void SetupPerformanceHUDEvents()
        {
            performanceHUDToggle.onValueChanged.AddListener(isOn =>
            {
                try
                {
                    PerformanceHUDSwitcher.SetPerformanceHUDVisible(isOn);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            performanceHUDWithPositionToggle.onValueChanged.AddListener(isOn =>
            {
                try
                {
                    var x = performanceHUDPositionXSlider.value;
                    var y = performanceHUDPositionYSlider.value;
                    PerformanceHUDSwitcher.SetPerformanceHUDVisible(isOn, x, y);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            performanceHUDPositionXSlider.onValueChanged.AddListener(x =>
            {
                try
                {
                    if (!performanceHUDWithPositionToggle.isOn) return;
                    var y = performanceHUDPositionYSlider.value;
                    PerformanceHUDSwitcher.SetPerformanceHUDVisible(true, x, y);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            performanceHUDPositionYSlider.onValueChanged.AddListener(y =>
            {
                try
                {
                    if (!performanceHUDWithPositionToggle.isOn) return;
                    var x = performanceHUDPositionXSlider.value;
                    PerformanceHUDSwitcher.SetPerformanceHUDVisible(true, x, y);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });
        }

        private void SetupPerformanceLoggingEvents()
        {
            performanceLoggingToggle.onValueChanged.AddListener(isOn =>
            {
                try
                {
                    PerformanceLogger.SetPerformanceLogging(isOn);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            fetchLogsButton.onClick.AddListener(() =>
            {
                try
                {
                    if (!int.TryParse(pastSecondsInputField.text, out var seconds))
                    {
                        Debug.LogWarning($"Past seconds is {seconds}");
                        return;
                    }

                    var filePath = GetFilePath();
                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath);
                    }

                    var success = PerformanceLogger.FetchPerformanceLogs(seconds, filePath);
                    Debug.Log($"Fetch logs {(success ? "succeeded" : "failed")}. Saved to {filePath}");

                    if (success)
                    {
                        _latestFetchLogFilePath = filePath;
                        shareFetchLogsButton.onClick.Invoke();
                    }
                    else
                    {
                        File.Delete(filePath);
                        _latestFetchLogFilePath = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            shareFetchLogsButton.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(_latestFetchLogFilePath))
                {
                    Debug.LogWarning("No fetched log file to share. Please fetch logs first.");
                    return;
                }

                _nativeShare.ShareFile(_latestFetchLogFilePath);
            });

            static string GetFilePath()
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"FetchLogging_{timestamp}.txt";
                var filePath = Path.Combine(Application.persistentDataPath, fileName);
                return filePath;
            }
        }
    }
}
