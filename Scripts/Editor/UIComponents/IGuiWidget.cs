namespace TwistCore.Editor.UIComponents
{
    public interface IGuiWidget<out T> where T : SettingsAsset
    {
        void BindWindow(IPackageSettingsWindow<SettingsAsset> window);
        void Draw();
        void SetComponentName(string name);
    }
}