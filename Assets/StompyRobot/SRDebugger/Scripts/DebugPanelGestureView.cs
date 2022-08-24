namespace SRDebugger
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class DebugPanelGestureView : MonoBehaviour
    {
        [Header("Structure")]
        [SerializeField]
        public int numOfCircleToShow = 1;

        private DebugPanelGestureChecker debugPanelGestureChecker;

        private void Awake()
        {
#if !DEBUG
            Destroy(gameObject);
            return;
#endif
            debugPanelGestureChecker = new DebugPanelGestureChecker(numOfCircleToShow);
        }

        private void Update()
        {
#if !UNITY_EDITOR
            if (debugPanelGestureChecker.IsGestureDone())
            {
                if (!Settings.Instance.IsEnabled)
                {
                    SRDebug.Init();
                }
                SRDebug.Instance.ShowDebugPanel(DefaultTabs.Console);
            }
#endif
        }
    }
}