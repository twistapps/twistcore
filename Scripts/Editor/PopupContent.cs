using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TwistCore.Editor
{
    public class PopupContent
    {
        public GUIContent[] entries;
        public int selectedIndex;

        public PopupContent()
        {
        }

        public PopupContent(IEnumerable<string> entries)
        {
            this.entries = entries.Select(e => new GUIContent(e)).ToArray();
        }

        public string Selected => entries[selectedIndex].text;

        public PopupContent Select(int index)
        {
            selectedIndex = index;
            return this;
        }
    }
}