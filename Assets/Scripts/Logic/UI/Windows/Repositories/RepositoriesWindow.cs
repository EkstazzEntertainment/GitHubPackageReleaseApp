namespace Assets.Scripts.Logic.UI.Windows.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Logic.Helpers;
    using global::Logic.Release;
    using global::Logic.Repositories;
    using global::Logic.UI.Windows.Repositories;
    using TMPro;
    using UnityEngine;

    
    public class RepositoriesWindow : MonoBehaviour
    {
        [SerializeField] private TMP_InputField shellScriptPathInputField;
        [SerializeField] private TMP_InputField accessTokenInputField;
        [SerializeField] private RepoCell repoCellPrefab;
        [SerializeField] private TMP_InputField repPathInputField;
        [SerializeField] private TMP_InputField repNameInputField;
        [SerializeField] private Transform repListContent;
        [SerializeField] private TextMeshProUGUI packageNameText;
        [SerializeField] private TMP_InputField releaseVersionInputField;
        [SerializeField] private TMP_InputField releaseChangeLogInputField;
        [SerializeField] private WarningPopUp warningPopUp;
        [SerializeField] private GameObject loadingPopup;
        
        private List<RepoCell> cells = new List<RepoCell>();
        private string currentSelectedPath;
        private string currentSelectedVersion;
        private RepositoryHandler repositoryHandler = new RepositoryHandler();
        private CommitAndReleaseHandler commitAndReleaseHandler = new CommitAndReleaseHandler();
        private DataBaseHelper dataBaseHelper = new DataBaseHelper();
        
        
        private void Awake()
        {
            PopulateRepList();
        }

        private void PopulateRepList()
        {
            CleanChildren();
            
            dataBaseHelper.CreateDatabaseForReposIfNeeded();
            repositoryHandler.ReadReposDataBase(out DataBaseFormat result);

            foreach (var rep in result.Reps)
            {
                var instance = Instantiate(repoCellPrefab, repListContent);
                instance.Init(rep.Key, rep.Value);
                instance.OnRepoRemoved += RemoveRep;
                instance.OnRepoSelected += SelectRepo;
                cells.Add(instance);
            }
        }

        private void CleanChildren()
        {
            cells.ForEach(cell =>
            {
                cell.OnRepoRemoved -= RemoveRep;
                cell.OnRepoSelected -= SelectRepo;
                DestroyImmediate(cell.gameObject);
            });
            cells.Clear();
        }

        public void AddShellScriptPathButton()
        {
            dataBaseHelper.CreateFileForShellScriptPathIfNeeded();
            if (!string.IsNullOrWhiteSpace(accessTokenInputField.text))
            {
                dataBaseHelper.WriteTextToFile(Application.persistentDataPath + dataBaseHelper.ShellFileName, shellScriptPathInputField.text);
            }
        }

        public void AddAccessTokenButton()
        {
            dataBaseHelper.CreateFileForTokenIfNeeded();
            if (!string.IsNullOrWhiteSpace(accessTokenInputField.text))
            {
                dataBaseHelper.WriteTextToFile(Application.persistentDataPath + dataBaseHelper.TokenFileName, accessTokenInputField.text);
            }
        }
        
        public void AddRepButton()
        {
            AddRep();
        }

        private void AddRep()
        {
            repositoryHandler.AddRepositoryLink(repPathInputField.text + "/", repNameInputField.text);
            
            PopulateRepList();
        }

        private void RemoveRep()
        {
            PopulateRepList();
        }

        private void SelectRepo(string path, string packName, string version)
        {
            currentSelectedPath = path;
            currentSelectedVersion = version;
            
            packageNameText.text = packName;
            releaseVersionInputField.text = currentSelectedVersion;
        }

        public async void ReleaseButton()
        {
            if (currentSelectedVersion == releaseVersionInputField.text)
            {
                Debug.Log("Choose a different release version");
                warningPopUp.Init();
            }
            else if (string.IsNullOrWhiteSpace(releaseChangeLogInputField.text))
            {
                Debug.Log("Type a commit and ChangeLog message");
                warningPopUp.Init();
            }
            else
            {
                loadingPopup.SetActive(true);
                await Task.Delay(1000);
                
                commitAndReleaseHandler.Release(
                    packageNameText.text,
                    currentSelectedVersion,
                    currentSelectedPath,
                    releaseVersionInputField.text, 
                    releaseChangeLogInputField.text,
                    () =>
                    {
                        loadingPopup.SetActive(false);
                    });
                CleanAfterRelease();
            }
        }

        private void CleanAfterRelease()
        {
            currentSelectedVersion = "";
            currentSelectedPath = "";
            releaseVersionInputField.text = "";
            releaseChangeLogInputField.text = "";
            packageNameText.text = "";
            
            PopulateRepList();
        }
    }
}