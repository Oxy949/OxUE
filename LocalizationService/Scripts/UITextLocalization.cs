using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OxUE
{
    public class UITextLocalization : MonoBehaviour
    {

        public bool useCaps = true;
        [SerializeField] private string _string;

        public string TextString
        {
            get { return _string; }
            set
            {
                _string = value;
                OnChangeLocalization();
            }
        }

        private Text UiText;
        private TextMeshProUGUI MeshText;

        #region Localize Logic

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => LocalizationService.Instance.IsInited);
            Initialize();
        }

        private void Initialize()
        {
            LocalizationService.Instance.OnChangeLocalization += OnChangeLocalization;
            UiText = gameObject.GetComponent<Text>();
            MeshText = gameObject.GetComponent<TextMeshProUGUI>();
            OnChangeLocalization();
        }

        private void OnChangeLocalization()
        {
            Localize();
        }

        private void Localize()
        {
            SetTextValue(LocalizationService.Instance.GetLocalizatedText(_string));
        }

        private void SetTextValue(string text)
        {
            text = ParceText(text);

            if (UiText != null)
                UiText.text = useCaps ? text.ToUpper() : text;

            if (MeshText != null)
                MeshText.text = useCaps ? text.ToUpper() : text;

            // error check
            if (text == "[EMPTY]" || text == string.Format("[ERROR KEY {0}]", _string))
            {
                //Debug.Log("" + _key + ",");
            }

        }

        private string ParceText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Replace("\\n", Environment.NewLine);
        }

        private void OnDestroy()
        {
            if (!(LocalizationService.Instance is null))
                LocalizationService.Instance.OnChangeLocalization -= OnChangeLocalization;
        }

        #endregion Localize Logic

        #region Helpers

        public bool IsHasOutputHelper()
        {
            UiText = gameObject.GetComponent<Text>();
            MeshText = gameObject.GetComponent<TextMeshProUGUI>();
            return UiText != null || MeshText != null;
        }

        #endregion Helpers
    }
}