using System.IO;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor.GuiWidgets
{
    public class SelectPackageWidget<T> : GuiWidget<T> where T : SettingsAsset
    {
        public override void Draw()
        {
            Window.ButtonLabel("Focus project view on this package", new Button("Ping", () =>
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject =
                    AssetDatabase.LoadAssetAtPath<Object>(Path.Combine("Packages", PackageName, "package.json"));
            }));
        }
    }
}