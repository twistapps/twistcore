using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
#if EDITOR_COROUTINES
using Unity.EditorCoroutines.Editor;
#endif

namespace TwistCore.Editor
{
    public static class TaskManager
    {
#if EDITOR_COROUTINES
        private const float WaitSecondsAfterAllTasksDone = .4f;

        public static readonly Queue<Task> Queue = new Queue<Task>();
        public static readonly List<TaskLogs> Logs = new List<TaskLogs>();

        public static Task CurrentTask;

        [UsedImplicitly] public static EditorCoroutine QueueRunnerCoroutine;

        private static ProgressWindow _window;

        public static void Enqueue(IEnumerator<TaskProgress> coroutine, string description,
            Action onComplete = null)
        {
            Queue.Enqueue(new Task(coroutine, description));
            if (onComplete != null) OnComplete.Add(onComplete);

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
            }

            yield return new EditorWaitForSeconds(WaitSecondsAfterAllTasksDone);
            _window.Close();

            CurrentTask = null;

            InvokeAllOnCompleteActions();

            EditorCoroutineUtility.StopCoroutine(QueueRunnerCoroutine);
            QueueRunnerCoroutine = null;
        }

        internal static void GatherLogsFrom(IEnumerator<TaskProgress> task)
        {
            foreach (var log in task.Current.Logs)
                Logs.Add(new TaskLogs
                {
                    Title = CurrentTask.Description,
                    Text = log.Text
                });
            task.Current.Logs.Clear();
        }
#else
        private static bool _hasInProgressTasks = false;
        
        public static void Enqueue(IEnumerator<TaskProgress> coroutine, string description,
            Action onComplete = null)
        {
            _hasInProgressTasks = true;
            if (onComplete != null) OnComplete.Add(onComplete);
            coroutine.FinishSynchronously();
            InvokeAllOnCompleteActions();
            _hasInProgressTasks = false;
        }

        public static void AddLogs(string text)
        {
        }
#endif
        private static readonly List<Action> OnComplete = new List<Action>();

        /// <summary>
        ///     Actions that will be performed synchronously after all tasks are done:
        ///     use for coroutine-breaking things like script recompilation.
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteOnCompletion(Action action)
        {
            OnComplete.Add(action);

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
            foreach (var action in OnComplete)
                action?.Invoke();
            OnComplete.Clear();
        }
    }
}