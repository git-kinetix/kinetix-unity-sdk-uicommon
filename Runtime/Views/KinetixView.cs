// // ----------------------------------------------------------------------------
// // <copyright file="KinetixView.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace Kinetix.UI.Common
{
    [DefaultExecutionOrder(-1)]
    public class KinetixView : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;
        public bool        Visible { get; private set; }

        protected virtual void Awake()
        {
            Visible = false;
            ForceHide();
        }

        public virtual void Show()
        {
            if (Visible)
                return;

            Visible                    = true;
            CanvasGroup.alpha          = 1.0f;
            CanvasGroup.interactable   = true;
            CanvasGroup.blocksRaycasts = true;
        }

        public virtual void Hide()
        {
            if (!Visible)
                return;

            ForceHide();            
        }

        private void ForceHide()
        {
            Visible                    = false;
            CanvasGroup.alpha          = 0.0f;
            CanvasGroup.interactable   = false;
            CanvasGroup.blocksRaycasts = false;
        }

        public void Switch()
        {
            if (Visible)
                Hide();
            else
                Show();
        }
    }
}
