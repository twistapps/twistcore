using System;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public class Button
    {
        private readonly string _innerText;
        private readonly GUIStyle _style = ButtonStyles.Default;
        private Action _onClick;

        private int _widthOverride;

        public Button(string innerText, Action onClick = null, int widthOverride = 0)
        {
            _innerText = innerText;
            _onClick = onClick;
            if (widthOverride != 0) OverrideWidth(widthOverride);
        }

        public Button(string innerText, GUIStyle style, Action onClick = null, int widthOverride = 0)
        {
            _innerText = innerText;
            _onClick = onClick;
            _style = style ?? ButtonStyles.Default;
            if (widthOverride != 0) OverrideWidth(widthOverride);
        }

        public void OverrideWidth(int forcedWidth)
        {
            _widthOverride = forcedWidth;
        }

        public Button Disable()
        {
            _onClick = null;
            return this;
        }

        public void Construct(int width = 120)
        {
            using (new EditorGUI.DisabledScope(_onClick == null))
            {
                if (_widthOverride != 0) width = _widthOverride;
                if (GUILayout.Button(_innerText, _style, GUILayout.Width(width)))
                    _onClick?.Invoke();
            }
        }

        public void Construct(params GUILayoutOption[] options)
        {
            using (new EditorGUI.DisabledScope(_onClick == null))
            {
                if (GUILayout.Button(_innerText, _style, options))
                    _onClick?.Invoke();
            }
        }
    }
}