using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TwistCore
{
    public class PersistentEditorData : ScriptableSingleton<PersistentEditorData>
    {
        [SerializeField] private bool gitInitialized;
        [SerializeField] private bool gitAvailable;
        [SerializeField] private string gitVersion;

        [SerializeField] private PackageData[] packagesInProject;

        public static IEnumerable<PackageData> PackagesInProject => instance.packagesInProject ??=
            DependencyManager.Manifest.packages.Select(package => (PackageData)PackageRegistry.Get(package.name)).ToArray();

        [SerializeField] private VersionComparison _coreUpdateInfo;
        public VersionComparison CoreUpdateInfo => _coreUpdateInfo ??= FetchCoreUpdates();

        public bool GitAvailable => gitInitialized ? gitAvailable : InitializeGit() != null;
        public string GitVersion => gitInitialized ? gitVersion : InitializeGit();

        private static VersionComparison FetchCoreUpdates()
        {
            var package = PackageRegistry.Get(TwistCore.PackageName);
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