﻿using System.Diagnostics;
using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Core.Models
{
    public class TargetWindow
    {
        //public static TargetWindow FromAutomationElement(IUIAutomationElement element)
        //{
        //    return new TargetWindow
        //    {
        //        WindowHandle = element.CurrentNativeWindowHandle,
        //        Title = element.CurrentName,
        //        ClassName = element.CurrentClassName,
        //        ProcessName = Process.GetProcessById(element.CurrentProcessId).ProcessName,
        //    };
        //}

        public static TargetWindow FromWindow(Window window)
        {
            return new TargetWindow(
                window.Handle,
                window.GetText(),
                window.Class!.Name,
                Process.GetProcessById((int)window.GetProcessId()).ProcessName);
        }

        private TargetWindow(nint windowHandle, string title, string className, string processName)
        {
            WindowHandle = windowHandle;
            Title = title;
            ClassName = className;
            ProcessName = processName;
        }

        public nint WindowHandle { get; }
        public string Title { get; }
        public string ClassName { get; }
        public string ProcessName { get; }

        public void ApplyBackdropRule(BackdropType type)
        {
            if (type == BackdropType.Default)
                return;

            if (DesktopWindowManager.IsBackdropTypeSupported)
            {
                DesktopWindowManager.SetBackdropType(WindowHandle, (DWM_SYSTEMBACKDROP_TYPE)type);
            }
            else if (DesktopWindowManager.IsUndocumentedMicaSupported && type < BackdropType.Acrylic)
            {
                DesktopWindowManager.SetMica(WindowHandle, type == BackdropType.Mica);
            }
        }

        public void ApplyTitlebarColorRule(TitlebarColorMode targetMode, TitlebarColorMode systemMode)
        {
            if (!DesktopWindowManager.IsImmersiveDarkModeSupported) return;
            switch (targetMode)
            {
                case TitlebarColorMode.Default:
                    return;
                case TitlebarColorMode.System:
                    if (systemMode == TitlebarColorMode.System) return;
                    ApplyTitlebarColorRule(systemMode, TitlebarColorMode.Default);
                    return;
                case TitlebarColorMode.Light:
                    DesktopWindowManager.SetImmersiveDarkMode(WindowHandle, false);
                    return;
                case TitlebarColorMode.Dark:
                    DesktopWindowManager.SetImmersiveDarkMode(WindowHandle, true);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ApplyCornerPreferenceRule(CornerPreference cornerPreference)
        {
            if (cornerPreference == CornerPreference.Default)
                return;

            if (DesktopWindowManager.IsCornerPreferenceSupported)
            {
                DesktopWindowManager.SetCornerPreference(WindowHandle, (DWM_WINDOW_CORNER_PREFERENCE)cornerPreference);
            }
        }

        public void ApplyCaptionColorRule(COLORREF captionColor)
        {
            if (!DesktopWindowManager.IsCaptionColorSupported) return;
            if (captionColor == COLORREF.Default) return;
            DesktopWindowManager.SetCaptionColor(WindowHandle, captionColor);
        }

        public void ApplyCaptionTextColorRule(COLORREF captionTextColor)
        {
            if (!DesktopWindowManager.IsCaptionTextColorSupported) return;
            if (captionTextColor == COLORREF.Default) return;
            DesktopWindowManager.SetCaptionTextColor(WindowHandle, captionTextColor);
        }

        public void ApplyBorderColorRule(COLORREF borderColor)
        {
            if (!DesktopWindowManager.IsBorderColorSupported)
                return;
            if (borderColor == COLORREF.Default)
                return;
            DesktopWindowManager.SetBorderColor(WindowHandle, borderColor);
        }

        public void ApplyRule(IRule rule, TitlebarColorMode systemTitlebarColorMode)
        {
#if DEBUG
            Debug.WriteLine($"Applying rule `{rule}` to `{Title}` ({ClassName}, {ProcessName})");
#endif
            ApplyTitlebarColorRule(rule.TitleBarColor, systemTitlebarColorMode);
            ApplyBackdropRule(rule.BackdropPreference);
            ApplyCornerPreferenceRule(rule.CornerPreference);
            if (rule.CaptionColor != null)
            {
                try
                {
                    ApplyCaptionColorRule(Convert.ToUInt32(rule.CaptionColor, 16));
                }
                catch (FormatException) 
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
            if (rule.CaptionTextColor != null)
            {
                try
                {
                    ApplyCaptionTextColorRule(Convert.ToUInt32(rule.CaptionTextColor, 16));
                }
                catch (FormatException)
                {
                }
                catch (ArgumentOutOfRangeException) {
                }
            }
            if (rule.BorderColor != null)
            {
                try
                {
                    ApplyBorderColorRule(Convert.ToUInt32(rule.BorderColor, 16));
                }
                catch (FormatException)
                {
                }
                catch (ArgumentOutOfRangeException) {
                }
            }
            
            if (rule.ExtendFrameIntoClientArea)
                DesktopWindowManager.ExtendFrameIntoClientArea(WindowHandle);

            if (rule.EnableBlurBehind)
                DesktopWindowManager.EnableBlurBehind(WindowHandle);
        }
    }
}
