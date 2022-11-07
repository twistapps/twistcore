using TwistCore.Editor;
using UnityEditor;

namespace TwistCore.DependencyManagement
{
    public class DependencyManagerSettingsWindow : PackageSettingsWindow<DependencyManagerSettings>
    {
        private string ManifestURL =>
            Settings.useCustomManifestURL ? Settings.manifestURL : DependencyManager.DefaultManifestURL;

        protected override void Draw()
        {
            var manifestSource = "Manifest source";
            AddSection("General", () =>
            {
                if (DependencyManager.instance.usingLocalManifest)
                {
                    LabelWarning(manifestSource, "Local (edit mode)", true, new Button("Switch", () =>
                    {
                        DependencyManager.LoadManifest();
                        ResetFoldouts();
                    }));
                }
                else
                {
                    LabelSuccess(manifestSource, Settings.useCustomManifestURL ? "Custom URL" : "TwistApps Registry",
                        !Settings.useCustomManifestURL, new Button("Switch", () =>
                        {
                            DependencyManager.LoadLocalManifest();
                            ResetFoldouts();
                        }));
                    ButtonLabel("Fetch manifest", new Button("Update", () =>
                    {
                        DependencyManager.LoadManifest();
                        DependencyManager.Manifest.Save();
                    }));
                }
            });

            if (!DependencyManager.instance.usingLocalManifest)
                AddSection("Manifest URL",
                    () => { InputFieldWide("Custom Manifest Link", ManifestURL, ref Settings.manifestURL); },
                    ref Settings.useCustomManifestURL);

            AddSection("Edit Manifest", () =>
            {
                if (DependencyManager.Manifest.packages.Length < 1)
                    CallToAction("Manifest is empty.");

                for (var i = 0; i < DependencyManager.Manifest.packages.Length; i++)
                {
                    var package = DependencyManager.Manifest.packages[i];

                    var nameParts = package.name.Split('.');
                    var organization = nameParts[1];
                    var packageName = nameParts[2];


                    if (Settings.editingPackage == i)
                    {
                        AddSection(package.name, () =>
                        {
                            InputField("Name", ref Settings.editingPackageName);
                            InputField("Organization", ref Settings.editingPackageOrganization);
                            InputField("GIT URL", ref Settings.editingPackageURL);
                            ButtonLabel("", new Button("Cancel", () => { Settings.editingPackage = -1; }), new Button(
                                "Save", () =>
                                {
                                    var fullName =
                                        $"com.{Settings.editingPackageOrganization}.{Settings.editingPackageName}";
                                    // ReSharper disable once AccessToModifiedClosure
                                    DependencyManager.EditPackage(i, fullName, Settings.editingPackageURL);
                                    Settings.editingPackage = -1;
                                }));
                        }, foldout: true);
                        continue;
                    }

                    AddSection(package.name, () =>
                    {
                        StatusLabel("Name", packageName, GUIStyles.DefaultLabel);
                        StatusLabel("Organization", organization, GUIStyles.DefaultLabel);
                        StatusLabel("GIT URL", package.url, EditorStyles.linkLabel);
                        ButtonLabel("", new Button("Edit", () =>
                        {
                            Settings.editingPackageName = packageName;
                            Settings.editingPackageOrganization = organization;
                            Settings.editingPackageURL = package.url;
                            // ReSharper disable once AccessToModifiedClosure
                            Settings.editingPackage = i;
                        }));
                    }, foldout: true);
                }
            });

            AddSection("Register Package", () =>
            {
                InputField("Full Name", ref Settings.newPackageName);
                InputField("Git URL", ref Settings.newPackageGitURL);
                HorizontalButton(new Button("Add to Manifest",
                    () => { DependencyManager.RegisterPackage(Settings.newPackageName, Settings.newPackageGitURL); }));
            });
        }

        public static void ShowSettings()
        {
            ShowWindow(out var window, true);
            window.ResetFoldouts();
        }
    }
}