// // ----------------------------------------------------------------------------
// // <copyright file="KinetixUIBehaviour.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Linq;
using Kinetix.Utils;
using UnityEngine;


namespace Kinetix.UI.Common
{
    public static class KinetixUIBehaviour
    {
        public static Action<EKinetixUICategory>   OnShow;
        public static Action<EKinetixUICategory[]> OnDisplayTabs;
        public static Action                       OnDisplayEnabledTabs;
        public static Action                       OnHideAll;
        
        public static  bool                 IsShown => isShown;
        private static bool                 isShown;
        private static EKinetixUICategory[] lastDisplayedTab;

        public static void Show(EKinetixUICategory _Category = EKinetixUICategory.EMOTE_SELECTOR)
        {
            if (lastDisplayedTab != null && lastDisplayedTab.Contains(_Category))
            {
                Show(lastDisplayedTab, _Category);
                return;
            }
            
            if (!isShown)
                KinetixAnalytics.SendEvent("Open_Wheel_UI", "", KinetixAnalytics.Page.None, KinetixAnalytics.Event_type.Click);
            
            isShown = true;
            OnDisplayEnabledTabs?.Invoke();
            OnShow?.Invoke(_Category);
        }

        public static void Show(EKinetixUICategory[] _DisplayedTabs, EKinetixUICategory _Category = EKinetixUICategory.EMOTE_SELECTOR)
        {
            if (!isShown)
                KinetixAnalytics.SendEvent("Open_Wheel_UI", "", KinetixAnalytics.Page.None, KinetixAnalytics.Event_type.Click);

            lastDisplayedTab = _DisplayedTabs;
            isShown          = true;
            OnDisplayTabs?.Invoke(_DisplayedTabs);
            OnShow?.Invoke(_Category);
        }

        public static void HideAll()
        {
            if (isShown)
                KinetixAnalytics.SendEvent("Close_Wheel_UI", "", KinetixAnalytics.Page.None, KinetixAnalytics.Event_type.Click);

            isShown = false;
            OnHideAll?.Invoke();
        }
    }
}
