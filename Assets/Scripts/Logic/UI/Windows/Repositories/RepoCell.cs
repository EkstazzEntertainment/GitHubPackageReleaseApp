namespace Assets.Scripts.Logic.UI.Windows.Repositories
{
    using System;
    using global::Logic.Helpers;
    using global::Logic.Repositories;
    using Structures;
    using TMPro;
    using UnityEngine;
 
    
    public class RepoCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI packageName;
        [SerializeField] private TextMeshProUGUI version;

        public event Action OnRepoRemoved;
        public event Action<string, string, string> OnRepoSelected;
        
        private string CachedPath;


        public void Init(string path, string packName)
        {
            CachedPath = path;

            packageName.text = packName;
            DataBaseHelper.ParseTxtIntoType(path + "package.json", out PackageJson parsedResult);
            version.text = parsedResult.version;
        }
        
        public void OnClickButton()
        {
            OnRepoSelected?.Invoke(CachedPath, packageName.text, version.text);
        }

        public void DeleteButton()
        {
            RepositoryHandler.RemoveRepositoryLink(CachedPath, packageName.text);
            OnRepoRemoved?.Invoke();
        }
    }
}
