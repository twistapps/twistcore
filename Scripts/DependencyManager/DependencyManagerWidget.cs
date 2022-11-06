using TwistCore.ProgressWindow.Editor;

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
                        TaskManager.Enqueue(DependencyManager.FetchManifestAsync(), "Fetching manifest",
                            DependencyManagerSettingsWindow.ShowSettings);
                    }));
        }
    }
}