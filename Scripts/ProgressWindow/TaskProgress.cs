using System.Collections.Generic;

namespace TwistCore.ProgressWindow
{
    public struct TaskLogs
    {
        public string title;
        public string text;
    }

    public class TaskProgress
    {
        // public delegate void LogsAddedHandler(TaskProgress task, string logs);
        //
        // public LogsAddedHandler onAddLogs;

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
            //TaskManager.AddLogs(text);
            Logs.Add(new TaskLogs { text = text });
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