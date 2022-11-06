using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistCore
{
    public class TwistCoreSettings : SettingsAsset
    {
        public bool packageDevelopment;
        
        public bool enablePackageCreation;
        public string newPackageName;
        public string newPackageDisplayname;
        public string newPackageOrganizationName = "twistapps";
        public string newPackageVersion = "0.0.1";
        public string newPackageDescription;

        public override string GetEditorWindowTitle()
        {
            return "Twist Core";
        }

        public override string GetPackageName()
        {
            return "com.twistapps.twistcore";
        }
    }
}
