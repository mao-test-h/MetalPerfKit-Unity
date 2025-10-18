using System;

namespace MetalPerfKit
{
    public class MetalPerfKitException : Exception
    {
        public MetalPerfKitException()
        {
        }

        public MetalPerfKitException(string message) : base(message)
        {
        }

        public MetalPerfKitException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
