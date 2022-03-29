using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace OxUE
{
    /// <summary>
    /// An advanced cursor behavior using state precedence
    /// </summary>
    public class CursorHandler : Singleton<CursorHandler>
    {
        [Header("Options:")]
        [SerializeField] private CursorIconMode defaultMode = CursorIconMode.HiddenLocked;
        [SerializeField] private Texture2D cursorTexture;

        /// <summary>
        /// HiddenLocked - cursor locked and centered at game window
        /// Normal - free and visible cursor
        /// HiddenFree - cursor is hidden but not locked
        /// </summary>
        public enum CursorIconMode
        {
            HiddenLocked,
            Normal,
            HiddenFree
        }

        private CursorIconMode _currentMode = CursorIconMode.HiddenLocked;
        private int _currentPriority = 0;


        private bool _priorityChanged = true;
        private bool _lockCursor = false;
        private bool _hideCursor = false;

        private Dictionary<int, CursorIconMode> _priorityList = new Dictionary<int, CursorIconMode>() { };

        // Initialization
        private void Start()
        {
            Reset();
        }

        private void Update()
        {
            if (_priorityChanged)
            {
                UpdateCursor();
            }

            // Debug
            //PrintPriorityStack();
        }

        /// <summary>
        /// Set target cursor priority and updates game cursor
        /// </summary>
        /// <param name="mode">Cursor mode to set</param>
        /// <param name="priority">Mode priority. Bigger the value - bigger the priority</param>
        public void SetCursorPriority(CursorIconMode mode, uint priority)
        {
            if (_priorityList.ContainsKey((int)priority))
            {
                _priorityList[(int)priority] = mode;
            }
            else
                _priorityList.Add((int)priority, mode);

            _priorityChanged = true;
        }

        /// <summary>
        /// Remove target cursor priority and updates game cursor
        /// </summary>
        /// <param name="priority"></param>
        public void RemoveCursorPriority(uint priority)
        {
            if (_priorityList.ContainsKey((int)priority))
            {
                _priorityList.Remove((int)priority);
                _priorityChanged = true;
            }
        }

        /// <summary>
        /// Reset all
        /// </summary>
        public void Reset()
        {
            _priorityList.Clear();
            _currentPriority = 0;
            _currentMode = defaultMode;
            UpdateCursor();
        }


        private void UpdateCursor()
        {
            CursorIconMode mode = defaultMode;
            int maxPriority = -1;

            foreach (KeyValuePair<int, CursorIconMode> var in _priorityList)
            {
                if (var.Key >= maxPriority)
                {
                    maxPriority = var.Key;
                    mode = var.Value;
                }
            }

            _currentMode = mode;
            _currentPriority = maxPriority;


            switch (_currentMode)
            {
                case CursorIconMode.HiddenLocked:
                    Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    _lockCursor = true;
                    _hideCursor = true;
                    break;
                case CursorIconMode.Normal:
                    Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    _lockCursor = false;
                    _hideCursor = false;
                    break;
                case CursorIconMode.HiddenFree:
                    Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    _lockCursor = false;
                    _hideCursor = true;
                    break;
            }

            Cursor.lockState = _lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !_hideCursor;

            _priorityChanged = false;
        }


        private void PrintPriorityStack()
        {
            foreach (var mode in _priorityList)
            {
                Debug.Log($"{mode.Key} - {mode.Value}");
            }
        }
    }
}