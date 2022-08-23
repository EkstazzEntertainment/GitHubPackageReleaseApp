namespace Logic.Release
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class CommitAndReleaseHandler
    {
        public static void Release(string path, string version, string commitMessage)
        {
            AddToChangeLogFile(path, version, commitMessage);
            ExecuteCommitCommand(path, version, commitMessage);
        }

        private static void AddToChangeLogFile(string path, string version, string commitMessage)
        {
            string jsonText = File.ReadAllText(path + "CHANGELOG.md");
            jsonText += "\n\n";
            jsonText += $"{version}   {commitMessage}";
            File.WriteAllText(path + "CHANGELOG.md", jsonText);
        }

        private static void ExecuteCommitCommand(string path, string version, string commitMessage)
        {
            var firstArg = path;
            var secondArg = $" 'v{version} released: {commitMessage}'";
            var arguments = firstArg + secondArg;
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "/Users/yuriialeksanian/Desktop/MyApps/CustomShellScripts/CommitRelease/CommitRelease";
            p.StartInfo.Arguments = arguments;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }
    }
}
