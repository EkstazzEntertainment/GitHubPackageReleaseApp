namespace Logic.UI.Windows.Repositories
{
    using System.Collections;
    using UnityEngine;

    public class WarningPopUp : MonoBehaviour
    {
        private Coroutine coroutine;
        
        
        public void Init()
        {
            gameObject.SetActive(true);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(AwaitAndClose());
        }

        private IEnumerator AwaitAndClose()
        {
            yield return new WaitForSeconds(3);
            gameObject.SetActive(false);
        }
    }
}
