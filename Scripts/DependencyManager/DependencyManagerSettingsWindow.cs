using TwistCore.Editor;

namespace TwistCore
{
    public class DependencyManagerSettingsWindow : PackageSettingsWindow<DependencyManagerSettings>
    {
        private string ManifestURL
        {
            get => Settings.useCustomManifestURL ? Settings.manifestURL : DependencyManager.DefaultManifestURL;
            set
            {
                if (Settings.useCustomManifestURL)
                    Settings.manifestURL = value;
            }
        }

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
                    LabelSuccess(manifestSource, "Network", false, new Button("Switch", () =>
                    {
                        DependencyManager.LoadLocalManifest();
                        ResetFoldouts();
                    }));
                    ButtonLabel("Fetch manifest", new Button("Update", DependencyManager.LoadManifest));
                    // GUILayout.Space(15);
                    //
                    // InputFieldWide("Manifest URL", ManifestURL, ref Settings.manifestURL, forceDisabled:!Settings.useCustomManifestURL);
                    // Checkbox("Use Custom URL", ref Settings.useCustomManifestURL);
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
                            ButtonLabel("", new Button("Save", () =>
                            {
                                var fullName =
                                    $"com.{Settings.editingPackageOrganization}.{Settings.editingPackageName}";
                                DependencyManager.EditPackage(i, fullName, Settings.editingPackageURL);
                                Settings.editingPackage = -1;
                            }));
                        }, foldout: true);
                        continue;
                    }


                    AddSection(package.name, () =>
                    {
                        StatusLabel("Name", packageName, GUIStyles.DefaultLabel, GUIStyles.IconInfo);
                        StatusLabel("Organization", organization, GUIStyles.DefaultLabel);
                        StatusLabel("GIT URL", package.url, GUIStyles.DefaultLabel);
                        ButtonLabel("", new Button("Edit", () =>
                        {
                            Settings.editingPackageName = packageName;
                            Settings.editingPackageOrganization = organization;
                            Settings.editingPackageURL = package.url;
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
            }, true);
        }

        public static void ShowSettings()
        {
            ShowWindow(out var window, true);
            window.ResetFoldouts();
        }
    }
}