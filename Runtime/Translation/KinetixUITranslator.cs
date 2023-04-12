 // // ----------------------------------------------------------------------------
// // <copyright file="KinetixUITranslator.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using UnityEngine;
using TMPro;

namespace Kinetix.UI.Common.Translation
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class KinetixUITranslator : MonoBehaviour
    {
        public string text { get { return textMesh.text; } set { TranslateAndUpdateComponent(value); } }

        [SerializeField] private string translationTag;
        private TextMeshProUGUI textMesh;


        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();

            TranslateAndUpdateComponent(translationTag);

            KinetixTranslationManager.RegisterLanguageChangedCallback(OnLanguageUpdate);
        }

        public void TranslateAndUpdateComponent(string text)
        {
            translationTag = text;
            textMesh.text = KinetixTranslationManager.GetSentenceTranslation(text);
        }

        public void OnLanguageUpdate(SystemLanguage language)
        {
            textMesh.text = KinetixTranslationManager.GetSentenceTranslationForLocale(translationTag, language.ToString());
        }
    }
}
