using TwistCore.DependencyManagement;
using TwistCore.ProgressWindow.Editor;

// ReSharper disable once CheckNamespace
namespace TwistCore.Editor.UIComponents
{
    public class DependencyManagerWidget : SettingsUIComponent<TwistCoreSettings>
    {
        public override void Draw()
        {
            Window.ButtonLabel("Dependency Manager Settings",
                new Button("Open",
                    () =>
                    {
                        TaskManager.Enqueue(DependencyManager.LoadManifestAsync(), "Fetching manifest",
                            DependencyManagerSettingsWindow.ShowSettings);
                    }));
        }
    }
}