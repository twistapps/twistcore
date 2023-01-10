using System.IO;

namespace TwistCore.CodeGen.Editor
{
    public static class CodeGenDefinitions
    {
        internal const string DefaultTemplate = "CodeGenTemplate";
        private static string TwistappsFolder => Path.Combine("Assets", "TwistApps");
        public static string GeneratedFolder => Path.Combine(TwistappsFolder, "GeneratedScripts");
        public static string TemplatesFolder => Path.Combine(TwistappsFolder, "ScriptTemplates");
    }
}