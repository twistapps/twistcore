using TwistCore.Editor;
using UnityEditor;
using UnityEditor.PackageManager;

namespace TwistCore.PackageRegistry
{
    public static class UPMEvents
    {
        [InitializeOnLoadMethod]
        private static void SubscribeToEvent()
        {
            // This causes the method to be invoked after the Editor registers the new list of packages.
            //Events.registeredPackages += PackageRegistryUtils.RegisteredPackagesEventHandler;
            Events.registeredPackages += ScriptingDefinesSetter.RegisteredPackagesEventHandler;
        }
    }
}