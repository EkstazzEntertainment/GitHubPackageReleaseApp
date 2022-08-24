namespace Logic.Helpers
{
    using UnityEngine;

    public class ShellScriptHelper
    {
        private const string SaveKey = "ShellScriptPathKey";
        
        public static void SaveShellScriptPath(string path)
        {
            PlayerPrefs.SetString(SaveKey, path);
            PlayerPrefs.Save();
        }
        
        public static string RetrieveShellScriptPath()
        {
            return PlayerPrefs.GetString(SaveKey);
        }
    }
}
