using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore
{
    public static class GithubVersionControl
    {
        public static string GetGithubURLByPackageName(string packageName)
        {
            var parts = packageName.Split('.');
            var organization = parts[1];
            var package = parts[2];

            var url = $"https://raw.githubusercontent.com/{organization}/{package}/main/package.json";
            return url;
        }
        
        public static VersionComparison CompareVersion(PackageInfo localPackage)
        {
            var url = GetGithubURLByPackageName(localPackage.name);
            var githubPackage = PackageFetch.Get(url);
            
            if (githubPackage == null) return new VersionComparison();
            if (githubPackage.version == localPackage.version) return new VersionComparison();
            
            Debug.Log($"{localPackage.name}: Local package version (#{localPackage.version}) is different from the latest version on GitHub: (#{githubPackage.version})");
            return new VersionComparison(localPackage.version, githubPackage.version);

        }

        public static VersionComparison CompareVersion(string packageName)
        {
            var localPackage = PackageRegistry.Get(packageName);
            return CompareVersion(localPackage);
        }
    }
}