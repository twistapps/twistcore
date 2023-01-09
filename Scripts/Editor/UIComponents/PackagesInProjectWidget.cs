using System.Collections.Generic;
using System.Linq;
using TwistCore.PackageDevelopment.Editor;
using TwistCore.PackageRegistry.Editor;
using UnityEditor;

namespace TwistCore.Editor.GuiWidgets
{
    [WidgetIncludesSection(true)]
    public class PackagesInProjectWidget : GuiWidget<TwistCoreSettings>
    {
        private void DrawUpToDatePackages(List<PackageData> packages)
        {
            if (packages.Count < 1) return;
            foreach (var package in packages)
            {
                Window.Heading(package.displayName);
                EditorGUI.indentLevel++;
                Window.StatusLabel("Full Name", package.name, GUIStyles.DefaultLabel);
                Window.LabelSuccess("Version", package.version, true);
                EditorGUI.indentLevel--;
                //Window.Divider();
            }
        }

        private void DrawOutdatedPackages(List<PackageData> packages)
        {
            if (packages.Count < 1) return;
            foreach (var package in packages)
            {
                Window.Heading(package.displayName);
                EditorGUI.indentLevel++;
                Window.StatusLabel("Full Name", package.name, GUIStyles.DefaultLabel);
                Window.LabelWarning("Version", package.version, true);
                Window.StatusLabel("New Version", package.UpdateInfo.NextVersion, EditorStyles.linkLabel);
                Window.ButtonLabel("", new Button("Download Update",
                    () =>
                    {
                        UPMInterface.Update(package.name);
                        UPMCollection.PurgeCache();
                    }, 120));
                EditorGUI.indentLevel--;
                //Window.Divider();
            }
        }

        private void DrawOtherPackages(List<PackageData> packages)
        {
            if (packages.Count < 1) return;
            Window.Heading("Other Packages");

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
        }

        public override void Draw()
        {
            Window.AddSection("Packages In Project:", () =>
            {
                Window.ButtonLabel("Reload Packages", new Button("Refresh", UPMCollection.PurgeCache));
                //Window.Space(10);
                var packages = PersistentEditorData.PackagesInProjectCached.ToArray();

                var upToDatePackages = new List<PackageData>();
                var outdatedPackages = new List<PackageData>();
                var otherPackages = new List<PackageData>();

                foreach (var packageData in packages)
                    if (PackageLock.IsGithubPackage(packageData.name))
                    {
                        //Debug.Log($"[{packageData.name}] - {packageData.UpdateInfo.NewVersion}");

                        if (packageData.UpdateInfo.HasUpdate)
                            outdatedPackages.Add(packageData);
                        else
                            upToDatePackages.Add(packageData);
                    }
                    else
                    {
                        otherPackages.Add(packageData);
                    }

                DrawOutdatedPackages(outdatedPackages);
                DrawUpToDatePackages(upToDatePackages);
                DrawOtherPackages(otherPackages);
            });
        }
    }
}