using TwistCore.PackageDevelopment.Editor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore.PackageRegistry.Editor
{
    public static class GithubVersionControl
    {
        public static VersionComparer FetchUpdates(PackageInfo localPackage)
        {
            var url = Github.GetPackageJsonURL(localPackage.name);
            var githubPackage = WebRequestUtility.FetchJSON<PackageData>(url);

            if (githubPackage == null) return new VersionComparer();
            if (githubPackage.version == localPackage.version) return new VersionComparer();

            Debug.Log(
                $"{localPackage.name}: Local package version (#{localPackage.version}) is different from the latest version on GitHub: (#{githubPackage.version})");
            return new VersionComparer(localPackage.version, githubPackage.version);
        }

        public static VersionComparer FetchUpdates(string packageName)
        {
            var localPackage = UPMCollection.Get(packageName);
            return localPackage == null ? new VersionComparer() : FetchUpdates(localPackage);
        }
    }
}