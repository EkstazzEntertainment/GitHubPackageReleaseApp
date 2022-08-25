namespace Logic.Release
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Helpers;
    using Newtonsoft.Json;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    
    public class CommitAndReleaseHandler
    {
        private DataBaseHelper dataBaseHelper = new DataBaseHelper();
        
        
        public void Release(string packName, string currentVersion, string path, string version, string commitMessage, Action callback)
        {
            AddToChangeLogFile(path, version, commitMessage);
            ChangeVersionInPackageFile(path, currentVersion, version);
            ExecuteCommitCommand(path, version, commitMessage, () =>
            {
                CreateReleaseViaAPI(callback, packName, version, commitMessage);
            });
        }

        private void AddToChangeLogFile(string path, string version, string commitMessage)
        {
            string jsonText = File.ReadAllText(path + "CHANGELOG.md");
            jsonText += "\n\n";
            jsonText += $"{version}   {commitMessage}";
            File.WriteAllText(path + "CHANGELOG.md", jsonText);
        }

        private void ChangeVersionInPackageFile(string path, string oldVersion, string newVersion)
        {
            string jsonText = File.ReadAllText(path + "package.json");
            var oldString = "version" + "\"" + ": " + "\"" + $"{oldVersion}";
            var newString = "version" + "\"" + ": " + "\"" + $"{newVersion}";
            var newPackageFileText = jsonText.Replace(oldString, newString);
            File.WriteAllText(path + "package.json", newPackageFileText);
        }
        
        private async void ExecuteCommitCommand(string path, string version, string commitMessage, Action callback)
        {
            var shellScriptPath = dataBaseHelper.ReadTextFromFile(Application.persistentDataPath + dataBaseHelper.ShellFileName);
            var firstArg = path;
            var secondArg = $" 'v{version} released: {commitMessage}'";
            var thirdArg = " " + shellScriptPath;
            var arguments = firstArg + secondArg + thirdArg;
            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = true; //false
                p.StartInfo.RedirectStandardOutput = false; //true
                Debug.Log(shellScriptPath + "/StartCommitReleasing");
                p.StartInfo.FileName = shellScriptPath + "/StartCommitReleasing";
                p.StartInfo.Arguments = arguments;
                p.Start();
                // string output = p.StandardOutput.ReadToEnd();
                // p.WaitForExit();
                // Debug.Log(output);
                // p.Close();
                
                //the comments do not work since GIT PUSH works only in first launch of the app but somehow fails in the succeeding ones.
            }

            await Task.Delay(20000);
            
            callback.Invoke();
        }
 
        private async void CreateReleaseViaAPI(Action callback, string package, string newVersion, string commitMessage)
        {
            var url = $"https://api.github.com/repos/EkstazzEntertainment/{package}/releases";
          
            HttpClient client = new HttpClient();
            AddHeadersToRequest(
                ref client, 
                BuildHeaders(dataBaseHelper.ReadTextFromFile(Application.persistentDataPath + dataBaseHelper.TokenFileName)));
            var content = new StringContent(AddBody(newVersion, commitMessage));
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            Debug.Log(responseString);
            callback.Invoke();
        }

        private string AddBody(string newVersion, string commitMessage)
        {
            var requestBody = new RequestBody { tag_name = newVersion, name = newVersion, body = commitMessage};
            var finalString = JsonConvert.SerializeObject(requestBody, Formatting.Indented);

            return finalString;
        }
        
        private List<Header> BuildHeaders(string token)
        {
            List<Header> headers = new List<Header>
            {
                new Header {Name = "Accept", Value = "application/vnd.github+json"},
                new Header {Name = "Authorization", Value = "token " + token},
                new Header {Name = "User-Agent", Value = "PostmanRuntime/7.29.2"},
            };
            return headers;
        }
        
        private void AddHeadersToRequest(ref HttpClient client, List<Header> headers)
        {
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Name, header.Value);
            }
        }
    }
    
    [Serializable]
    public class Header
    {
        public string Name;
        public string Value;
    }

    [Serializable]
    public class RequestBody
    {
        public string tag_name;
        public string target_commitish = "develop";
        public string name;
        public string body = "default";
    }
}


