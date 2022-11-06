using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;

namespace TwistCore.ProgressWindow.Editor
{
    public class TwistTask
    {
        private readonly IEnumerator<TaskProgress> _coroutine;
        public readonly string Description;
        public bool Completed;
        public float ProgressAmount;
        
        public TaskProgress Progress => _coroutine.Current ?? new TaskProgress();

        public TwistTask(IEnumerator<TaskProgress> coroutine, string description)
        {
            _coroutine = coroutine;
            Description = description;
        }

        public IEnumerator Execute(ProgressWindow window)
        {
            while (_coroutine.MoveNext())
            {
                ProgressAmount = (float)Progress.CurrentStep / Progress.TotalSteps;
                window.Repaint();
                
                yield return Progress.ShouldSleepForSeconds > 0
                    ? new EditorWaitForSeconds(Progress.ShouldSleepForSeconds)
                    : null;
            }

            Completed = true;
            window.Repaint();
        }
    }
}