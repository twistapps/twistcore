using UnityEditor;

namespace TwistCore.Editor.UIComponents
{
    public class PackagesInProjectWidget : SettingsUIComponent<TwistCoreSettings>
    {
        public override void Draw()
        {
            Window.AddSection("Packages In Project", () =>
            {
                var packages = PersistentEditorData.PackagesInProject;
                foreach (var packageData in packages)
                {
                    var nameParts = packageData.name.Split('.');
                    var organization = nameParts[1];
                    var packageName = nameParts[2];

                    Window.AddSection(packageData.name, () =>
                    {
                        Window.StatusLabel("Name", packageName, GUIStyles.DefaultLabel);
                        Window.StatusLabel("Organization", organization, GUIStyles.DefaultLabel);
                        if (packageData.repository?.url != null)
                            Window.StatusLabel("GIT URL", packageData.repository.url, EditorStyles.linkLabel);
                    }, foldout: true);
                }
            });
        }
    }
}