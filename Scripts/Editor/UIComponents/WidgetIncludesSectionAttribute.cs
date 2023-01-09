using System;

namespace TwistCore.Editor.GuiWidgets
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WidgetIncludesSectionAttribute : Attribute
    {
        public bool SectionIsIncluded = false;

        public WidgetIncludesSectionAttribute(bool sectionIsIncluded)
        {
            SectionIsIncluded = sectionIsIncluded;
        }
    }
}