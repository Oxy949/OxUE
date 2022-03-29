using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OxUE
{
    /// <summary>
    /// An async loading screen with animations and text.
    /// </summary>
    public class LoadingUI : Singleton<LoadingUI>
    {
        public Action<bool> StateChanged { get; set; }

        [Header("Options:")]
        [SerializeField] private bool showLoadingUIOnStart = true;

        [Header("References:")]
        [SerializeField] private CanvasGroup canvas;

        [SerializeField] private Image bg;
        [SerializeField] private Text text;

        // Initialization
        private void Start()
        {
            if (showLoadingUIOnStart)
            {
                Show();
            }
        }

        /// <summary>
        /// Update loading text
        /// </summary>
        /// <param name="loadingText">text</param>
        public void SetLoadingText(string loadingText = null)
        {
            text.gameObject.SetActive(!string.IsNullOrWhiteSpace(loadingText));
            text.text = loadingText;
        }

        /// <summary>
        /// Shows the loading screen UI with options
        /// </summary>
        /// <param name="ready">Action to do when animation done</param>
        /// <param name="textStr">Optional text</param>
        /// <param name="time">Animation time</param>
        /// <param name="bgColor">Base color</param>
        /// <param name="useCustomColor"></param>
        public void Show(Action ready = null, string textStr = null, float time = 0.5f, bool useCustomColor = false, Color bgColor = default)
        {
            canvas.gameObject.SetActive(true);
            if (useCustomColor)
                bg.color = bgColor;
            text.gameObject.SetActive(!string.IsNullOrWhiteSpace(textStr));
            text.text = textStr;
            canvas.DOFade(1, time).OnComplete(() =>
            {
                ready?.Invoke();
                StateChanged?.Invoke(true);
            });
        }

        /// <summary>
        /// Hide the loading screen UI
        /// </summary>
        /// <param name="time"></param>
        public void Hide(float time = 0.5f)
        {
            canvas.DOFade(0, time).OnComplete(() => StateChanged?.Invoke(false));
        }
    }
}