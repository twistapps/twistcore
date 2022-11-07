using TwistCore.Editor.UIComponents;
using TwistCore.PackageDevelopment.Editor;
using TwistCore.PackageRegistry;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public class TwistCoreSettingsWindow : PackageSettingsWindow<TwistCoreSettings>
    {
        protected override void Draw()
        {
            BeginSection("Requirements");

            if (PersistentEditorData.instance.GitAvailable)
                LabelSuccess("Git", "Available", true, new Button(PersistentEditorData.instance.GitVersion));
            else
                LabelWarning("Git", "Not Available", false,
                    new Button("Fix", () => { Application.OpenURL("https://git-scm.com/"); }));


            // var corePackageName = TwistCoreDefinitions.PackageName;
            // var corePackage = PackageRegistry.Get(corePackageName);
            //
            // if (!PackagesLock.IsInDevelopmentMode(corePackageName))
            // {
            //     LabelWarning(corePackage.displayName, "Development Mode", true, new Button(corePackage.version));
            // }
            // else
            // {
            //     var status = "Up To Date";
            //     var version = GithubVersionControl.CompareVersion(corePackage);
            //     if (version.HasMajorUpdate()) status = "Major Update!";
            //     else if (version.HasMinorUpdate()) status = "Has Updates";
            //     else if (version.HasPatchUpdate()) status = "New Patch";
            //     
            //     var style = version.HasMinorUpdate() ? GUIStyles.WarningLabel : GUIStyles.DefaultLabel;
            //     var icon = version.hasUpdate ? GUIStyles.IconWarning : GUIStyles.IconSuccess;
            //     StatusLabel(corePackage.displayName, status, style, icon, new Button(corePackage.version));
            //     
            //     if (!version.hasUpdate)
            //         HorizontalButtons(new Button("Update Core", UpdatePackage));
            // }

            EndSection();

            AddSection("Package Development", () =>
            {
                // if (!Settings.packageDevelopment)
                //     HorizontalButton(new Button("Enter Development Mode", () => { Settings.packageDevelopment = true; },
                //         150));
                //
                // if (Settings.packageDevelopment)
                //     HorizontalButton(new Button("Exit Development Mode", () => { Settings.packageDevelopment = false; },
                //         150));

                this.DrawCachedComponent("DependencyManagerWidget");
            });

            BeginSection("Create Package", ref Settings.enablePackageCreation, true);
            InputField("Name", ref Settings.newPackageName);
            InputField("Displayname", ref Settings.newPackageDisplayname);
            InputField("Description", ref Settings.newPackageDescription);

            HorizontalButton(new Button("Create Package", PackageCreationTool.CreatePackageBasedOnSettings));
            EndSection();

            this.DrawCachedComponent("PackagesInProjectWidget");
        }

        // ReSharper disable once UnusedMember.Local
        private void UpdatePackage()
        {
            var package = TwistCore.PackageName;
            UPMInterface.Update(package);
        }

        [MenuItem("Tools/Twist Apps/Twist Core Settings")]
        public static void OnMenuItemClick()
        {
            ShowWindow();
        }
    }
}