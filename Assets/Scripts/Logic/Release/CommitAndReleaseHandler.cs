namespace Logic.Release
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using Helpers;
    using UnityEngine;
    using UnityEngine.Networking;

    public class CommitAndReleaseHandler
    {
        public static void Release(string packName, string currentVersion, string path, string version, string commitMessage, Action callback)
        {
            AddToChangeLogFile(path, version, commitMessage);
            ChangeVersionInPackageFile(path, currentVersion, version);
            ExecuteCommitCommand(path, version, commitMessage, () =>
            {
                CreateReleaseViaAPI(callback, packName, version);
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

        private static void CreateReleaseViaAPI(Action callback, string package, string newVersion)
        {
            var url = $"https://api.github.com/repos/EkstazzEntertainment/{package}/releases";
            
            WWWForm form = new WWWForm();
            AddBody(ref form, newVersion);
            
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            AddHeadersToRequest(ref uwr, BuildGetHeaders("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"));
            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                Thread.Sleep(50);
            }

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                callback.Invoke();
            }
            else
            {
                callback.Invoke();
            }
            
            uwr.Dispose();
        }

        private static void AddBody(ref WWWForm form, string newVersion)
        {
            form.AddField("tag_name", newVersion);
            form.AddField("target_commitish", "develop");
            form.AddField("name", newVersion);
            form.AddField("body", "default");
            form.AddField("draft", 0);
            form.AddField("prerelease", 0);
            form.AddField("generate_release_notes", 0);
        }
        
        private static void AddHeadersToRequest(ref UnityWebRequest uwr, List<Header> headers)
        {
            foreach (var header in headers)
            {
                uwr.SetRequestHeader(header.Name, header.Value);
            }
        }
        
        private static List<Header> BuildGetHeaders(string token)
        {
            List<Header> headers = new List<Header>
            {
                new Header {Name = "Accept", Value = "application/vnd.github+json"},
                new Header {Name = "Authorization", Value = "token " + token}
            };
            return headers;
        }
    }
    
    public class Header
    {
        public string Name;
        public string Value;
    }
}
