﻿using System;
using System.Linq;
using TwistCore.Editor;
using UnityEngine;

namespace TwistCore.DependencyManagement
{
    public class DependencyListWindow : PackageSettingsWindow<DependencyManagerSettings>
    {
        private int _currentPackageIndex;
        private DependencyManifest.Package _editingPackage;
        private string _editingPackageShortname;
        private string _heading = "Select Dependencies";
        private string _newDependencyName = "";

        private bool[] _selectedPackagesMask;

        private void OnDestroy()
        {
            if (_editingPackage.name == null) Settings.newPackageDependencies = _editingPackage.dependencies;
            DependencyManager.UpdateDependencies(_editingPackage, _editingPackage.dependencies);
        }

        private void OnPackageSelectionChange(int packageIndex, bool selected)
        {
            var manifestPackageName = DependencyManager.Manifest.packages[packageIndex].name;

            if (!selected)
            {
                _editingPackage.dependencies.RemoveAll(pkg => pkg == manifestPackageName);
                return;
            }

            if (!_editingPackage.dependencies.Contains(manifestPackageName))
                _editingPackage.dependencies.Add(manifestPackageName);
        }

        private void OnPackageSelectionChange(bool selected)
        {
            OnPackageSelectionChange(_currentPackageIndex, selected);
        }

        private Button MakeRemoveButtonForDependency(string dependency)
        {
            return new Button("-", () => { _editingPackage.dependencies.Remove(dependency); }, 24);
        }

        private Button MakeAddDependencyButton()
        {
            return new Button("+",
                () =>
                {
                    _editingPackage.dependencies.Add(_newDependencyName);
                    _newDependencyName = "";
                }, 24);
        }

        protected override void Draw()
        {
            var manifestPackagesAmount = DependencyManager.Manifest.packages.Length;

            if (_selectedPackagesMask == null || manifestPackagesAmount != _selectedPackagesMask.Length)
                _selectedPackagesMask = new bool[manifestPackagesAmount];

            AddSection(_heading, () =>
            {
                for (var i = 0; i < manifestPackagesAmount; i++)
                {
                    _currentPackageIndex = i;
                    var manifestPackage = DependencyManager.Manifest.packages[i];
                    Checkbox(manifestPackage.name, ref _selectedPackagesMask[i], OnPackageSelectionChange,
                        expandWidth: true);
                }
            });

            AddSection("Custom/external", () =>
            {
                for (var i = 0; i < _editingPackage.dependencies.Count; i++)
                {
                    var dependency = _editingPackage.dependencies[i];
                    if (DependencyManager.Manifest.PackageExists(dependency)) continue;
                    StatusLabel($"[{i}]", 35, dependency, GUIStyles.DefaultLabel, null,
                        buttons: MakeRemoveButtonForDependency(dependency));
                }

                InputField($"[{_editingPackage.dependencies.Count}]", 35, ref _newDependencyName, 
                    buttons: MakeAddDependencyButton());
                
                GUILayout.FlexibleSpace();
                HorizontalButton(new Button("Done", Close));
            });
        }

        public static void ShowSettings(DependencyManifest.Package package)
        {
            ShowWindow(out var w, true);
            var window = w as DependencyListWindow;
            if (window == null) return;

            var manifestPackagesAmount = DependencyManager.Manifest.packages.Length;
            window._selectedPackagesMask = new bool[manifestPackagesAmount];
            window._editingPackage = package ?? new DependencyManifest.Package();
            window._heading = "Select Dependencies";

            if (package != null)
            {
                window._editingPackageShortname = package.name.Split('.').LastOrDefault();
                window._heading = "Select dependencies of " + window._editingPackageShortname;

                foreach (var dependency in package.dependencies)
                {
                    var dependencyIndex =
                        Array.FindIndex(DependencyManager.Manifest.packages, p => p.name == dependency);
                    if (dependencyIndex != -1)
                        window._selectedPackagesMask[dependencyIndex] = true;
                }
            }

            w.minSize = new Vector2(250, 300);
            w.maxSize = new Vector2(500, 500);
        }
    }
}