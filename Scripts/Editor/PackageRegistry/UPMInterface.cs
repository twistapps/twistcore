using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TwistCore.Editor
{
    public static class UPMInterface
    {
        public static PackageCollection List()
        {
            var listRequest = Client.List();
            while (!listRequest.IsCompleted) Thread.Sleep(100);

            switch (listRequest.Status)
            {
                case StatusCode.Success:
                    return listRequest.Result;
                case StatusCode.Failure:
                    Debug.LogError(
                        $"Unable to list UPM packages: (#{listRequest.Error.errorCode}) {listRequest.Error.message}");
                    break;
            }

            return null;
        }

        public static PackageInfo Update(PackageInfo packageInfo)
        {
            var version = GithubVersionControl.FetchUpdates(packageInfo);
            if (!version.HasUpdate) return packageInfo;

            var addRequest = Client.Add(packageInfo.repository.url);
            while (!addRequest.IsCompleted) Thread.Sleep(100);

            switch (addRequest.Status)
            {
                case StatusCode.Success:
                    AssetDatabase.Refresh();
                    return addRequest.Result;
                case StatusCode.Failure:
                    Debug.LogError(
                        $"Unable to list UPM packages: (#{addRequest.Error.errorCode}) {addRequest.Error.message}");
                    break;
            }

            return null;
        }

        public static PackageInfo Update(string packageName)
        {
            var packageInfo = UPMCollection.Get(packageName);
            return Update(packageInfo);
        }

        public static PackageInfo Install(string packageGitUrl)
        {
            var addRequest = Client.Add(packageGitUrl);
            while (!addRequest.IsCompleted) Thread.Sleep(100);

            switch (addRequest.Status)
            {
                case StatusCode.Success:
                    return addRequest.Result;
                case StatusCode.Failure:
                    Debug.LogError(
                        $"Unable to install UPM packages: (#{addRequest.Error.errorCode}) {addRequest.Error.message}");
                    break;
            }

            return null;
        }

        public static PackageInfo Embed(string packageName)
        {
            var embedRequest = Client.Embed(packageName);
            while (!embedRequest.IsCompleted) Thread.Sleep(100);

            switch (embedRequest.Status)
            {
                case StatusCode.Success:
                    return embedRequest.Result;
                case StatusCode.Failure:
                    Debug.LogError(
                        $"Unable to install UPM packages: (#{embedRequest.Error.errorCode}) {embedRequest.Error.message}");
                    break;
            }

            return null;
        }
    }
}