using System;
using System.Collections.Generic;
using TwistCore.Editor;
using TwistCore.PackageRegistry;
using UnityEditor;
using UnityEngine;

namespace TwistCore.DependencyManagement
{
    public class DependencyManagerSettingsWindow : PackageSettingsWindow<DependencyManagerSettings>
    {
        private bool _justEnteredManifestEditMode;

        private static string ManifestURL =>
            Settings.useCustomManifestURL ? Settings.manifestURL : DependencyManager.DefaultManifestURL;

        private void SetEditingPackage(DependencyManifest.Package package)
        {
            var nameParts = package.name.Split('.');
            var organization = nameParts[1];
            var packageName = nameParts[2];

            Settings.editingPackageName = packageName;
            Settings.editingPackageOrganization = organization;
            Settings.editingPackageURL = package.url;
            // ReSharper disable once AccessToModifiedClosure
            Settings.editingPackage = Array.FindIndex(DependencyManager.Manifest.packages, p => p.name == package.name);
            DependencyManager.instance.usingLocalManifest = true;
            _justEnteredManifestEditMode = true;
        }

        private static void RemovePackage(DependencyManifest.Package package)
        {
            if (EditorUtility.DisplayDialog(
                    "Package Removal Confirmation",
                    $"Are you sure you want to remove {package.name} from manifest?",
                    "Remove",
                    "Cancel"))
                DependencyManager.RemovePackageFromManifest(package);
        }

        private void ListDependencies(DependencyManifest.Package package)
        {
            var dependenciesAmount = package.dependencies?.Count ?? 0;
            var dependenciesStatus = $"Dependencies[{dependenciesAmount}]";
            ButtonLabelShrinkWidth(dependenciesStatus,
                new Button("Change", () => { DependencyListWindow.ShowSettings(package); }, 60));
        }

        protected override void Draw()
        {
            var manifestSource = "Manifest source";
            AddSection("General", () =>
            {
                if (DependencyManager.instance.usingLocalManifest)
                {
                    LabelWarning(manifestSource, "Local (edit mode)", true,
                        new Button("Switch", () =>
                        {
                            DependencyManager.LoadManifestFromURL();
                            ResetFoldouts();
                            Settings.editingPackage = -1;
                            PackageRegistryUtils.PurgeCollection();
                        }));
                }
                else
                {
                    LabelSuccess(manifestSource, Settings.useCustomManifestURL ? "Custom URL" : "TwistApps Registry", 
                        !Settings.useCustomManifestURL,
                        new Button("Switch", () =>
                        {
                            DependencyManager.LoadManifestFromFile();
                            ResetFoldouts();
                            PackageRegistryUtils.PurgeCollection();
                        }));
                    ButtonLabel("Download Manifest from web", new Button("Update", () =>
                    {
                        DependencyManager.LoadManifestFromURL();
                        DependencyManager.Manifest.Save();
                    }));
                }
            });

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
                            // if (_justEnteredManifestEditMode && !FoldoutManager.CurrentElementIsOpen)
                            // {
                            //     FoldoutManager.SetCurrentElement(true);
                            //     _justEnteredManifestEditMode = false;
                            // }

                            ListDependencies(package);

                            InputField("Name", ref Settings.editingPackageName);
                            InputField("Organization", ref Settings.editingPackageOrganization);
                            InputField("GIT URL", ref Settings.editingPackageURL);

                            ButtonLabel("",
                                //new Button("Cancel", () => { Settings.editingPackage = -1; }), 
                                new Button(
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
                        var editButton = new Button("Edit",
                            () => { SetEditingPackage(package); });
                        var removeButton = new Button("Remove",
                            () => { RemovePackage(package); });

                        ListDependencies(package);

                        StatusLabel("Name", packageName, GUIStyles.DefaultLabel);
                        StatusLabel("Organization", organization, GUIStyles.DefaultLabel);
                        StatusLabel("GIT URL", package.url, EditorStyles.linkLabel);


                        ButtonLabel("", removeButton, editButton);
                    }, foldout: true);
                }
            });
            
            AddSection("Add Package to Manifest", () =>
            {
                InputField("Full Name", ref Settings.newPackageName);
                InputField("Git URL", ref Settings.newPackageGitURL);
                
                var dependenciesStatus = $"Dependencies[{Settings.newPackageDependencies?.Count ?? 0}]";
                ButtonLabelShrinkWidth(dependenciesStatus, 16,
                    new Button("Change", () => { DependencyListWindow.ShowSettings(null); }, 60));

                ButtonLabel("", new Button("Add Package",
                    () =>
                    {
                        DependencyManager.RegisterPackage(Settings.newPackageName, Settings.newPackageGitURL,
                            Settings.newPackageDependencies);
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