namespace TwistCore.Editor.GuiWidgets
{
    public abstract class GuiWidget<T> : IGuiWidget<T> where T : SettingsAsset
    {
        protected string ComponentName;
        protected IPackageSettingsWindow<SettingsAsset> Window;
        protected string PackageName => Window.GetSettings().GetPackageName();

        public void BindWindow(IPackageSettingsWindow<SettingsAsset> window)
        {
            Window = window;
        }

        public abstract void Draw();

        public void SetComponentName(string name)
        {
            ComponentName = name;
        }
    }
}