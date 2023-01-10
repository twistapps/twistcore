using System;

namespace TwistCore.Editor.GuiWidgets
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WidgetIncludesSectionAttribute : Attribute
    {
        public readonly bool SectionIsIncluded;

        public WidgetIncludesSectionAttribute(bool sectionIsIncluded)
        {
            SectionIsIncluded = sectionIsIncluded;
        }
    }
}