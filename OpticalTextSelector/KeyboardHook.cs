using Reactive.Bindings;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace OpticalTextSelector
{
    public class KeyboardHook
    {
        private enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr extraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(
            HookType code, Callback func, IntPtr instance, int threadID);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(
            IntPtr hook, int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        IntPtr handle = IntPtr.Zero;
        Callback callback = null;

        private delegate int Callback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        public KeyboardHook()
        {
            callback = new Callback(HookCallback);
            
            Install();
        }

        ~KeyboardHook()
        {
            Uninstall();
        }

        public ReactiveProperty<HookEventArgs> events { get; } = new ReactiveProperty<HookEventArgs>();

        private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (code < 0)
            {
                return CallNextHookEx(handle, code, wParam, ref lParam);
            }

            if ((lParam.flags & 0x80) != 0)
            {
                this.events.Value = new HookEventArgs(lParam.vkCode, false);
            }
            
            if ((lParam.flags & 0x80) == 0)
            {
                this.events.Value = new HookEventArgs(lParam.vkCode, true);
            }

            return CallNextHookEx(handle, code, wParam, ref lParam);
        }

        private void Install()
        {
            if (handle != IntPtr.Zero)
            {
                return;
            }

            Module[] list = System.Reflection.Assembly.GetExecutingAssembly().GetModules();
            System.Diagnostics.Debug.Assert(list != null && list.Length > 0);

            handle = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, callback, Marshal.GetHINSTANCE(list[0]), 0);
        }

        private void Uninstall()
        {
            if (handle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(handle);
                handle = IntPtr.Zero;
            }
        }
    }

    public class HookEventArgs : EventArgs
    {
        public Keys Key;
        public bool Alt;
        public bool Control;
        public bool Shift;
        public bool IsKeyDown;

        public HookEventArgs(uint keyCode, bool isKeyDown)
        {
            this.Key = (Keys)keyCode;
            this.Alt = (System.Windows.Forms.Control.ModifierKeys & Keys.Alt) != 0;
            this.Control = (System.Windows.Forms.Control.ModifierKeys & Keys.Control) != 0;
            this.Shift = (System.Windows.Forms.Control.ModifierKeys & Keys.Shift) != 0;
            this.IsKeyDown = isKeyDown;
        }
    }
}
