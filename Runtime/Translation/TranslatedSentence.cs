// // ----------------------------------------------------------------------------
// // <copyright file="TranslatedSentence.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kinetix.UI.Common.Translation
{
    internal class TranslatedSentence
    {
        public Dictionary<string, string> Translations { get { return translations; } }

        private Dictionary<string, string> translations;

        public TranslatedSentence()
        {
            this.translations = new Dictionary<string, string>();
        }

        public void AddTranslation(string locale, string translation)
        {
            translations.Add(locale, translation);
        }

        public bool ContainsTranslationForLocale(string locale) {
            return translations.ContainsKey(locale);
        }

        public string GetTranslationForLocale(string locale)
        {
            string translation = null;

            if (!translations.TryGetValue(locale, out translation)) {
                // TODO Kinetix log warning
            }

            return translation;
        }
    }
}
