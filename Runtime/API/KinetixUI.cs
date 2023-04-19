// // ----------------------------------------------------------------------------
// // <copyright file="KinetixUI.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using Kinetix.UI.Common;

namespace Kinetix.UI
{
    public static class KinetixUI
    {
        public static Action<EKinetixUICategory> OnShowView;
        public static Action<EKinetixUICategory> OnHideView;
        public static Action<AnimationIds>       OnPlayedAnimationWithEmoteSelector;
        
        public static bool IsShown => KinetixUIBehaviour.IsShown;

        /// <summary>
        /// Open the EmoteWheel and enable the inputs
        /// </summary>
        public static void Show(EKinetixUICategory _KinetixUICategory = EKinetixUICategory.EMOTE_SELECTOR)
        {
            KinetixUIBehaviour.Show(_KinetixUICategory);
            KinetixInputManager.Enable();
        }

        /// <summary>
        /// Close the EmoteWheel and disable the inputs
        /// </summary>
        public static void HideAll()
        {
            KinetixInputManager.Disable();
            KinetixUIBehaviour.HideAll();
        }

        public static void ChangeLanguage(UnityEngine.SystemLanguage language)
        {
            Common.Translation.KinetixTranslationManager.OnLanguageUpdate(language);
        }
    }
}
