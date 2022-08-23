namespace Assets.Scripts.Logic.UI.Windows.Repositories
{
    using System.Collections.Generic;
    using global::Logic.Repositories;
    using TMPro;
    using UnityEngine;

    
    public class RepositoriesWindow : MonoBehaviour
    {
        [SerializeField] private RepoCell repoCellPrefab;
        [SerializeField] private TMP_InputField repPathInputField;
        [SerializeField] private TMP_InputField repNameInputField;
        [SerializeField] private Transform repListContent;
        [SerializeField] private TextMeshProUGUI packageNameText;
        [SerializeField] private TMP_InputField releaseVersionInputField;
        [SerializeField] private TMP_InputField releaseChangeLogInputField;

        private List<RepoCell> cells = new List<RepoCell>();
        
        
        private void Awake()
        {
            PopulateRepList();
        }

        private void PopulateRepList()
        {
            CleanChildren();
            
            RepositoryHandler.ReadReposDataBase(out DataBaseFormat result);

            foreach (var rep in result.Reps)
            {
                var instance = Instantiate(repoCellPrefab, repListContent);
                instance.Init(rep.Key, rep.Value);
                instance.OnRepoRemoved += RemoveRep;
                cells.Add(instance);
            }
        }

        private void CleanChildren()
        {
            cells.ForEach(cell =>
            {
                cell.OnRepoRemoved -= RemoveRep;
                DestroyImmediate(cell.gameObject);
            });
            cells.Clear();
        }
        
        public void AddRepButton()
        {
            AddRep();
        }

        private void AddRep()
        {
            RepositoryHandler.AddRepositoryLink(repPathInputField.text, repNameInputField.text);
            
            PopulateRepList();
        }

        private void RemoveRep()
        {
            PopulateRepList();
        }

        public void ReleaseButton()
        {
            
        }
    }
}