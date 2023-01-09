using TwistCore.PackageDevelopment.Editor;
using TwistCore.ProgressWindow.Editor;
using UnityEngine;

namespace TwistCore.Editor.GuiWidgets
{
    public class CoreUnpackWidget<T> : GuiWidget<T> where T : SettingsAsset
    {
        public override void Draw()
        {
            Window.ButtonLabel("Unpack Core into this package", new Button("Unpack", UnpackCore));
            Window.ButtonLabel("Remove Core", new Button("Remove", RemoveCore));
        }

        private void UnpackCore()
        {
            Debug.Log("Unpacking core library...");
            TaskManager.Enqueue(
                CoreUnpacker.UnpackIntoPackageFolderCoroutine(PackageName, TwistCore.PackageName, "TwistCore"),
                "Unpacking Core Library");
        }

        private void RemoveCore()
        {
            TaskManager.Enqueue(
                CoreUnpacker.RemoveUnpacked(PackageName, TwistCore.PackageName, "TwistCore"),
                "Removing Unpacked Library");
        }
    }
}