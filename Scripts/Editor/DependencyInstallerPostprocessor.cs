using UnityEditor;

namespace TwistCore.Editor
{
    public class DependencyInstallerPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            DependencyInstallerWindow.OnReloadAssets();
        }
    }
}