using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoClicker
{
    public class Win32
    {
        #region Cursor

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        #region Cursor Moving

        [DllImport("user32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        #endregion Cursor Moving

        #region Cursor Clicking

        public const int WM_HOTKEY = 0x0312;

        public enum SendInputEventType
        {
            InputMouse = 0,
            InputKeyboard = 1
        }

        public enum MouseEventFlags
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            Wheel = 0x0080,
            XDown = 0x0100,
            XUp = 0x0200,
            Absolute = 0x8000
        }

        public enum SystemMetric
        {
            CXScreen = 0x0000,
            CYScreen = 0x0001,
        }

        public enum fsModifiers : uint
        {
            Alt = 0x0001,
            Control = 0x0002,
            Shift = 0x0004,
            Windows = 0x0008,
            NoRepeat = 0x4000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public SendInputEventType type; // 0 = INPUT_MOUSE(デフォルト), 1 = INPUT_KEYBOARD
            public MOUSEINPUT mi;
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public MouseEventFlags dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern uint SendInput(
                uint nInputs,    // INPUT 構造体の数(イベント数)
                INPUT[] pInputs, // INPUT 構造体
                int cbSize       // INPUT 構造体のサイズ
        );

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);

        public static int CalculateAbsoluteCoordinateX(int x)
        {
            return (x * 65536) / GetSystemMetrics(SystemMetric.CXScreen);
        }

        public static int CalculateAbsoluteCoordinateY(int y)
        {
            return (y * 65536) / GetSystemMetrics(SystemMetric.CYScreen);
        }

        #endregion Cursor Clicking

        #endregion Cursor

        #region Keyboard

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646310(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        #region Key Codes

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        public enum KeyCode : ushort
        {
            #region Media

            /// <summary>
            /// Next track if a song is playing
            /// </summary>
            MEDIA_NEXT_TRACK = 0xb0,

            /// <summary>
            /// Play pause
            /// </summary>
            MEDIA_PLAY_PAUSE = 0xb3,

            /// <summary>
            /// Previous track
            /// </summary>
            MEDIA_PREV_TRACK = 0xb1,

            /// <summary>
            /// Stop
            /// </summary>
            MEDIA_STOP = 0xb2,

            #endregion Media

            #region math

            /// <summary>Key "+"</summary>
            ADD = 0x6b,

            /// <summary>
            /// "*" key
            /// </summary>
            MULTIPLY = 0x6a,

            /// <summary>
            /// "/" key
            /// </summary>
            DIVIDE = 0x6f,

            /// <summary>
            /// Subtract key "-"
            /// </summary>
            SUBTRACT = 0x6d,

            #endregion math

            #region Browser

            /// <summary>
            /// Go Back
            /// </summary>
            BROWSER_BACK = 0xa6,

            /// <summary>
            /// Favorites
            /// </summary>
            BROWSER_FAVORITES = 0xab,

            /// <summary>
            /// Forward
            /// </summary>
            BROWSER_FORWARD = 0xa7,

            /// <summary>
            /// Home
            /// </summary>
            BROWSER_HOME = 0xac,

            /// <summary>
            /// Refresh
            /// </summary>
            BROWSER_REFRESH = 0xa8,

            /// <summary>
            /// browser search
            /// </summary>
            BROWSER_SEARCH = 170,

            /// <summary>
            /// Stop
            /// </summary>
            BROWSER_STOP = 0xa9,

            #endregion Browser

            #region Numpad numbers

            /// <summary>
            ///
            /// </summary>
            NUMPAD0 = 0x60,

            /// <summary>
            ///
            /// </summary>
            NUMPAD1 = 0x61,

            /// <summary>
            ///
            /// </summary>
            NUMPAD2 = 0x62,

            /// <summary>
            ///
            /// </summary>
            NUMPAD3 = 0x63,

            /// <summary>
            ///
            /// </summary>
            NUMPAD4 = 100,

            /// <summary>
            ///
            /// </summary>
            NUMPAD5 = 0x65,

            /// <summary>
            ///
            /// </summary>
            NUMPAD6 = 0x66,

            /// <summary>
            ///
            /// </summary>
            NUMPAD7 = 0x67,

            /// <summary>
            ///
            /// </summary>
            NUMPAD8 = 0x68,

            /// <summary>
            ///
            /// </summary>
            NUMPAD9 = 0x69,

            #endregion Numpad numbers

            #region Fkeys

            /// <summary>
            /// F1
            /// </summary>
            F1 = 0x70,

            /// <summary>
            /// F10
            /// </summary>
            F10 = 0x79,

            /// <summary>
            ///
            /// </summary>
            F11 = 0x7a,

            /// <summary>
            ///
            /// </summary>
            F12 = 0x7b,

            /// <summary>
            ///
            /// </summary>
            F13 = 0x7c,

            /// <summary>
            ///
            /// </summary>
            F14 = 0x7d,

            /// <summary>
            ///
            /// </summary>
            F15 = 0x7e,

            /// <summary>
            ///
            /// </summary>
            F16 = 0x7f,

            /// <summary>
            ///
            /// </summary>
            F17 = 0x80,

            /// <summary>
            ///
            /// </summary>
            F18 = 0x81,

            /// <summary>
            ///
            /// </summary>
            F19 = 130,

            /// <summary>
            ///
            /// </summary>
            F2 = 0x71,

            /// <summary>
            ///
            /// </summary>
            F20 = 0x83,

            /// <summary>
            ///
            /// </summary>
            F21 = 0x84,

            /// <summary>
            ///
            /// </summary>
            F22 = 0x85,

            /// <summary>
            ///
            /// </summary>
            F23 = 0x86,

            /// <summary>
            ///
            /// </summary>
            F24 = 0x87,

            /// <summary>
            ///
            /// </summary>
            F3 = 0x72,

            /// <summary>
            ///
            /// </summary>
            F4 = 0x73,

            /// <summary>
            ///
            /// </summary>
            F5 = 0x74,

            /// <summary>
            ///
            /// </summary>
            F6 = 0x75,

            /// <summary>
            ///
            /// </summary>
            F7 = 0x76,

            /// <summary>
            ///
            /// </summary>
            F8 = 0x77,

            /// <summary>
            ///
            /// </summary>
            F9 = 120,

            #endregion Fkeys

            #region Other

            /// <summary>
            ///
            /// </summary>
            OEM_1 = 0xba,

            /// <summary>
            ///
            /// </summary>
            OEM_102 = 0xe2,

            /// <summary>
            ///
            /// </summary>
            OEM_2 = 0xbf,

            /// <summary>
            ///
            /// </summary>
            OEM_3 = 0xc0,

            /// <summary>
            ///
            /// </summary>
            OEM_4 = 0xdb,

            /// <summary>
            ///
            /// </summary>
            OEM_5 = 220,

            /// <summary>
            ///
            /// </summary>
            OEM_6 = 0xdd,

            /// <summary>
            ///
            /// </summary>
            OEM_7 = 0xde,

            /// <summary>
            ///
            /// </summary>
            OEM_8 = 0xdf,

            /// <summary>
            ///
            /// </summary>
            OEM_CLEAR = 0xfe,

            /// <summary>
            ///
            /// </summary>
            OEM_COMMA = 0xbc,

            /// <summary>
            ///
            /// </summary>
            OEM_MINUS = 0xbd,

            /// <summary>
            ///
            /// </summary>
            OEM_PERIOD = 190,

            /// <summary>
            ///
            /// </summary>
            OEM_PLUS = 0xbb,

            #endregion Other

            #region KEYS

            /// <summary>
            ///
            /// </summary>
            KEY_0 = 0x30,

            /// <summary>
            ///
            /// </summary>
            KEY_1 = 0x31,

            /// <summary>
            ///
            /// </summary>
            KEY_2 = 50,

            /// <summary>
            ///
            /// </summary>
            KEY_3 = 0x33,

            /// <summary>
            ///
            /// </summary>
            KEY_4 = 0x34,

            /// <summary>
            ///
            /// </summary>
            KEY_5 = 0x35,

            /// <summary>
            ///
            /// </summary>
            KEY_6 = 0x36,

            /// <summary>
            ///
            /// </summary>
            KEY_7 = 0x37,

            /// <summary>
            ///
            /// </summary>
            KEY_8 = 0x38,

            /// <summary>
            ///
            /// </summary>
            KEY_9 = 0x39,

            /// <summary>
            ///
            /// </summary>
            KEY_A = 0x41,

            /// <summary>
            ///
            /// </summary>
            KEY_B = 0x42,

            /// <summary>
            ///
            /// </summary>
            KEY_C = 0x43,

            /// <summary>
            ///
            /// </summary>
            KEY_D = 0x44,

            /// <summary>
            ///
            /// </summary>
            KEY_E = 0x45,

            /// <summary>
            ///
            /// </summary>
            KEY_F = 70,

            /// <summary>
            ///
            /// </summary>
            KEY_G = 0x47,

            /// <summary>
            ///
            /// </summary>
            KEY_H = 0x48,

            /// <summary>
            ///
            /// </summary>
            KEY_I = 0x49,

            /// <summary>
            ///
            /// </summary>
            KEY_J = 0x4a,

            /// <summary>
            ///
            /// </summary>
            KEY_K = 0x4b,

            /// <summary>
            ///
            /// </summary>
            KEY_L = 0x4c,

            /// <summary>
            ///
            /// </summary>
            KEY_M = 0x4d,

            /// <summary>
            ///
            /// </summary>
            KEY_N = 0x4e,

            /// <summary>
            ///
            /// </summary>
            KEY_O = 0x4f,

            /// <summary>
            ///
            /// </summary>
            KEY_P = 80,

            /// <summary>
            ///
            /// </summary>
            KEY_Q = 0x51,

            /// <summary>
            ///
            /// </summary>
            KEY_R = 0x52,

            /// <summary>
            ///
            /// </summary>
            KEY_S = 0x53,

            /// <summary>
            ///
            /// </summary>
            KEY_T = 0x54,

            /// <summary>
            ///
            /// </summary>
            KEY_U = 0x55,

            /// <summary>
            ///
            /// </summary>
            KEY_V = 0x56,

            /// <summary>
            ///
            /// </summary>
            KEY_W = 0x57,

            /// <summary>
            ///
            /// </summary>
            KEY_X = 0x58,

            /// <summary>
            ///
            /// </summary>
            KEY_Y = 0x59,

            /// <summary>
            ///
            /// </summary>
            KEY_Z = 90,

            #endregion KEYS

            #region Volume

            /// <summary>
            /// Decrese volume
            /// </summary>
            VOLUME_DOWN = 0xae,

            /// <summary>
            /// Mute volume
            /// </summary>
            VOLUME_MUTE = 0xad,

            /// <summary>
            /// Increase volue
            /// </summary>
            VOLUME_UP = 0xaf,

            #endregion volume

            /// <summary>
            /// Take snapshot of the screen and place it on the clipboard
            /// </summary>
            SNAPSHOT = 0x2c,

            /// <summary>Send right click from keyboard "key that is 2 keys to the right of space bar"</summary>
            RightClick = 0x5d,

            /// <summary>
            /// Go Back or delete
            /// </summary>
            BACKSPACE = 8,

            /// <summary>
            /// Control + Break "When debuging if you step into an infinite loop this will stop debug"
            /// </summary>
            CANCEL = 3,

            /// <summary>
            /// Caps lock key to send cappital letters
            /// </summary>
            CAPS_LOCK = 20,

            /// <summary>
            /// Ctlr key
            /// </summary>
            CONTROL = 0x11,

            /// <summary>
            /// Alt key
            /// </summary>
            ALT = 18,

            /// <summary>
            /// "." key
            /// </summary>
            DECIMAL = 110,

            /// <summary>
            /// Delete Key
            /// </summary>
            DELETE = 0x2e,

            /// <summary>
            /// Arrow down key
            /// </summary>
            DOWN = 40,

            /// <summary>
            /// End key
            /// </summary>
            END = 0x23,

            /// <summary>
            /// Escape key
            /// </summary>
            ESC = 0x1b,

            /// <summary>
            /// Home key
            /// </summary>
            HOME = 0x24,

            /// <summary>
            /// Insert key
            /// </summary>
            INSERT = 0x2d,

            /// <summary>
            /// Open my computer
            /// </summary>
            LAUNCH_APP1 = 0xb6,

            /// <summary>
            /// Open calculator
            /// </summary>
            LAUNCH_APP2 = 0xb7,

            /// <summary>
            /// Open default email in my case outlook
            /// </summary>
            LAUNCH_MAIL = 180,

            /// <summary>
            /// Opend default media player (itunes, winmediaplayer, etc)
            /// </summary>
            LAUNCH_MEDIA_SELECT = 0xb5,

            /// <summary>
            /// Left control
            /// </summary>
            LCONTROL = 0xa2,

            /// <summary>
            /// Left arrow
            /// </summary>
            LEFT = 0x25,

            /// <summary>
            /// Left shift
            /// </summary>
            LSHIFT = 160,

            /// <summary>
            /// left windows key
            /// </summary>
            LWIN = 0x5b,

            /// <summary>
            /// Next "page down"
            /// </summary>
            PAGEDOWN = 0x22,

            /// <summary>
            /// Num lock to enable typing numbers
            /// </summary>
            NUMLOCK = 0x90,

            /// <summary>
            /// Page up key
            /// </summary>
            PAGE_UP = 0x21,

            /// <summary>
            /// Right control
            /// </summary>
            RCONTROL = 0xa3,

            /// <summary>
            /// Return key
            /// </summary>
            ENTER = 13,

            /// <summary>
            /// Right arrow key
            /// </summary>
            RIGHT = 0x27,

            /// <summary>
            /// Right shift
            /// </summary>
            RSHIFT = 0xa1,

            /// <summary>
            /// Right windows key
            /// </summary>
            RWIN = 0x5c,

            /// <summary>
            /// Shift key
            /// </summary>
            SHIFT = 0x10,

            /// <summary>
            /// Space back key
            /// </summary>
            SPACE_BAR = 0x20,

            /// <summary>
            /// Tab key
            /// </summary>
            TAB = 9,

            /// <summary>
            /// Up arrow key
            /// </summary>
            UP = 0x26
        }

    #endregion

        #endregion Keyboard
    }
}