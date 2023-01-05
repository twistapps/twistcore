using System.IO;
using System.Linq;
using TwistCore.CodeGen.Editor;
using TwistCore.Editor;
using UnityEditor.PackageManager;

namespace TwistCore.PackageDevelopment.Editor
{
    public static class PackageCreationTool
    {
        private const char Separator = CodeGenBuilder.SeparatorSymbol;

        private static readonly string[] CodeGenWhitelist =
        {
            "LICENSE",
            $"{Separator}PACKAGE{Separator}.asmdef",
            "package.json",

            $"{Separator}NAME{Separator}Settings.cs",
            $"{Separator}NAME{Separator}SettingsWindow.cs"
        };

        private static TwistCoreSettings Settings => SettingsUtility.Load<TwistCoreSettings>();

        internal static void CreatePackageBasedOnSettings()
        {
            var newPackageFolder = Path.Combine("Packages", Settings.newPackageName.ToLower());
            Directory.CreateDirectory(newPackageFolder);
            GitCmd.ExecuteCommand(newPackageFolder, "init");

            var files = Directory
                .EnumerateFiles(global::TwistCore.TwistCore.NewPackageTemplateFolder, "*.*", SearchOption.AllDirectories)
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

            Client.Resolve();
        }

        private static string GetCurrentDirRelativeToPackageRoot(string path)
        {
            var directory =
                path.Substring(0, path.Length - Path.GetFileName(path).Length - 1); //Get current file's directory
            directory = FolderSync.MakeRelativePath(directory,
                global::TwistCore.TwistCore.NewPackageTemplateFolder); //Trim file path relative to template folder as root dir
            return directory;
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