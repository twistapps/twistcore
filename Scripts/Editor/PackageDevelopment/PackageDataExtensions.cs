using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TwistCore.Editor
{
    [Serializable]
    public class AsmdefReferenceObject
    {
        [SerializeField] public string reference;

        [NonSerialized] public string file;

        public void WriteToFile()
        {
            var json = JsonUtility.ToJson(this);
            File.WriteAllText(file, json);
        }
    }

    public static class PackageDataExtensions
    {
        private const string EmptyGuid = "GUID:00000000000000000000000000000000";
        public const string EditorAsmdefSuffix = ".editor";

        public static string Alias(this PackageInfo package)
        {
            return package.name.Split('.').LastOrDefault();
        }

        public static string Asmdef(this PackageInfo package, bool editor = false)
        {
            var editorSuffix = editor ? EditorAsmdefSuffix : string.Empty;

            bool FileMatch(string file)
            {
                Debug.Log("Looking for " + package.Alias() + editorSuffix + ".asmdef");
                return file.ToLower().EndsWith(package.Alias() + editorSuffix + ".asmdef");
            }

            var packageFolder = package.assetPath;
            var files = Directory.GetFiles(packageFolder, "*.asmdef", SearchOption.AllDirectories);
            foreach (var file in files) Debug.Log("Found " + file);
            var asmdef = files.FirstOrDefault(FileMatch);
            //if (editor && string.IsNullOrEmpty(asmdef)) return Asmdef(package, false);
            return !string.IsNullOrEmpty(asmdef) ? asmdef : null;
        }

        public static List<AsmdefReferenceObject> AsmdefReferences(this PackageInfo package, bool editor = false)
        {
            var guid = "GUID:" + package.AsmdefGuid(editor);

            var packageFolder = package.assetPath;
            var files = Directory.GetFiles(packageFolder, "*.asmref", SearchOption.AllDirectories);

            var references = new List<AsmdefReferenceObject>();

            foreach (var file in files)
            {
                var refObject = JsonUtility.FromJson<AsmdefReferenceObject>(File.ReadAllText(file));
                if (refObject.reference != guid) continue;

                refObject.file = file;
                references.Add(refObject);
            }

            return references;
        }

        public static string AsmdefGuid(this PackageInfo package, bool editor = false)
        {
            var asmdef = Asmdef(package, editor);
            Debug.Log($"[asmdef]{package.name}: {asmdef} -- {AssetDatabase.AssetPathToGUID(asmdef)}");
            return AssetDatabase.AssetPathToGUID(Asmdef(package, editor));
            //return AssetDatabase.GUIDFromAssetPath(Asmdef(package));
        }
    }
}