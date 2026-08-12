using System;
using System.Globalization;

namespace MetalPerfKit
{
    /// <summary>
    /// StateReporter へ渡すメタデータの値
    /// </summary>
    public readonly struct StateReporterMetadataValue
    {
        internal StateReporterMetadataValueType Type { get; }
        internal string SerializedValue { get; }

        private StateReporterMetadataValue(StateReporterMetadataValueType type, string serializedValue)
        {
            Type = type;
            SerializedValue = serializedValue;
        }

        public static implicit operator StateReporterMetadataValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new StateReporterMetadataValue(StateReporterMetadataValueType.String, value);
        }

        public static implicit operator StateReporterMetadataValue(bool value)
        {
            return new StateReporterMetadataValue(
                StateReporterMetadataValueType.Boolean,
                value ? "true" : "false");
        }

        public static implicit operator StateReporterMetadataValue(sbyte value)
        {
            return CreateInteger(value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(byte value)
        {
            return CreateInteger(value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(short value)
        {
            return CreateInteger(value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(ushort value)
        {
            return CreateInteger(value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(int value)
        {
            return CreateInteger(value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(uint value)
        {
            return CreateInteger(value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(long value)
        {
            return CreateInteger(value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(ulong value)
        {
            return CreateInteger(value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(float value)
        {
            return new StateReporterMetadataValue(
                StateReporterMetadataValueType.FloatingPoint,
                value.ToString("R", CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(double value)
        {
            return new StateReporterMetadataValue(
                StateReporterMetadataValueType.FloatingPoint,
                value.ToString("R", CultureInfo.InvariantCulture));
        }

        public static implicit operator StateReporterMetadataValue(DateTimeOffset value)
        {
            return new StateReporterMetadataValue(
                StateReporterMetadataValueType.Date,
                value.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture));
        }

        private static StateReporterMetadataValue CreateInteger(string value)
        {
            return new StateReporterMetadataValue(StateReporterMetadataValueType.Integer, value);
        }
    }

    internal enum StateReporterMetadataValueType
    {
        String = 0,
        Boolean = 1,
        Integer = 2,
        FloatingPoint = 3,
        Date = 4
    }
}
