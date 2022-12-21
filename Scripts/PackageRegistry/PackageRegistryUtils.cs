using System.Collections.Generic;
using System.Linq;
using TwistCore.DependencyManagement;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore.PackageRegistry
{
    public static class PackageRegistryUtils
    {
        private static List<PackageInfo> _collection;
        private static PackageCollection _fullCollection;
        public static List<PackageInfo> Collection => _collection ?? LoadCoreDependentPackages();
        public static PackageCollection FullCollection
        {
            get
            {
                if (_fullCollection == null)
                    LoadCoreDependentPackages();
                return _fullCollection;
            }
        }

        public static List<PackageInfo> LoadCoreDependentPackages()
        {
            var namesMask = DependencyManager.Manifest.packages.Select(package => package.name).ToArray();
            _fullCollection = UPMInterface.List();
            var filteredPackages = new List<PackageInfo>();
            foreach (var package in _fullCollection) filteredPackages.AddRange(from mask in namesMask where package.name == mask select package);
            _collection = filteredPackages;
            return filteredPackages;
        }

        public static void PurgeCollection()
        {
            Debug.Log("Purging package registry collection...");
            _collection = null;
        }

        public static PackageInfo Get(string packageName)
        {
            return Collection.FirstOrDefault(pkg => pkg.name == packageName);
        }

        public static PackageInfo GetFromFullCollection(string packageName)
        {
            return FullCollection.FirstOrDefault(pkg => pkg.name == packageName);
        }

        public static bool IsEmbedded(string packageName)
        {
            var package = Get(packageName);
            return package.source == PackageSource.Embedded;
        }
    }
}