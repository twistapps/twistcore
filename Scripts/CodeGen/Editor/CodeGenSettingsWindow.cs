﻿using TwistCore.Editor;
using UnityEditor;

namespace TwistCore.CodeGen.Editor
{
    internal class CodeGenWindow : PackageSettingsWindow<CodeGenSettings>
    {
        protected override void Draw()
        {
            BeginSection("CodeGen Settings");
            Checkbox("Auto Generate Scripts On Compile", ref Settings.autoGenerateOnCompile);
            Checkbox("Debug Mode", ref Settings.debugMode);
            EndSection();

            CallToAction(
                $"Generate all files marked with {nameof(IMarkedForCodeGen)}:",
                new Button("Generate CS", () => CodeGen.GenerateScripts(true)));

            BeginSection("Create codegen-supported script:", true);
            InputField("Classname");
            InputField("SomeOtherField");
            HorizontalButtons(new Button("Create"));
            EndSection();
        }

        [MenuItem("Tools/Twist Apps/CodeGen Settings")]
        public static void OnMenuItemClick()
        {
            ShowWindow();
        }
    }
}