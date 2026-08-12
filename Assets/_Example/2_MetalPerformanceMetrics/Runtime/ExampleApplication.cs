using System;
using System.Collections.Generic;
using MetalPerfKit;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Example.MetalPerformanceMetrics
{
    internal sealed class ExampleApplication : MonoBehaviour
    {
        private const string ScreenStateLabel = "Screen";
        private const string GraphicsStateLabel = "Graphics";
        private const string PerformantQualityName = "Performant";
        private const string BalancedQualityName = "Balanced";
        private const string HighFidelityQualityName = "High Fidelity";

        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private Transform cubesContainer;
        [SerializeField] private GameObject cubeTemplate;

        private readonly List<GameObject> _generatedCubes = new List<GameObject>();

        private VisualElement _root;
        private VisualElement _safeArea;
        private VisualElement _titlePage;
        private VisualElement _mainPage;
        private VisualElement _settingsPage;

        private Button _openMainButton;
        private Button _openSettingsButton;
        private Button _add100Button;
        private Button _add1000Button;
        private Button _add10000Button;
        private Button _mainBackButton;
        private Button _settingsBackButton;

        private Label _cubeCountLabel;
        private DropdownField _frameRateDropdown;
        private DropdownField _qualityDropdown;

        private StateReporter _screenReporter;
        private StateReporter _graphicsReporter;
        private System.Random _random = new System.Random(0);
        private Rect _lastSafeArea;
        private int _lastScreenWidth;
        private int _lastScreenHeight;
        private ScreenType _currentScreen;
        private GraphicsQuality _currentQuality;

        private int CubeCount => 1 + _generatedCubes.Count;

        private void Start()
        {
            InitializeUIElements();
            RegisterUIEvents();
            InitializeSettings();
            InitializeStateReporters();
            ShowScreen(ScreenType.Title);
        }

        private void OnDestroy()
        {
            UnregisterUIEvents();

            if (_root != null)
            {
                _root.UnregisterCallback<GeometryChangedEvent>(OnRootGeometryChanged);
            }

            ReportSafely(() => _screenReporter?.ReportTransitionToStateLabel(null));
            ReportSafely(() => _graphicsReporter?.ReportTransitionToStateLabel(null));
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea ||
                _lastScreenWidth != Screen.width ||
                _lastScreenHeight != Screen.height)
            {
                UpdateSafeArea();
            }
        }

        private void InitializeUIElements()
        {
            _root = uiDocument.rootVisualElement;
            _safeArea = _root.Q<VisualElement>("safe-area");

            _titlePage = _root.Q<VisualElement>("title-page");
            _mainPage = _root.Q<VisualElement>("main-page");
            _settingsPage = _root.Q<VisualElement>("settings-page");

            _openMainButton = _root.Q<Button>("open-main-button");
            _openSettingsButton = _root.Q<Button>("open-settings-button");
            _add100Button = _root.Q<Button>("add-100-button");
            _add1000Button = _root.Q<Button>("add-1000-button");
            _add10000Button = _root.Q<Button>("add-10000-button");
            _mainBackButton = _root.Q<Button>("main-back-button");
            _settingsBackButton = _root.Q<Button>("settings-back-button");

            _cubeCountLabel = _root.Q<Label>("cube-count-label");
            _frameRateDropdown = _root.Q<DropdownField>("frame-rate-dropdown");
            _qualityDropdown = _root.Q<DropdownField>("quality-dropdown");

            _frameRateDropdown.choices = new List<string> { "30", "60" };
            _qualityDropdown.choices = new List<string> { "Low", "Medium", "High" };

            _root.RegisterCallback<GeometryChangedEvent>(OnRootGeometryChanged);
        }

        private void OnRootGeometryChanged(GeometryChangedEvent evt)
        {
            UpdateSafeArea();
        }

        private void UpdateSafeArea()
        {
            var safeArea = Screen.safeArea;
            var panelWidth = _root.resolvedStyle.width;
            var panelHeight = _root.resolvedStyle.height;

            if (Screen.width <= 0 || Screen.height <= 0 ||
                float.IsNaN(panelWidth) || float.IsNaN(panelHeight))
            {
                return;
            }

            _safeArea.style.left = safeArea.xMin / Screen.width * panelWidth;
            _safeArea.style.right = (Screen.width - safeArea.xMax) / Screen.width * panelWidth;
            _safeArea.style.top = (Screen.height - safeArea.yMax) / Screen.height * panelHeight;
            _safeArea.style.bottom = safeArea.yMin / Screen.height * panelHeight;

            _lastSafeArea = safeArea;
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
        }

        private void RegisterUIEvents()
        {
            _openMainButton.clicked += OpenMain;
            _openSettingsButton.clicked += OpenSettings;
            _add100Button.clicked += Add100Cubes;
            _add1000Button.clicked += Add1000Cubes;
            _add10000Button.clicked += Add10000Cubes;
            _mainBackButton.clicked += BackToTitle;
            _settingsBackButton.clicked += BackToTitle;
            _frameRateDropdown.RegisterValueChangedCallback(OnFrameRateChanged);
            _qualityDropdown.RegisterValueChangedCallback(OnQualityChanged);
        }

        private void UnregisterUIEvents()
        {
            if (_openMainButton == null)
            {
                return;
            }

            _openMainButton.clicked -= OpenMain;
            _openSettingsButton.clicked -= OpenSettings;
            _add100Button.clicked -= Add100Cubes;
            _add1000Button.clicked -= Add1000Cubes;
            _add10000Button.clicked -= Add10000Cubes;
            _mainBackButton.clicked -= BackToTitle;
            _settingsBackButton.clicked -= BackToTitle;
            _frameRateDropdown.UnregisterValueChangedCallback(OnFrameRateChanged);
            _qualityDropdown.UnregisterValueChangedCallback(OnQualityChanged);
        }

        private void InitializeSettings()
        {
            Application.targetFrameRate = 60;
            _frameRateDropdown.SetValueWithoutNotify("60");
            ApplyQuality(GraphicsQuality.Medium, false);
        }

        private void InitializeStateReporters()
        {
            _screenReporter = StateReporter.ReporterForDomain($"{Application.identifier}.screen");
            _graphicsReporter = StateReporter.ReporterForDomain($"{Application.identifier}.graphics");
            ReportGraphicsState();
        }

        private void OpenMain()
        {
            ShowScreen(ScreenType.Main);
        }

        private void OpenSettings()
        {
            ShowScreen(ScreenType.Settings);
        }

        private void BackToTitle()
        {
            ShowScreen(ScreenType.Title);
        }

        private void Add100Cubes()
        {
            AddCubes(100);
        }

        private void Add1000Cubes()
        {
            AddCubes(1000);
        }

        private void Add10000Cubes()
        {
            AddCubes(10000);
        }

        private void ShowScreen(ScreenType screen)
        {
            if (_currentScreen == ScreenType.Main && screen != ScreenType.Main)
            {
                DestroyGeneratedCubes();
            }

            SetDisplayed(_titlePage, screen == ScreenType.Title);
            SetDisplayed(_mainPage, screen == ScreenType.Main);
            SetDisplayed(_settingsPage, screen == ScreenType.Settings);

            _currentScreen = screen;
            UpdateCubeCountLabel();
            ReportScreenState();
        }

        private static void SetDisplayed(VisualElement element, bool displayed)
        {
            element.style.display = displayed ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void AddCubes(int count)
        {
            var origin = cubeTemplate.transform.localPosition;

            for (var i = 0; i < count; i++)
            {
                var cube = Instantiate(cubeTemplate, cubesContainer, false);
                cube.name = "Cube";
                cube.transform.localPosition = origin + new Vector3(
                    NextRandom(-2f, 2f),
                    NextRandom(-2f, 2f),
                    NextRandom(-2f, 2f));
                cube.transform.localRotation = Quaternion.Euler(
                    NextRandom(0f, 360f),
                    NextRandom(0f, 360f),
                    NextRandom(0f, 360f));
                _generatedCubes.Add(cube);
            }

            UpdateCubeCountLabel();
            ReportCubeCount();
        }

        private float NextRandom(float min, float max)
        {
            return (float)(_random.NextDouble() * (max - min) + min);
        }

        private void DestroyGeneratedCubes()
        {
            foreach (var cube in _generatedCubes)
            {
                if (cube == null)
                {
                    continue;
                }

                cube.SetActive(false);
                Destroy(cube);
            }

            _generatedCubes.Clear();
            _random = new System.Random(0);
            UpdateCubeCountLabel();
        }

        private void UpdateCubeCountLabel()
        {
            _cubeCountLabel.text = $"CubeCount: {CubeCount:N0}";
        }

        private void OnFrameRateChanged(ChangeEvent<string> evt)
        {
            Application.targetFrameRate = int.Parse(evt.newValue);
        }

        private void OnQualityChanged(ChangeEvent<string> evt)
        {
            var quality = evt.newValue switch
            {
                "Low" => GraphicsQuality.Low,
                "Medium" => GraphicsQuality.Medium,
                "High" => GraphicsQuality.High,
                _ => throw new ArgumentOutOfRangeException(nameof(evt.newValue), evt.newValue, null)
            };

            ApplyQuality(quality, true);
        }

        private void ApplyQuality(GraphicsQuality quality, bool reportState)
        {
            var qualityName = GetQualityLevelName(quality);
            var qualityIndex = Array.IndexOf(QualitySettings.names, qualityName);
            if (qualityIndex < 0)
            {
                Debug.LogError($"品質レベル '{qualityName}' が見つかりません。");
                return;
            }

            QualitySettings.SetQualityLevel(qualityIndex, true);
            _currentQuality = quality;
            _qualityDropdown.SetValueWithoutNotify(GetQualityDisplayName(quality));

            if (reportState)
            {
                ReportGraphicsState();
            }
        }

        private void ReportScreenState()
        {
            ReportSafely(() => _screenReporter.ReportTransitionToStateLabel(
                ScreenStateLabel,
                new Dictionary<string, StateReporterMetadataValue>
                {
                    ["ScreenName"] = _currentScreen.ToString()
                },
                new Dictionary<string, StateReporterMetadataValue>
                {
                    ["CubeCount"] = CubeCount
                }));
        }

        private void ReportCubeCount()
        {
            ReportSafely(() => _screenReporter.ReportVolatileMetadataUpdate(
                new Dictionary<string, StateReporterMetadataValue>
                {
                    ["CubeCount"] = CubeCount
                }));
        }

        private void ReportGraphicsState()
        {
            ReportSafely(() => _graphicsReporter.ReportTransitionToStateLabel(
                GraphicsStateLabel,
                new Dictionary<string, StateReporterMetadataValue>
                {
                    ["Quality"] = GetQualityMetadataValue(_currentQuality)
                }));
        }

        private static void ReportSafely(Action report)
        {
            try
            {
                report();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static string GetQualityLevelName(GraphicsQuality quality)
        {
            return quality switch
            {
                GraphicsQuality.Low => PerformantQualityName,
                GraphicsQuality.Medium => BalancedQualityName,
                GraphicsQuality.High => HighFidelityQualityName,
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, null)
            };
        }

        private static string GetQualityDisplayName(GraphicsQuality quality)
        {
            return quality switch
            {
                GraphicsQuality.Low => "Low",
                GraphicsQuality.Medium => "Medium",
                GraphicsQuality.High => "High",
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, null)
            };
        }

        private static string GetQualityMetadataValue(GraphicsQuality quality)
        {
            return quality switch
            {
                GraphicsQuality.Low => "URP-Performant",
                GraphicsQuality.Medium => "URP-Balanced",
                GraphicsQuality.High => "URP-HighFidelity",
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, null)
            };
        }

        private enum ScreenType
        {
            Title,
            Main,
            Settings
        }

        private enum GraphicsQuality
        {
            Low,
            Medium,
            High
        }
    }
}
