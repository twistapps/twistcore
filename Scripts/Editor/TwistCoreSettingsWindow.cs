using System.Linq;
using TwistCore.Editor.GuiWidgets;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public class TwistCoreSettingsWindow : PackageSettingsWindow<TwistCoreSettings>
    {
        protected override void DrawGUI()
        {
            BeginSection("Requirements");

            if (PersistentEditorData.instance.GitAvailable)
                LabelSuccess("Git", "Available", true, new Button(PersistentEditorData.instance.GitVersion));
            else
                LabelWarning("Git", "Not Available", false,
                    new Button("Fix", () => { Application.OpenURL("https://git-scm.com/"); }));


            var corePackage = UPMCollection.Get(TwistCore.PackageName);

            if (PackageLock.IsInDevelopmentMode(TwistCore.PackageName))
            {
                LabelWarning(corePackage.displayName, "Development Mode", true, new Button(corePackage.version));
            }
            else
            {
                //todo: extract to its own method
                var status = "Up to date";
                var version = PersistentEditorData.PackagesInProjectCached
                    .FirstOrDefault(p => p.name == TwistCore.PackageName)
                    ?.UpdateInfo;
                if (version != null)
                {
                    if (version.HasMajorUpdate()) status = "Major Update!";
                    else if (version.HasMinorUpdate()) status = "Has Updates";
                    else if (version.HasPatchUpdate()) status = "New Patch";

                    var style = version.HasMinorUpdate() ? GUIStyles.WarningLabel : GUIStyles.DefaultLabel;
                    var icon = version.HasUpdate ? GUIStyles.IconWarning : GUIStyles.IconSuccess;
                    StatusLabel(corePackage.displayName, status, style, icon, new Button(corePackage.version));

                    if (version.HasUpdate)
                        HorizontalButtons(
                            new Button("Changelog", () => { Application.OpenURL(corePackage.changelogUrl); }),
                            new Button("Update", UpdatePackage));
                }
            }

            EndSection();

            AddSection("Package Development", () =>
            {
                Checkbox("Core Debug Mode", ref Settings.debug, val => { CustomScriptingDefines.UpdateAll(); });
                this.DrawCachedComponent("ManifestEditorWidget");
            });

            this.DrawCachedComponent("PackagesInProjectWidget");

            BeginSection("Create Package", ref Settings.enablePackageCreation, true);
            InputField("Name", ref Settings.newPackageName);
            InputField("Displayname", ref Settings.newPackageDisplayname);
            InputField("Description", ref Settings.newPackageDescription);

            var _ = false;
            Checkbox("Add to Manifest", ref _);
            Checkbox("Initialize Git", ref _);

            HorizontalButton(new Button("Create Package", PackageCreationTool.CreatePackageBasedOnSettings));
            EndSection();
        }

        // ReSharper disable once UnusedMember.Local
        private void UpdatePackage()
        {
            var package = TwistCore.PackageName;
            UPMInterface.Update(package);
            UPMCollection.PurgeCache();
        }

        [MenuItem("Tools/Twist Apps/Twist Core Settings")]
        public static void OnMenuItemClick()
        {
            ShowWindow();
        }
    }
}