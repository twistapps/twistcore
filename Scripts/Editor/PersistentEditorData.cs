using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TwistCore.Editor
{
    public class PersistentEditorData : ScriptableSingleton<PersistentEditorData>
    {
        [SerializeField] private bool gitInitialized;
        [SerializeField] private bool gitAvailable;
        [SerializeField] private string gitVersion;

        [SerializeField] private VersionComparer coreUpdateInfo;

        public VersionComparer CoreUpdateInfo => coreUpdateInfo ??= FetchCoreUpdates();

        public bool GitAvailable => gitInitialized ? gitAvailable : InitializeGit() != null;
        public string GitVersion => gitInitialized ? gitVersion : InitializeGit();

        private static VersionComparer FetchCoreUpdates()
        {
            var package = UPMCollection.Get(TwistCore.PackageName);
            return GithubVersionControl.FetchUpdates(package);
        }

        private string InitializeGit()
        {
            try
            {
                gitVersion = GitCmd.GetVersion();
                gitAvailable = true;
            }
            catch
            {
                gitVersion = null;
                gitAvailable = false;
            }

            gitInitialized = true;
            return gitVersion;
        }

        #region Dependency Management

        [SerializeField] private PackageData[] packagesInProject;

        public static IEnumerable<PackageData> PackagesInProjectCached =>
            instance.packagesInProject ??= UPMCollection.Packages.Select(ToPackageData).ToArray();

        public static IEnumerable<PackageData> PackagesInProject =>
            UPMCollection.Packages.Select(ToPackageData).ToArray();

        private static PackageData ToPackageData(PackageInfo packageInfo)
        {
            return (PackageData)packageInfo;
        }

        public static Manifest.Package FindManifestPackage(PackageData package)
        {
            return ManifestEditor.Manifest.Get(package.name);
        }

        [InitializeOnLoadMethod]
        private static void RegisterEvents()
        {
            UPMCollection.CachePurgedEvent += PurgePackagesInProjectCache;
        }

        private static void PurgePackagesInProjectCache()
        {
            instance.packagesInProject = null;
        }

        #endregion
    }
}