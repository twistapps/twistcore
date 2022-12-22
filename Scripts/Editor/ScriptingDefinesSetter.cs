using System;
using System.Collections.Generic;
using System.Linq;
using TwistCore.DependencyManagement;
using TwistCore.PackageRegistry;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;

namespace TwistCore.Editor
{
    public class ScriptingDefinesSetter : ScriptableSingleton<ScriptingDefinesSetter>
    {
        [SerializeField] private bool startupInitializationComplete = false;
        
        [InitializeOnLoadMethod]
        private static void RefreshSymbolsOnStartup()
        {
            if (instance.startupInitializationComplete) return;
            RefreshAllSymbols();
            instance.startupInitializationComplete = true;
        }
        
        [InitializeOnLoadMethod]
        private static void SubscribeToEvent()
        {
            // This causes the method to be invoked after the Editor registers the new list of packages.
            Events.registeredPackages += RegisteredPackagesEventHandler;
        }

        private static void RegisteredPackagesEventHandler(PackageRegistrationEventArgs packageRegistrationEventArgs)
        {
            foreach (var packageInfo in packageRegistrationEventArgs.added)
            {
                var manifestPackage = DependencyManager.Manifest.Get(packageInfo.name);
                if (manifestPackage != null)
                    AddSymbols(manifestPackage.scriptingDefineSymbols);
            }

            foreach (var packageInfo in packageRegistrationEventArgs.removed)
            {
                var manifestPackage = DependencyManager.Manifest.Get(packageInfo.name);
                if (manifestPackage != null)
                    RemoveSymbols(manifestPackage.scriptingDefineSymbols);
            }
        }
        
        private static void RefreshAllSymbols()
        {
            var packagesInProject = PackageRegistryUtils.Collection;
            foreach (var manifestPackage in DependencyManager.Manifest.packages)
            {
                if (string.IsNullOrEmpty(manifestPackage.scriptingDefineSymbols))
                    continue;
                var package = packagesInProject.FirstOrDefault(pkg => pkg.name == manifestPackage.name);
                if (package == default)
                {
                    RemoveSymbols(manifestPackage.scriptingDefineSymbols);
                }
                else
                {
                    AddSymbols(manifestPackage.scriptingDefineSymbols);
                }
            }
        }
        
        public static void AddSymbols(string entry)
        {
            var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;

            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
            var symbolsHashSet = new HashSet<string>(symbols.Split(';'));
            
            symbolsHashSet.UnionWith(entry.Split(';'));

            var modifiedSymbols = string.Join(";", symbolsHashSet);
            if (symbols == modifiedSymbols) return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, modifiedSymbols);
            Debug.Log($"Added '{entry}' to scripting defines...");
        }

        public static void RemoveSymbols(string entry)
        {
            var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;

            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
            var symbolsHashSet = new HashSet<string>(symbols.Split(';'));

            symbolsHashSet.ExceptWith(entry.Split(';'));

            var modifiedSymbols = string.Join(";", symbolsHashSet);
            if (symbols == modifiedSymbols) return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, modifiedSymbols);
            Debug.Log($"Removed '{entry}' from scripting defines...");
        }

        public static void ReplaceSymbols(string oldSymbols, string newSymbols)
        {
            RemoveSymbols(oldSymbols);
            AddSymbols(newSymbols);
        }
    }
}