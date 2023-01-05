using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public static class ButtonStyles
    {
        public static readonly GUIStyle Default = new GUIStyle("Button");

        public static readonly GUIStyle Dimm =
            EditorGUIUtility.isProSkin ? new GUIStyle("ToolbarButton") : new GUIStyle("Button");
    }
}