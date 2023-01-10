using System.Collections.Generic;

namespace TwistCore.Editor
{
    public class FoldoutManager
    {
        private readonly Dictionary<int, bool> _state = new Dictionary<int, bool>();

        private bool _currentElementIsActive;

        private int _foldoutCounter;
        private int _prevFoldoutCount;
        public bool CurrentElementIsFoldout => _currentElementIsActive && _state.ContainsKey(_foldoutCounter);
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

        public void NextSectionStart(string sectionName = null)
        {
            _foldoutCounter++;
            _currentElementIsActive = true;
        }

        public void SectionEnd()
        {
            _currentElementIsActive = false;
        }
    }
}