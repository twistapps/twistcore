using TwistCore.PackageRegistry.Editor;

namespace TwistCore.Editor
{
    [PackageName(PackageName)]
    public class TMProSymbols : ConditionalDefineSymbols
    {
        // ReSharper disable once IdentifierTypo
        private const string TMPRO = "TMPRO";
        private const string PackageName = "com.unity.textmeshpro";
        public override string GetSymbols()
        {
            return TMPRO;
        }

        public override bool ShouldSetDefines()
        {
            return UPMCollection.GetFromAllPackages(PackageName) != null;
        }
    }
}