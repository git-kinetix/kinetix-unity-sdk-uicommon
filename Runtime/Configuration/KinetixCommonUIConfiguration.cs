// // ----------------------------------------------------------------------------
// // <copyright file="KinetixCommonUIConfiguration.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kinetix.UI.Common
{
    public class KinetixCommonUIConfiguration
    {
        [Tooltip("Action Map for the Input Manager")]
        public KinetixInputMapSO kinetixInputActionMap;

        [Tooltip("Language")]
        public SystemLanguage baseLanguage = SystemLanguage.English;
    }
}
