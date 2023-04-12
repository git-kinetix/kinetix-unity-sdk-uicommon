// // ----------------------------------------------------------------------------
// // <copyright file="KinetixLanguageSelector.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Kinetix.UI.Common.Translation
{
    internal class KinetixLanguageSelector : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        private List<string> dropdownLanguages;

        private void Start()
        {
           dropdownLanguages = new List<string>();

           dropdownLanguages.Add(SystemLanguage.English.ToString());
           dropdownLanguages.Add(SystemLanguage.French.ToString());
           dropdownLanguages.Add(SystemLanguage.ChineseSimplified.ToString());
           dropdownLanguages.Add(SystemLanguage.Japanese.ToString());
           dropdownLanguages.Add(SystemLanguage.German.ToString());
           dropdownLanguages.Add(SystemLanguage.Spanish.ToString());
           dropdownLanguages.Add(SystemLanguage.Portuguese.ToString());

           dropdown.AddOptions(dropdownLanguages);

           dropdown.value = dropdownLanguages.IndexOf(KinetixTranslationManager.Language.ToString());
        }

        public void OnLanguageDropdownChanged(int newLanguageIndex)
        {
            if (newLanguageIndex > dropdownLanguages.Count - 1) {
                Debug.LogWarning("TestTranslator: Index out of range for language dropdown.");

                return;
            }
            
            KinetixTranslationManager.OnLanguageUpdate((SystemLanguage) System.Enum.Parse(typeof(SystemLanguage), dropdownLanguages[newLanguageIndex]));
        }
    }
}
