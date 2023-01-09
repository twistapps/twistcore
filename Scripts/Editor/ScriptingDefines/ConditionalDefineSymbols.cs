namespace TwistCore.Editor
{
    public abstract class ConditionalDefineSymbols
    {
        public abstract string GetSymbols();
        public abstract bool ShouldSetDefines();
    }
}