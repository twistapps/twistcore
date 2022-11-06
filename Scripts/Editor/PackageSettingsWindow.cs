using System;
using System.Diagnostics;
using TwistCore.Utils;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public abstract class PackageSettingsWindow<TSettings> : EditorWindow, IPackageSettingsWindow<TSettings> where TSettings : SettingsAsset
    {
        private const int DefaultLabelWidth = 250; //150
        private const int HorizontalButtonsMargin = 10;
        protected static TSettings Settings;
        public TSettings GetSettings() => Settings;

        private float _memorizedLabelWidth = -1;
        
        //todo: research - do we need it after resolving #11?
        //private UnityEditor.Editor _settingsEditor;

        protected virtual void OnGUI()
        {
            if (Settings == null) Settings = SettingsUtility.Load<TSettings>();
        }

        /// <summary>
        /// Update the actual asset file if any value above this has changed.
        /// </summary>
        protected internal void WatchChangesAbove()
        {
            if (GUI.changed)
                EditorUtility.SetDirty(Settings);
        }

        private static Type TraceCallingType()
        {
            var stackTrace = new StackTrace();
            var thisType = stackTrace.GetFrame(0).GetMethod().DeclaringType;

            for (var i = 1; i < stackTrace.FrameCount; i++)
            {
                var declaringType = stackTrace.GetFrame(i).GetMethod().DeclaringType;
                if (declaringType != thisType) return declaringType;
            }

            return thisType;
        }

        protected static void ShowWindow(out EditorWindow window, bool utility = false, Vector2 minSize = default)
        {
            Settings = SettingsUtility.Load<TSettings>();
            window = GetWindow(TraceCallingType(), utility, Settings.GetEditorWindowTitle());
            if (minSize != default) window.minSize = minSize;
        }
        
        protected static void ShowWindow(bool utility = false, Vector2 minSize = default)
        {
            //minSize = new Vector2(420, 300);
            ShowWindow(out _, utility, minSize);
        }

        /// <summary>
        ///     Creates Editor for Settings ScriptableObject
        ///     so changes are saved to asset file as soon as they are made.
        /// </summary>
        // private void CreateCachedSettingsEditor()
        // {
        //     if (Settings != null && _settingsEditor != null) return;
        //     Settings = SettingsUtility.Load<TSettings>();
        //     _settingsEditor = UnityEditor.Editor.CreateEditor(Settings);
        // }

        protected Section CurrentSection;

        public class SectionProperties
        {
            public static SectionProperties Default = new SectionProperties();
            public bool AddDivider;
            public bool ForceDisabled;
            public int Width = -1;
        }

        /// <summary>
        /// Create section using one method only. Put UI elements code into insides parameter to draw them inside this section.
        /// </summary>
        /// <param name="innerActions">Actions to execute inside the section.</param>
        public void AddSection(string heading, Action innerActions, bool addDivider = false, bool forceDisabled = false,
            int width = -1)
        {
            BeginSection(heading, addDivider, forceDisabled, width);
            innerActions?.Invoke();
            EndSection();
        }

        public void AddSection(string heading, Action innerActions, ref bool enabled, bool addDivider = false, Action<bool> onEnabledChange = null, int width = -1)
        {
            BeginSection(heading, ref enabled, addDivider, onEnabledChange, width);
            innerActions?.Invoke();
            EndSection();
        }

        // public void BeginSection(string heading, SectionProperties properties)
        // {
        //     EditorGUILayout.BeginVertical(new GUIStyle("ObjectPickerBackground"));
        //     if (properties.AddDivider) EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //     
        //     
        //     EditorGUILayout.LabelField(heading, new GUIStyle("BoldLabel"));
        //     
        //     EditorGUI.indentLevel++;
        //     ChangeLabelWidth(properties.Width == -1 ? DefaultLabelWidth : properties.Width);
        //     CurrentSection = new Section() {Disabled = properties.ForceDisabled};
        // }

        public void BeginSection(string heading, bool addDivider = false, bool forceDisabled = false, int width = -1)
        {
            EditorGUILayout.BeginVertical(new GUIStyle("ObjectPickerBackground"));
            if (addDivider) EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            
            EditorGUILayout.LabelField(heading, new GUIStyle("BoldLabel"));
            
            EditorGUI.indentLevel++;
            ChangeLabelWidth(width == -1 ? DefaultLabelWidth : width);
            CurrentSection = new Section() {Disabled = forceDisabled};
        }
        
        public void BeginSection(string heading, ref bool enabled, bool addDivider = false, Action<bool> onEnabledChange = null, int width = -1)
        {
            EditorGUILayout.BeginVertical(new GUIStyle("ObjectPickerBackground"));
            if (addDivider) EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            using (var l = new EditorGUILayout.HorizontalScope())
            {
                ChangeLabelWidth(width == -1 ? DefaultLabelWidth : width);
                EditorGUILayout.LabelField(heading, new GUIStyle("BoldLabel"));
                RestoreLabelWidth();
                GUILayout.FlexibleSpace();
                ChangeLabelWidth(90);
                Checkbox("enable section: ", ref enabled, onEnabledChange, true);
                RestoreLabelWidth();
            }
            
            EditorGUI.indentLevel++;
            ChangeLabelWidth(width == -1 ? DefaultLabelWidth : width);
            CurrentSection = new Section() {Disabled = !enabled};
        }

        public void EndSection()
        {
            RestoreLabelWidth();
            EditorGUI.indentLevel--;
            GUILayout.Space(20);

            EditorGUILayout.EndVertical();
            CurrentSection = null;
        }

        public void Checkbox(string text, ref bool value, Action<bool> onValueChanged = null, bool forceEnabled = false, GUIStyle style = null)
        {
            using (new EditorGUI.DisabledScope(!forceEnabled && CurrentSection.Disabled))
            {
                var oldValue = value;
                value = style == null 
                    ? EditorGUILayout.Toggle(text, value) 
                    : EditorGUILayout.Toggle(text, value, style);
                if (oldValue != value) onValueChanged?.Invoke(value);
            }
        }

        public void EnumPopup<T>(string text, ref T value, Action<T> onValueChanged = null, bool forceEnabled = false) where T : Enum
        {
            using (new EditorGUI.DisabledScope(!forceEnabled && CurrentSection.Disabled))
            {
                var oldValue = value;
                value = (T)EditorGUILayout.EnumPopup(text, value);
                if (!Equals(oldValue, value)) onValueChanged?.Invoke(value);
            }
        }

        public void ButtonLabel(string labelText, params Button[] buttons)
        {
            var memLabelWidth = EditorGUIUtility.labelWidth;
            RestoreLabelWidth();
            ChangeLabelWidth(1);
            using (var l = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(labelText);
                if (buttons.Length > 0)
                {
                    foreach (var button in buttons)
                    {
                        button.Construct(70);
                        GUILayout.Space(3);
                    }
                }
            }
            RestoreLabelWidth();
            ChangeLabelWidth(memLabelWidth);
        }

        public void StatusLabel(string text, string status, GUIStyle statusStyle, string iconId, params Button[] buttons)
        {
            var memLabelWidth = EditorGUIUtility.labelWidth;
            RestoreLabelWidth();
            ChangeLabelWidth(1);
            using (var l = new EditorGUILayout.HorizontalScope())
            {
                if (text != null) EditorGUILayout.LabelField(text);
                //GUILayout.FlexibleSpace();
                
                var content = EditorGUIUtility.IconContent(iconId);
                content.text = status;
                EditorGUILayout.LabelField(content, statusStyle);
                if (buttons.Length > 0)
                {
                    foreach (var button in buttons)
                    {
                        button.Construct(50);
                        GUILayout.Space(3);
                    }
                }
            }
            RestoreLabelWidth();
            ChangeLabelWidth(memLabelWidth);
        }

        public void LabelSuccess(string text, string status, bool suppressColor = false, params Button[] buttons)
        {
            var statusStyle = !suppressColor ? GUIStyles.SuccessLabel : GUIStyles.DefaultLabel;
            StatusLabel(text, status, statusStyle, GUIStyles.IconSuccess, buttons);
        }

        public void LabelFailure(string text, string status, bool suppressColor = false, params Button[] buttons)
        {
            var statusStyle = !suppressColor ? GUIStyles.FailureLabel : GUIStyles.DefaultLabel;
            StatusLabel(text, status, statusStyle, GUIStyles.IconError, buttons);
        }

        public void LabelWarning(string text, string status, bool suppressColor = false, params Button[] buttons)
        {
            var statusStyle = !suppressColor ? GUIStyles.WarningLabel : GUIStyles.DefaultLabel;
            StatusLabel(text, status, statusStyle, GUIStyles.IconWarning, buttons);
        }

        public void InputField(string text, ref string value, bool forceEnabled = false)
        {
            using (new EditorGUI.DisabledScope(!forceEnabled && CurrentSection.Disabled))
            {
                EditorGUILayout.LabelField(text);
                EditorGUI.indentLevel++;
                value = EditorGUILayout.TextField(value);
                EditorGUI.indentLevel--;
                GUILayout.Space(5);
            }
        }

        public void InputField(string text, bool disabled = false)
        {
            var empty = "";
            InputField(text, ref empty, disabled);
        }

        public void HorizontalButtons(params Button[] buttons)
        {
            using (new EditorGUI.DisabledScope(CurrentSection?.Disabled ?? false))
            {
                GUILayout.Space(HorizontalButtonsMargin);
                // Horizontally centered
                using (var l = new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    foreach (var button in buttons)
                    {
                        button.Construct();
                        GUILayout.Space(5);
                    }

                    GUILayout.FlexibleSpace();
                }
            }
        }

        public void HorizontalButton(Button button)
        {
            HorizontalButtons(button);
        }

        public void CallToAction(string heading, params Button[] buttons)
        {
            GUILayout.Space(-25);
            EditorGUILayout.BeginVertical(new GUIStyle("NotificationBackground"));
            GUILayout.Space(-25);
            EditorGUI.indentLevel -= 2;
            EditorGUILayout.LabelField(heading, new GUIStyle("PR Label"));
            EditorGUI.indentLevel += 2;

            GUILayout.Space(-HorizontalButtonsMargin);
            GUILayout.Space(5);
            HorizontalButtons(buttons);
            GUILayout.Space(-5);
            //GUILayout.Space(-HorizontalButtonsMargin);

            GUILayout.Space(-20);
            EditorGUILayout.EndVertical();
            GUILayout.Space(-25);
        }

        private void ChangeLabelWidth(float newWidth)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_memorizedLabelWidth != -1) return;
            _memorizedLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = newWidth;
        }

        private void RestoreLabelWidth()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_memorizedLabelWidth == -1) return;
            EditorGUIUtility.labelWidth = _memorizedLabelWidth;
            _memorizedLabelWidth = -1;
        }
    }

    public class Button
    {
        private readonly string _innerText;
        private readonly Action _onClick;

        private int _widthOverride = 0;

        public void OverrideWidth(int forcedWidth)
        {
            _widthOverride = forcedWidth;
        }

        public Button(string innerText, Action onClick = null, int widthOverride = 0)
        {
            _innerText = innerText;
            _onClick = onClick;
            if (widthOverride != 0) OverrideWidth(widthOverride);
        }

        public void Construct(int width=120)
        {
            using (new EditorGUI.DisabledScope(_onClick == null))
            {
                if (_widthOverride != 0) width = _widthOverride;
                if (GUILayout.Button(_innerText, new GUIStyle("ToolbarButton"), GUILayout.Width(width)))
                    _onClick?.Invoke();
            }
        }
    }

    public class Section
    {
        public bool Disabled;
    }
}