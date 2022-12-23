﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
#if EDITOR_COROUTINES
using System.Collections;
using Unity.EditorCoroutines.Editor;
#endif

namespace TwistCore.ProgressWindow.Editor
{
    public static class TaskManager
    {
#if EDITOR_COROUTINES
        private const float WaitSecondsAfterAllTasksDone = 1;

        public static readonly Queue<Task> Queue = new Queue<Task>();
        public static readonly List<string> Logs = new List<string>();

        public static Task CurrentTask;

        [UsedImplicitly] public static EditorCoroutine QueueRunnerCoroutine;

        private static ProgressWindow _window;


        public static void AddLogs(string text)
        {
            Logs.Add(CurrentTask.Description + ": " + text);
        }

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
            }

            yield return new EditorWaitForSeconds(WaitSecondsAfterAllTasksDone);
            _window.Close();

            CurrentTask = null;

            InvokeAllOnCompleteActions();

            EditorCoroutineUtility.StopCoroutine(QueueRunnerCoroutine);
            QueueRunnerCoroutine = null;
        }
#else
        public static void Enqueue(IEnumerator<TaskProgress> coroutine, string description,
            Action onComplete = null)
        {
            if (onComplete != null) _onComplete.Add(onComplete);
            coroutine.FinishSynchronously();
            InvokeAllOnCompleteActions();
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