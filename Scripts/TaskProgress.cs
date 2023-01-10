using System.Collections.Generic;

namespace TwistCore
{
    public struct TaskLogs
    {
        public string Title;
        public string Text;
    }

    public class TaskProgress
    {
        public readonly List<TaskLogs> Logs = new List<TaskLogs>();
        public int CurrentStep;

        public float ShouldSleepForSeconds;
        public int TotalSteps;

        public TaskProgress(int totalSteps = 0)
        {
            TotalSteps = totalSteps;
        }

        public string LatestLog { get; private set; }

        public TaskProgress Log(string text)
        {
            LatestLog = text;
            Logs.Add(new TaskLogs { Text = text });
            return this;
        }

        public TaskProgress ClearLatestLogField()
        {
            LatestLog = string.Empty;
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

        public TaskProgress Complete(string logs = null)
        {
            CurrentStep = TotalSteps;
            if (logs != null)
                Log(logs);
            return this;
        }
    }
}