using TwistCore.DependencyManagement;
using TwistCore.ProgressWindow.Editor;

// ReSharper disable once CheckNamespace
namespace TwistCore.Editor.UIComponents
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
            Window.ButtonLabel("Dependency Manager Settings",
                new Button("Open", Open));
        }
    }
}