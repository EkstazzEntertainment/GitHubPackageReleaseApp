namespace Assets.Scripts.Logic.UI.Windows.Repositories
{
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

        
        private void Awake()
        {
            PopulateRepList();
        }

        private void PopulateRepList()
        {
            CleanChildren(repListContent);
            
            RepositoryHandler.ReadReposDataBase(out DataBaseFormat result);

            foreach (var rep in result.Reps)
            {
                repoCellPrefab.Init(rep.Key, rep.Value);
                Instantiate(repoCellPrefab, repListContent);
            }
        }
        
        private void CleanChildren(Transform tr) 
        {
            int nbChildren = tr.childCount;

            for (int i = nbChildren - 1; i >= 0; i--) {
                DestroyImmediate(tr.GetChild(i).gameObject);
            }
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