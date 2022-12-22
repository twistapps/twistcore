using System.Collections.Generic;

namespace TwistCore.ProgressWindow.Editor
{
    public static class TwistTaskFramework
    {
        public static TaskProgress FinishSynchronously(this IEnumerator<TaskProgress> task)
        {
            var progress = task.Current;
            while (task.MoveNext())
            {
                progress = task.Current;
            }

            return progress;
        }
    }
}
