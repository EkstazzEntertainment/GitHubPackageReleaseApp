namespace Logic.UI.Windows.Repositories
{
    using UnityEngine;

    public class SRDebuggerButton : MonoBehaviour
    {
        public void OpenSRDebugger()
        {
            SRDebug.Instance.ShowDebugPanel();
        }
    }
}
