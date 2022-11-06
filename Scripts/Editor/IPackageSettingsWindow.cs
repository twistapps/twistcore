using System;
using UnityEngine;

namespace TwistCore.Editor
{
    public interface IPackageSettingsWindow<out TSettings> where TSettings : SettingsAsset
    {
        TSettings GetSettings();

        /// <summary>
        /// Create section using one method only. Put UI elements code into insides parameter to draw them inside this section.
        /// </summary>
        /// <param name="innerActions">Actions to execute inside the section.</param>
        void AddSection(string heading, Action innerActions, bool addDivider = false, bool forceDisabled = false,
            int width = -1);

        void AddSection(string heading, Action innerActions, ref bool enabled, bool addDivider = false, Action<bool> onEnabledChange = null, int width = -1);
        void BeginSection(string heading, bool addDivider = false, bool forceDisabled = false, int width = -1);
        void BeginSection(string heading, ref bool enabled, bool addDivider = false, Action<bool> onEnabledChange = null, int width = -1);
        void EndSection();
        void Checkbox(string text, ref bool value, Action<bool> onValueChanged = null, bool forceEnabled = false, GUIStyle style = null);
        void EnumPopup<T>(string text, ref T value, Action<T> onValueChanged = null, bool forceEnabled = false) where T : Enum;
        void ButtonLabel(string labelText, params Button[] buttons);
        void StatusLabel(string text, string status, GUIStyle statusStyle, string iconId, params Button[] buttons);
        void LabelSuccess(string text, string status, bool suppressColor = false, params Button[] buttons);
        void LabelFailure(string text, string status, bool suppressColor = false, params Button[] buttons);
        void LabelWarning(string text, string status, bool suppressColor = false, params Button[] buttons);
        void InputField(string text, ref string value, bool forceEnabled = false);
        void InputField(string text, bool disabled = false);
        void HorizontalButtons(params Button[] buttons);
        void HorizontalButton(Button button);
        void CallToAction(string heading, params Button[] buttons);
        void Focus();
        void ShowPopup();
        void Close();
        bool maximized { get; set; }
        bool hasFocus { get; }
        bool docked { get; }
        Vector2 minSize { get; set; }
        Vector2 maxSize { get; set; }
        string title { get; set; }
        Rect position { get; set; }
    }
}