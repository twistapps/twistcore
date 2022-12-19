using System.IO;
using Newtonsoft.Json.Linq;
using TwistCore.PackageRegistry;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore.PackageDevelopment
{
    public static class PackageLock
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
        private static readonly string LockfilePath = Path.Combine("Packages", "packages-lock.json");

        public static PackageSource GetSource(string packageName)
        {
            var json = JObject.Parse(File.ReadAllText(LockfilePath));
            var source = (string)json["dependencies"]?[packageName]?["source"];

            return source switch
            {
                GIT => PackageSource.Git,
                EMBEDDED => PackageSource.Embedded,
                REGISTRY => PackageSource.UnityRegistry,
                _ => PackageSource.Unknown
            };
        }

        public static bool IsGithubPackage(string packageName)
        {
            if (GetSource(packageName) != PackageSource.Git) return false;
            var packageInfo = PackageRegistryUtils.Get(packageName);
            return packageInfo.repository.url.Contains("https://github.com/");
        }

        public static PackageData GetInfo(string packageName)
        {
            var path = Path.Combine("Packages", packageName, "package.json");
            return !File.Exists(path) ? null : JsonUtility.FromJson<PackageData>(path);
        }

        public static PackageData GetInfoByPath(string packageDirectory)
        {
            var path = Path.Combine(packageDirectory, "package.json");
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