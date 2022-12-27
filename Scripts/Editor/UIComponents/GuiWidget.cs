﻿namespace TwistCore.Editor.UIComponents
{
    public interface IGuiWidget<out T> where T : SettingsAsset
    {
        void BindWindow(IPackageSettingsWindow<SettingsAsset> window);
        void Draw();
        void SetComponentName(string name);
    }

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