using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.EditorCoroutines.Editor;

namespace TwistCore.ProgressWindow.Editor
{
    public static class TaskManager
    {
        private const float WaitSecondsAfterAllTasksDone = 1;
        
        public static readonly Queue<TwistTask> Queue = new Queue<TwistTask>();
        public static readonly List<string> Logs = new List<string>();

        public static TwistTask CurrentTask;
        
        [UsedImplicitly]
        private static EditorCoroutine _queueRunnerCoroutine;
        private static ProgressWindow _window;
        private static List<Action> _completionActions = new List<Action>();


        public static void AddLogs(string text)
        {
            Logs.Add(CurrentTask.Description + ": " + text);
        }

        /// <summary>
        ///     Actions that will be performed synchronously after all tasks are done:
        ///     use for coroutine-breaking things like script recompilation.
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteOnCompletion(Action action)
        {
            _completionActions.Add(action);
        }

        public static void Enqueue(IEnumerator<TaskProgress> coroutine, string description,
            Action completionAction = null)
        {
            Queue.Enqueue(new TwistTask(coroutine, description));
            if (completionAction != null) _completionActions.Add(completionAction);

            if (_window == null)
            {
                Logs.Clear();
                _window = ProgressWindow.ShowWindow();
            }

            _queueRunnerCoroutine ??= EditorCoroutineUtility.StartCoroutineOwnerless(
                ExecuteTasks());
        }

        private static IEnumerator ExecuteTasks()
        {
            while (CurrentTask is { Completed: false }) yield return null;
            while (Queue.Count > 0)
            {
                CurrentTask = Queue.Dequeue();
                yield return CurrentTask.Execute(_window);
            }

            yield return new EditorWaitForSeconds(WaitSecondsAfterAllTasksDone);
            _window.Close();

            foreach (var action in _completionActions)
                action?.Invoke();
            _completionActions = new List<Action>();
        }
    }
}