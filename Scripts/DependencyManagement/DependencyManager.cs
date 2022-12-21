using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwistCore.PackageRegistry;
using TwistCore.ProgressWindow.Editor;
using UnityEditor;
using UnityEngine;

namespace TwistCore.DependencyManagement
{
    public class DependencyManager : ScriptableSingleton<DependencyManager>
    {
        [SerializeField] private DependencyManifest manifest;
        [SerializeField] public bool usingLocalManifest;

        public static DependencyManifest Manifest => instance.manifest ??= FetchManifest();
        private static DependencyManagerSettings Settings => SettingsUtility.Load<DependencyManagerSettings>();

        public static string DefaultManifestURL =>
            Github.GetPackageRootURL(TwistCore.PackageName) + TwistCore.ManifestFilename;

        private static DependencyManifest FetchManifest()
        {
            instance.usingLocalManifest = false;
            var url = Settings.useCustomManifestURL ? Settings.manifestURL : DefaultManifestURL;
            var manifest = WebRequestUtility.FetchJSON<DependencyManifest>(url);
            return manifest;
        }

        public static void LoadManifestFromURL()
        {
            instance.usingLocalManifest = false;
            instance.manifest = FetchManifest();
        }

        public static void LoadManifestFromFile()
        {
            instance.usingLocalManifest = true;
            instance.manifest = File.Exists(TwistCore.ManifestPath)
                ? JsonUtility.FromJson<DependencyManifest>(File.ReadAllText(TwistCore.ManifestPath))
                : new DependencyManifest();
        }

        public static IEnumerator<TaskProgress> LoadManifestAsync()
        {
            if (instance.usingLocalManifest)
            {
                yield return new TaskProgress(1).Next("Loading from local file...");
                LoadManifestFromFile();
                yield break;
            }

            var url = Settings.useCustomManifestURL ? Settings.manifestURL : DefaultManifestURL;
            var coroutine =
                WebRequestUtility.FetchJSONTask<DependencyManifest>(url, result => { instance.manifest = result; });

            while (coroutine.MoveNext()) yield return coroutine.Current;
            yield return new TaskProgress(2).Complete();
        }
        
        public static void RegisterPackage(string fullName, string url, IEnumerable<string> dependencies)
        {
            var source = url.StartsWith("https://github.com/") ? "github" : "other";
            RegisterPackage(fullName, url, source, dependencies);
            SaveManifest();
        }

        public static void RegisterPackage(string fullName, string url, string source, IEnumerable<string> dependencies)
        {
            var package = new DependencyManifest.Package
            {
                name = fullName, url = url, source = source,
                dependencies = dependencies.ToList()
            };
            Manifest.AddPackage(package);
            SaveManifest();
        }

        public static void EditPackage(int index, string fullName, string url)
        {
            var source = url.StartsWith("https://github.com/") ? "github" : "other";

            var pkg = Manifest.packages[index];
            pkg.name = fullName;
            pkg.url = url;
            pkg.source = source;
            Manifest.packages[index] = pkg;

            SaveManifest();
        }

        public static void UpdateDependencies(DependencyManifest.Package package, IEnumerable<string> dependencies)
        {
            var index = Array.FindIndex(Manifest.packages, p => p.name == package.name);
            if (index < 0 || Manifest.packages.Length <= index) return;

            Manifest.packages[index].dependencies = dependencies.ToList();
            SaveManifest();
        }

        public static void UpdateDependencies(string packageName, IEnumerable<string> dependencies)
        {
            var index = Array.FindIndex(Manifest.packages, package => package.name == packageName);
            if (index == -1) return;
            UpdateDependencies(index, dependencies);
        }

        public static void UpdateDependencies(int packageIndex, IEnumerable<string> dependencies)
        {
            if (packageIndex < 0 || Manifest.packages.Length <= packageIndex) return;
            Manifest.packages[packageIndex].dependencies = dependencies.ToList();
            SaveManifest();
        }

        public static void RemovePackageFromManifest(DependencyManifest.Package package)
        {
            Manifest.RemovePackage(package.name);
            SaveManifest();
        }

        private static void SaveManifest()
        {
            instance.usingLocalManifest = true;
            Manifest.Save();
            PackageRegistryUtils.PurgeCollection();
        }
    }
}