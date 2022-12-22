namespace TwistCore.ProgressWindow.Editor
{
    public class TaskProgress
    {
        public int CurrentStep;

        public float ShouldSleepForSeconds;
        public int TotalSteps;
        
        public string LatestLog { get; private set; }

        public TaskProgress(int totalSteps = 0)
        {
            TotalSteps = totalSteps;
        }

        public TaskProgress Log(string text)
        {
            LatestLog = text;
            TaskManager.AddLogs(text);
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