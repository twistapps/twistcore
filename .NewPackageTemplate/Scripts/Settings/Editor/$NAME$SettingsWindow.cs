using TwistCore.Editor;
using UnityEditor;
using UnityEngine;

namespace TwistCore
{
    
    public class $NAME$SettingsWindow : PackageSettingsWindow<$NAME$Settings>
    {
        protected override void OnGUI()
        {
            base.OnGUI();
            
            BeginSection("General");
            HorizontalButtons(
                new Button("Dummy", () => Debug.Log("Hello World")),
                new Button("Dummy 2", () => Debug.Log("Hello World!!!")));
            EndSection();
            
            WatchChangesAbove(); 
        }

        [MenuItem("Tools/Twist Apps/$DISPLAYNAME$ Settings")]
        public static void OnMenuItemClick()
        {
            ShowWindow();
        }
    }
        
}
