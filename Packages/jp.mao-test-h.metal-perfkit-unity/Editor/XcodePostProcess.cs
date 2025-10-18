#if UNITY_IOS
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace MetalPerfKit.Editor
{
    internal static class XcodePostProcess
    {
        [PostProcessBuild]
        private static void OnPostProcessBuild(BuildTarget target, string xcodeprojPath)
        {
            if (target != BuildTarget.iOS) return;

            EnablingPerformanceHUD(xcodeprojPath);
            SetEnvironmentVariables(xcodeprojPath);
        }

        // HUD の表示はデフォルトで有効化
        private static void EnablingPerformanceHUD(string outputPath)
        {
            var schemePath = $"{outputPath}/Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme";
            var xcScheme = new XcScheme();
            xcScheme.ReadFromFile(schemePath);

            // Performance HUD を有効化するための API が存在しないので、`XcScheme` が持つ `m_Doc` を取得して直接編集する
            var fieldInfo = typeof(XcScheme).GetField("m_Doc",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldInfo == null || fieldInfo.GetValue(xcScheme) is not XDocument xDocument || xDocument.Root == null)
            {
                Debug.LogError("Failed to get XDocument from XcScheme");
                return;
            }

            // `LaunchAction` に `showGraphicsOverview` と `logGraphicsOverview` を追加
            var xElement = xDocument.Root.XPathSelectElement("./LaunchAction");
            Assert.IsNotNull(xElement, "The XcScheme document does not contain build configuration setting");
            xElement.SetAttributeValue((XName)"showGraphicsOverview", "Yes");
            //xElement.SetAttributeValue((XName)"logGraphicsOverview", "Yes");

            xcScheme.WriteToFile(schemePath);
        }

        // Metal Performance HUD の環境変数を設定
        private static void SetEnvironmentVariables(string outputPath)
        {
            const string settingsAssetPath = "Packages/jp.mao-test-h.metal-perfkit-unity/Editor/MetalPerfKit_LaunchEnvironment.asset";
            var settings = AssetDatabase.LoadAssetAtPath<LaunchEnvironment>(settingsAssetPath);
            if (settings == null)
            {
                Debug.Log("[MetalPerformanceHUD] Settings asset not found. Skipping environment variables setup.");
                return;
            }

            var variables = settings.GetEnabledEnvironmentVariables();
            if (variables.Length == 0)
            {
                Debug.Log("[MetalPerformanceHUD] No environment variables enabled.");
                return;
            }

            var schemePath = $"{outputPath}/Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme";
            var xcScheme = new XcScheme();
            xcScheme.ReadFromFile(schemePath);

            var fieldInfo = typeof(XcScheme).GetField("m_Doc",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldInfo == null || fieldInfo.GetValue(xcScheme) is not XDocument xDocument || xDocument.Root == null)
            {
                Debug.LogError("[MetalPerformanceHUD] Failed to get XDocument from XcScheme");
                return;
            }

            // LaunchAction を取得
            var launchAction = xDocument.Root.XPathSelectElement("./LaunchAction");
            Assert.IsNotNull(launchAction, "The XcScheme document does not contain LaunchAction");

            // 既存の EnvironmentVariables 要素を取得、または新規作成
            var environmentVariablesElement = launchAction.Element("EnvironmentVariables");
            if (environmentVariablesElement == null)
            {
                environmentVariablesElement = new XElement("EnvironmentVariables");
                launchAction.Add(environmentVariablesElement);
            }

            // 環境変数を追加
            foreach (var variable in variables)
            {
                var key = variable.Key;
                var value = variable.Value;

                // 既存の同じキーの環境変数を削除
                var existingVariable = environmentVariablesElement.Elements("EnvironmentVariable")
                    .FirstOrDefault(e => e.Attribute("key")?.Value == key);
                existingVariable?.Remove();

                // 新しい環境変数を追加
                var envVariable = new XElement("EnvironmentVariable");
                envVariable.SetAttributeValue("key", key);
                envVariable.SetAttributeValue("value", value);
                envVariable.SetAttributeValue("isEnabled", variable.Enabled ? "YES" : "NO");
                environmentVariablesElement.Add(envVariable);

                Debug.Log($"[MetalPerformanceHUD] Added environment variable: {key}={value}");
            }

            xcScheme.WriteToFile(schemePath);
        }
    }
}
#endif
