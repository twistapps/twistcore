namespace TwistCore.Editor.UIComponents
{
    public class UnsupportedNotification : GuiWidget<SettingsAsset>
    {
        public override void Draw()
        {
            Window.LabelFailure(null, $"Unsupported component: {ComponentName}");
        }
    }
}