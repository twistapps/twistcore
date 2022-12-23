using System;
using System.Collections.Generic;
using TwistCore.Editor;
using TwistCore.PackageRegistry;
using UnityEditor;
using UnityEngine;

namespace TwistCore.DependencyManagement
{
    public class ManifestEditorSettingsWindow : PackageSettingsWindow<ManifestEditorSettings>
    {

        private static string ManifestURL =>
            Settings.useCustomManifestURL ? Settings.manifestURL : ManifestEditor.DefaultManifestURL;

        private void SetEditingPackage(Manifest.Package package)
        {
            var nameParts = package.name.Split('.');
            var organization = nameParts[1];
            var packageName = nameParts[2];

            Settings.editingPackageName = packageName;
            Settings.editingPackageOrganization = organization;
            Settings.editingPackageURL = package.url;
            Settings.editingPackageDefineSymbols = package.scriptingDefineSymbols;
            // ReSharper disable once AccessToModifiedClosure
            Settings.editingPackage = Array.FindIndex(ManifestEditor.Manifest.packages, p => p.name == package.name);
            ManifestEditor.instance.usingLocalManifest = true;
        }

        private static void RemovePackage(Manifest.Package package)
        {
            if (EditorUtility.DisplayDialog(
                    "Package Removal Confirmation",
                    $"Are you sure you want to remove {package.name} from manifest?",
                    "Remove",
                    "Cancel"))
                ManifestEditor.RemovePackageFromManifest(package);
        }

        private void ListDependencies(Manifest.Package package)
        {
            var dependenciesAmount = package.dependencies?.Count ?? 0;
            var dependenciesStatus = $"Dependencies[{dependenciesAmount}]";
            ButtonLabelShrinkWidth(dependenciesStatus,
                new Button("Change", () => { DependencyPickerWindow.Show(package); }, 60));
        }

        protected override void DrawGUI()
        {
            var manifestSource = "Manifest source";
            AddSection("General", () =>
            {
                if (ManifestEditor.instance.usingLocalManifest)
                {
                    LabelWarning(manifestSource, "Local (edit mode)", true,
                        new Button("Switch", ButtonStyles.Dimm, () =>
                        {
                            ManifestEditor.LoadManifestFromURL();
                            ResetFoldouts();
                            Settings.editingPackage = -1;
                            UPMCollection.PurgeCache();
                        }));
                }
                else
                {
                    LabelSuccess(manifestSource, Settings.useCustomManifestURL ? "Custom URL" : "TwistApps Registry", 
                        !Settings.useCustomManifestURL,
                        new Button("Switch", ButtonStyles.Dimm, () =>
                        {
                            ManifestEditor.LoadManifestFromFile();
                            ResetFoldouts();
                            UPMCollection.PurgeCache();
                        }));
                    ButtonLabel("Download Manifest from web", new Button("Update", ButtonStyles.Dimm, () =>
                    {
                        ManifestEditor.LoadManifestFromURL();
                        ManifestEditor.Manifest.Save();
                    }));
                }
            });

            AddSection("Manifest URL",
                () => { InputFieldWide("Custom Manifest Link", ManifestURL, ref Settings.manifestURL); },
                ref Settings.useCustomManifestURL);

            AddSection("Edit Manifest", () =>
            {
                if (ManifestEditor.Manifest.packages.Length < 1)
                    CallToAction("Manifest is empty.");

                for (var i = 0; i < ManifestEditor.Manifest.packages.Length; i++)
                {
                    var package = ManifestEditor.Manifest.packages[i];

                    var nameParts = package.name.Split('.');
                    var organization = nameParts[1];
                    var packageName = nameParts[2];

                    if (Settings.editingPackage == i)
                    {
                        AddSection(package.name, () =>
                        {
                            ListDependencies(package);

                            InputField("Name", ref Settings.editingPackageName);
                            InputField("Organization", ref Settings.editingPackageOrganization);
                            InputField("GIT URL", ref Settings.editingPackageURL);
                            InputField("Scripting Defines", ref Settings.editingPackageDefineSymbols);

                            ButtonLabel("",
                                new Button("Cancel", () => { Settings.editingPackage = -1; }), 
                                new Button(
                                    "Save", () =>
                                    {
                                        var fullName =
                                            $"com.{Settings.editingPackageOrganization}.{Settings.editingPackageName}";
                                        // ReSharper disable once AccessToModifiedClosure
                                        ManifestEditor.EditPackage(i, fullName, Settings.editingPackageURL);
                                        ManifestEditor.SetDefineSymbols(i, Settings.editingPackageDefineSymbols);
                                        Settings.editingPackage = -1;
                                    }));
                        }, foldout: true);
                        continue;
                    }

                    AddSection(package.name, () =>
                    {
                        var editButton = new Button("Edit", ButtonStyles.Dimm,
                            () => { SetEditingPackage(package); });
                        var removeButton = new Button("Remove", ButtonStyles.Dimm,
                            () => { RemovePackage(package); });

                        ListDependencies(package);

                        StatusLabel("Name", packageName, GUIStyles.DefaultLabel);
                        StatusLabel("Organization", organization, GUIStyles.DefaultLabel);
                        StatusLabel("GIT URL", package.url, EditorStyles.linkLabel);
                        StatusLabel("Scripting Defines", package.scriptingDefineSymbols, GUIStyles.DefaultLabel);


                        ButtonLabel("", removeButton, editButton);
                    }, foldout: true);
                }
            });
            
            AddSection("Add Package to Manifest", () =>
            {
                InputField("Full Name", ref Settings.newPackageName);
                InputField("Git URL", ref Settings.newPackageGitURL);
                InputField("Scripting Defines", ref Settings.newPackageDefineSymbols);
                
                var dependenciesStatus = $"Dependencies[{Settings.newPackageDependencies?.Count ?? 0}]";
                ButtonLabelShrinkWidth(dependenciesStatus, 16,
                    new Button("Change", () => { DependencyPickerWindow.Show(null); }, 60));

                ButtonLabel("", new Button("Add Package", ButtonStyles.Dimm,
                    () =>
                    {
                        ManifestEditor.RegisterPackage(Settings.newPackageName, Settings.newPackageGitURL,
                            Settings.newPackageDependencies, Settings.newPackageDefineSymbols);
                    }, 110));
            }, ref Settings.addPackageEnabled);
        }

        public static void ShowSettings()
        {
            ShowWindow(out var window, true);
            Settings.newPackageDependencies = new List<string>();
            window.ResetFoldouts();
            window.minSize = new Vector2(370, 500);
        }
    }
}