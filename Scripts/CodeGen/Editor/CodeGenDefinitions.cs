using System.IO;

namespace RequestForMirror.Editor
{
    public static class CodeGenDefinitions
    {
        private const string PackageName = "com.twistapps.request-for-mirror";

        private const string SettingsFilename = "CodeGenSettings";
        internal const string DefaultTemplate = "CodeGenTemplate";
        private static string TwistappsFolder => Path.Combine("Assets", "TwistApps");
        private static string AssetFolder => Path.Combine(TwistappsFolder, "RequestForMirror");
        internal static string TemplatesFolder => Path.Combine("Packages", PackageName, "ScriptTemplates");
        internal static string GeneratedFolder => Path.Combine(AssetFolder, "GeneratedScripts");
    }
}