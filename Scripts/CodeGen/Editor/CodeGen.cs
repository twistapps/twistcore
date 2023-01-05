using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwistCore.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace TwistCore.CodeGen.Editor
{
    public static class CodeGen
    {
        public delegate void BeforeCsFileGeneration(CodeGenTemplateBuilder builder, Type type);

        public delegate bool ShouldGenerateCsCheck(Type type);

        private static CodeGenSettings _settings;

        public static BeforeCsFileGeneration OnBeforeCsFileGeneration;
        public static ShouldGenerateCsCheck ShouldGenerateCs;
        private static CodeGenSettings Settings => _settings ??= SettingsUtility.Load<CodeGenSettings>();

        public static string FindTxtTemplate(Type type)
        {
            var parts = type.BaseType.Name.Split('`');
            var parentClassName = parts.FirstOrDefault();
            var hasGenericTypes = int.TryParse(parts.LastOrDefault(), out var genericArgsAmount);

            var genericSpecificTemplate =
                GetTxtPath(CodeGenDefinitions.TemplatesFolder, $"{parentClassName}`{genericArgsAmount}");

            var basicTemplateForClass =
                GetTxtPath(CodeGenDefinitions.TemplatesFolder, parentClassName ?? CodeGenDefinitions.DefaultTemplate);

            if (hasGenericTypes && genericArgsAmount > 0 && File.Exists(genericSpecificTemplate))
                return genericSpecificTemplate;
            return basicTemplateForClass;
        }

        private static string GetOutputCsPath(Type type)
        {
            return Path.ChangeExtension(Path.Combine(CodeGenDefinitions.GeneratedFolder, type.Name), ".cs");
        }

        private static string GetTxtPath(params string[] pathParts)
        {
            return Path.ChangeExtension(Path.Combine(pathParts), ".txt");
        }

        [DidReloadScripts(1)]
        private static void OnScriptsReloaded()
        {
            if (Settings.autoGenerateOnCompile)
                GenerateScripts();
        }

        /// <summary>
        ///     Find types in assembly that should be complemented with generated code.
        /// </summary>
        /// <returns>Array of types derived from any abstract class implementing IMarkedForCodegen.</returns>
        public static IEnumerable<Type> GetTypes()
        {
            //string baseType = "Fetch`1";
            return EditorUtils.GetDerivedFrom<IMarkedForCodeGen>(typeof(IMarkedForCodeGen))
                .Where(type => !type.Name.Contains('`') && !type.IsAbstract);
        }

        private static void AddPartialModifierToClassDefinition(string typeName)
        {
            var guids = AssetDatabase.FindAssets(typeName);
            var scriptFilePaths = guids.Select(AssetDatabase.GUIDToAssetPath);
            foreach (var path in scriptFilePaths)
            {
                var modified = false;
                var lines = File.ReadAllLines(path);
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (line == string.Empty) continue;
                    var isClassDefinition = line.Contains("class " + typeName);
                    if (!isClassDefinition) continue;
                    if (line.Contains("partial")) break;

                    var insertionIndex = line.IndexOf("class", StringComparison.Ordinal);
                    lines[i] = line.Insert(insertionIndex, "partial ");
                    modified = true;
                    break;
                }

                if (!modified) continue;

                File.WriteAllLines(path, lines);
                if (Settings.debugMode) Debug.Log($"Adding 'partial' modifier to {path}");
            }
        }

        private static bool ShouldGenerateCsForType(Type type)
        {
            var shouldGenerateChecks = ShouldGenerateCs?.GetInvocationList().Cast<ShouldGenerateCsCheck>();
            return shouldGenerateChecks?.All(shouldGenerateCheck => shouldGenerateCheck(type)) ?? true;
        }

        private static bool ShouldGenerateCsForType(string className)
        {
            var type = GetTypes().FirstOrDefault(type => type.Name == className);
            return ShouldGenerateCsForType(type);
        }

        public static void GenerateScripts(bool forceRegenerateExisting = false)
        {
            var types = GetTypes();
            var builder = new CodeGenTemplateBuilder();

            foreach (var type in types)
            {
                if (!ShouldGenerateCsForType(type)) continue;
                var outputPath = GetOutputCsPath(type);
                var generatedFileIsRegistered = Settings.generatedFiles?.Contains(outputPath) ?? false;
                if (!forceRegenerateExisting
                    && generatedFileIsRegistered
                    && File.Exists(outputPath))
                {
                    if (Settings.debugMode)
                        Debug.Log($"Skipping {outputPath} because it has already been generated previously.");
                    continue;
                }

                var templatePath = FindTxtTemplate(type);
                builder.SetVariablesForType(type);
                OnBeforeCsFileGeneration?.Invoke(builder, type);
                builder.GenerateFromTemplate(templatePath);

                const string autoRefresh = "kAutoRefresh";
                var autoRefreshState = EditorPrefs.GetInt(autoRefresh);
                EditorPrefs.SetInt(autoRefresh, 0);
                builder.SaveToCsFile(outputPath);
                AddPartialModifierToClassDefinition(type.Name);
                EditorPrefs.SetInt(autoRefresh, autoRefreshState);

                if (!generatedFileIsRegistered)
                    Settings.generatedFiles!.Add(outputPath);
            }

            CleanupFolder();
        }

        private static bool TypeIsMarkedWithInterface(string className)
        {
            return GetTypes().FirstOrDefault(type => type.Name == className) != null;
        }

        private static void CleanupFolder()
        {
            if (!Directory.Exists(CodeGenDefinitions.GeneratedFolder)) return;
            var files = Directory.GetFiles(CodeGenDefinitions.GeneratedFolder, "*.cs");
            foreach (var file in files)
            {
                var className = Path.GetFileNameWithoutExtension(file);

                // if original class that was using IMarkedForCodeGen interface has been deleted,
                // remove the auto-generated code too
                if (TypeIsMarkedWithInterface(className) && ShouldGenerateCsForType(className))
                    continue;

                File.Delete(file);
                if (Settings.generatedFiles.Contains(file))
                    Settings.generatedFiles.RemoveAll(entry => entry == file);
            }
        }
    }
}