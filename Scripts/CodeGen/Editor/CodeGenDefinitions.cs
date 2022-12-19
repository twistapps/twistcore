﻿using System.IO;

namespace TwistCore.CodeGen.Editor
{
    public static class CodeGenDefinitions
    {
        private const string PackageName = "com.twistapps.request-for-mirror";

        internal const string DefaultTemplate = "CodeGenTemplate";
        private static string TwistappsFolder => Path.Combine("Assets", "TwistApps");
        private static string AssetFolder => Path.Combine(TwistappsFolder, "RequestForMirror");
        internal static string TemplatesFolder => Path.Combine("Packages", PackageName, "ScriptTemplates");
        internal static string GeneratedFolder => Path.Combine(AssetFolder, "GeneratedScripts");
    }
}