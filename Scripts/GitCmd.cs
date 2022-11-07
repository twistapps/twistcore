using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace TwistCore
{
    public static class GitCmd
    {
        public static string GetVersion()
        {
            //returns git version 2.33.0.windows.1
            var str = ExecuteCommand("Packages", "--version");

            //keep just the numerical value separated by '.'
            str = str.Replace("git version ", string.Empty);
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsDigit(c) || char.IsPunctuation(c)) continue;
                str = str.Substring(0, i - 1);
                break;
            }

            return str;
        }

        public static string ExecuteCommand(string workingDirectory, string command)
        {
            var processInfo = new ProcessStartInfo("git", command)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory
            };

            // Set up the Process
            var process = new Process
            {
                StartInfo = processInfo
            };

            try
            {
                process.Start();
            }
            catch (Exception e)
            {
                Debug.LogError("Git is not set-up correctly, required to be on PATH, and to be a git project.");
                Debug.Log(e.Message);
                throw;
            }


            var output = process.StandardOutput.ReadToEnd();
            var errorOutput = process.StandardError.ReadToEnd();

            process.WaitForExit();
            process.Close();


            if (output.Contains("fatal") || output == "no-git" || output == "")
                throw new Exception("Command: git " + command + " Failed\n" + output + errorOutput);

            if (errorOutput != "") Debug.LogError("Git Error: " + errorOutput);

            return output;
        }
    }
}