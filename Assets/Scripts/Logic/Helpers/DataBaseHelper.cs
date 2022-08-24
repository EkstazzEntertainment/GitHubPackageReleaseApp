namespace Logic.Helpers
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Repositories;
    using UnityEngine;

    
    public class DataBaseHelper
    {
        public string DataBaseFileName = "/ReleasePackages.txt";
        public string TokenFileName = "/Token.txt";
        public string ShellFileName = "/ShellScriptPath.txt";

        
        public void ParseTxtIntoType<T>(string txtPath, out T parsedResult)
        {
            string jsonText = File.ReadAllText(txtPath);
            parsedResult = JsonConvert.DeserializeObject<T>(jsonText);
        }

        public void SerializeJsonIntoText(string path, object obj)
        {
            var serializedText = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(path, serializedText);
        }

        public void WriteTextToFile(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        public string ReadTextFromFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void CreateDatabaseForReposIfNeeded()
        {
            CreateDataBaseIfNeeded(Application.persistentDataPath + DataBaseFileName, () =>
            {
                SetUpReposDataBaseFormat();
            });
        }

        public void CreateFileForTokenIfNeeded()
        {
            CreateDataBaseIfNeeded(Application.persistentDataPath + TokenFileName, () =>
            {
                
            });
        }

        public void CreateFileForShellScriptPathIfNeeded()
        {
            CreateDataBaseIfNeeded(Application.persistentDataPath + ShellFileName, () =>
            {
                
            });
        }
        
        private void CreateDataBaseIfNeeded(string path, Action callback = null)
        {
            if (!File.Exists(path))
            {
                CreateFile(path);
                callback?.Invoke();
            }
        }
        
        private void CreateFile(string path)
        {
            StreamWriter writer = new StreamWriter(path, true);
            writer.Close();
        }

        private void SetUpReposDataBaseFormat()
        {
            var initialStructure = new DataBaseFormat();
            SerializeJsonIntoText(Application.persistentDataPath + DataBaseFileName, initialStructure);
        }
    }
}
