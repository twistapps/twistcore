using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwistCore.ProgressWindow.Editor;

namespace TwistCore
{
    public static class FolderSync
    {
        private const string CopyingFilesSlug = "Copying files...";
        private const string DoneCountingFiles = "(done counting files)";

        private const float SleepTimeBetweenFileCopyActions = .05f;

        /// <summary>
        ///     Trims path to be relative to specified folder.
        /// </summary>
        /// <param name="path">Absolute path to requested file/dir.</param>
        /// <param name="newRootFolder">New root directory. Has to be part of initial path.</param>
        /// <returns>Initial 'path' trimmed to be relative to newRootFolder</returns>
        public static string MakeRelativePath(string path, string newRootFolder)
        {
            var index = path.IndexOf(newRootFolder, StringComparison.Ordinal);
            var newRoot = path.Substring(index + newRootFolder.Length);
            return newRoot.StartsWith(Path.DirectorySeparatorChar.ToString()) ? newRoot.Substring(1) : newRoot;
        }

        /// <summary>
        ///     Given a path to a file, move its root location to newRootFolder, keeping relative path.
        /// </summary>
        /// <param name="pathToFile">Path to modify</param>
        /// <param name="oldRootFolder">Directory after which relative part of the path begins which shouldn't be changed.</param>
        /// <param name="newRootFolder"></param>
        /// <returns></returns>
        public static string ChangeRootDirectoryOfPath(string pathToFile, string oldRootFolder, string newRootFolder)
        {
            var relativePathToFile = MakeRelativePath(pathToFile, oldRootFolder);
            return Path.Combine(newRootFolder, relativePathToFile);
        }

        public static string ChangeRootAndGetDirectory(string pathToFile, string oldRootFolder, string newRootFolder)
        {
            return Path.GetDirectoryName(ChangeRootDirectoryOfPath(pathToFile, oldRootFolder, newRootFolder));
        }

        /// <summary>
        ///     Move the specified file, keeping the original and relative path
        /// </summary>
        /// <param name="pathToFile">File to clone</param>
        /// <param name="oldRootFolder"></param>
        /// <param name="newRootFolder"></param>
        /// <param name="overwrite">should force overwrite file</param>
        public static void CloneFile(string pathToFile, string oldRootFolder, string newRootFolder,
            bool overwrite = true)
        {
            var filename = Path.GetFileName(pathToFile);
            var outputFolderPath = ChangeRootAndGetDirectory(pathToFile, oldRootFolder, newRootFolder);
            Directory.CreateDirectory(outputFolderPath);
            File.Copy(pathToFile, Path.Combine(outputFolderPath, filename), overwrite);
        }

        public static void Clone(string path, string oldRoot, string newRoot)
        {
            CloneTask(path, oldRoot, newRoot).FinishSynchronously();
        }

        public static IEnumerator<TaskProgress> CloneTask(string path, string oldRoot, string newRoot)
        {
            var progress = new TaskProgress(3);
            yield return progress.Log("Counting files...");

            // var fileCount = (from file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            //     select file).Count();
            var fileCount = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Count();
            progress = new TaskProgress(fileCount);
            yield return progress.Log($"Found {fileCount} to be cloned").Log(DoneCountingFiles).Sleep(1);
            yield return progress.Log(CopyingFilesSlug);

            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                CloneFile(file, oldRoot, newRoot);
                yield return progress.Next();
            }

            yield return progress.Complete();
        }

        public static void SyncFolders(string outputFolder, params string[] inputFolders)
        {
            SyncFoldersTask(outputFolder, inputFolders).FinishSynchronously();
        }

        public static IEnumerator<TaskProgress> SyncFoldersTask(string outputFolder, params string[] inputFolders)
        {
            // overallProgress = new TaskProgress(inputFolders.Length);
            var progress = new TaskProgress(inputFolders.Length);
            yield return progress.Log($"Syncing {inputFolders.Length} folders...").Sleep(1.3f);

            var previousStepsSummary = 0;

            foreach (var inputFolder in inputFolders)
            {
                var cloneTask = CloneTask(inputFolder, inputFolder, outputFolder);
                while (cloneTask.MoveNext())
                {
                    var current = cloneTask.Current;
                    if (current == null) continue;

                    if (current.LatestLog == DoneCountingFiles)
                        current.ClearLatestLogField();
                    else
                    {
                        progress.TotalSteps = current.TotalSteps + previousStepsSummary;
                    }
                    progress.CurrentStep = current.CurrentStep + previousStepsSummary;

                    yield return progress.Sleep(current.ShouldSleepForSeconds + SleepTimeBetweenFileCopyActions);
                    current.ShouldSleepForSeconds = 0;
                }

                previousStepsSummary += cloneTask.Current.TotalSteps;

                yield return progress.Log($"[{progress.CurrentStep}/{progress.TotalSteps}] Cloned").Sleep(.8f);
            }
        }
    }
}