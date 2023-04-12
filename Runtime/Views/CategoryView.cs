// // ----------------------------------------------------------------------------
// // <copyright file="CategoryView.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using UnityEngine;

namespace Kinetix.UI.Common
{
    public abstract class CategoryView : KinetixView
    {
        [SerializeField] protected EKinetixUICategory Category;

        protected override void Awake()
        {
            base.Awake();
            KinetixUIBehaviour.OnShow    += Show;
            KinetixUIBehaviour.OnHideAll += Hide;
        }

        protected virtual void OnDestroy()
        {
            KinetixUIBehaviour.OnShow    -= Show;
            KinetixUIBehaviour.OnHideAll -= Hide;
        }

        public override void Show()
        {
            if (Visible)
                return;

            base.Show();
            KinetixUI.OnShowView?.Invoke(Category);
        }

        public override void Hide()
        {
            if (!Visible)
                return;

            base.Hide();
            KinetixUI.OnHideView?.Invoke(Category);
        }

        protected void Show(EKinetixUICategory _Category)
        {
            if (_Category != Category)
                return;

            Show();
        }

        protected void Hide(EKinetixUICategory _Category)
        {
            if (_Category != Category)
                return;

            Hide();
        }

        protected void Switch(EKinetixUICategory _Category)
        {
            if (Visible)
                Hide(_Category);
            else
                Show(_Category);
        }
    }
}
