﻿using System;
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
            Debug.Log(packageName);
            //var packageInfo = PackageRegistry.Collection.FirstOrDefault(p => p.name == packageName);
            var packageInfo = PackagesLock.GetInfo(packageName);
            if (packageInfo == null) return null;
            
            if (!PackagesLock.IsInDevelopmentMode(packageName)) return null;

            var coreDir = Path.Combine(packageInfo.assetPath, UnpackedCoreDirName);
            if (!Directory.Exists(coreDir)) return null;
            
            var corePackage = PackagesLock.GetInfoByPath(coreDir);
            if (corePackage != null) corePackage.assetPath = coreDir;
            return corePackage;
        }

        public static void UnpackIntoPackageFolder(string packageName, string packageToUnpack, string outputDirectoryName)
        {
            if (!PackagesLock.IsInDevelopmentMode(packageName)) //requested package is not in development mode 
            {
                Debug.LogError($"The requested package '{packageName}' is not in development mode so it won't be modified.");
                return;
            }

            var core = PackageRegistry.Get(packageToUnpack);
            var corePath = core.assetPath;

            var package = PackageRegistry.Get(packageName);
            var packagePath = package.assetPath;

            var unpackedCorePath = Path.Combine(packagePath, outputDirectoryName);

            var ignore = File.ReadAllLines(Path.Combine(corePath, UnpackIgnore));
            var ignoredFiles = new List<string>();
            foreach (var searchPattern in ignore)
            {
                const string exceptKeyword = " !except ";
                var exceptKeywordIndex = searchPattern.LastIndexOf(exceptKeyword, StringComparison.Ordinal);

                var pattern = searchPattern;
                var excludeFromSearch = Array.Empty<string>();
                
                if (exceptKeywordIndex != -1)
                {
                    pattern = searchPattern.Substring(0, exceptKeywordIndex);
                    var keep = searchPattern.Substring(pattern.Length + exceptKeyword.Length);
                    excludeFromSearch = Directory.GetFiles(corePath, Path.Combine(keep), SearchOption.AllDirectories);
                }
                
                var found = Directory.GetFiles(corePath, Path.Combine(pattern), SearchOption.AllDirectories);
                found = found.Except(excludeFromSearch).ToArray();
                
                ignoredFiles.AddRange(found);
            }
            
            var files = Directory.EnumerateFiles(corePath, "*.*", SearchOption.AllDirectories);
            foreach (var sourcePath in files)
            {
                if (ignoredFiles.Contains(sourcePath)) continue;
                
                var relativePath = PackageCreationTool.TrimRoot(sourcePath, corePath);
                var outputPath = Path.Combine(unpackedCorePath, relativePath);
                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (outputDirectory != null && !Directory.Exists(outputDirectory))
                        Directory.CreateDirectory(outputDirectory);
                File.Copy(sourcePath, outputPath, true);
            }

            var outputFiles = Directory.GetFiles(unpackedCorePath, "*.*", SearchOption.AllDirectories);
            foreach (var unpackedFile in outputFiles)
            {
                var relativePath = PackageCreationTool.TrimRoot(unpackedFile, unpackedCorePath);
                var sourcePath = Path.Combine(corePath, relativePath);
                if (!File.Exists(sourcePath)) File.Delete(unpackedFile);
            }
            
            RemoveAsmdefDependency(packageName, packageToUnpack);
            AssetDatabase.Refresh();
        }

        public static void RemoveUnpacked(string packageName, string unpackedPackage, string outputDirectoryName)
        {
            
        }

        public static void RemoveAsmdefDependency(string packageName, string dependencyName)
        {
            var package = PackageRegistry.Get(packageName);
            var asmdef = package.Asmdef();

            if (!PackagesLock.IsInDevelopmentMode(package)) //requested package is not in development mode
                return;

            var dependencyGuid = "GUID:" + PackageRegistry.Get(dependencyName).AsmdefGuid();
            
            var o = JObject.Parse(File.ReadAllText(asmdef));
            var references = o["references"]?.Values<string>().ToList() ?? new List<string>();
            
            var index = references.IndexOf(dependencyGuid);
            if (index != -1) references.RemoveAt(index);
            
            o["references"] = new JArray(references);
            var json = o.ToString();
            
            File.WriteAllText(asmdef, json);
        }

        public static void AddAsmdefDependency(string packageName, string dependencyName)
        {
            var package = PackageRegistry.Get(packageName);
            
            if (!PackagesLock.IsInDevelopmentMode(package)) //requested package is not in development mode
                return;

            var asmdef = package.Asmdef();
            var o = JObject.Parse(File.ReadAllText(asmdef));
            var references = o["references"]?.Values<string>().ToList() ?? new List<string>();
            
            var dependencyGuid = "GUID:" + PackageRegistry.Get(dependencyName).AsmdefGuid();
            if (references.IndexOf(dependencyGuid) != -1) return;
            references.Add(dependencyGuid);
            
            o["references"] = new JArray(references);
            File.WriteAllText(asmdef, o.ToString());
        }
    }
}