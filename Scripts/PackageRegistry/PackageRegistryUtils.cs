using System.Collections.Generic;
using System.Linq;
using TwistCore.DependencyManagement;
using UnityEditor.PackageManager;

namespace TwistCore.PackageRegistry
{
    public static class PackageRegistryUtils
    {
        private static List<PackageInfo> _collection;
        public static List<PackageInfo> Collection => _collection ?? LoadCoreDependentPackages();

        public static List<PackageInfo> LoadCoreDependentPackages()
        {
            var namesMask = DependencyManager.Manifest.packages.Select(package => package.name).ToArray();
            var collection = UPMInterface.List();
            var filteredPackages = new List<PackageInfo>();
            foreach (var package in collection)
                filteredPackages.AddRange(from mask in namesMask where package.name == mask select package);
            _collection = filteredPackages;
            return filteredPackages;
        }

        public static PackageInfo Get(string packageName)
        {
            return Collection.FirstOrDefault(pkg => pkg.name == packageName);
        }
    }
}