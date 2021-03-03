using System.Collections.Generic;
using UnityEngine;

namespace OxUE
{
    public class CursorHandler : Singleton<CursorHandler>
    {
        public enum CursorIconMode { Hidden, Normal }

        public CursorIconMode currentMode = CursorIconMode.Hidden;
        [HideInInspector]
        public int currentPriority = 0;

        private Texture2D _normalCursor;
        private bool _cursorChanged = true;
        private bool lockCursor = false;

        private Dictionary<int, CursorIconMode> _priorityList = new Dictionary<int, CursorIconMode> () { };

        // Use this for initialization
        void Start ()
        {
            _normalCursor = Resources.Load<Texture2D>("Cursor/cursor_normal");
            UpdateCursor();
        }

        public void Reset()
        {
            _priorityList.Clear();
            currentPriority = 0;
            currentMode = CursorIconMode.Hidden;
            UpdateCursor();
        }

        // Update is called once per frame
        void Update ()
        {
            if (_cursorChanged)
            {
                UpdateCursor();
            }

            //PrintPriorityStack();
        }

        private void PrintPriorityStack()
        {
            foreach (var mode in _priorityList)
            {
                Debug.Log(mode.Key + "-" + mode.Value);
            }
        }

        public void SetCursorPriority(CursorIconMode mode, int priority)
        {
            if (_priorityList.ContainsKey(priority))
            {
                _priorityList[priority] = mode;
            }
            else
                _priorityList.Add(priority, mode);

            _cursorChanged = true;
        }

        public void RemoveCursorPriority(int priority)
        {
            if (_priorityList.ContainsKey(priority))
            {
                _priorityList.Remove(priority);
                _cursorChanged = true;
            }
        }

        private void UpdateCursor()
        {
            CursorIconMode mode = CursorIconMode.Hidden;
            int maxPriority = -1;

            foreach (KeyValuePair<int, CursorIconMode> var in _priorityList)
            {
                if (var.Key >= maxPriority)
                {
                    maxPriority = var.Key;
                    mode = var.Value;
                }
            }

            currentMode = mode;
            currentPriority = maxPriority;


            switch (currentMode)
            {
                case CursorIconMode.Hidden:
                    Cursor.SetCursor(_normalCursor, Vector2.zero, CursorMode.Auto);
                    lockCursor = true;
                    break;
                case CursorIconMode.Normal:
                    Cursor.SetCursor(_normalCursor, Vector2.zero, CursorMode.Auto);
                    lockCursor = false;
                    break;
            }

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            _cursorChanged = false;
        }
    }
}
