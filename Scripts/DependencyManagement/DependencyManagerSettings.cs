using System.Collections.Generic;
using UnityEngine;

namespace TwistCore.DependencyManagement
{
    public class DependencyManagerSettings : SettingsAsset
    {
        public bool useCustomManifestURL;
        public bool addPackageEnabled;
        public string manifestURL = "https://raw.githubusercontent.com/twistapps/twistcore/main/package-manifest.json";

        public int editingPackage = -1;
        public string editingPackageName;
        public string editingPackageOrganization;
        public string editingPackageURL;

        public string newPackageName;
        public string newPackageGitURL;
        [SerializeField] public List<string> newPackageDependencies = new List<string>();

        public override string GetEditorWindowTitle()
        {
            return "Dependency Management";
        }

        public override string GetPackageName()
        {
            return "com.twistapps.twistcore";
        }
    }
}