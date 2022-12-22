using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwistCore.CodeGen.Editor;
using TwistCore.DependencyManagement;
using TwistCore.PackageRegistry;
using TwistCore.ProgressWindow.Editor;
using UnityEditor;
using UnityEngine;

namespace TwistCore.CodeGen
{
    public static class TemplateFoldersWatcher
    {
        [MenuItem("Tools/Twist Apps/Commands/Sync CodeGen Templates")]
        public static void SyncTemplateFoldersFromAllRegisteredPackages()
        {
            const string templateFolderSlug = "ScriptTemplates";
            var sourceFolders = PackageRegistryUtils.Collection.Select(
                package => Path.Combine(package.assetPath, templateFolderSlug))
                .Where(Directory.Exists)
                .ToArray();
            
            TaskManager.Enqueue(
                FolderSync.SyncFoldersTask(CodeGenDefinitions.TemplatesFolder, true, sourceFolders), 
                "Gathering templates");
        }
    }
}