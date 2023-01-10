#if TMPRO
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TwistCore
{
    public class TwistCoreRuntimeChecker : MonoBehaviour
    {
        [FormerlySerializedAs("gitVersionLabel")]
        public TextMeshProUGUI status;
        
        private void Start()
        {
            var settings = SettingsUtility.Load<TwistCoreSettings>();
            status.text = $"Git Version: {GitCmd.GetVersion()}\n" +
                          $"New Package Organization Name: {settings.newPackageOrganizationName}";
        }
    }
}
#endif