namespace TwistCore.Editor.UIComponents
{
    public class UnsupportedNotification : SettingsUIComponent<SettingsAsset>
    {
        public override void Draw()
        {
            Window.LabelFailure(null, $"Unsupported component: {ComponentName}");
        }
    }
}