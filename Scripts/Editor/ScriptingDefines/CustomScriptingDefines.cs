using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;

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
            SetAll();
            _initialized = true;
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