namespace Logic.Release
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using Helpers;
    using Newtonsoft.Json;
    using Debug = UnityEngine.Debug;

    
    public class CommitAndReleaseHandler
    {
        public static void Release(string packName, string currentVersion, string path, string version, string commitMessage, Action callback)
        {
            AddToChangeLogFile(path, version, commitMessage);
            ChangeVersionInPackageFile(path, currentVersion, version);
            ExecuteCommitCommand(path, version, commitMessage, () =>
            {
                CreateReleaseViaAPI(callback, packName, version, commitMessage);
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
 
        private static async void CreateReleaseViaAPI(Action callback, string package, string newVersion, string commitMessage)
        {
            var url = $"https://api.github.com/repos/EkstazzEntertainment/{package}/releases";
          
            HttpClient client = new HttpClient();
            AddHeadersToRequest(ref client, BuildHeaders("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"));
            var content = new StringContent(AddBody(newVersion, commitMessage));
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            Debug.Log(responseString);
            callback.Invoke();
        }

        private static string AddBody(string newVersion, string commitMessage)
        {
            var requestBody = new RequestBody { tag_name = newVersion, name = newVersion, body = commitMessage};
            var finalString = JsonConvert.SerializeObject(requestBody, Formatting.Indented);

            return finalString;
        }
        
        private static List<Header> BuildHeaders(string token)
        {
            List<Header> headers = new List<Header>
            {
                new Header {Name = "Accept", Value = "application/vnd.github+json"},
                new Header {Name = "Authorization", Value = "token " + token},
                new Header {Name = "User-Agent", Value = "PostmanRuntime/7.29.2"},
            };
            return headers;
        }
        
        private static void AddHeadersToRequest(ref HttpClient client, List<Header> headers)
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


