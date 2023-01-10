using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TwistCore.Editor
{
    public class ScriptingDefinesSetter : ScriptableSingleton<ScriptingDefinesSetter>
    {
        [SerializeField] private bool startupInitializationComplete;

        private static BuildTargetGroup BuildTarget => EditorUserBuildSettings.selectedBuildTargetGroup;
        private static string Symbols => PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTarget);

        [InitializeOnLoadMethod]
        private static void RefreshSymbolsOnStartup()
        {
            if (instance.startupInitializationComplete) return;
            RefreshAllSymbols();
            instance.startupInitializationComplete = true;
        }

        public static void RegisteredPackagesEventHandler(PackageRegistrationEventArgs args)
        {
            foreach (var packageInfo in args.added)
            {
                var manifestPackage = ManifestEditor.Manifest.Get(packageInfo.name);
                if (manifestPackage != null)
                    AddSymbols(manifestPackage.scriptingDefineSymbols);
            }

            foreach (var packageInfo in args.removed)
            {
                var manifestPackage = ManifestEditor.Manifest.Get(packageInfo.name);
                if (manifestPackage != null)
                    RemoveSymbols(manifestPackage.scriptingDefineSymbols);
            }
        }

        public static void RefreshAllSymbols()
        {
            var packagesInProject = UPMCollection.Packages;
            foreach (var manifestPackage in ManifestEditor.Manifest.packages)
            {
                if (string.IsNullOrEmpty(manifestPackage.scriptingDefineSymbols))
                    continue;

                bool Match(PackageInfo pkg)
                {
                    return pkg.name == manifestPackage.name;
                }

                var package = packagesInProject.FirstOrDefault(Match);
                if (package == default)
                    RemoveSymbols(manifestPackage.scriptingDefineSymbols);
                else
                    AddSymbols(manifestPackage.scriptingDefineSymbols);
            }
        }

        private static void ModifySymbolsIfDiffers(IEnumerable<string> newSymbols, string input,
            ModifyAction operationType = ModifyAction.Unknown)
        {
            var modifiedSymbols = string.Join(";", newSymbols);
            if (Symbols == modifiedSymbols) return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTarget, modifiedSymbols);

            if (operationType == ModifyAction.Unknown) return;
            Debug.Log($"{operationType.ToString()} '{input}' in scripting defines...");
        }

        public static void AddSymbols(string input)
        {
            if (string.IsNullOrEmpty(input)) return;

            var symbolsHashSet = new HashSet<string>(Symbols.Split(';'));
            symbolsHashSet.UnionWith(input.Split(';'));

            ModifySymbolsIfDiffers(symbolsHashSet, input, ModifyAction.Added);
        }

        public static void RemoveSymbols(string input)
        {
            if (string.IsNullOrEmpty(input)) return;

            var symbolsHashSet = new HashSet<string>(Symbols.Split(';'));
            symbolsHashSet.ExceptWith(input.Split(';'));

            ModifySymbolsIfDiffers(symbolsHashSet, input, ModifyAction.Removed);
        }

        private enum ModifyAction
        {
            Unknown,
            Added,
            Removed
        }
    }
}