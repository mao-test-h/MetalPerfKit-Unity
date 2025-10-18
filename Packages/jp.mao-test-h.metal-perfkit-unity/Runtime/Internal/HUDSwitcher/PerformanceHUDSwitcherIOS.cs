#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MetalPerfKit
{
    /// <summary>
    /// iOS 実装の Performance HUD Switcher
    /// </summary>
    internal sealed class PerformanceHUDSwitcherIOS : IPerformanceHUDSwitcher
    {
        public bool GetPerformanceHUDVisible()
        {
            var result = MetalPerfKit_GetHUDVisible();
            if (result == (int)Status.Error)
            {
                throw new MetalPerfKitException("Failed to get Performance HUD visibility.");
            }

            return result == (int)Status.Success;
        }

        public Vector2 GetPerformanceHUDPosition()
        {
            var x = 0f;
            var y = 0f;
            var handleX = GCHandle.Alloc(x, GCHandleType.Pinned);
            var handleY = GCHandle.Alloc(y, GCHandleType.Pinned);
            try
            {
                var result = MetalPerfKit_GetHUDPosition(handleX.AddrOfPinnedObject(), handleY.AddrOfPinnedObject());
                if (result == (int)Status.Error)
                {
                    throw new MetalPerfKitException("Failed to get Performance HUD position.");
                }

                x = (float)handleX.Target;
                y = (float)handleY.Target;

                return new Vector2(x, y);
            }
            finally
            {
                handleX.Free();
                handleY.Free();
            }
        }

        public void SetPerformanceHUDVisible(bool visible)
        {
            var result = MetalPerfKit_SetHUDVisible((byte)(visible ? 1 : 0));
            if (result == (int)Status.Error)
            {
                throw new MetalPerfKitException("Failed to set Performance HUD visibility.");
            }
        }

        public void SetPerformanceHUDVisible(bool visible, float x, float y)
        {
            var result = MetalPerfKit_SetHUDVisibleWithPosition((byte)(visible ? 1 : 0), x, y);
            if (result == (int)Status.Error)
            {
                throw new MetalPerfKitException("Failed to set Performance HUD visibility with position.");
            }
        }

        [DllImport("__Internal", EntryPoint = "MetalPerfKit_GetHUDVisible")]
        private static extern int MetalPerfKit_GetHUDVisible();

        [DllImport("__Internal", EntryPoint = "MetalPerfKit_GetHUDPosition")]
        private static extern int MetalPerfKit_GetHUDPosition(IntPtr x, IntPtr y);

        [DllImport("__Internal", EntryPoint = "MetalPerfKit_SetHUDVisible")]
        private static extern int MetalPerfKit_SetHUDVisible(byte visible);

        [DllImport("__Internal", EntryPoint = "MetalPerfKit_SetHUDVisibleWithPosition")]
        private static extern int MetalPerfKit_SetHUDVisibleWithPosition(byte visible, float x, float y);
    }
}
#endif
