using System.Collections.Generic;

namespace TwistCore
{
    public class CodeGenSettings : SettingsAsset
    {
        public bool autoGenerateOnCompile = true;
        public bool debugMode;
        public List<string> generatedFiles = new List<string>();

        public override string GetEditorWindowTitle()
        {
            return "CodeGen Settings";
        }

        public override string GetPackageName()
        {
            return "com.twistapps.codegen";
        }
    }
}