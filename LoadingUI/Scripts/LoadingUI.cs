using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OxUE
{
    public class LoadingUI : Singleton<LoadingUI>
    {
        public Action<bool> StateChanged;
        public bool applyOnStart = true;
        public GameObject canvas;
        public Image bg;
        public Text text;

        public new void Awake()
        {
            base.Awake();
            if (applyOnStart)
            {
                Show(Color.black, null, null, 0);
            }
        }
        
        public void SetLoadingText(string textStr = null)
        {
            text.gameObject.SetActive(!string.IsNullOrWhiteSpace(textStr));
            text.text = textStr;
        }

        // Update is called once per frame
        public void Show(Color bgColor, Action finished = null, string textStr = null, float time = 0.5f)
        {
            canvas.SetActive(true);
            bg.color = bgColor;
            text.gameObject.SetActive(!string.IsNullOrWhiteSpace(textStr));
            text.text = textStr;
            canvas.GetComponent<CanvasGroup>().DOFade(1, time).OnComplete(()=> { finished?.Invoke(); StateChanged?.Invoke(true); });
        }

        // Update is called once per frame
        public void Hide(float time = 0.5f)
        {
            canvas.GetComponent<CanvasGroup>().DOFade(0, time).OnComplete(() => StateChanged?.Invoke(false));
        }
    }
}
