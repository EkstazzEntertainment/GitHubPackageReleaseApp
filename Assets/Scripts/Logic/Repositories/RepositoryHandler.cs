namespace Logic.Repositories
{
    using System.IO;
    using Helpers;
    using UnityEditor;
    using UnityEngine;

    public class RepositoryHandler
    {
        private const string DataBaseFileName = "/ReleasePackages.txt";


        public static void ReadReposDataBase(out DataBaseFormat result)
        {
            DataBaseHelper.ParseTxtIntoType(Application.persistentDataPath + DataBaseFileName, out DataBaseFormat parsedResult);
            result = parsedResult;
        }
        
        public static void AddRepositoryLink(string path, string name)
        {
            CreateDataBaseIfNeeded();
            
            DataBaseHelper.ParseTxtIntoType(Application.persistentDataPath + DataBaseFileName, out DataBaseFormat parsedResult);
            parsedResult.Reps.Add(path, name);
            DataBaseHelper.SerializeJsonIntoText(Application.persistentDataPath + DataBaseFileName, parsedResult);
        }

        public static void RemoveRepositoryLink(string path, string name)
        {
            
        }

        private static void CreateDataBaseIfNeeded()
        {
            if (!File.Exists(Application.persistentDataPath + DataBaseFileName))
            {
                DataBaseHelper.CreateFile(Application.persistentDataPath + DataBaseFileName);
                
                var initialStructure = new DataBaseFormat();
                DataBaseHelper.SerializeJsonIntoText(Application.persistentDataPath + DataBaseFileName, initialStructure);
            }
        }
    }
}
