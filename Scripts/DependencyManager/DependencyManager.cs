using System.Collections.Generic;
using System.IO;
using TwistCore.ProgressWindow.Editor;
using TwistCore.Utils;
using UnityEditor;
using UnityEngine;

namespace TwistCore
{
    public class DependencyManager : ScriptableSingleton<DependencyManager>
    {
        [SerializeField] private DependencyManifest manifest;
        [SerializeField] public bool usingLocalManifest;

        public static DependencyManifest Manifest => instance.manifest ??= FetchManifest();
        private static DependencyManagerSettings Settings => SettingsUtility.Load<DependencyManagerSettings>();

        public static string DefaultManifestURL =>
            Github.GetPackageRootURL(TwistCore.PackageName) + TwistCore.ManifestFilename;

        public static DependencyManifest FetchManifest()
        {
            instance.usingLocalManifest = false;
            var url = Settings.useCustomManifestURL ? Settings.manifestURL : DefaultManifestURL;
            var manifest = WebRequestUtility.FetchJSON<DependencyManifest>(url);
            return manifest;
        }

        public static void LoadManifest()
        {
            instance.usingLocalManifest = false;
            instance.manifest = FetchManifest();
        }

        public static void LoadLocalManifest()
        {
            instance.usingLocalManifest = true;
            instance.manifest = File.Exists(TwistCore.ManifestPath)
                ? JsonUtility.FromJson<DependencyManifest>(File.ReadAllText(TwistCore.ManifestPath))
                : new DependencyManifest();
        }

        public static IEnumerator<TaskProgress> FetchManifestAsync()
        {
            instance.usingLocalManifest = false;
            var url = Settings.useCustomManifestURL ? Settings.manifestURL : DefaultManifestURL;
            var coroutine =
                WebRequestUtility.FetchJSONTask<DependencyManifest>(url, result => { instance.manifest = result; });

            while (coroutine.MoveNext()) yield return coroutine.Current;

            yield return new TaskProgress(2).Complete();
        }

        public static void RegisterPackage(string fullName, string url)
        {
            var source = url.StartsWith("https://github.com/") ? "github" : "other";
            RegisterPackage(fullName, url, source);
        }

        public static void RegisterPackage(string fullName, string url, string source)
        {
            var package = new DependencyManifest.Package
            {
                name = fullName, url = url, source = source
            };
            Manifest.AddPackage(package);
        }

        public static void EditPackage(int index, string fullName, string url)
        {
            var source = url.StartsWith("https://github.com/") ? "github" : "other";

            var pkg = Manifest.packages[index];
            pkg.name = fullName;
            pkg.url = url;
            pkg.source = source;
            Manifest.packages[index] = pkg;

            Manifest.Save();
        }

        public static void RemovePackageFromManifest(string fullName)
        {
            Manifest.RemovePackage(fullName);
        }

        private void Warmup()
        {
        }
    }
}