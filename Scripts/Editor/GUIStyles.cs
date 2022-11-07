using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public static class GUIStyles
    {
        public const string IconSuccess = "d_FilterSelectedOnly";
        public const string IconError = "console.erroricon.sml@2x";
        public const string IconWarning = "console.warnicon.sml@2x";
        public const string IconInfo = "console.infoicon.sml@2x";

        public static readonly GUIStyle DefaultLabel = EditorStyles.label;

        public static readonly GUIStyle SuccessLabel = new GUIStyle(EditorStyles.label)
        {
            normal =
            {
                textColor = Color.green
            }
        };

        public static readonly GUIStyle FailureLabel = new GUIStyle(EditorStyles.label)
        {
            normal =
            {
                textColor = Color.red
            }
        };

        public static readonly GUIStyle WarningLabel = new GUIStyle(EditorStyles.label)
        {
            normal =
            {
                textColor = Color.yellow
            }
        };
    }
}