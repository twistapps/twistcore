using UnityEngine;

namespace TwistCore
{
    public abstract class SettingsAsset : ScriptableObject
    {
        public abstract string GetEditorWindowTitle();
        public abstract string GetPackageName();
    }
}