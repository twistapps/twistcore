using System.IO;
using UnityEngine;

namespace TwistCore.Editor.CodeGen
{
    public class ScriptInfo
    {
        private readonly string _path; //without extension

        public ScriptInfo(params string[] path)
        {
            var concatenatedPath = Path.Combine(path);
            WarnIfUnintendedExtension(concatenatedPath);
            _path = Path.ChangeExtension(concatenatedPath, "");
        }

        public string CsPath => Path.ChangeExtension(_path, ".cs");
        public string MetaPath => CsPath + ".meta";
        public string Classname => Path.GetFileNameWithoutExtension(_path);

        private static void WarnIfUnintendedExtension(string path)
        {
            var extension = Path.GetExtension(path);
            if (extension != string.Empty
                && extension != ".meta"
                && extension != ".cs")
                Debug.LogWarning(
                    $"{nameof(ScriptInfo)} is not intended to use for '{extension}' files. Use it with .cs or .meta files only");
        }

        public bool Delete()
        {
            var fileDidExist = false;
            foreach (var file in new[] { CsPath, MetaPath })
            {
                if (!File.Exists(file)) continue;

                File.Delete(file);
                fileDidExist = true;
            }

            return fileDidExist;
        }
    }
}