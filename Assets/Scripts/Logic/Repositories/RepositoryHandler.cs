namespace Logic.Repositories
{
    using System.IO;
    using Helpers;
    using UnityEngine;

    public class RepositoryHandler
    {
        private DataBaseHelper dataBaseHelper = new DataBaseHelper();
        

        public void ReadReposDataBase(out DataBaseFormat result)
        {
            dataBaseHelper.ParseTxtIntoType(Application.persistentDataPath + dataBaseHelper.DataBaseFileName, out DataBaseFormat parsedResult);
            result = parsedResult;
        }
        
        public void AddRepositoryLink(string path, string name)
        {
            dataBaseHelper.CreateDatabaseForReposIfNeeded();
            
            dataBaseHelper.ParseTxtIntoType(Application.persistentDataPath + dataBaseHelper.DataBaseFileName, out DataBaseFormat parsedResult);
            parsedResult.Reps.Add(path, name);
            dataBaseHelper.SerializeJsonIntoText(Application.persistentDataPath + dataBaseHelper.DataBaseFileName, parsedResult);
        }

        public void RemoveRepositoryLink(string path, string name)
        {
            dataBaseHelper.ParseTxtIntoType(Application.persistentDataPath + dataBaseHelper.DataBaseFileName, out DataBaseFormat parsedResult);
            parsedResult.Reps.Remove(path);
            dataBaseHelper.SerializeJsonIntoText(Application.persistentDataPath + dataBaseHelper.DataBaseFileName, parsedResult);
        }
    }
}
