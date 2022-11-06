using System.IO;
using System.Linq;
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TwistCore
{
    public static class PackageDataExtensions
    {
        public static string Alias(this PackageInfo package)
        {
            return package.name.Split('.').LastOrDefault();
        }

        public static string Alias(this PackageData package)
        {
            return package.name.Split('.').LastOrDefault();
        }

        public static string Asmdef(this PackageInfo package)
        {
            var packageFolder = package.assetPath;
            return Directory.GetFiles(packageFolder)
                .FirstOrDefault(file => file.ToLower().EndsWith(package.Alias() + ".asmdef"));
        }

        public static string Asmdef(this PackageData package)
        {
            var packageFolder = package.assetPath;
            return Directory.GetFiles(packageFolder)
                .FirstOrDefault(file => file.ToLower().EndsWith(package.Alias() + ".asmdef"));
        }

        public static GUID AsmdefGuid(this PackageInfo package)
        {
            return AssetDatabase.GUIDFromAssetPath(Asmdef(package));
        }

        public static GUID AsmdefGuid(this PackageData package)
        {
            return AssetDatabase.GUIDFromAssetPath(Asmdef(package));
        }
    }
}