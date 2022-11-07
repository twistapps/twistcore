﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TwistCore.CodeGen.Editor
{
    public class CodeGenTemplateBuilder : CodeGenBuilder
    {
        public const string BaseSlug = "BASE";

        public const string GenericArgumentSlug = "GENERIC_ARGUMENT";

        //todo: move so called 'cursor' to position of a variable and ability to perform standard Builder's actions from that position,
        // then reset cursor
        private string _mem;
        public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>();

        public string[] VariableNames => Variables.Keys.ToArray();

        public void SetVariable(string key, string replacementValue)
        {
            if (Variables.ContainsKey(key))
                Variables[key] = replacementValue;
            else
                Variables.Add(key, replacementValue);
        }

        public string GetVariable(string key)
        {
            return Variables[key];
        }

        public void SetVariablesForType(Type type)
        {
            SetVariable("CLASSNAME", type.Name);
            FindAndSetGenericArguments(type);
            if (type.BaseType != null) FindAndSetGenericArguments(type.BaseType, BaseSlug);
        }

        private void FindAndSetGenericArguments(Type type, string prefix = "")
        {
            //default variables
            var genericArguments = type.GetGenericArguments();

            for (var i = 0; i < genericArguments.Length; i++)
            {
                var genericArgument = genericArguments[i];
                var variableName = $"{prefix}_{GenericArgumentSlug}_{i + 1}".Replace("_1", "");
                if (SettingsUtility.Load<CodeGenSettings>().debugMode)
                    Debug.Log($"Setting builder variable: ${variableName}$ to {genericArgument.Name}");
                SetVariable(variableName, genericArgument.Name);
            }
        }

        public void GenerateFromTemplate(string templatePath)
        {
            var text = File.ReadAllText(templatePath);

            var parts = text.Split(SeparatorSymbol);
            for (var i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1)
                {
                    if (!Variables.ContainsKey(parts[i]))
                    {
                        Debug.LogWarning(
                            $"CodeGen: variable ${parts[i]}$ is not set for template {Path.GetFileName(templatePath)}. " +
                            "It will be replaced with empty string");
                        parts[i] = string.Empty;
                        continue;
                    }

                    parts[i] = Variables[parts[i]];
                }

                Append(parts[i]);
            }
        }

        public void MoveCursorToVariable(string variableName)
        {
            var generated = StringBuilder.ToString();
        }
    }
}