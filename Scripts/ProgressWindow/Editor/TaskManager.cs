using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#if EDITOR_COROUTINES
#endif

namespace TwistCore.ProgressWindow.Editor
{
    public static class TaskManager
    {
#if EDITOR_COROUTINES
        private const float WaitSecondsAfterAllTasksDone = 1;

        public static readonly Queue<Task> Queue = new Queue<Task>();
        public static readonly List<TaskLogs> Logs = new List<TaskLogs>();

        public static Task CurrentTask;

        [UsedImplicitly] public static EditorCoroutine QueueRunnerCoroutine;

        private static ProgressWindow _window;


        // public static void AddLogs(string text)
        // {
        //     Logs.Add(CurrentTask.Description + ": " + text);
        // }

        public static void Enqueue(IEnumerator<TaskProgress> coroutine, string description,
            Action onComplete = null)
        {
            Queue.Enqueue(new Task(coroutine, description));
            if (onComplete != null) _onComplete.Add(onComplete);

            if (_window == null)
            {
                Logs.Clear();
                _window = ProgressWindow.ShowWindow();
            }

            QueueRunnerCoroutine ??= EditorCoroutineUtility.StartCoroutineOwnerless(
                ExecuteTasks());
        }

        private static IEnumerator ExecuteTasks()
        {
            while (CurrentTask is { Completed: false }) yield return null;
            while (Queue.Count > 0)
            {
                CurrentTask = Queue.Dequeue();
                yield return CurrentTask.Execute(_window);
                GatherLogsFrom(CurrentTask);
            }

            yield return new EditorWaitForSeconds(WaitSecondsAfterAllTasksDone);
            _window.Close();

            CurrentTask = null;

            InvokeAllOnCompleteActions();

            EditorCoroutineUtility.StopCoroutine(QueueRunnerCoroutine);
            QueueRunnerCoroutine = null;
        }

        private static void GatherLogsFrom(Task task)
        {
            foreach (var log in task.Progress.Logs)
            {
                Logs.Add(new TaskLogs
                {
                    title = CurrentTask.Description,
                    text = log.text
                });
            }
            task.Progress.Logs.Clear();
        }
#else
        private static bool _hasInProgressTasks = false;
        
        public static void Enqueue(IEnumerator<TaskProgress> coroutine, string description,
            Action onComplete = null)
        {
            _hasInProgressTasks = true;
            if (onComplete != null) _onComplete.Add(onComplete);
            coroutine.FinishSynchronously();
            InvokeAllOnCompleteActions();
            _hasInProgressTasks = false;
        }

        public static void AddLogs(string text)
        {
        }
#endif
        private static List<Action> _onComplete = new List<Action>();

        /// <summary>
        ///     Actions that will be performed synchronously after all tasks are done:
        ///     use for coroutine-breaking things like script recompilation.
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteOnCompletion(Action action)
        {
            _onComplete.Add(action);

#if EDITOR_COROUTINES
            if (CurrentTask == null && Queue.Count == 0)
                InvokeAllOnCompleteActions();
#else
            if (!_hasInProgressTasks)
                InvokeAllOnCompleteActions();
#endif
        }

        private static void InvokeAllOnCompleteActions()
        {
            foreach (var action in _onComplete)
                action?.Invoke();
            _onComplete = new List<Action>();
        }
    }
}