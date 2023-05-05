 // // ----------------------------------------------------------------------------
// // <copyright file="FileTranslationProvider.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kinetix.UI.Common.Translation
{
    internal static class FileTranslationProvider
    {
        
        internal static Dictionary<string, TranslatedSentence> GetAllTranslatedSentencesByKey()
        {
            Dictionary<string, TranslatedSentence> translatedSentences = new Dictionary<string, TranslatedSentence>();


            string overrideTranslationText = Resources.Load<TextAsset>(TranslationConst.OVERRIDE_FILE_PATH)?.text;
            string translationText = Resources.Load<TextAsset>("Translations/Kinetix SDK UI Translations")?.text;
            
            // The order here is important, the first loaded file have the priority, so we start with the overrides
            translatedSentences = AddTranslatedSentencesFromText(overrideTranslationText, translatedSentences);
            translatedSentences = AddTranslatedSentencesFromText(translationText, translatedSentences);

            return translatedSentences;
        }

        private static Dictionary<string, TranslatedSentence> AddTranslatedSentencesFromText(string translationText, Dictionary<string, TranslatedSentence> currentTranslatedSentences)
        {
            string[] translationHeaders = null;

            if (translationText == null) {
                return currentTranslatedSentences;
            }

            foreach (string translationLine in translationText.Split('\n'))
            {
                // The csv must use tab as separator
                string[] localeSeparatedSentences = translationLine.Split('\t');

                // The first line is the headers, so save them and go to the second line
                if (translationHeaders == null) {
                    translationHeaders = localeSeparatedSentences;
                    continue;
                }

                if (localeSeparatedSentences.Length > 1) {
                    TranslatedSentence translatedSentence = new TranslatedSentence();
                    string translationKey = localeSeparatedSentences[0].Trim();

                    if (currentTranslatedSentences.ContainsKey(translationKey)) {
                        translatedSentence = currentTranslatedSentences[translationKey];
                    }

                    // Register only if we don't have a translation in a specific locale
                    for (int i = 1; i < localeSeparatedSentences.Length; i++) {

                        if (!translatedSentence.ContainsTranslationForLocale(translationHeaders[i].Trim())) {
                            translatedSentence.AddTranslation(translationHeaders[i].Trim(), localeSeparatedSentences[i].Trim());
                        }
                    }

                    if (!currentTranslatedSentences.ContainsKey(translationKey)) {
                        currentTranslatedSentences.Add(translationKey, translatedSentence);
                    } else {
                        currentTranslatedSentences[translationKey] = translatedSentence;
                    }
                }
            }

            return currentTranslatedSentences;
        }
    }
}
