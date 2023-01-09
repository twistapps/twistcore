namespace TwistCore.Editor.GuiWidgets
{
    public class UnsupportedNotification : GuiWidget<SettingsAsset>
    {
        public override void Draw()
        {
            Window.LabelFailure(null, $"Unsupported component: {ComponentName}");
        }
    }
}