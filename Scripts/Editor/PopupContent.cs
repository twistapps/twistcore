using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TwistCore.Editor
{
    public class PopupContent
    {
        public readonly GUIContent[] Entries;
        private int _selectedIndex;

        public PopupContent()
        {
        }

        public PopupContent(IEnumerable<string> entries)
        {
            Entries = entries.Select(e => new GUIContent(e)).ToArray();
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => Select(value);
        }

        public string Selected => Entries[_selectedIndex].text;

        private PopupContent Select(int index)
        {
            _selectedIndex = index;
            return this;
        }
    }
}