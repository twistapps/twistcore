using System.Linq;
using TwistCore.PackageRegistry;
using UnityEditor;
using UnityEditor.PackageManager;

namespace TwistCore.Editor
{
    public static class EditorCoroutinesDefineSymbolsSetter
    {
        private const string EditorCoroutines = "EDITOR_COROUTINES";
        private const string PackageName = "com.unity.editorcoroutines";
        
        [InitializeOnLoadMethod]
        private static void CheckEditorCoroutinesInstalled()
        {
            if (UPMCollection.GetFromAllPackages(PackageName) == null)
                ScriptingDefinesSetter.RemoveSymbols(EditorCoroutines);
            else
                ScriptingDefinesSetter.AddSymbols(EditorCoroutines);
        }
        
        [InitializeOnLoadMethod]
        private static void SubscribeToUPMEvent()
        {
            // This causes the method to be invoked after the Editor registers the new list of packages.
            Events.registeringPackages += RegisteringPackagesEventHandler;
        }
        
        public static void RegisteringPackagesEventHandler(PackageRegistrationEventArgs packageRegistrationEventArgs)
        {
            if (packageRegistrationEventArgs.removed.FirstOrDefault(pkg => pkg.name == PackageName) != null)
            {
                ScriptingDefinesSetter.RemoveSymbols(EditorCoroutines);
                return;
            }
            
            if (packageRegistrationEventArgs.added.FirstOrDefault(pkg => pkg.name == PackageName) != null)
            {
                ScriptingDefinesSetter.AddSymbols(EditorCoroutines);
            }
        }
    }
}