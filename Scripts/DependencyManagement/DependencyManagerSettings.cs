namespace TwistCore.DependencyManagement
{
    public class DependencyManagerSettings : SettingsAsset
    {
        public bool useCustomManifestURL;
        public string manifestURL = "https://raw.githubusercontent.com/twistapps/twistcore/main/package-manifest.json";

        public string newPackageName;
        public string newPackageGitURL;

        public int editingPackage = -1;
        public string editingPackageName;
        public string editingPackageOrganization;
        public string editingPackageURL;

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