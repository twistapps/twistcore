#if EDITOR_COROUTINES
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TwistCore.Editor
{
    public class ProgressWindow : EditorWindow
    {
        private void OnGUI()
        {
            var currentTask = TaskManager.CurrentTask;
            var progressBarRect = new Rect(3, 10, 343, 25);

            var statusText = "Idle";
            if (currentTask != null)
                statusText =
                    $"{currentTask.Description}: [{currentTask.Progress.CurrentStep}/{currentTask.Progress.TotalSteps}]";
            else if (TaskManager.Queue.Count < 1 && TaskManager.QueueRunnerCoroutine == null)
                Close();

            EditorGUI.ProgressBar(progressBarRect, currentTask?.ProgressAmount ?? 0, statusText);
            EditorGUILayout.Space(40);

            var logs = TaskManager.Logs.Select(l => $"{l.Title}: {l.Text}").ToArray();
            for (var i = logs.Length - 1; i >= 0; i--)
                EditorGUILayout.LabelField(logs[i]);
        }

        public static ProgressWindow ShowWindow()
        {
            var window = GetWindow<ProgressWindow>(true);
            window.titleContent = new GUIContent("Executing tasks...");
            var windowSize = new Vector2(350, 140);
            window.minSize = windowSize;
            window.maxSize = windowSize;
            return window;
        }

#if TWISTCORE_DEBUG
        [MenuItem("Tools/Twist Apps/Commands/Test Progress Window")]
        public static void OnMenuItemClick()
        {
            const int seconds = 3;
            TaskManager.Enqueue(TasksCommon.Sleep(seconds), $"Sleeping for {seconds}s");
        }
#endif
    }
}
#endif