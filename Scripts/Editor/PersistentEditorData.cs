using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public class PersistentEditorData : ScriptableSingleton<PersistentEditorData>
    {
        #region Git

        [SerializeField] private bool gitInitialized;
        [SerializeField] private bool gitAvailable;
        [SerializeField] private string gitVersion;

        public bool GitAvailable => gitInitialized ? gitAvailable : InitializeGit() != null;
        public string GitVersion => gitInitialized ? gitVersion : InitializeGit();

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

        #endregion

        #region Dependency Management

        [SerializeField] private PackageData[] packagesInProject;

        public static IEnumerable<PackageData> PackagesInProjectCached =>
            instance.packagesInProject ??= UPMCollection.Packages.ToPackageData();

        public static IEnumerable<PackageData> PackagesInProject =>
            UPMCollection.Packages.ToPackageData();

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

        // [SerializeField] private VersionComparer coreUpdateInfo;
        // public VersionComparer CoreUpdateInfo => coreUpdateInfo ??= FetchCoreUpdates();

        // private static VersionComparer FetchCoreUpdates()
        // {
        //     var package = UPMCollection.Get(TwistCore.PackageName);
        //     return GithubVersionControl.FetchUpdates(package);
        // }
    }
}