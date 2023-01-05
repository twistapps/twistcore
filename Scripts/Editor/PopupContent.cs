using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TwistCore.Editor
{
    public class PopupContent
    {
        public readonly GUIContent[] Entries;
        public int SelectedIndex;

        public PopupContent()
        {
        }

        public PopupContent(IEnumerable<string> entries)
        {
            this.Entries = entries.Select(e => new GUIContent(e)).ToArray();
        }

        public string Selected => Entries[SelectedIndex].text;

        public PopupContent Select(int index)
        {
            SelectedIndex = index;
            return this;
        }
    }
}