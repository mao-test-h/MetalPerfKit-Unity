using System;
using System.IO;
using iOSUtility.NativeShare;
using MetalPerfKit;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Example
{
    internal sealed class ExampleApplication : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private Toggle _performanceHUDToggle;
        private Toggle _performanceHUDWithPositionToggle;
        private Toggle _switch60FPSToggle;

        private Slider _performanceHUDPositionXSlider;
        private Slider _performanceHUDPositionYSlider;

        private Toggle _performanceLoggingToggle;
        private Button _fetchLogsButton;
        private IntegerField _pastSecondsInputField;
        private Button _shareFetchLogsButton;

        private Label _resolutionText;
        private VisualElement _resolutionButtonContainer;
        private Button _resetResolutionButton;

        private string _latestFetchLogFilePath;
        private Resolution _originalResolution;

        private void Start()
        {
            InitializeUIElements();
            SetupCommonEvents();
            SetupPerformanceHUDEvents();
            SetupPerformanceLoggingEvents();
            SetupResolutionEvent();
            InitializePerformanceHUD();
        }

        private void InitializeUIElements()
        {
            var root = uiDocument.rootVisualElement;

            _performanceHUDToggle = root.Q<Toggle>("performance-hud-toggle");
            _performanceHUDWithPositionToggle = root.Q<Toggle>("performance-hud-with-position-toggle");
            _switch60FPSToggle = root.Q<Toggle>("switch-60fps-toggle");

            _performanceHUDPositionXSlider = root.Q<Slider>("performance-hud-position-x-slider");
            _performanceHUDPositionYSlider = root.Q<Slider>("performance-hud-position-y-slider");

            _performanceLoggingToggle = root.Q<Toggle>("performance-logging-toggle");
            _fetchLogsButton = root.Q<Button>("fetch-logs-button");
            _pastSecondsInputField = root.Q<IntegerField>("past-seconds-input");
            _shareFetchLogsButton = root.Q<Button>("share-fetch-logs-button");

            _resolutionText = root.Q<Label>("resolution-text");
            _resolutionButtonContainer = root.Q<VisualElement>("resolution-button-container");
            _resetResolutionButton = root.Q<Button>("reset-resolution-button");
        }

        private void InitializePerformanceHUD()
        {
            _performanceHUDToggle.SetValueWithoutNotify(true);
            _performanceLoggingToggle.SetValueWithoutNotify(false);

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

                _performanceHUDToggle.SetValueWithoutNotify(false);
                _performanceHUDWithPositionToggle.SetValueWithoutNotify(false);
                _performanceLoggingToggle.SetValueWithoutNotify(false);
                _performanceHUDPositionXSlider.SetValueWithoutNotify(0f);
                _performanceHUDPositionYSlider.SetValueWithoutNotify(0f);
            }
        }

        private void SetupCommonEvents()
        {
            _switch60FPSToggle.RegisterValueChangedCallback(evt =>
            {
                try
                {
                    Application.targetFrameRate = evt.newValue ? 60 : 30;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });
        }

        private void SetupPerformanceHUDEvents()
        {
            _performanceHUDToggle.RegisterValueChangedCallback(evt =>
            {
                try
                {
                    PerformanceHUDSwitcher.SetPerformanceHUDVisible(evt.newValue);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            _performanceHUDWithPositionToggle.RegisterValueChangedCallback(evt =>
            {
                try
                {
                    var x = _performanceHUDPositionXSlider.value;
                    var y = _performanceHUDPositionYSlider.value;
                    PerformanceHUDSwitcher.SetPerformanceHUDVisible(evt.newValue, x, y);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            _performanceHUDPositionXSlider.RegisterValueChangedCallback(evt =>
            {
                try
                {
                    if (!_performanceHUDWithPositionToggle.value) return;
                    var y = _performanceHUDPositionYSlider.value;
                    PerformanceHUDSwitcher.SetPerformanceHUDVisible(true, evt.newValue, y);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            _performanceHUDPositionYSlider.RegisterValueChangedCallback(evt =>
            {
                try
                {
                    if (!_performanceHUDWithPositionToggle.value) return;
                    var x = _performanceHUDPositionXSlider.value;
                    PerformanceHUDSwitcher.SetPerformanceHUDVisible(true, x, evt.newValue);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });
        }

        private void SetupPerformanceLoggingEvents()
        {
            _performanceLoggingToggle.RegisterValueChangedCallback(evt =>
            {
                try
                {
                    PerformanceLogger.SetPerformanceLogging(evt.newValue);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            _fetchLogsButton.RegisterCallback<ClickEvent>(_ =>
            {
                try
                {
                    var seconds = _pastSecondsInputField.value;
                    if (seconds <= 0)
                    {
                        Debug.LogWarning($"Past seconds is {seconds}");
                        return;
                    }

                    var filePath = FileUtility.GenerateFetchLoggingFilePath();
                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath).Dispose();
                    }

                    var success = PerformanceLogger.FetchPerformanceLogs(seconds, filePath);
                    Debug.Log($"Fetch logs {(success ? "succeeded" : "failed")}. Saved to {filePath}");

                    if (success)
                    {
                        _latestFetchLogFilePath = filePath;
                        // Trigger share button click
                        using var clickEvent = ClickEvent.GetPooled();
                        clickEvent.target = _shareFetchLogsButton;
                        _shareFetchLogsButton.SendEvent(clickEvent);
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

            _shareFetchLogsButton.RegisterCallback<ClickEvent>(_ =>
            {
                if (string.IsNullOrEmpty(_latestFetchLogFilePath))
                {
                    Debug.LogWarning("No fetched log file to share. Please fetch logs first.");
                    return;
                }

                NativeShare.ShareFile(_latestFetchLogFilePath);
            });
        }

        private void SetupResolutionEvent()
        {
            _originalResolution = Screen.currentResolution;
            _resolutionText.text = $"Resolution ({_originalResolution.width} x {_originalResolution.height})";

            // 解像度のリスト (width)
            var resolutions = new[]
            {
                270,
                540,
                720,
                1080,
            };

            foreach (var width in resolutions)
            {
                var resolution = CalcResolution(width);
                var button = new Button { text = $"{resolution.width}p" };
                button.AddToClassList("button");
                button.AddToClassList("resolution-button");
                _resolutionButtonContainer.Add(button);
                AddListener(resolution.width, resolution.height, button);
            }

            AddListener(_originalResolution.width, _originalResolution.height, _resetResolutionButton);
            return;

            void AddListener(int width, int height, Button button)
            {
                button.RegisterCallback<ClickEvent>(_ =>
                {
                    _resolutionText.text = $"Resolution ({width} x {height})";
                    Screen.SetResolution(width, height, true);

                    if (_performanceHUDWithPositionToggle.value)
                    {
                        var x = _performanceHUDPositionXSlider.value;
                        var y = _performanceHUDPositionYSlider.value;
                        PerformanceHUDSwitcher.SetPerformanceHUDVisible(true, x, y);
                    }
                });
            }

            // width から height を計算し、解像度を返す
            (int width, int height) CalcResolution(int width)
            {
                var aspectRatio = (float)_originalResolution.height / _originalResolution.width;
                var newHeight = (int)(width * aspectRatio);
                return (width, newHeight);
            }
        }
    }
}
