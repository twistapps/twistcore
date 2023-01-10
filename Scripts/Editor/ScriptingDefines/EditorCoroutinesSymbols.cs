using TwistCore.PackageRegistry.Editor;

namespace TwistCore.Editor
{
    [PackageName(PackageName)]
    public class EditorCoroutinesSymbols : ConditionalDefineSymbols
    {
        private const string EDITOR_COROUTINES = "EDITOR_COROUTINES";
        private const string PackageName = "com.unity.editorcoroutines";

        public override string GetSymbols()
        {
            return EDITOR_COROUTINES;
        }

        public override bool ShouldSetDefines()
        {
            return UPMCollection.GetFromAllPackages(PackageName) != null;
        }
    }
}