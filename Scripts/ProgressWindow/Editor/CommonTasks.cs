using System.Collections.Generic;
using UnityEditor;

namespace TwistCore.ProgressWindow.Editor
{
    public static class CommonTasks
    {
        public static IEnumerator<TaskProgress> Sleep(int seconds)
        {
            var progress = new TaskProgress(seconds);

            var startTime = EditorApplication.timeSinceStartup;
            var now = startTime;
            var endTime = startTime + seconds;

            while (now <= endTime)
            {
                var elapsed = (int)(now - startTime);
                if (progress.CurrentStep != elapsed)
                    progress.CurrentStep = elapsed;
                //TaskManager.AddLogs(elapsed + " seconds elapsed.");
                now = EditorApplication.timeSinceStartup;
                yield return progress;
            }

            yield return progress.Complete("Done!");
        }
    }
}