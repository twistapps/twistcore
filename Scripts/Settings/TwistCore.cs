using System.IO;
using TwistCore.Utils;

// ReSharper disable once CheckNamespace
namespace TwistCore
{
    public static class TwistCore
    {
        internal static string PackageName => SettingsUtility.Load<TwistCoreSettings>().GetPackageName();

        internal static string PackageTemplateFolder => Path.Combine("Packages", PackageName, ".NewPackageTemplate");

        internal static string PackageRegistryNameMask =>
            Path.Combine("Packages", PackageName, ".package-registry-mask");
    }
}