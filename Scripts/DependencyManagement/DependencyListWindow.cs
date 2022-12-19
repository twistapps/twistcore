using System;
using System.Linq;
using TwistCore.Editor;
using UnityEngine;

namespace TwistCore.DependencyManagement
{
    public class DependencyListWindow : PackageSettingsWindow<DependencyManagerSettings>
    {
        private string _newDependencyName = "";
        private DependencyManifest.Package _package;
        private bool[] _selections;
        private string _shortname;

        private void OnDestroy()
        {
            if (_package.name == null) Settings.newPackageDependencies = _package.dependencies;
            DependencyManager.UpdateDependencies(_package, _package.dependencies);
        }

        protected override void Draw()
        {
            var packagesLength = DependencyManager.Manifest.packages.Length;

            if (_selections == null || packagesLength != _selections.Length)
                _selections = new bool[packagesLength];

            var heading = _package.name == null
                ? "Select Dependencies"
                : "Select dependencies of " + _shortname;

            AddSection(heading, () =>
            {
                for (var i = 0; i < packagesLength; i++)
                {
                    var manifestPackage = DependencyManager.Manifest.packages[i];
                    Checkbox(manifestPackage.name, ref _selections[i], selected =>
                    {
                        if (selected)
                        {
                            if (!_package.dependencies.Contains(manifestPackage.name))
                                _package.dependencies.Add(manifestPackage.name);
                        }
                        else
                        {
                            _package.dependencies.RemoveAll(pkg => pkg == manifestPackage.name);
                        }
                    }, expandWidth: true);
                }
            });
            AddSection("Custom/external", () =>
            {
                for (var i = 0; i < _package.dependencies.Count; i++)
                {
                    var dependency = _package.dependencies[i];
                    if (DependencyManager.Manifest.PackageExists(dependency)) continue;
                    StatusLabel($"[{i}]", 35, dependency, GUIStyles.DefaultLabel, null,
                        new Button("-", () => { _package.dependencies.Remove(dependency); }, 24));
                }

                var addButton = new Button("+",
                    () =>
                    {
                        _package.dependencies.Add(_newDependencyName);
                        _newDependencyName = "";
                    }, 24);

                InputField($"[{_package.dependencies.Count}]", 35, ref _newDependencyName, buttons: addButton);


                GUILayout.FlexibleSpace();
                HorizontalButton(new Button("Done", Close));
            });
        }

        public static void ShowSettings(DependencyManifest.Package package)
        {
            ShowWindow(out var w, true);
            var window = w as DependencyListWindow;
            if (window == null) return;

            window._selections = new bool[DependencyManager.Manifest.packages.Length];
            window._package = package ?? new DependencyManifest.Package();

            if (package != null)
            {
                window._shortname = package.name.Split('.').LastOrDefault();
                foreach (var dependency in package.dependencies)
                {
                    var dependencyIndex =
                        Array.FindIndex(DependencyManager.Manifest.packages, p => p.name == dependency);
                    if (dependencyIndex != -1)
                        window._selections[dependencyIndex] = true;
                }
            }

            w.minSize = new Vector2(250, 300);
            w.maxSize = new Vector2(500, 500);
        }
    }
}