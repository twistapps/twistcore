using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TwistCore.Editor
{
    [Serializable]
    public class Manifest
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

        public bool PackageExists(string name)
        {
            return packages.FirstOrDefault(p => p.name == name) != null;
        }

        public int IndexOf(string name)
        {
            return Array.FindIndex(packages, p => p.name == name);
        }

        public void Save()
        {
            File.WriteAllText(ManifestEditor.ManifestPath, JsonUtility.ToJson(this, true));
        }

        public Package Get(string packageName)
        {
            return packages.FirstOrDefault(pkg => pkg.name == packageName);
        }

        [Serializable]
        public class Package
        {
            public string name;
            public string url;
            public string source; //github || other
            public string scriptingDefineSymbols;
            [SerializeField] public List<string> dependencies = new List<string>();
        }
    }
}