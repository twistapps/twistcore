using System.IO;
using System.Linq;
using RequestForMirror.Editor;
using TwistCore.Utils;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore
{
    public static class PackageCreationTool
    {
        private static TwistCoreSettings Settings => SettingsUtility.Load<TwistCoreSettings>();
        private const char Separator = CodeGenBuilder.SeparatorSymbol;

        private static readonly string[] CodeGenWhitelist = new[]
        {
            "LICENSE",
            $"{Separator}PACKAGE{Separator}.asmdef",
            "package.json",
            
            $"{Separator}NAME{Separator}Settings.cs",
            $"{Separator}NAME{Separator}SettingsWindow.cs",
        };

        internal static void CreatePackage()
        {
            var newPackageFolder = Path.Combine("Packages", Settings.newPackageName.ToLower());
            Directory.CreateDirectory(newPackageFolder);
            GitCmd.ExecuteCommand(newPackageFolder, "init");
            //var files = Directory.GetFiles(TwistCoreDefinitions.PackageTemplateFolder);

            var files = Directory
                .EnumerateFiles(TwistCore.PackageTemplateFolder, "*.*", SearchOption.AllDirectories)
                .ToArray();
            
            var builder = CreateCodeGenBuilder();
            foreach (var path in files)
            {
                var filename = Path.GetFileName(path);
                var outputFilename = filename;
                if (CodeGenWhitelist.Contains(filename))
                {
                    builder.GenerateFromTemplate(path);
                    foreach (var variableName in builder.VariableNames)
                    {
                        var inlineVariable =
                            Separator + variableName + Separator;
                        if (filename.Contains(inlineVariable))
                            outputFilename = filename.Replace(inlineVariable, builder.GetVariable(variableName));
                    }

                    var currentRelativeDir = GetCurrentDirRelativeToPackageRoot(path);
                    builder.SaveToFile(Path.Combine(newPackageFolder, currentRelativeDir, outputFilename));
                }
                else
                {
                    var currentRelativeDir = GetCurrentDirRelativeToPackageRoot(path);
                    var dir = Path.Combine(newPackageFolder, currentRelativeDir);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    File.Copy(path, Path.Combine(dir, filename));
                }
            }
            
            //AssetDatabase.Refresh();
            // CompilationPipeline.RequestScriptCompilation();
            Client.Resolve();
        }

        private static string GetCurrentDirRelativeToPackageRoot(string path)
        {
            var directory = path.Substring(0, path.Length-Path.GetFileName(path).Length-1); //Get current file's directory
            directory = TrimRoot(directory, TwistCore.PackageTemplateFolder); //Trim file path relative to template folder as root dir
            return directory;
        }
        
        /// <summary>
        /// Trims path to be relative to specified rootDirectory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newRootPath">New root directory. Has to be a part of initial path.</param>
        /// <returns></returns>
        private static string TrimRoot(string path, string newRootPath)
        {
            var newRoot = path.Substring(newRootPath.Length);
            return newRoot.StartsWith(Path.DirectorySeparatorChar.ToString()) ? newRoot.Substring(1) : newRoot;
        }

        private static CodeGenTemplateBuilder CreateCodeGenBuilder()
        {
            var builder = new CodeGenTemplateBuilder();
            var packageName = Settings.newPackageName;
            builder.SetVariable("NAME", packageName);
            builder.SetVariable("NAME_LOWERCASE", packageName.ToLower());
            builder.SetVariable("PACKAGE", packageName);
            builder.SetVariable("DISPLAYNAME", Settings.newPackageDisplayname);
            builder.SetVariable("DESCRIPTION", Settings.newPackageDescription);
            builder.SetVariable("VERSION", Settings.newPackageVersion);
            builder.SetVariable("ORGANIZATION", Settings.newPackageOrganizationName);
            return builder;
        }
    }
}