using System.Collections.Generic;
using System.Linq;
using TwistCore.DependencyManagement;
using TwistCore.PackageDevelopment;
using TwistCore.PackageRegistry;
using TwistCore.PackageRegistry.Versioning;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public class PersistentEditorData : ScriptableSingleton<PersistentEditorData>
    {
        [SerializeField] private bool gitInitialized;
        [SerializeField] private bool gitAvailable;
        [SerializeField] private string gitVersion;

        [SerializeField] private VersionComparison coreUpdateInfo;

        public VersionComparison CoreUpdateInfo => coreUpdateInfo ??= FetchCoreUpdates();

        public bool GitAvailable => gitInitialized ? gitAvailable : InitializeGit() != null;
        public string GitVersion => gitInitialized ? gitVersion : InitializeGit();

        private static VersionComparison FetchCoreUpdates()
        {
            var package = PackageRegistryUtils.Get(TwistCore.PackageName);
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
            instance.packagesInProject ??= ListPackagesInProject().ToArray();

        public static IEnumerable<PackageData> ListPackagesInProject()
        {
            instance.packagesInProject = DependencyManager.Manifest.packages
                .Select(package => (PackageData)PackageRegistryUtils.Get(package.name))
                .Where(p => p != null)
                .ToArray();

            return instance.packagesInProject;
        }

        public static void PurgePackagesInProjectCache()
        {
            instance.packagesInProject = null;
            PackageRegistryUtils.LoadCoreDependentPackages();
        }

        #endregion
    }
}