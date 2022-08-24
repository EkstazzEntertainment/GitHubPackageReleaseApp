namespace Logic.Helpers
{
    using System.IO;
    using Newtonsoft.Json;

    public class DataBaseHelper
    {
        public string DataBaseFileName = "/ReleasePackages.txt";

        
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
        
        public void CreateFile(string path)
        {
            StreamWriter writer = new StreamWriter(path, true);
            writer.Close();
        }
    }
}
