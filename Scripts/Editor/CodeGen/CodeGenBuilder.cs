using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace TwistCore.CodeGen.Editor
{
    public enum ClassModifier
    {
        Abstract,
        Static,
        Partial
    }

    public class CodeGenBuilder
    {
        public const char SeparatorSymbol = '$';
        private const string Semicolon = ";";
        private const char Indent = ' ';
        private const int IndentAmountPerStep = 4;
        protected readonly StringBuilder StringBuilder;
        private int _indent;

        public CodeGenBuilder()
        {
            StringBuilder = new StringBuilder();
        }

        public void Using(string name)
        {
            AppendLine("using $;", name);
        }

        public void Class(Scope scope, string name, Type parentClass = null, params ClassModifier[] modifiers)
        {
            var parentClassString = parentClass?.Name;
            if (parentClass != null)
            {
                var genericArguments = parentClass.GenericTypeArguments.Select(type => type.Name);
                var genericArgumentsString = string.Join(", ", genericArguments);
                if (parentClass.ContainsGenericParameters) parentClassString += $"<{genericArgumentsString}>";
            }

            Class(scope, name, parentClassString, modifiers);
        }

        public void Class(Scope scope, string name, params ClassModifier[] modifiers)
        {
            Class(scope, name, "", modifiers);
        }

        public void Class(Scope scope, string name, string parentClass = null, params ClassModifier[] modifiers)
        {
            switch (scope)
            {
                case Scope.Public:
                    Append("public");
                    break;
                case Scope.Protected:
                    Append("protected");
                    break;
                case Scope.Private:
                    Append("private");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }

            Space();

            foreach (var classModifier in modifiers)
                switch (classModifier)
                {
                    case ClassModifier.Abstract:
                        Append("abstract ");
                        break;
                    case ClassModifier.Static:
                        Append("static ");
                        break;
                    case ClassModifier.Partial:
                        Append("partial ");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            Append("class $", name);
            if (!string.IsNullOrEmpty(parentClass)) Append(" : $", parentClass);
            OpenCurly();
        }

        public CodeGenBuilder Append(string line, params string[] lineInsertions)
        {
            var parts = line.Split(SeparatorSymbol);

            var counter = 0;
            foreach (var (text, insertion) in Tuples.Make(parts, lineInsertions, true))
            {
                StringBuilder.Append(text + insertion);
                counter++;
            }

            for (var i = counter; i < parts.Length; i++) StringBuilder.Append(parts[i]);

            return this;
        }

        private string GetIndent()
        {
            return new string(Indent, _indent * IndentAmountPerStep);
        }

        public CodeGenBuilder AppendLine(string line, params string[] lineInsertions)
        {
            StringBuilder.AppendLine();
            StringBuilder.Append(GetIndent());
            Append(line, lineInsertions);
            return this;
        }

        public CodeGenBuilder AppendLine(string line, bool assureSemicolon, params string[] lineInsertions)
        {
            AppendLine(line, lineInsertions);
            if (assureSemicolon) AssureSemicolon();

            return this;
        }

        public CodeGenBuilder AppendLine(bool ignoreIndent = false)
        {
            StringBuilder.AppendLine();
            if (!ignoreIndent) StringBuilder.Append(GetIndent());
            return this;
        }

        public CodeGenBuilder AssureSemicolon()
        {
            if (!StringBuilder.ToString().EndsWith(Semicolon))
                Append(Semicolon);
            return this;
        }

        public void EmptyLines(int lines)
        {
            for (var i = 0; i < lines; i++) AppendLine();
        }

        private void Space(int amount = 1)
        {
            for (var i = 0; i < amount; i++) Append(" ");
        }

        public void OpenCurly()
        {
            AppendLine("{");
            _indent++;
        }

        public void CloseCurly()
        {
            _indent--;
            AppendLine("}");
        }

        public void Endfile()
        {
            while (_indent > 0) CloseCurly();
        }

        public void SaveToCsFile(string path, bool keepBuilderDirty = false)
        {
            path = Path.ChangeExtension(path, ".cs");
            SaveToFile(path, keepBuilderDirty);
            AssetDatabase.Refresh();
        }

        public void SaveToFile(string path, bool keepBuilderDirty = false)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, StringBuilder.ToString());
            if (!keepBuilderDirty) Clear();
        }

        public void Clear()
        {
            StringBuilder.Clear();
            _indent = 0;
        }
    }
}