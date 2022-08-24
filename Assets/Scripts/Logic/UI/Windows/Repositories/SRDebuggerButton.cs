namespace Logic.UI.Windows.Repositories
{
    using SRDebugger;
    using UnityEngine;

    public class SRDebuggerButton : MonoBehaviour
    {
        public void OpenSRDebugger()
        {
            SRDebug.Instance.ShowDebugPanel();
        }
    }
}
