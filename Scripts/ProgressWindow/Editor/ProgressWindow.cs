using UnityEditor;
using UnityEngine;

namespace TwistCore.ProgressWindow.Editor
{
    public class ProgressWindow : EditorWindow
    {
        public static ProgressWindow ShowWindow()
        {
            var window = GetWindow<ProgressWindow>(true);
            window.titleContent = new GUIContent("Executing tasks...");
            var windowSize = new Vector2(350, 140);
            window.minSize = windowSize;
            window.maxSize = windowSize;
            return window;
        }
        
        private void OnGUI()
        {
            var currentTask = TaskManager.CurrentTask;
            var progressBarRect = new Rect(3, 10, 343, 25);

            var statusText = "Idle";
            if (currentTask != null)
                statusText =
                    $"{currentTask.Description}: [{currentTask.Progress.CurrentStep}/{currentTask.Progress.TotalSteps}]";
            else if (TaskManager.Queue.Count < 1)
                Close();
            
            EditorGUI.ProgressBar(progressBarRect, currentTask?.ProgressPercentage ?? 0, statusText);
            EditorGUILayout.Space(40);
            
            var logs = TaskManager.Logs;
            for (var i = logs.Count-1; i >= 0; i--)
                EditorGUILayout.LabelField(logs[i]);
        }

        [MenuItem("Tools/Twist Apps/Progress Window")]
        public static void OnMenuItemClick()
        {
            const int seconds = 3;
            TaskManager.Enqueue(CommonTasks.Sleep(seconds), $"Sleeping for {seconds}s");
        }
    }
}