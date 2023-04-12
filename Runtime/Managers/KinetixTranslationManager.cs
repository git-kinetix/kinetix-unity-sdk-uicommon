// // ----------------------------------------------------------------------------
// // <copyright file="KinetixTranslationManager.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kinetix.UI.Common.Translation
{
    public static class KinetixTranslationManager
    {
        public static SystemLanguage Language { get { return language; } }
        private static Dictionary<string, TranslatedSentence> translatedSentencesByKey;
        private static List<Action<SystemLanguage>> languageChangedCallbacks;
        private static SystemLanguage language;

        public static void Initialize()
        {
            Initialize(SystemLanguage.English);
        }

        public static void Initialize(SystemLanguage chosenLanguage)
        {
            if (translatedSentencesByKey == null) {
                translatedSentencesByKey = FileTranslationProvider.GetAllTranslatedSentencesByKey();
                languageChangedCallbacks = new List<Action<SystemLanguage>>();
            }
            
            OnLanguageUpdate(chosenLanguage);
        }

        /// <summary>
        /// Returns the sentence's translation for the wanted language
        /// </summary>
        /// <param name="sentence">Sentence to translate</param>
        /// <param name="locale">The locale for the translation</param>
        /// <returns>The translation</returns>
        public static string GetSentenceTranslationForLocale(string sentence, string locale)
        {
            if (translatedSentencesByKey == null) {
                Initialize();
            }

            sentence = sentence.ToLower();

            // Set the given sentence as fallback
            string returnedTranslation = sentence;

            TranslatedSentence translatedSentenceOrWord;

            // If the whole sentence is a tag, translate it
            if (translatedSentencesByKey.TryGetValue(sentence, out translatedSentenceOrWord)) {
                returnedTranslation = translatedSentenceOrWord.GetTranslationForLocale(locale);
            }

            // If the whole sentence is not a tag by itself, translate individual words
            if (translatedSentenceOrWord == null) {
                string translation = string.Empty;
                string[] sentenceWords = sentence.Split(' ');
                
                for (int i = 0; i < sentenceWords.Length; i++) {
                    // Try to find a translation for the word
                    if (translatedSentencesByKey.TryGetValue(sentenceWords[i], out translatedSentenceOrWord)) {
                        translation += translatedSentenceOrWord.GetTranslationForLocale(locale);
                    } else {
                        translation += sentenceWords[i];
                    }

                    // We add a space if this is not the last word
                    if (i < sentenceWords.Length - 1) {
                        translation += " ";
                    }
                }

                returnedTranslation = translation;
            }

            if (returnedTranslation == sentence) {
                Debug.LogWarning("No translation found for " + sentence);
            }

            return returnedTranslation;
        }

        /// <summary>
        /// Returns the sentence's translation for the current language
        /// </summary>
        /// <param name="sentence">Sentence to translate</param>
        /// <returns>The translation</returns>
        public static string GetSentenceTranslation(string sentence)
        {
            if (translatedSentencesByKey == null) {
                Initialize();
            }

            return GetSentenceTranslationForLocale(sentence, language.ToString());
        }

        /// <summary>
        /// Allows for a callback call when the manager is notified of a language change
        /// </summary>
        /// <param name="callback">the callback</param>
        public static void RegisterLanguageChangedCallback(Action<SystemLanguage> callback)
        {
            languageChangedCallbacks.Add(callback);
        }

        /// <summary>
        /// To call when the languaged is changed at runtime, allowing the Kinetix UI to be updated with the new language
        /// </summary>
        /// <param name="newLanguage">the new language to use</param>
        public static void OnLanguageUpdate(SystemLanguage newLanguage)
        {
            language = newLanguage;
            
            foreach (Action<SystemLanguage> callback in languageChangedCallbacks)
            {
                callback(language);
            }
        }
    }
}
