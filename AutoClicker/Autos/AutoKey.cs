using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClicker
{
    class AutoKey
    {

        public static void WriteString(String input)
        {
            foreach (var item in input.ToCharArray())
            {
                KeysConverter.Convert(((System.Windows.Forms.Keys)item & Keys.KeyCode));
                Keys o = (Keys)Enum.Parse(typeof(Keys), ((int)item).ToString());
            }
        }

        /// <summary>
        /// simulate key press
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyPress(Win32.KeyCode keyCode)
        {
            Win32.INPUT input = new Win32.INPUT
            {
                type = Win32.SendInputEventType.InputKeyboard
            };
            input.ki = new Win32.KEYBDINPUT()
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = 0,
                Time = 0,
                ExtraInfo = IntPtr.Zero,
            };

            Win32.INPUT input2 = new Win32.INPUT
            {
                type = Win32.SendInputEventType.InputKeyboard
            };
            input2.ki = new Win32.KEYBDINPUT()
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = 2,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };
            Win32.INPUT[] inputs = new Win32.INPUT[] { input, input2 };
            if (Win32.SendInput(2, inputs, Marshal.SizeOf(typeof(Win32.INPUT))) == 0)
                throw new Exception();
        }

        /// <summary>
        /// Send a key down and hold it down until sendkeyup method is called
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyDown(Win32.KeyCode keyCode)
        {
            Win32.INPUT input = new Win32.INPUT
            {
                type = Win32.SendInputEventType.InputKeyboard
            };
            input.ki = new Win32.KEYBDINPUT
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = 0,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };
            Win32.INPUT[] inputs = new Win32.INPUT[] { input };
            if (Win32.SendInput(1, inputs, Marshal.SizeOf(typeof(Win32.INPUT))) == 0)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Release a key that is being hold down
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyUp(Win32.KeyCode keyCode)
        {
            Win32.INPUT input = new Win32.INPUT
            {
                type = Win32.SendInputEventType.InputKeyboard
            };
            input.ki = new Win32.KEYBDINPUT
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = 2,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };
            Win32.INPUT[] inputs = new Win32.INPUT[] { input };
            if (Win32.SendInput(1, inputs, Marshal.SizeOf(typeof(Win32.INPUT))) == 0)
                throw new Exception();

        }
    }
}
