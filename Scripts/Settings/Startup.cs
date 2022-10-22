using UnityEditor;
using UnityEngine;

namespace TwistCore
{
    [InitializeOnLoad]
    public class Startup
    {
        public static PackageData PackageData;
        public static readonly bool GitAvailable;
        public static readonly string GitVersion;
        
        static Startup()
        {
            try
            {
                GitVersion = GitCmd.GetVersion();
                GitAvailable = true;
            }
            catch
            {
                GitAvailable = false;
            }
            //packageInfo = PackageFetch.Get()
        }
    }

}