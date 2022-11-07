using System.IO;
using TwistCore.Utils;
using UnityEditor.PackageManager;

// ReSharper disable once CheckNamespace
namespace TwistCore
{
    public static class TwistCore
    {
        internal const string ManifestFilename = "package-manifest.json";
        internal static string PackageName => SettingsUtility.Load<TwistCoreSettings>().GetPackageName();

        internal static string PackageTemplateFolder => Path.Combine("Packages", PackageName, ".NewPackageTemplate");
        internal static string SettingsFolder => Path.Combine("Assets", "TwistApps", "Resources", "Settings");

        internal static string ManifestPath => PackageRegistry.Get(PackageName).source == PackageSource.Embedded
            ? Path.Combine("Packages", PackageName, ManifestFilename)
            : Path.Combine("Assets", "TwistApps", "Resources", ManifestFilename);

        internal static string PackageRegistryNameMask =>
            Path.Combine("Packages", PackageName, ".package-registry-mask");
    }
}