namespace TwistCore.Editor.GuiWidgets
{
    // ReSharper disable once UnusedTypeParameter
    public interface IGuiWidget<out T> where T : SettingsAsset
    {
        void BindWindow(IPackageSettingsWindow<SettingsAsset> window);
        void Draw();
        void SetComponentName(string name);
    }
}