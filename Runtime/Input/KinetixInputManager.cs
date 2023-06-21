// // ----------------------------------------------------------------------------
// // <copyright file="InputManager.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;

namespace Kinetix.UI
{
    public class KinetixInputManager : MonoBehaviour
    {
        private static bool IsInitialized = false;

        public static Action          OnHitNextPage;
        public static Action          OnHitPrevPage;
        public static Action          OnHitNextTab;
        public static Action          OnHitPrevTab;
        public static Action<Vector2> OnNavigate;
        public static Action          OnCancelNavigate;
        public static Action          OnSelect;
        public static Action          OnCancel;
        public static Action          OnDeleteMode;

        public static Action<string> OnChangeDevice;
        public static Vector2        positionTouchOrMouse;

        private static InputActionAsset assetInputAction;
        private static string           currentDevice = "";

        public static void Initialize(InputActionMap currentActionMap)
        {
            if (IsInitialized) return;

            assetInputAction = ScriptableObject.CreateInstance<InputActionAsset>();
            assetInputAction.AddActionMap(currentActionMap);

            assetInputAction[InputMappingConstants.NEXT_PAGE].performed   += _ctx => { HitNextPage(_ctx); };
            assetInputAction[InputMappingConstants.PREV_PAGE].performed   += _ctx => { HitPrevPage(_ctx); };
            assetInputAction[InputMappingConstants.NEXT_TAB].performed    += _ctx => { HitNextTab(_ctx); };
            assetInputAction[InputMappingConstants.PREV_TAB].performed    += _ctx => { HitPrevTab(_ctx); };
            assetInputAction[InputMappingConstants.NAVIGATION].performed  += _ctx => { HitNavigation(_ctx); };
            assetInputAction[InputMappingConstants.NAVIGATION].canceled   += _ctx => { HitNavigationCancel(_ctx); };
            assetInputAction[InputMappingConstants.SELECT].performed      += _ctx => { HitSelect(_ctx); };
            assetInputAction[InputMappingConstants.CANCEL].performed      += _ctx => { HitCancel(_ctx); };
            assetInputAction[InputMappingConstants.DELETE_MODE].performed += _ctx => { HitDeleteMode(_ctx); };
            assetInputAction[InputMappingConstants.MOUSE_OR_TOUCH_MOVE].performed +=
                _ctx => { PerformTouchOrMouseMove(_ctx); };

            InputSystem.onEvent += OnInputSystemDeviceChange;

            IsInitialized = true;
        }

        private void OnDestroy()
        {
            InputSystem.onEvent -= OnInputSystemDeviceChange;
        }

        private static void OnInputSystemDeviceChange(InputEventPtr eventPtr, InputDevice device)
        {
            if (device.displayName == currentDevice) 
                return;
            
            currentDevice = device.displayName;
            if (device is Gamepad)
                EventSystem.current.SetSelectedGameObject(null);
            OnChangeDevice?.Invoke(currentDevice);
        }

        public static void Enable()
        {
            if (assetInputAction && !assetInputAction.enabled)
                assetInputAction.Enable();
        }

        public static void Disable()
        {
            if (assetInputAction && assetInputAction.enabled)
                assetInputAction.Disable();
        }

        public static Vector2 PositionTouchOrMouse()
        {
            return assetInputAction[InputMappingConstants.MOUSE_OR_TOUCH_MOVE].ReadValue<Vector2>();
        }

        public static bool IsPressed()
        {
            return assetInputAction[InputMappingConstants.PRESS].ReadValue<float>() > 0f;
        }

        public static bool WasReleasedThisFrame()
        {
            return assetInputAction[InputMappingConstants.PRESS].triggered == false &&
                   assetInputAction[InputMappingConstants.PRESS].ReadValue<float>() <= 0f;
        }

        private static void PerformTouchOrMouseMove(InputAction.CallbackContext ctx)
        {
            positionTouchOrMouse = assetInputAction[InputMappingConstants.MOUSE_OR_TOUCH_MOVE].ReadValue<Vector2>();
        }

        private static void HitNextPage(InputAction.CallbackContext ctx)
        {
            OnHitNextPage?.Invoke();
        }

        private static void HitPrevPage(InputAction.CallbackContext ctx)
        {
            OnHitPrevPage?.Invoke();
        }

        private static void HitNextTab(InputAction.CallbackContext ctx)
        {
            OnHitNextTab?.Invoke();
        }

        private static void HitPrevTab(InputAction.CallbackContext ctx)
        {
            OnHitPrevTab?.Invoke();
        }

        private static void HitNavigation(InputAction.CallbackContext ctx)
        {
            Vector2 inputVector = ctx.ReadValue<Vector2>();
            OnNavigate?.Invoke(inputVector);
        }

        private static void HitNavigationCancel(InputAction.CallbackContext ctx)
        {
            OnCancelNavigate?.Invoke();
        }

        private static void HitSelect(InputAction.CallbackContext ctx)
        {
            OnSelect?.Invoke();
        }

        private static void HitCancel(InputAction.CallbackContext ctx)
        {
            OnCancel?.Invoke();
        }

        private static void HitDeleteMode(InputAction.CallbackContext ctx)
        {
            OnDeleteMode?.Invoke();
        }
    }
}
