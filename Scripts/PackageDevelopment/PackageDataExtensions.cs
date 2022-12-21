using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TwistCore.PackageDevelopment
{
    public static class PackageDataExtensions
    {
        private const string EmptyGuid = "GUID:00000000000000000000000000000000";
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
            var files = Directory.GetFiles(packageFolder);
            var asmdef = files.FirstOrDefault(file => file.ToLower().EndsWith(package.Alias() + ".asmdef"));
            if (!string.IsNullOrEmpty(asmdef)) return asmdef;
            Debug.Log($"Looking for {package.Alias()}");
            foreach (var assembly in CompilationPipeline.GetAssemblies())
            {
                if (assembly.name.ToLower().Contains(package.Alias()))
                    return assembly.outputPath;
            }
            Debug.Log($"{package.Alias()} not found");

            return null;
        }

        public static string Asmdef(this PackageData package)
        {
            var packageFolder = package.assetPath;
            return Directory.GetFiles(packageFolder)
                .FirstOrDefault(file => file.ToLower().EndsWith(package.Alias() + ".asmdef"));
        }

        public static string AsmdefGuid(this PackageInfo package)
        {
            Debug.Log($"[asmdef]{package.name}: {Asmdef(package)} -- {AssetDatabase.AssetPathToGUID(Asmdef(package))}");
            return AssetDatabase.AssetPathToGUID(Asmdef(package));
            //return AssetDatabase.GUIDFromAssetPath(Asmdef(package));
        }

        public static GUID AsmdefGuid(this PackageData package)
        {
            return AssetDatabase.GUIDFromAssetPath(Asmdef(package));
        }
    }
}