using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TwistCore.Editor;
using TwistCore.PackageRegistry.Editor;
using TwistCore.ProgressWindow;
using TwistCore.ProgressWindow.Editor;
using UnityEditor;
using UnityEngine;

namespace TwistCore.PackageDevelopment.Editor
{
    public static class CoreUnpacker
    {
        private const string UnpackedCoreDirName = "TwistCore";
        private const string UnpackIgnore = ".unpackignore";

        public static PackageData GetUnpackedCorePackage(string packageName)
        {
            Debug.Log(packageName);
            //var packageInfo = PackageRegistry.Collection.FirstOrDefault(p => p.name == packageName);
            var packageInfo = PackageLock.GetInfo(packageName);
            if (packageInfo == null) return null;

            if (!PackageLock.IsInDevelopmentMode(packageName)) return null;

            var coreDir = Path.Combine(packageInfo.assetPath, UnpackedCoreDirName);
            if (!Directory.Exists(coreDir)) return null;

            var corePackage = PackageLock.GetInfoByPath(coreDir);
            if (corePackage != null) corePackage.assetPath = coreDir;
            return corePackage;
        }

        public static void UnpackIntoPackageFolder(string packageName, string packageToUnpack,
            string outputDirectoryName)
        {
            UnpackIntoPackageFolderCoroutine(packageName, packageToUnpack, outputDirectoryName)
                .FinishSynchronously();
        }

        public static IEnumerator<TaskProgress> UnpackIntoPackageFolderCoroutine(string packageName,
            string packageToUnpack, string outputDirectoryName)
        {
            // task steps:
            // 0 - find ignored files
            // 1 - copy files
            // 2 - remove excess files from output dir
            // 3 - remove asmdef dependency of core lib
            var progress = new TaskProgress(4);

            if (!PackageLock.IsInDevelopmentMode(packageName)) //requested package is not in development mode 
            {
                Debug.LogError(
                    $"The requested package '{packageName}' is not in development mode so it won't be modified.");
                yield break;
            }

            var core = UPMCollection.Get(packageToUnpack);
            var corePath = core.assetPath;

            Debug.Log(packageName);
            var package = UPMCollection.GetFromAllPackages(packageName);
            var packagePath = package.assetPath;

            var unpackedCorePath = Path.Combine(packagePath, outputDirectoryName);
            Debug.Log($"Unpacked core path: {unpackedCorePath}");

            var ignore = File.ReadAllLines(Path.Combine(corePath, UnpackIgnore));
            var ignoredFiles = new List<string>();

            // if (!PackageRegistryUtils.IsEmbedded(core.name))
            // {
            //     progress.TotalSteps++;
            //     yield return progress.Log("Embedding source pkg...");
            //     UPMInterface.Embed(core.name);
            //     yield return progress.Next("Done embedding").Sleep(1);
            // }

            yield return progress.Log("Filtering ignored files...");

            foreach (var searchPattern in ignore)
            {
                const string exceptKeyword = " !except ";
                var exceptKeywordIndex = searchPattern.LastIndexOf(exceptKeyword, StringComparison.Ordinal);

                var pattern = searchPattern;
                var excludeFromSearch = new List<string>();

                if (exceptKeywordIndex != -1)
                {
                    pattern = searchPattern.Substring(0, exceptKeywordIndex);
                    var keep = searchPattern.Substring(pattern.Length + exceptKeyword.Length).Split('&');
                    foreach (var s in keep)
                        excludeFromSearch.AddRange(Directory.GetFiles(corePath, Path.Combine(s),
                            SearchOption.AllDirectories));
                }

                var found = Directory.GetFiles(corePath, Path.Combine(pattern), SearchOption.AllDirectories);
                found = found.Except(excludeFromSearch).ToArray();

                ignoredFiles.AddRange(found);
                yield return progress;
            }

            yield return progress.Next("Copying files...");

            var files = Directory.EnumerateFiles(corePath, "*.*", SearchOption.AllDirectories);
            var filecount = 60; //todo: count files;
            var secondaryProgress = new TaskProgress(filecount);

            foreach (var sourcePath in files)
            {
                if (ignoredFiles.Contains(sourcePath)) continue;

                var relativePath = FolderSync.MakeRelativePath(sourcePath, corePath);
                var outputPath = Path.Combine(unpackedCorePath, relativePath);
                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (outputDirectory != null && !Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);
                File.Copy(sourcePath, outputPath, true);

                yield return secondaryProgress.Next();
            }

            yield return progress.Next("Source files copied.").Sleep(.5f);
            progress.TotalSteps++;
            yield return progress.Log("Removing excess files");

            var outputFiles = Directory.GetFiles(unpackedCorePath, "*.*", SearchOption.AllDirectories);
            foreach (var unpackedFile in outputFiles)
            {
                var relativePath = FolderSync.MakeRelativePath(unpackedFile, unpackedCorePath);
                var sourcePath = Path.Combine(corePath, relativePath);
                if (!File.Exists(sourcePath)) File.Delete(unpackedFile);
            }

            yield return progress.Next("Redirecting dependencies...");
            RemoveAsmdefDependency(packageName, packageToUnpack);

            //add dependencies of embedded package
            foreach (var dependency in ManifestEditor.Manifest.Get(packageToUnpack).dependencies)
                AddExternalAsmdefDependency(packageName, dependency);
            //AddAsmdefDependency(packageName, dependency);

            yield return progress.Next().Sleep(1);

            yield return progress.Next("Requesting asset db refresh...").Sleep(.5f);
            TaskManager.ExecuteOnCompletion(AssetDatabase.Refresh);
        }

        public static IEnumerator<TaskProgress> RemoveUnpacked(string packageName, string unpackedPackage,
            string outputDirectoryName)
        {
            // task steps:
            // 0 - remove dir
            // 1 - add asmdef dependency
            // 2 - refresh assets
            var progress = new TaskProgress(3);
            yield return progress.Next();

            progress.Log("Removing dir " + outputDirectoryName);
            var package = UPMCollection.GetFromAllPackages(packageName);
            var packagePath = package.assetPath;

            var unpackedCorePath = Path.Combine(packagePath, outputDirectoryName);
            var files = Directory.EnumerateFiles(unpackedCorePath, "*", SearchOption.AllDirectories);

            progress.TotalSteps = 150;
            foreach (var path in files)
            {
                File.Delete(path);
                yield return progress.Next().Sleep(.005f);
            }

            progress.Log("Removing root folder .meta");
            File.Delete(Path.ChangeExtension(unpackedCorePath, ".meta"));

            yield return progress.Sleep(.5f);
            progress.Log("Adding " + unpackedPackage.Split('.').Last() + " ref to asmdef");
            progress.TotalSteps = 3;
            progress.CurrentStep = 2;
            yield return progress;

            AddAsmdefDependency(packageName, unpackedPackage);

            //remove dependencies of embedded package
            foreach (var dependency in ManifestEditor.Manifest.Get(unpackedPackage).dependencies)
                RemoveExternalAsmdefDependency(packageName, dependency);

            progress.Log("Requesting asset db refresh...");
            TaskManager.ExecuteOnCompletion(AssetDatabase.Refresh);
            yield return progress.Next().Sleep(.5f);
        }

        public static void RemoveAsmdefDependency(string packageName, string dependencyName)
        {
            var package = UPMCollection.GetFromAllPackages(packageName);
            var asmdef = package.Asmdef();

            if (!PackageLock.IsInDevelopmentMode(package)) //requested package is not in development mode
                return;

            var dependencyGuid = "GUID:" + UPMCollection.GetFromAllPackages(dependencyName).AsmdefGuid();

            var o = JObject.Parse(File.ReadAllText(asmdef));
            var references = o["references"]?.Values<string>().ToList() ?? new List<string>();

            var index = references.IndexOf(dependencyGuid);
            if (index != -1) references.RemoveAt(index);

            o["references"] = new JArray(references);
            var json = o.ToString();

            File.WriteAllText(asmdef, json);
        }

        private static string GetExternalPackageGuid(string packageName)
        {
            var alias = UPMCollection.GetFromAllPackages(packageName).Alias();

            var files = Directory.GetFiles(Path.Combine("Packages", packageName), "*.asmdef",
                SearchOption.AllDirectories);
            if (files.Length < 1) return null;
            var guid = AssetDatabase.GUIDFromAssetPath(files[0]);
            return "GUID:" + guid;
        }

        public static void AddExternalAsmdefDependency(string packageName, string dependencyName)
        {
            var package = UPMCollection.GetFromAllPackages(packageName);

            if (!PackageLock.IsInDevelopmentMode(package)) //requested package is not in development mode
                return;

            var dependencyGuid = GetExternalPackageGuid(dependencyName);

            var asmdef = package.Asmdef();
            var o = JObject.Parse(File.ReadAllText(asmdef));
            var references = o["references"]?.Values<string>().ToList() ?? new List<string>();

            if (references.IndexOf(dependencyGuid) != -1) return;
            references.Add(dependencyGuid);

            o["references"] = new JArray(references);
            File.WriteAllText(asmdef, o.ToString());
        }

        public static void RemoveExternalAsmdefDependency(string packageName, string dependencyName)
        {
            var package = UPMCollection.GetFromAllPackages(packageName);
            var asmdef = package.Asmdef();

            if (!PackageLock.IsInDevelopmentMode(package)) //requested package is not in development mode
                return;

            var dependencyGuid = GetExternalPackageGuid(dependencyName);

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
            var package = UPMCollection.GetFromAllPackages(packageName);

            if (!PackageLock.IsInDevelopmentMode(package)) //requested package is not in development mode
                return;

            var asmdef = package.Asmdef();
            var o = JObject.Parse(File.ReadAllText(asmdef));
            var references = o["references"]?.Values<string>().ToList() ?? new List<string>();

            var dependencyGuid = "GUID:" + UPMCollection.GetFromAllPackages(dependencyName).AsmdefGuid();

            //CompilationPipeline.GUIDToAssemblyDefinitionReferenceGUID()

            if (references.IndexOf(dependencyGuid) != -1) return;
            references.Add(dependencyGuid);

            o["references"] = new JArray(references);
            File.WriteAllText(asmdef, o.ToString());
        }
    }
}