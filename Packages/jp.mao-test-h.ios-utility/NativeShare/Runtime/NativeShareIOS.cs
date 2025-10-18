#if UNITY_IOS
using System.Runtime.InteropServices;

namespace iOSUtility.NativeShare
{
    public sealed class NativeShareIOS : INativeShare
    {
        public void ShareFile(string filePath, string subject = "", string text = "")
        {
            NativeMethod(filePath, subject, text);
            return;

            [DllImport("__Internal", EntryPoint = "iOSUtility_NativeShare_ShareFile")]
            static extern void NativeMethod(string filePath, string subject, string text);
        }
    }
}
#endif
