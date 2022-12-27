using TwistCore.Editor;
using UnityEditor;
using UnityEngine;

namespace TwistCore
{
    
    public class $NAME$SettingsWindow : PackageSettingsWindow<$NAME$Settings>
    {
        protected override void DrawGUI()
        {
            AddSection("General", () =>
            {
                Heading("Hello World!");
                Heading("Edit this in 'VisualEditorSettingsWindow' file.", true);
            });
        }

        [MenuItem("Tools/Twist Apps/$DISPLAYNAME$ Settings")]
        public static void OnMenuItemClick()
        {
            ShowWindow();
        }
    }
        
}
