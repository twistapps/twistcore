using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore
{
    public static class PackageRegistry
    {
        private static List<PackageInfo> _collection;
        public static List<PackageInfo> Collection => _collection ?? LoadCoreDependentPackages();

        public static List<PackageInfo> LoadCoreDependentPackages()
        {
            var namesMask = File.ReadAllLines(TwistCore.PackageRegistryNameMask);
            var collection = UPMInterface.List();
            var filteredPackages = new List<PackageInfo>();
            foreach (var package in collection)
            {
                foreach (var mask in namesMask)
                {
                    var parts = mask.Split('.');
                    if (parts.Length < 1) //empty line
                        continue;
                    if (parts.Length < 3) //masked by organization name
                    {
                        if (package.name.Split('.')[1] == parts.Last())
                            filteredPackages.Add(package);
                    }
                    else if (package.name == mask) //masked by exact name
                    {
                        filteredPackages.Add(package);
                    }
                }
            }

            // foreach (var package in filteredPackages)
            // {
            //     Debug.Log($"{package.assetPath} | DevMode: {PackagesLock.IsInDevelopmentMode(package.name)}");
            // }

            _collection = filteredPackages;
            return filteredPackages;
        }

        public static PackageInfo Get(string packageName)
        {
            return Collection.FirstOrDefault(pkg => pkg.name == packageName);
        }
    }
}
