using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public class DependencyInstallerWindow : PackageSettingsWindow<TwistCoreSettings>
    {
        private static bool _somePackagesNotInstalled, _somePackagesHaveUpdates;
        private static List<Manifest.Package> _notInstalled = new List<Manifest.Package>();
        private static List<Manifest.Package> _haveUpdates = new List<Manifest.Package>();

        private static bool ActionRequired => _somePackagesNotInstalled || _somePackagesHaveUpdates;

        protected override void DrawGUI()
        {
            var heading = ActionRequired ? "Some dependencies require to be installed" : "yaay";
            AddSection(heading, () =>
            {
                if (!ActionRequired)
                {
                    //Heading("No updates required.", true, true, new Button("Close", ButtonStyles.Dimm, Close));
                    //HorizontalButton(new Button("Close this window", ButtonStyles.Dimm, Close));
                    GUILayout.Space(15);
                    Heading("Everything is up to date!", true, true);
                }
                else
                {
                    if (_somePackagesNotInstalled)
                    {
                        Heading("Not Installed");
                        EditorGUI.indentLevel++;
                        foreach (var package in _notInstalled) DrawInfo(package);
                        EditorGUI.indentLevel--;
                    }

                    if (_somePackagesHaveUpdates)
                    {
                        Heading("Have updates");
                        EditorGUI.indentLevel++;
                        foreach (var package in _haveUpdates) DrawInfo(package);
                        EditorGUI.indentLevel--;
                    }
                }

                GUILayout.FlexibleSpace();

                var installButton = new Button("Install All", InstallAllDependencies);
                var updateButton = new Button("Update All", UpdateAllDependencies);

                if (!_somePackagesNotInstalled) installButton.Disable();
                if (!_somePackagesHaveUpdates) updateButton.Disable();

                if (ActionRequired)
                    HorizontalButtons(updateButton, installButton);
                else
                    HorizontalButton(new Button("Done", Close));
                Divider();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    CheckboxSmall("don't show this again", ref Settings.dontAutoShowDependencyInstaller);
                }

                GUILayout.Space(-15);
            });
        }

        private void DrawInfo(Manifest.Package package)
        {
            AddSection(package.name, () =>
            {
                StatusLabel("Source:", 90, package.source,
                    buttons: new Button("Resolve", () => { InstallPackage(package); }));
                StatusLabel("Parent:", 90, package.url);
            }, foldout: true);
        }

        [InitializeOnLoadMethod]
        private static void ListMissingDependencies()
        {
            _notInstalled = new List<Manifest.Package>();
            _haveUpdates = new List<Manifest.Package>();

            foreach (var packageData in PersistentEditorData.PackagesInProject)
            {
                var manifestPackage = PersistentEditorData.FindManifestPackage(packageData);
                foreach (var dependency in manifestPackage.dependencies)
                {
                    var dependencyInfo = (PackageData)UPMCollection.GetFromAllPackages(dependency);
                    if (dependencyInfo == null)
                    {
                        _notInstalled.Add(ManifestEditor.Manifest.Get(dependency) ?? new Manifest.Package
                        {
                            name = dependency,
                            source = "Unity Registry",
                            url = manifestPackage.name
                        });
                        continue;
                    }

                    if (dependencyInfo.UpdateInfo.HasUpdate)
                        _haveUpdates.Add(ManifestEditor.Manifest.Get(dependency));
                }
            }

            _somePackagesNotInstalled = _notInstalled.Count > 0;
            _somePackagesHaveUpdates = _haveUpdates.Count > 0;
        }

        private static void InstallPackage(Manifest.Package package)
        {
            var sourceAddress = package.source == "github" ? package.url : package.name;
            UPMInterface.Install(sourceAddress);
        }

        private static void InstallAllDependencies()
        {
            foreach (var package in _notInstalled) InstallPackage(package);
        }

        private static void UpdateAllDependencies()
        {
            foreach (var package in _haveUpdates) InstallPackage(package);
        }

        public static void OnReloadAssets()
        {
            if (Settings == null) Settings = SettingsUtility.Load<TwistCoreSettings>();
            if (_somePackagesNotInstalled && !Settings.dontAutoShowDependencyInstaller)
                ShowSettings();
        }

        [MenuItem("Tools/Twist Apps/Update Dependencies", priority = -1)]
        public static void ShowSettings()
        {
            ShowWindow(out var window, true);
            window.ResetFoldouts();
            window.minSize = new Vector2(370, 220);
            //window.maxSize = new Vector2(1000, 1000);
        }
    }
}