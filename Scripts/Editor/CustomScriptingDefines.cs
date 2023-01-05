using System.Collections.Generic;
using UnityEditor;

namespace TwistCore.Editor
{
    public class CustomScriptingDefines
    {
        private const string CORE_DEBUG = "TWISTCORE_DEBUG";

        public HashSet<string> toAdd = new HashSet<string>();
        public HashSet<string> toRemove = new HashSet<string>();

        private void Test(string symbol, bool condition)
        {
            if (condition)
                toAdd.Add(symbol);
            else
                toRemove.Add(symbol);
        }

        public static CustomScriptingDefines GetAll()
        {
            var defines = new CustomScriptingDefines();
            
        /////////////////////
        // add any custom defines below
        //
            defines.Test(CORE_DEBUG, SettingsUtility.Load<TwistCoreSettings>().debug); 
        //
        // use 'Test' method for easier management
        /////////////////////
        
            return defines;
        }

        private static bool initialized = false;
        [InitializeOnLoadMethod]
        private static void Init()
        {
            if (initialized) return;
            SetAll();
            initialized = true;
        }

        public static void SetAll()
        {
            var defines = GetAll();
            
            foreach (var symbol in defines.toAdd)
            {
                ScriptingDefinesSetter.AddSymbols(symbol);
            }
            foreach (var symbol in defines.toRemove)
            {
                ScriptingDefinesSetter.RemoveSymbols(symbol);
            }
        }
    }
}