using System;
using System.Collections.Generic;
using System.Threading;
using TwistCore.ProgressWindow;
using UnityEngine;
using UnityEngine.Networking;

namespace TwistCore
{
    public class WebRequestUtility
    {
        /// <summary>
        ///     Synchronously load json data from URL. Main thread freezes while waiting for the result.
        /// </summary>
        /// <param name="url">URI to fetch.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FetchJSON<T>(string url) where T : new()
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
                    return new T();
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(url + ": HTTP Error: " + webRequest.error);
                    return new T();
                case UnityWebRequest.Result.Success:
                    //Debug.Log(url + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }

            var packageInfo = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
            return packageInfo;
        }

        public static IEnumerator<TaskProgress> FetchJSONTask<T>(string url, Action<T> onResult) where T : new()
        {
            var progress = new TaskProgress(2);
            yield return progress.Next(url);

            using var webRequest = UnityWebRequest.Get(url);
            var request = webRequest.SendWebRequest();
            while (!request.isDone) yield return progress;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(url + ": Error: " + webRequest.error);
                    onResult(new T());
                    yield break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(url + ": HTTP Error: " + webRequest.error);
                    onResult(new T());
                    yield break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(url + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }

            yield return progress.Next("Deserializing JSON...");
            onResult(JsonUtility.FromJson<T>(webRequest.downloadHandler.text));
            yield return progress.Complete();
        }
    }
}