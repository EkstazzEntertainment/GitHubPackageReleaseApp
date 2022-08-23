namespace Logic.Release
{
    using System.IO;

    public class CommitAndReleaseHandler
    {
        public static void Release(string path, string version, string commitMessage)
        {
            AddToChangeLogFile(path, version, commitMessage);
        }

        private static void AddToChangeLogFile(string path, string version, string commitMessage)
        {
            string jsonText = File.ReadAllText(path + "CHANGELOG.md");
            File.WriteAllText(path + "CHANGELOG.md", jsonText);
        }
    }
}
