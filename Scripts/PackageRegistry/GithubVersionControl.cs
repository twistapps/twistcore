using TwistCore.PackageDevelopment;
using TwistCore.PackageRegistry.Versioning;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore.PackageRegistry
{
    public static class GithubVersionControl
    {
        public static VersionComparison FetchUpdates(PackageInfo localPackage)
        {
            var url = Github.GetPackageJsonURL(localPackage.name);
            var githubPackage = WebRequestUtility.FetchJSON<PackageData>(url);

            if (githubPackage == null) return new VersionComparison();
            if (githubPackage.version == localPackage.version) return new VersionComparison();

            Debug.Log(
                $"{localPackage.name}: Local package version (#{localPackage.version}) is different from the latest version on GitHub: (#{githubPackage.version})");
            return new VersionComparison(localPackage.version, githubPackage.version);
        }

        public static VersionComparison FetchUpdates(string packageName)
        {
            var localPackage = PackageRegistryUtils.Get(packageName);
            return FetchUpdates(localPackage);
        }
    }
}