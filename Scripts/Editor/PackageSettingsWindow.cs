using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public class FoldoutManager
    {
        private readonly Dictionary<int, bool> _state = new Dictionary<int, bool>();
        //private readonly Dictionary<string, bool> _stateByName = new Dictionary<string, bool>();

        private int _foldoutCounter;
        private int _prevFoldoutCount;

        public bool CurrentElementIsFoldout => _state.ContainsKey(_foldoutCounter);
        public bool CurrentElementIsOpen => !CurrentElementIsFoldout || _state[_foldoutCounter];

        public bool Current
        {
            get => CurrentElementIsFoldout && _state[_foldoutCounter];
            set => SetCurrentElement(value);
        }

        public void SetCurrentElement(bool isOpen)
        {
            _state[_foldoutCounter] = isOpen;
        }

        // public void SetStateByName(string sectionName, bool isOpen)
        // {
        //     _stateByName[sectionName] = isOpen;
        // }

        // public void SetOpenByName(string sectionName)
        // {
        //     _stateByName[sectionName] = true;
        // }

        public void Reset()
        {
            _state.Clear();
            _prevFoldoutCount = 0;
        }

        public void GUICycle()
        {
            if (_foldoutCounter != _prevFoldoutCount && _prevFoldoutCount != 0)
                Reset();
            _prevFoldoutCount = _foldoutCounter;
            _foldoutCounter = 0;
        }

        //private string currentElementName;

        public void NextSectionStart(string sectionName = null)
        {
            _foldoutCounter++;

            //currentElementName = sectionName;
            //if (sectionName == null || !_stateByName.ContainsKey(sectionName)) return;
            //Debug.Log("Yes");
            //_state[_foldoutCounter] = _stateByName[sectionName];
            //_stateByName.Remove(sectionName);
        }
    }

    public abstract class PackageSettingsWindow<TSettings> : EditorWindow, IPackageSettingsWindow<TSettings>
        where TSettings : SettingsAsset
    {
        private const int HorizontalButtonsMargin = 10;
        private const int ElementMarginBottom = 3;

        protected static TSettings Settings;
        protected readonly FoldoutManager FoldoutManager = new FoldoutManager();

        private Section _currentSection;
        private Vector2 _scrollPosition;
        private int _sectionDepth;

        private static GUILayoutOption[] DefaultLabelLayoutOptions =>
            new[] { GUILayout.ExpandWidth(false), GUILayout.MinWidth(38) };

        private void OnGUI()
        {
            if (Settings == null) Settings = SettingsUtility.Load<TSettings>();
            FoldoutManager.GUICycle();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            GUILayout.Space(5);
            Draw();
            EditorGUILayout.EndScrollView();
            WatchChangesAbove();
        }

        public TSettings GetSettings()
        {
            return Settings;
        }

        // public void SetFoldoutSectionOpenByName(string name)
        // {
        //     FoldoutManager.SetOpenByName(name);
        // }

        /// <summary>
        ///     Create section using one method only. Put UI elements code into insides parameter to draw them inside this section.
        /// </summary>
        /// <param name="innerActions">Actions to execute inside the section.</param>
        public void AddSection(string heading, Action innerActions, bool addDivider = false, bool forceDisabled = false,
            int width = -1, bool foldout = false)
        {
            BeginSection(heading, addDivider, forceDisabled, width, foldout);
            innerActions?.Invoke();
            EndSection();
        }

        public void AddSection(string heading, Action innerActions, ref bool enabled, bool addDivider = false,
            Action<bool> onEnabledChange = null, int width = -1)
        {
            BeginSection(heading, ref enabled, addDivider, onEnabledChange, width);
            innerActions?.Invoke();
            EndSection();
        }

        public void BeginSection(string heading, bool addDivider = false, bool forceDisabled = false, int width = -1,
            bool foldout = false)
        {
            FoldoutManager.NextSectionStart(foldout ? heading : null);
            EditorGUILayout.BeginVertical(new GUIStyle("ObjectPickerBackground"));
            if (addDivider) EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var style = new GUIStyle("BoldLabel");

            if (foldout)
                FoldoutManager.Current = EditorGUILayout.Foldout(FoldoutManager.Current, heading);
            else
                EditorGUILayout.LabelField(heading, style);

            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth = 80;

            if (foldout && FoldoutManager.CurrentElementIsOpen) Space(5);

            _currentSection = new Section { Disabled = forceDisabled };
            _sectionDepth++;
        }

        public void BeginSection(string heading, ref bool enabled, bool addDivider = false,
            Action<bool> onEnabledChange = null, int width = -1)
        {
            FoldoutManager.NextSectionStart();
            EditorGUILayout.BeginVertical(new GUIStyle("ObjectPickerBackground"));
            if (addDivider) EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            using (var l = new EditorGUILayout.HorizontalScope())
            {
                EditorGUIUtility.labelWidth = 95;
                EditorGUILayout.LabelField(heading, new GUIStyle("BoldLabel"));
                GUILayout.FlexibleSpace();
                EditorGUIUtility.labelWidth = 45;
                //CheckboxSmall("enable section: ", ref enabled, onEnabledChange, true);
                CheckboxSmall("enable: ", ref enabled, onEnabledChange, true);
                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth = 80;

            _currentSection = new Section { Disabled = !enabled };
            _sectionDepth++;
        }

        public void EndSection()
        {
            EditorGUI.indentLevel--;
            if (FoldoutManager.CurrentElementIsFoldout && FoldoutManager.CurrentElementIsOpen)
            {
                GUILayout.Space(-7);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Space(5);
            }

            GUILayout.Space(20);
            EditorGUILayout.EndVertical();
            _currentSection = null;
            EditorGUIUtility.labelWidth = 0;

            if (FoldoutManager.CurrentElementIsFoldout && _sectionDepth > 1)
                GUILayout.Space(-20);
            _sectionDepth--;
        }

        public void Checkbox(string text, ref bool value, Action<bool> onValueChanged = null, bool forceEnabled = false,
            bool expandWidth = false,
            GUIStyle style = null)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (new EditorGUI.DisabledScope(!forceEnabled && _currentSection.Disabled))
            {
                using (var l = new EditorGUILayout.HorizontalScope())
                {
                    var oldValue = value;
                    var layoutOptions = expandWidth ? new[] { GUILayout.ExpandWidth(true) } : DefaultLabelLayoutOptions;
                    EditorGUILayout.LabelField(text, layoutOptions);
                    value = EditorGUILayout.Toggle(value, GUILayout.Width(25));
                    GUILayout.Space(10);
                    if (oldValue != value) onValueChanged?.Invoke(value);
                }
            }

            GUILayout.Space(ElementMarginBottom);
        }

        public void EnumPopup<T>(string text, ref T value, Action<T> onValueChanged = null, bool forceEnabled = false)
            where T : Enum
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (new EditorGUI.DisabledScope(!forceEnabled && _currentSection.Disabled))
            {
                var oldValue = value;
                value = (T)EditorGUILayout.EnumPopup(text, value);
                if (!Equals(oldValue, value)) onValueChanged?.Invoke(value);
            }
        }

        public void ButtonLabel(string labelText, params Button[] buttons)
        {
            ButtonLabel(labelText, false, 30, buttons);
        }

        public void StatusLabel(string text, string status, GUIStyle statusStyle = null, string iconId = null,
            params Button[] buttons)
        {
            StatusLabel(text, -1, status, statusStyle, iconId, buttons);
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

        public void InputField(string text, ref string value, bool forceEnabled = false, bool forceDisabled = false,
            params Button[] buttons)
        {
            InputField(text, -1, ref value, forceEnabled, forceDisabled, buttons);
        }

        public void InputField(string text)
        {
            var empty = "";
            InputField(text, ref empty);
        }

        public void InputField(string text, string value, ref string outValue, bool forceEnabled = false,
            bool forceDisabled = false,
            params Button[] buttons)
        {
            InputField(text, -1, value, ref outValue, forceEnabled, forceDisabled, buttons);
        }

        public void InputFieldWide(string text, ref string value, bool forceEnabled = false,
            bool forceDisabled = false, params Button[] buttons)
        {
            InputFieldWide(text, value, ref value, forceEnabled, forceDisabled, buttons);
        }

        public void InputFieldWide(string text, string value, ref string outValue, bool forceEnabled = false,
            bool forceDisabled = false, params Button[] buttons)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (new EditorGUI.DisabledScope(!forceEnabled && _currentSection.Disabled || forceDisabled))
            {
                if (text != null)
                    EditorGUILayout.LabelField(text + ":", EditorStyles.boldLabel);

                using (var l = new EditorGUILayout.HorizontalScope())
                {
                    if (outValue != null)
                        outValue = EditorGUILayout.TextField(value);
                    else
                        EditorGUILayout.TextField(value);
                    InputFieldButtons(buttons);
                }
            }

            GUILayout.Space(ElementMarginBottom);
        }

        public void HorizontalButtons(params Button[] buttons)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (new EditorGUI.DisabledScope(_currentSection?.Disabled ?? false))
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
            var style = new GUIStyle("PR Label");
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(heading, style);
            EditorGUI.indentLevel += 2;

            if (buttons.Length > 0)
            {
                GUILayout.Space(-HorizontalButtonsMargin);
                GUILayout.Space(5);
                HorizontalButtons(buttons);
                GUILayout.Space(-5);
            }
            else
            {
                GUILayout.Space(-8);
            }

            GUILayout.Space(-20);
            EditorGUILayout.EndVertical();
            GUILayout.Space(-25);
        }

        public void ResetFoldouts()
        {
            FoldoutManager.Reset();
        }

        public void Divider()
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        public void CheckboxSmall(string text, ref bool value, Action<bool> onValueChanged = null,
            bool forceEnabled = false,
            GUIStyle style = null)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (new EditorGUI.DisabledScope(!forceEnabled && _currentSection.Disabled))
            {
                var oldValue = value;
                value = style == null
                    ? EditorGUILayout.Toggle(text, value)
                    : EditorGUILayout.Toggle(text, value, style);
                if (oldValue != value) onValueChanged?.Invoke(value);
            }

            GUILayout.Space(ElementMarginBottom);
        }

        public void Space(int pixels)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            GUILayout.Space(pixels);
        }

        public void Heading(string text, params Button[] buttons)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (var l = new EditorGUILayout.HorizontalScope())
            {
                var layoutOptions =
                    buttons.Length > 0 ? new[] { GUILayout.ExpandWidth(true) } : DefaultLabelLayoutOptions;
                EditorGUILayout.LabelField(text, EditorStyles.boldLabel, layoutOptions);
                if (buttons.Length > 0)
                    foreach (var button in buttons)
                    {
                        button.Construct(70);
                        GUILayout.Space(9);
                    }
            }

            GUILayout.Space(ElementMarginBottom);
        }

        private void ButtonLabel(string labelText, bool shrinkWidth, int marginLeft = 30, params Button[] buttons)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (new EditorGUI.DisabledScope(_currentSection.Disabled))
            {
                using (var l = new EditorGUILayout.HorizontalScope())
                {
                    var layoutOptions = shrinkWidth ? DefaultLabelLayoutOptions : null;
                    EditorGUILayout.LabelField(labelText, layoutOptions);
                    if (buttons.Length > 0)
                    {
                        if (shrinkWidth) GUILayout.Space(marginLeft);
                        foreach (var button in buttons)
                        {
                            button.Construct(70);
                            GUILayout.Space(9);
                        }
                    }
                }
            }

            GUILayout.Space(ElementMarginBottom);
        }

        public void ButtonLabelShrinkWidth(string labelText, params Button[] buttons)
        {
            ButtonLabel(labelText, true, 30, buttons);
        }

        public void ButtonLabelShrinkWidth(string labelText, int marginLeft, params Button[] buttons)
        {
            ButtonLabel(labelText, true, marginLeft, buttons);
        }

        public void StatusLabel(string text, int textWidthOverride, string status, GUIStyle statusStyle = null,
            string iconId = null,
            params Button[] buttons)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (var l = new EditorGUILayout.HorizontalScope())
            {
                var lw = EditorGUIUtility.labelWidth;
                //EditorGUIUtility.labelWidth = 40;
                var layoutOptions = textWidthOverride != -1
                    ? new[] { GUILayout.Width(textWidthOverride) }
                    : DefaultLabelLayoutOptions;

                if (text != null) EditorGUILayout.LabelField(text, layoutOptions);

                var content = iconId != null ? EditorGUIUtility.IconContent(iconId) : new GUIContent();
                content.text = status;
                EditorGUILayout.LabelField(content, statusStyle ?? EditorStyles.label);
                EditorGUIUtility.labelWidth = lw;
                if (buttons.Length > 0)
                    foreach (var button in buttons)
                    {
                        button.Construct(70);
                        GUILayout.Space(9);
                    }
            }

            GUILayout.Space(ElementMarginBottom);
        }

        public void InputField(string text, int textWidthOverride, ref string value, bool forceEnabled = false,
            bool forceDisabled = false, params Button[] buttons)
        {
            InputField(text, textWidthOverride, value, ref value, forceEnabled, forceDisabled, buttons);
        }

        public void InputField(string text, int textWidthOverride, string value, ref string outValue,
            bool forceEnabled = false,
            bool forceDisabled = false, params Button[] buttons)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (new EditorGUI.DisabledScope(!forceEnabled && _currentSection.Disabled || forceDisabled))
            {
                using (var l = new EditorGUILayout.HorizontalScope())
                {
                    var layoutOptions = textWidthOverride != -1
                        ? new[] { GUILayout.Width(textWidthOverride) }
                        : DefaultLabelLayoutOptions;

                    EditorGUILayout.LabelField(text, layoutOptions);
                    if (outValue != null)
                        outValue = EditorGUILayout.TextField(value);
                    else
                        EditorGUILayout.TextField(value);
                    GUILayout.Space(5);
                    InputFieldButtons(buttons);
                }
            }

            GUILayout.Space(ElementMarginBottom);
        }

        public void ButtonsLeft(params Button[] buttons)
        {
            if (!FoldoutManager.CurrentElementIsOpen) return;
            using (new EditorGUI.DisabledScope(_currentSection?.Disabled ?? false))
            {
                GUILayout.Space(HorizontalButtonsMargin);
                // Horizontally centered
                using (var l = new EditorGUILayout.HorizontalScope())
                {
                    foreach (var button in buttons)
                    {
                        button.Construct(DefaultLabelLayoutOptions);
                        GUILayout.Space(5);
                    }
                }
            }
        }

        private void InputFieldButtons(Button[] buttons)
        {
            if (buttons.Length > 0)
            {
                var spaceBetweenButtons = 4;
                var marginRight = 9;
                foreach (var button in buttons)
                {
                    button.Construct();
                    GUILayout.Space(spaceBetweenButtons);
                }

                GUILayout.Space(marginRight - spaceBetweenButtons);
            }
        }

        protected abstract void Draw();

        /// <summary>
        ///     Update the actual asset file if any value above this has changed.
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

        protected static void ShowWindow(out PackageSettingsWindow<TSettings> window, bool utility = false,
            Vector2 minSize = default)
        {
            Settings = SettingsUtility.Load<TSettings>();
            window =
                GetWindow(TraceCallingType(), utility, Settings.GetEditorWindowTitle()) as
                    PackageSettingsWindow<TSettings>;
            if (minSize != default) window.minSize = minSize;
            else window.minSize = new Vector2(275, 55);
        }

        protected static void ShowWindow(bool utility = false, Vector2 minSize = default)
        {
            //minSize = new Vector2(420, 300);
            ShowWindow(out _, utility, minSize);
        }
    }

    public class Button
    {
        private readonly string _innerText;
        private readonly Action _onClick;

        private int _widthOverride;

        public Button(string innerText, Action onClick = null, int widthOverride = 0)
        {
            _innerText = innerText;
            _onClick = onClick;
            if (widthOverride != 0) OverrideWidth(widthOverride);
        }

        public void OverrideWidth(int forcedWidth)
        {
            _widthOverride = forcedWidth;
        }

        public void Construct(int width = 120)
        {
            using (new EditorGUI.DisabledScope(_onClick == null))
            {
                if (_widthOverride != 0) width = _widthOverride;
                if (GUILayout.Button(_innerText, new GUIStyle("ToolbarButton"), GUILayout.Width(width)))
                    _onClick?.Invoke();
            }
        }

        public void Construct(params GUILayoutOption[] options)
        {
            using (new EditorGUI.DisabledScope(_onClick == null))
            {
                if (GUILayout.Button(_innerText, new GUIStyle("ToolbarButton"), options))
                    _onClick?.Invoke();
            }
        }
    }

    public class Section
    {
        public bool Disabled;
    }
}