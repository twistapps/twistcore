using TwistCore.ProgressWindow.Editor;

namespace TwistCore.Editor.GuiWidgets
{
    public class ManifestEditorWidget : GuiWidget<TwistCoreSettings>
    {
        private static void Open()
        {
            TaskManager.Enqueue(ManifestEditor.LoadManifestAsync(), "Fetching manifest",
                ManifestEditorSettingsWindow.ShowSettings);
        }

        public override void Draw()
        {
            Window.ButtonLabel("Manifest Editor",
                new Button("Open", Open));
        }
    }
}