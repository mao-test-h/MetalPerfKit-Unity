using UnityEngine;

namespace iOSUtility.NativeShare
{
    public static class NativeShareFactory
    {
        public static INativeShare Create()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new NativeShareIOS();
#elif UNITY_EDITOR
            return new NativeShareEditor();
#else
            return new NativeShareDummy();
#endif
        }
    }
}
