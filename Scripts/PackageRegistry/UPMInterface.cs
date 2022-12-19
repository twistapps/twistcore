﻿using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TwistCore.PackageRegistry
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
            var packageInfo = PackageRegistryUtils.Get(packageName);
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
    }
}