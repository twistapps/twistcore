using System;
using System.Diagnostics;
using TwistCore.Utils;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public abstract class PackageSettingsWindow<TSettings> : EditorWindow where TSettings : SettingsAsset
    {
        private const int DefaultLabelWidth = 150;
        private const int HorizontalButtonsMargin = 10;
        protected static TSettings Settings;

        private float _memorizedLabelWidth = -1;
        
        //todo: research - do we need it after resolving #11?
        private UnityEditor.Editor _settingsEditor;

        protected virtual void OnGUI()
        {
            CreateCachedSettingsEditor();
        }

        /// <summary>
        /// Update the actual asset file if any value above this has changed.
        /// </summary>
        protected void WatchChangesAbove()
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
        
        protected static void ShowWindow()
        {
            Settings = SettingsUtility.Load<TSettings>();
            var window = GetWindow(TraceCallingType(), false, Settings.GetEditorWindowTitle());
            window.minSize = new Vector2(420, 300);
        }

        /// <summary>
        ///     Creates Editor for Settings ScriptableObject
        ///     so changes are saved to asset file as soon as they are made.
        /// </summary>
        private void CreateCachedSettingsEditor()
        {
            if (Settings != null && _settingsEditor != null) return;
            Settings = SettingsUtility.Load<TSettings>();
            _settingsEditor = UnityEditor.Editor.CreateEditor(Settings);
        }

        protected Section CurrentSection;

        protected void BeginSection(string heading, bool addDivider = false, bool forceDisabled = false, int width = -1)
        {
            EditorGUILayout.BeginVertical(new GUIStyle("ObjectPickerBackground"));
            if (addDivider) EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField(heading, new GUIStyle("BoldLabel"));
            EditorGUI.indentLevel++;
            ChangeLabelWidth(width == -1 ? DefaultLabelWidth : width);
            CurrentSection = new Section() {Disabled = forceDisabled};
        }

        protected void BeginSection(string heading, ref bool enabled, bool addDivider = false, Action<bool> onEnabledChange = null, int width = -1)
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

        protected void EndSection()
        {
            RestoreLabelWidth();
            EditorGUI.indentLevel--;
            GUILayout.Space(20);

            EditorGUILayout.EndVertical();
            CurrentSection = null;
        }

        protected void Checkbox(string text, ref bool value, Action<bool> onValueChanged = null, bool forceEnabled = false)
        {
            using (new EditorGUI.DisabledScope(!forceEnabled && CurrentSection.Disabled))
            {
                var oldValue = value;
                value = EditorGUILayout.Toggle(text, value);
                if (oldValue != value) onValueChanged?.Invoke(value);
            }
        }

        protected void EnumPopup<T>(string text, ref T value, Action<T> onValueChanged = null, bool forceEnabled = false) where T : Enum
        {
            using (new EditorGUI.DisabledScope(!forceEnabled && CurrentSection.Disabled))
            {
                var oldValue = value;
                value = (T)EditorGUILayout.EnumPopup(text, value);
                if (!Equals(oldValue, value)) onValueChanged?.Invoke(value);
            }
        }

        protected void InputField(string text, ref string value, bool forceEnabled = false)
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

        protected void InputField(string text, bool disabled = false)
        {
            var empty = "";
            InputField(text, ref empty, disabled);
        }

        protected void HorizontalButtons(params Button[] buttons)
        {
            using (new EditorGUI.DisabledScope(CurrentSection.Disabled))
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

        protected void CallToAction(string heading, params Button[] buttons)
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

        public Button(string innerText, Action onClick = null)
        {
            _innerText = innerText;
            _onClick = onClick;
        }

        public void Construct(int width=120)
        {
            using (new EditorGUI.DisabledScope(_onClick == null))
            {
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