using System.IO;
using System.Linq;
using TwistCore.Editor;
using TwistCore.PackageRegistry.Editor;
using TwistCore.ProgressWindow.Editor;
using UnityEditor;

namespace TwistCore.CodeGen.Editor
{
    public static class TemplateFoldersWatcher
    {
        [MenuItem("Tools/Twist Apps/Commands/Sync CodeGen Templates")]
        public static void SyncTemplateFoldersFromAllRegisteredPackages()
        {
            const string templateFolderSlug = "ScriptTemplates";
            var sourceFolders = UPMCollection.Packages.Select(
                    package => Path.Combine(package.assetPath, templateFolderSlug))
                .Where(Directory.Exists)
                .ToArray();

            TaskManager.Enqueue(
                FolderSync.SyncFoldersTask(CodeGenDefinitions.TemplatesFolder, true, sourceFolders),
                "Gathering templates");
        }
    }
}