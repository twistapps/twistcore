using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;

namespace TwistCore.ProgressWindow.Editor
{
    public class TaskProgress
    {
        public int CurrentStep;

        public float ShouldSleepForSeconds;
        public int TotalSteps;

        public TaskProgress(int totalSteps = 0)
        {
            TotalSteps = totalSteps;
        }

        public TaskProgress Log(string text)
        {
            TaskManager.AddLogs(text);
            return this;
        }

        public TaskProgress Sleep(float seconds)
        {
            ShouldSleepForSeconds = seconds;
            return this;
        }

        public TaskProgress Next(string logs = null)
        {
            CurrentStep++;
            if (logs != null)
                Log(logs);
            return this;
        }
    }

    public class TwistTask
    {
        private readonly IEnumerator<TaskProgress> _task;
        public readonly string Description;
        public bool Completed;
        public float ProgressPercentage;

        public TwistTask(IEnumerator<TaskProgress> task, string description)
        {
            Description = description;
            _task = task;
        }

        public TaskProgress Progress => _task.Current ?? new TaskProgress();

        public IEnumerator Execute(ProgressWindow window)
        {
            while (_task.MoveNext())
            {
                ProgressPercentage = (float)Progress.CurrentStep / Progress.TotalSteps;
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