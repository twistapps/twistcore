using UnityEngine;

namespace TwistCore
{
    //todo:consider renaming to ScriptableSettings
    public abstract class SettingsAsset : ScriptableObject
    {
        public abstract string GetEditorWindowTitle();
        public abstract string GetPackageName();
    }
}