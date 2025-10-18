using System;
using System.IO;
using UnityEngine;

namespace MetalPerfKit
{
    public static class FileUtility
    {
        public static string GenerateFetchLoggingFilePath()
        {
            var deviceModel = SystemInfo.deviceModel.Replace(" ", "_");
            var os = SystemInfo.operatingSystem.Replace(" ", "_");
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"FetchLogging_{deviceModel}_{os}_{timestamp}.txt";
            var filePath = Path.Combine(Application.persistentDataPath, fileName);
            return filePath;
        }
    }
}
