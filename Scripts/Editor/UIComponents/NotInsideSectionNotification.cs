namespace TwistCore.Editor.GuiWidgets
{
    public class NotInsideSectionNotification : GuiWidget<SettingsAsset>
    {
        public override void Draw()
        {
            Window.LabelFailure(null, $"Widget {ComponentName} should be inside a section");
        }
    }
}