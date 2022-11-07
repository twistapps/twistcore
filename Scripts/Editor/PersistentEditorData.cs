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

        [SerializeField] private PackageData[] packagesInProject;

        [SerializeField] private VersionComparison coreUpdateInfo;

        public static IEnumerable<PackageData> PackagesInProject => instance.packagesInProject ??=
            DependencyManager.Manifest.packages
                .Select(package => (PackageData)PackageRegistryUtils.Get(package.name)).Where(p => p != null).ToArray();

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
    }
}