using System.IO;

// ReSharper disable once CheckNamespace
namespace TwistCore
{
    public static class TwistCore
    {
        public const string ManifestFilename = "package-manifest.json";
        public static string PackageName => SettingsUtility.Load<TwistCoreSettings>().GetPackageName();

        public static string NewPackageTemplateFolder => Path.Combine("Packages", PackageName, ".NewPackageTemplate");
        internal static string SettingsFolder => Path.Combine("Assets", "TwistApps", "Resources", "Settings");
    }
}