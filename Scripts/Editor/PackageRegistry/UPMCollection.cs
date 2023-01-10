using System;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore.Editor
{
    public static class UPMCollection
    {
        private static PackageInfo[] _packages;
        private static PackageCollection _allPackages;

        public static Action CachePurgedEvent;
        public static PackageInfo[] Packages => _packages ?? ListPackages();
        public static PackageCollection AllPackages => _allPackages ?? ListAllPackages();

        public static PackageInfo[] ListPackages()
        {
            _allPackages = UPMInterface.List();

            _packages = ManifestEditor.Manifest.packages
                .Select(FindExistingPackageInfo)
                .Where(p => p != null)
                .ToArray();

            return _packages;
        }

        private static PackageCollection ListAllPackages()
        {
            ListPackages();
            return _allPackages;
        }


        public static void RegisteredPackagesEventHandler(PackageRegistrationEventArgs packageRegistrationEventArgs)
        {
            PurgeCache();
        }

        public static void PurgeCache()
        {
            Debug.Log("Purging package registry collection...");
            _packages = null;
            _allPackages = null;
            CachePurgedEvent.Invoke();
        }

        public static PackageInfo Get(string packageName)
        {
            return Packages.FirstOrDefault(pkg => pkg.name == packageName);
        }

        public static PackageInfo GetFromAllPackages(string packageName)
        {
            return AllPackages.FirstOrDefault(pkg => pkg.name == packageName);
        }

        private static PackageInfo FindExistingPackageInfo(Manifest.Package manifestPackage)
        {
            return _allPackages.FirstOrDefault(package => package.name == manifestPackage.name);
        }

        public static bool IsEmbedded(string packageName)
        {
            var package = Get(packageName);
            return package.source == PackageSource.Embedded;
        }
    }
}