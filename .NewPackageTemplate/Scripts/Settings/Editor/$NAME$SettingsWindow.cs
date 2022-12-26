using TwistCore.Editor;
using UnityEditor;
using UnityEngine;

namespace TwistCore
{
    
    public class $NAME$SettingsWindow : PackageSettingsWindow<$NAME$Settings>
    {
        private override void OnDrawGUI()
        {
            AddSection("General", () =>
            {
                Header("Hello World!");
                Header("Edit this in '$NAME$SettingsWindow' file.");
            })
        }

        [MenuItem("Tools/Twist Apps/$DISPLAYNAME$ Settings")]
        public static void OnMenuItemClick()
        {
            ShowWindow();
        }
    }
        
}
