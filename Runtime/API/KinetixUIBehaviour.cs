// // ----------------------------------------------------------------------------
// // <copyright file="KinetixUIBehaviour.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using UnityEngine;


namespace Kinetix.UI.Common
{
    public static class KinetixUIBehaviour
    {
        public static Action<EKinetixUICategory> OnShow;
        public static Action                     OnHideAll;

        public static Action OnEnableInput;
        public static Action OnDisableInput;

        public static  bool               IsShown => isShown;
        private static bool               isShown;
        public static EKinetixUICategory currentShownCategory;

        public static void Show(EKinetixUICategory _Category = EKinetixUICategory.EMOTE_SELECTOR)
        {
            isShown = true;
            currentShownCategory = _Category;
            OnShow?.Invoke(_Category);

            KinetixAnalytics.SendEvent("Open_Wheel_UI", "", KinetixAnalytics.Page.None, KinetixAnalytics.Event_type.Click);
        }

        public static void HideAll()
        {
            isShown = false;
            OnHideAll?.Invoke();
            
            KinetixAnalytics.SendEvent("Close_Wheel_UI", "", KinetixAnalytics.Page.None, KinetixAnalytics.Event_type.Click);
        }

        public static void EnableInput()
        {
            OnEnableInput?.Invoke();
        }

        public static void DisableInput()
        {
            OnDisableInput?.Invoke();
        }
    }
}
