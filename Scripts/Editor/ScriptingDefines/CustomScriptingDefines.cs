using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TwistCore.Editor
{
    public class CustomScriptingDefines
    {
        private static bool _initialized;

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly HashSet<string> ToAdd = new HashSet<string>();
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly HashSet<string> ToRemove = new HashSet<string>();

        private void Test(string symbol, bool condition)
        {
            if (condition)
                ToAdd.Add(symbol);
            else
                ToRemove.Add(symbol);
        }

        public static CustomScriptingDefines GetAll()
        {
            var defines = new CustomScriptingDefines();

            var conditionalDefines = EditorUtils.GetDerivedTypesExcludingSelf<ConditionalDefineSymbols>();
            foreach (var type in conditionalDefines)
            {
                var instance = Activator.CreateInstance(type) as ConditionalDefineSymbols;
                defines.Test(instance!.GetSymbols(), instance.ShouldSetDefines());
            }

            return defines;
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            if (_initialized) return;
            _initialized = true;
            
            SetAll();
            
            // This causes the method to be invoked after the Editor registers the new list of packages.
            Events.registeringPackages += OnRegisteringPackages;
        }
        
        public static void OnRegisteringPackages(PackageRegistrationEventArgs args)
        {
            var conditionalDefines = EditorUtils.GetDerivedTypesExcludingSelf<ConditionalDefineSymbols>();
            
            foreach (var type in conditionalDefines)
            {
                var name = type.GetCustomAttribute<PackageNameAttribute>()?.PackageName;
                if (string.IsNullOrEmpty(name)) continue;
                
                bool Match(PackageInfo pkg) => pkg.name == name;
                string GetSymbols() => ((ConditionalDefineSymbols)Activator.CreateInstance(type)).GetSymbols();
                
                if (args.removed.FirstOrDefault(Match) != null)
                    ScriptingDefinesSetter.RemoveSymbols(GetSymbols());
            
                else if (args.added.FirstOrDefault(Match) != null)
                    ScriptingDefinesSetter.AddSymbols(GetSymbols());
            }
        }

        public static void SetAll()
        {
            var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            var symbolsBefore = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
            
            var defines = GetAll();

            foreach (var symbol in defines.ToAdd) ScriptingDefinesSetter.AddSymbols(symbol);
            foreach (var symbol in defines.ToRemove) ScriptingDefinesSetter.RemoveSymbols(symbol);

            var symbolsAfter = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
            if (symbolsBefore != symbolsAfter)
                CompilationPipeline.RequestScriptCompilation();
        }
    }
}