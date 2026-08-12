using System;
using System.Collections.Generic;

namespace MetalPerfKit
{
    /// <summary>
    /// アプリケーションの状態遷移を報告するクラス
    /// </summary>
    public sealed class StateReporter
    {
        private static readonly object LockObject = new object();
        private static readonly Dictionary<string, StateReporter> Reporters = new Dictionary<string, StateReporter>();

        private readonly IStateReporter _instance;

        private StateReporter(string domain)
        {
            _instance = StateReporterFactory.Create(domain);
        }

        /// <summary>
        /// 指定したドメインの StateReporter を取得する
        /// </summary>
        /// <param name="domain">状態を識別する逆 DNS 形式のドメイン</param>
        public static StateReporter ReporterForDomain(string domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }

            if (domain.Length == 0)
            {
                throw new ArgumentException("ドメインは空にできません。", nameof(domain));
            }

            lock (LockObject)
            {
                if (Reporters.TryGetValue(domain, out var reporter))
                {
                    return reporter;
                }

                reporter = new StateReporter(domain);
                Reporters.Add(domain, reporter);
                return reporter;
            }
        }

        /// <summary>
        /// 新しい状態への遷移を報告する
        /// </summary>
        /// <param name="stateLabel">新しい状態のラベル。null の場合はメタデータを無視して現在の状態を終了する</param>
        /// <param name="stableMetadata">状態ラベルとともに状態を識別する安定したメタデータ</param>
        /// <param name="volatileMetadata">状態内で変化するメタデータ</param>
        public void ReportTransitionToStateLabel(
            string stateLabel,
            IReadOnlyDictionary<string, StateReporterMetadataValue> stableMetadata = null,
            IReadOnlyDictionary<string, StateReporterMetadataValue> volatileMetadata = null)
        {
            if (stateLabel == null)
            {
                _instance.ReportTransitionToStateLabel(null, null, null);
                return;
            }

            if (stateLabel.Length == 0)
            {
                throw new ArgumentException("状態ラベルは空にできません。", nameof(stateLabel));
            }

            _instance.ReportTransitionToStateLabel(stateLabel, stableMetadata, volatileMetadata);
        }

        /// <summary>
        /// 現在の状態を維持したまま volatile metadata を更新する
        /// </summary>
        /// <param name="updatedMetadata">更新するメタデータ。null の場合は現在の volatile metadata を消去する</param>
        public void ReportVolatileMetadataUpdate(
            IReadOnlyDictionary<string, StateReporterMetadataValue> updatedMetadata)
        {
            _instance.ReportVolatileMetadataUpdate(updatedMetadata);
        }
    }
}
