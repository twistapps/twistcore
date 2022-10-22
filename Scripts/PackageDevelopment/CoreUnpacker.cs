using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace TwistCore
{
    public static class CoreUnpacker
    {
        private const string UnpackedCoreDirName = "TwistCore";
        private const string UnpackIgnore = ".unpackignore";
        
        public static PackageData GetUnpackedCorePackage(string packageName)
        {
            var packageInfo = PackageRegistry.Collection.FirstOrDefault(p => p.name == packageName);
            if (packageInfo == null) return null;
            
            if (!PackagesLock.IsInDevelopmentMode(packageName)) return null;

            var coreDir = Path.Combine(packageInfo.assetPath, UnpackedCoreDirName);
            if (!Directory.Exists(coreDir)) return null;
            
            var corePackage = PackagesLock.GetInfoByPath(coreDir);
            if (corePackage != null) corePackage.assetPath = coreDir;
            return corePackage;
        }

        public static void UnpackIntoPackageFolder(string packageName)
        {
            if (!PackagesLock.IsInDevelopmentMode(packageName)) //requested package is not in development mode
                return;

            var core = PackageRegistry.Get(TwistCore.PackageName);
            var corePath = core.assetPath;

            var package = PackageRegistry.Get(packageName);
            var packagePath = package.assetPath;

            var unpackedCorePath = Path.Combine(packagePath, UnpackedCoreDirName);

            var ignore = File.ReadAllLines(Path.Combine(corePath, UnpackIgnore));
            var ignoredFiles = new List<string>();
            foreach (var searchPattern in ignore)
            {
                ignoredFiles.AddRange(Directory.GetFiles(corePath, searchPattern, SearchOption.AllDirectories));
                
            }
            
            var files = Directory.EnumerateFiles(corePath, "*.*", SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                if (ignoredFiles.Contains(filePath)) continue;
                
            }

            var outputFiles = Directory.GetFiles(unpackedCorePath, "*.*", SearchOption.AllDirectories);
            foreach (var unpackedFile in outputFiles)
            {
                
            }
        }

        public static void RebindAsmdefDependencies(string packageName)
        {
            var package = PackageRegistry.Get(packageName);
            var asmdef = package.Asmdef();
            
            if (!PackagesLock.IsInDevelopmentMode(package)) //requested package is not in development mode
                return;

            var core = PackageRegistry.Get(TwistCore.PackageName);
            var coreGuid = core.AsmdefGuid();

            var unpackedCore = GetUnpackedCorePackage(package.name);
            var unpackedCoreGuid = unpackedCore.AsmdefGuid();

            var oldGuidReference = "GUID:" + coreGuid;
            var newGuidReference = "GUID:" + unpackedCoreGuid;

            var o = JObject.Parse(File.ReadAllText(asmdef));
            var references = o["references"]?.Values<string>().ToList() ?? new List<string> {newGuidReference};
            var index = references.IndexOf(oldGuidReference);
            if (index != -1) references[index] = newGuidReference;

            o["references"] = new JArray(references);
            var json = o.ToString();
            
            File.WriteAllText(asmdef, json);
        }
    }
}