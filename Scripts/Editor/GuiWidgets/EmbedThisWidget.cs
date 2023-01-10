using JetBrains.Annotations;

namespace TwistCore.Editor.GuiWidgets
{
    [UsedImplicitly]
    public class EmbedThisWidget<T> : GuiWidget<T> where T : SettingsAsset
    {
        public override void Draw()
        {
            Window.ButtonLabel("Embed This Package", new Button("Embed", Embed));
        }

        private void Embed()
        {
            UPMInterface.Embed(PackageName);
        }
    }
}