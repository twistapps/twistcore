using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace TwistCore
{
    public static class PackageFetch
    {
        public static PackageData Get(string url)
        {
            using var webRequest = UnityWebRequest.Get(url);
            var request = webRequest.SendWebRequest();
            while (!request.isDone) Thread.Sleep(300);

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(url + ": Error: " + webRequest.error);
                    return null;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(url + ": HTTP Error: " + webRequest.error);
                    return null;
                case UnityWebRequest.Result.Success:
                    Debug.Log(url + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }

            var packageInfo = JsonUtility.FromJson<PackageData>(webRequest.downloadHandler.text);
            //Debug.Log($"{packageInfo.versionInfo.Major}.{packageInfo.versionInfo.Minor}.{packageInfo.versionInfo.Patch}");
            return packageInfo;
        }
    }
}