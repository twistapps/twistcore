namespace TwistCore.Editor
{
    public class CoreDebugSymbols : ConditionalDefineSymbols
    {
        private const string CORE_DEBUG = "TWISTCORE_DEBUG";

        public override string GetSymbols()
        {
            return CORE_DEBUG;
        }

        public override bool ShouldSetDefines()
        {
            return SettingsUtility.Load<TwistCoreSettings>().debug;
        }
    }
}