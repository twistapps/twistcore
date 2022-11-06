using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace TwistCore
{
    [Serializable]
    public class DependencyManifest
    {
        public Package[] packages = Array.Empty<Package>();

        public void AddPackage(Package package)
        {
            var temp = packages?.ToList() ?? new List<Package>();
            temp.Add(package);
            packages = temp.ToArray();
            Save();
        }

        public void RemovePackage(string name)
        {
            packages = packages.Where(p => p.name != name).ToArray();
            Save();
        }

        public void EditPackage(int index, Package package)
        {
            packages[index] = package;
            Save();
        }

        public void Save()
        {
            File.WriteAllText(TwistCore.ManifestPath, JsonUtility.ToJson(this));
        }

        [Serializable]
        public class Package
        {
            public string name;
            public string url;
            public string source; //github || other
        }
    }
}