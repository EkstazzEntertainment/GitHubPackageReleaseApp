namespace SRDebugger
{
    using System.Collections.Generic;
    using UnityEngine;

    public class DebugPanelGestureChecker
    {
        private int GestureBase => (Screen.width + Screen.height) / 4;
        private Vector2 gestureSum = Vector2.zero;
        private readonly List<Vector2> gestureDetector = new List<Vector2>();
        private readonly int numberOfCircles;
        private int gestureCount;
        private float gestureLength;
        private float deltaThreshold = 10f;
        private int minPointForGesturePattern = 10;


        public DebugPanelGestureChecker(int numberOfCircles)
        {
            this.numberOfCircles = numberOfCircles;
        }

        public bool IsGestureDone()
        {
            if (IsDeviceMobile())
            {
                ProcessDragForPhone();
            }
            else
            {
                ProcessDragForMouse();
            }

            return gestureDetector.Count >= minPointForGesturePattern && ProcessGestureCalculation();
        }

        private bool IsDeviceMobile()
        {
            return Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer;
        }

        private void ProcessDragForPhone()
        {
            if (Input.touches.Length != 1)
            {
                CleanUpGesturePattern();
            }
            else
            {
                var currentTouchPhase = Input.touches[0].phase;
                if (currentTouchPhase == TouchPhase.Canceled || currentTouchPhase == TouchPhase.Ended)
                {
                    gestureDetector.Clear();
                }
                else if (currentTouchPhase == TouchPhase.Moved)
                {
                    var currentScreenPosition = Input.touches[0].position;
                    ValidateAndProcessNewDragPosition(currentScreenPosition);
                }
            }
        }

        private void ProcessDragForMouse()
        {
            if (Input.GetMouseButtonUp(0))
            {
                CleanUpGesturePattern();
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    var currentScreenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    ValidateAndProcessNewDragPosition(currentScreenPos);
                }
            }
        }

        private bool ProcessGestureCalculation()
        {
            var prevPos = gestureDetector[gestureDetector.Count - 2];
            var curPos = gestureDetector[gestureDetector.Count - 1];
            UpdateGestureState(prevPos, curPos);
            var dot = CalculateGestureCoDirection(prevPos, curPos);
            if (dot < 0f)
            {
                CleanUpGesturePattern();
                return false;
            }

            if (gestureLength > GestureBase && gestureSum.magnitude < GestureBase / 2)
            {
                EndSuccessGestureStage();
                if (gestureCount >= numberOfCircles)
                    return true;
            }

            return false;
        }

        private void UpdateGestureState(Vector2 prevPos, Vector2 curPos)
        {
            var delta = prevPos - curPos;
            var deltaLength = delta.magnitude;
            gestureSum += delta;
            gestureLength += deltaLength;
        }

        private void EndSuccessGestureStage()
        {
            gestureDetector.Clear();
            gestureCount++;
            gestureSum = Vector2.zero;
            gestureLength = 0;
        }

        private float CalculateGestureCoDirection(Vector2 prevPos, Vector2 curPos)
        {
            return Vector2.Dot(prevPos, curPos);
        }

        private void ValidateAndProcessNewDragPosition(Vector2 screenPosition)
        {
            if (IsCurrentPositionValidForGesture(screenPosition))
            {
                gestureDetector.Add(screenPosition);
            }
        }

        private bool IsCurrentPositionValidForGesture(Vector2 screenPosition)
        {
            return gestureDetector.Count == 0 ||
                   (screenPosition - gestureDetector[gestureDetector.Count - 1]).magnitude > deltaThreshold;
        }

        private void CleanUpGesturePattern()
        {
            gestureDetector.Clear();
            gestureCount = 0;
        }
    }
}