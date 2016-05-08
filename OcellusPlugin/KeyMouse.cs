using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace OcellusPlugin
{
    internal class KeyMouse
    {
        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        public static void PressKeys(List<ushort> scanCodes)
        {
            var forwards = 0;
            var backwards = scanCodes.Count * 2 - 1;
            var inputs = new Input[scanCodes.Count * 2];
            foreach (var scanCode in scanCodes)
            {
                inputs[forwards++] = BuildInput(scanCode, true);
                inputs[backwards--] = BuildInput(scanCode, false);
            }
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        private static Input BuildInput(ushort scanCode, bool keyDown)
        {
            var keybdInput = new KeybdInput
            {
                wVk = scanCode,
                wScan = 0,
                dwFlags = keyDown ? 0 : KEYEVENTF_KEYUP,
                dwExtraInfo = GetMessageExtraInfo()
            };
            var inputUnion = new InputUnion {ki = keybdInput};
            return new Input {type = INPUT_KEYBOARD, u = inputUnion};
        }

        private struct Input
        {
#pragma warning disable 414
            public int type;
            public InputUnion u;
#pragma warning restore 414
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public readonly MouseInput mi;
            [FieldOffset(0)] public KeybdInput ki;
            [FieldOffset(0)] public readonly HardwareInput hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            private readonly int dx;
            private readonly int dy;
            private readonly uint mouseData;
            private readonly uint dwFlags;
            private readonly uint time;
            private readonly IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeybdInput
        {
            /*Virtual Key code.  Must be from 1-254.  If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0.*/
            public ushort wVk;
            /*A hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application.*/
            public ushort wScan;
            /*Specifies various aspects of a keystroke.  See the KEYEVENTF_ constants for more information.*/
            public uint dwFlags;
            /*The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp.*/
            public readonly uint time;
            /*An additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information.*/
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HardwareInput
        {
            public readonly uint uMsg;
            public readonly ushort wParamL;
            public readonly ushort wParamH;
        }
    }
}