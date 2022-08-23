namespace Logic.Release
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Helpers;

    public class CommitAndReleaseHandler
    {
        public static void Release(string currentVersion, string path, string version, string commitMessage, Action callback)
        {
            AddToChangeLogFile(path, version, commitMessage);
            ChangeVersionInPackageFile(path, currentVersion, version);
            ExecuteCommitCommand(path, version, commitMessage, () =>
            {
                CreateReleaseViaAPI(callback);
            });
        }

        private static void AddToChangeLogFile(string path, string version, string commitMessage)
        {
            string jsonText = File.ReadAllText(path + "CHANGELOG.md");
            jsonText += "\n\n";
            jsonText += $"{version}   {commitMessage}";
            File.WriteAllText(path + "CHANGELOG.md", jsonText);
        }

        private static void ChangeVersionInPackageFile(string path, string oldVersion, string newVersion)
        {
            string jsonText = File.ReadAllText(path + "package.json");
            var oldString = "version" + "\"" + ": " + "\"" + $"{oldVersion}";
            var newString = "version" + "\"" + ": " + "\"" + $"{newVersion}";
            var newPackageFileText = jsonText.Replace(oldString, newString);
            File.WriteAllText(path + "package.json", newPackageFileText);
        }
        
        private static void ExecuteCommitCommand(string path, string version, string commitMessage, Action callback)
        {
            var shellScriptPath = ShellScriptHelper.RetrieveShellScriptPath();
            
            var firstArg = path;
            var secondArg = $" 'v{version} released: {commitMessage}'";
            var arguments = firstArg + secondArg;
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = shellScriptPath;
            p.StartInfo.Arguments = arguments;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            
            callback.Invoke();
        }

        private static void CreateReleaseViaAPI(Action callback)
        {
            callback.Invoke();
        }
    }
}
