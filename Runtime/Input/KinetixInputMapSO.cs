
// // ----------------------------------------------------------------------------
// // <copyright file="KinetixInputSO.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.InputSystem;

namespace Kinetix.UI
{
    [CreateAssetMenu(menuName="Kinetix/CustomKinetixInputActionMap")]
    public class KinetixInputMapSO : ScriptableObject
    {
        [SerializeField]
        public InputActionMap kinetixActionMap;
    }
}
