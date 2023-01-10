#if EDITOR_COROUTINES
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;

namespace TwistCore.Editor
{
    public class Task
    {
        private readonly IEnumerator<TaskProgress> _coroutine;
        public readonly string Description;
        public bool Completed;
        public float ProgressAmount;

        public Task(IEnumerator<TaskProgress> coroutine, string description)
        {
            _coroutine = coroutine;
            Description = description;
        }

        public TaskProgress Progress => _coroutine.Current ?? new TaskProgress();

        public IEnumerator Execute(ProgressWindow window)
        {
            while (_coroutine.MoveNext())
            {
                ProgressAmount = (float)Progress.CurrentStep / Progress.TotalSteps;
                window.Repaint();

                TaskManager.GatherLogsFrom(_coroutine);

                yield return Progress.ShouldSleepForSeconds > 0
                    ? new EditorWaitForSeconds(Progress.ShouldSleepForSeconds)
                    : null;

                Progress.ShouldSleepForSeconds = 0;
            }

            Completed = true;
            window.Repaint();
        }
    }
}
#endif