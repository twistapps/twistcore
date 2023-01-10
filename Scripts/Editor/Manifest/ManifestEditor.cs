using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore.Editor
{
    public class ManifestEditor : ScriptableSingleton<ManifestEditor>
    {
        [SerializeField] private Manifest manifest;
        [SerializeField] public bool usingLocalManifest;

        internal static string ManifestPath => UPMCollection.Get(TwistCore.PackageName).source == PackageSource.Embedded
            ? Path.Combine("Packages", TwistCore.PackageName, TwistCore.ManifestFilename)
            : Path.Combine("Assets", "TwistApps", "Resources", TwistCore.ManifestFilename);

        public static Manifest Manifest => instance.manifest ??= FetchManifest();
        private static ManifestEditorSettings Settings => SettingsUtility.Load<ManifestEditorSettings>();

        public static string DefaultManifestURL =>
            Github.GetPackageRootURL(TwistCore.PackageName) + TwistCore.ManifestFilename;

        private static Manifest FetchManifest()
        {
            instance.usingLocalManifest = false;
            var url = Settings.useCustomManifestURL ? Settings.manifestURL : DefaultManifestURL;
            var manifest = WebRequestUtility.FetchJSON<Manifest>(url);
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
            instance.manifest = File.Exists(ManifestPath)
                ? JsonUtility.FromJson<Manifest>(File.ReadAllText(ManifestPath))
                : new Manifest();
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
                WebRequestUtility.FetchJSONTask<Manifest>(url, result => { instance.manifest = result; });

            while (coroutine.MoveNext()) yield return coroutine.Current;
            yield return new TaskProgress(2).Complete();
        }

        public static void RegisterPackage(string fullName, string url, IEnumerable<string> dependencies,
            string scriptingDefines)
        {
            var source = url.StartsWith("https://github.com/") ? "github" : "other";
            RegisterPackage(fullName, url, source, dependencies, scriptingDefines);
            SaveManifest();
        }

        public static void RegisterPackage(string fullName, string url, string source, IEnumerable<string> dependencies,
            string scriptingDefines)
        {
            var package = new Manifest.Package
            {
                name = fullName, url = url, source = source,
                dependencies = dependencies.ToList(),
                scriptingDefineSymbols = scriptingDefines
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

        public static void SetDefineSymbols(int index, string newSymbols)
        {
            var pkg = Manifest.packages[index];
            var oldSymbols = pkg.scriptingDefineSymbols;
            pkg.scriptingDefineSymbols = newSymbols;
            Manifest.packages[index] = pkg;

            SaveManifest();
            ScriptingDefinesSetter.RemoveSymbols(oldSymbols);
            ScriptingDefinesSetter.RefreshAllSymbols();
        }

        public static void UpdateDependencies(Manifest.Package package, IEnumerable<string> dependencies)
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

        public static void RemovePackageFromManifest(Manifest.Package package)
        {
            Manifest.RemovePackage(package.name);
            SaveManifest();
        }

        private static void SaveManifest()
        {
            instance.usingLocalManifest = true;
            Manifest.Save();
            UPMCollection.PurgeCache();
        }
    }
}