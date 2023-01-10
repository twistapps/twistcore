#if TMPRO
using TMPro;
using UnityEngine;

namespace TwistCore
{
    public class TwistCoreRuntimeChecker : MonoBehaviour
    {
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