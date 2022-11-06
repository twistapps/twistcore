using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore
{
    public static class PackagesLock
    {
        public enum PackageSource
        {
            Git,
            Embedded,
            UnityRegistry,
            Unknown
        }

        private const string GIT = "git";
        private const string EMBEDDED = "embedded";
        private const string REGISTRY = "registry";
        private static readonly string Lockfile = Path.Combine("Packages", "packages-lock.json");

        public static PackageSource GetSource(string packageName)
        {
            var json = JObject.Parse(File.ReadAllText(Lockfile));
            var source = (string)json["dependencies"]?[packageName]?["source"];

            return source switch
            {
                GIT => PackageSource.Git,
                EMBEDDED => PackageSource.Embedded,
                REGISTRY => PackageSource.UnityRegistry,
                _ => PackageSource.Unknown
            };
        }

        public static PackageData GetInfo(string name)
        {
            var path = Path.Combine("Packages", name, "package.json");
            return !File.Exists(path) ? null : JsonUtility.FromJson<PackageData>(path);
        }

        public static PackageData GetInfoByPath(string directory)
        {
            var path = Path.Combine(directory, "package.json");
            return !File.Exists(path) ? null : JsonUtility.FromJson<PackageData>(path);
        }

        public static bool IsInDevelopmentMode(string packageName)
        {
            var source = GetSource(packageName);
            return source == PackageSource.Embedded;
        }

        public static bool IsInDevelopmentMode(PackageInfo package)
        {
            return IsInDevelopmentMode(package.name);
        }
    }
}