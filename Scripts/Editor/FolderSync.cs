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

        public static void Clone(string path, string oldRoot, string newRoot, string ignoreMask=null)
        {
            CloneTask(path, oldRoot, newRoot, ignoreMask).FinishSynchronously();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="oldRoot"></param>
        /// <param name="newRoot"></param>
        /// <param name="ignoreMask">Filter all files containing this string. Add * in the beginning to check files' extension only.</param>
        /// <param name="initialTotalSteps"></param>
        /// <returns></returns>
        public static IEnumerator<TaskProgress> CloneTask(string path, string oldRoot, string newRoot, string ignoreMask=null, int initialTotalSteps=0)
        {
            var progress = new TaskProgress(initialTotalSteps);
            yield return progress.Log("Counting files...");

            // var fileCount = (from file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            //     select file).Count();
            var fileCount = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Count();
            progress = new TaskProgress(fileCount);
            yield return progress.Log($"Found {fileCount} to clone").Log(DoneCountingFiles).Sleep(1);
            yield return progress.Log(CopyingFilesSlug);

            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (ignoreMask != null)
                {
                    if (ignoreMask.StartsWith("*"))
                    {
                        if (file.EndsWith(ignoreMask.Substring(1)))
                        {
                            yield return progress.Next();
                            continue;
                        }
                    }
                    else if (file.Contains(ignoreMask))
                    {
                        yield return progress.Next();
                        continue;
                    }
                }
                CloneFile(file, oldRoot, newRoot);
                yield return progress.Next();
            }

            yield return progress.Complete();
        }

        public static void SyncFolders(string outputFolder, params string[] inputFolders)
        {
            SyncFoldersTask(outputFolder, inputFolders).FinishSynchronously();
        }

        public static IEnumerator<TaskProgress> SyncFoldersTask(string outputFolder,  bool ignoreMetafiles, params string[] inputFolders)
        {
            const string metafileMask = "*.meta"; 
            
            // overallProgress = new TaskProgress(inputFolders.Length);
            var progress = new TaskProgress(inputFolders.Length);
            yield return progress.Log($"Syncing {inputFolders.Length} folders...").Sleep(1.3f);

            var ignoreMask = ignoreMetafiles ? "*.meta" : null;
            
            var previousStepsSummary = 0;
            foreach (var inputFolder in inputFolders)
            {
                var cloneTask = CloneTask(inputFolder, inputFolder, outputFolder, ignoreMask, inputFolders.Length);
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

                if (cloneTask.Current != null) previousStepsSummary += cloneTask.Current.TotalSteps;

                yield return progress.Log($"[{progress.CurrentStep}/{progress.TotalSteps}] Cloned").Sleep(.8f);
            }
        }

        public static IEnumerator<TaskProgress> SyncFoldersTask(string outputFolder,
            params string[] inputFolders)
        {
            return SyncFoldersTask(outputFolder, false, inputFolders);
        }
    }
}