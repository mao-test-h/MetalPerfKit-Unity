#if UNITY_IOS
using System;
using UnityEngine;

namespace MetalPerfKit.Editor
{
    [CreateAssetMenu(fileName = "MetalPerfKit_LaunchEnvironment", menuName = "MetalPerfKit/Launch Environment")]
    internal sealed class LaunchEnvironment : ScriptableObject
    {
        [Serializable]
        public sealed class EnvironmentVariable
        {
            [SerializeField] private bool enabled;
            [SerializeField] private string key;
            [SerializeField] private string value;

            public bool Enabled => enabled;
            public string Key => key;
            public string Value => value;

            public EnvironmentVariable(bool enabled, string key, string value)
            {
                this.enabled = enabled;
                this.key = key;
                this.value = value;
            }
        }

        [Header("MTL_HUD_OPACITY")]
        [SerializeField] private bool enableOpacity = false;
        [SerializeField, Range(0f, 1f)] private float opacity = 1.0f;

        [Header("MTL_HUD_SCALE")]
        [SerializeField] private bool enableScale = false;
        [SerializeField, Range(0f, 1f)] private float scale = 0.2f;

        [Header("MTL_HUD_ALIGNMENT")]
        [SerializeField] private bool enableAlignment = false;
        [SerializeField] private HudAlignment alignment = HudAlignment.TopRight;

        [Header("MTL_HUD_INSIGHTS_ENABLED")]
        [SerializeField] private bool enableInsights = false;
        [SerializeField] private int insightTimeout = 10;
        [SerializeField] private int insightReportInterval = 5;

        private enum HudAlignment
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

        public EnvironmentVariable[] GetEnabledEnvironmentVariables()
        {
            var variables = new System.Collections.Generic.List<EnvironmentVariable>();
            variables.Add(new EnvironmentVariable(enableOpacity, "MTL_HUD_OPACITY", $"{opacity:F1}"));
            variables.Add(new EnvironmentVariable(enableScale, "MTL_HUD_SCALE", $"{scale:F1}"));

            var alignmentValue = alignment switch
            {
                HudAlignment.TopLeft => "topleft",
                HudAlignment.TopCenter => "topcenter",
                HudAlignment.TopRight => "topright",
                HudAlignment.CenterLeft => "centerleft",
                HudAlignment.Centered => "centered",
                HudAlignment.CenterRight => "centerright",
                HudAlignment.BottomLeft => "bottomleft",
                HudAlignment.BottomCenter => "bottomcenter",
                HudAlignment.BottomRight => "bottomright",
                _ => "topright"
            };
            variables.Add(new EnvironmentVariable(enableAlignment, "MTL_HUD_ALIGNMENT", $"{alignmentValue}"));

            variables.Add(new EnvironmentVariable(enableInsights, "MTL_HUD_INSIGHTS_ENABLED", "1"));
            variables.Add(new EnvironmentVariable(enableInsights, "MTL_HUD_INSIGHT_TIMEOUT", $"{insightTimeout}"));
            variables.Add(new EnvironmentVariable(enableInsights, "MTL_HUD_INSIGHT_REPORT_INTERVAL", $"{insightReportInterval}"));

            return variables.ToArray();
        }
    }
}
#endif
