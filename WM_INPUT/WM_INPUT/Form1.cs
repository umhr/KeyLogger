using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WM_INPUT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            setUp();
        }
        private void setUp()
        {

            // http://hongliang.seesaa.net/article/18076786.html

            const int RIDEV_INPUTSINK = 0x00000100;
            //Console.WriteLine(this.Handle);

            int size = Marshal.SizeOf(typeof(RawInputDevice));
            RawInputDevice[] devices = new RawInputDevice[2];
            // UsagePage=1,Usage=2 でマウスデバイスを指す
            // UsagePage=1,Usage=6 でキーボードデバイスを指す
            // UsagePage=13,Usage=4 でタッチスクリーンデバイスを指す
            devices[0].UsagePage = 1;

            devices[0].Usage = 2;
            //WM_INPUT を受け取るウィンドウ
            devices[0].Target = this.Handle;
            // windowが最前面でなくても受け取れるように！
            devices[0].Flags = RIDEV_INPUTSINK;

            devices[1].UsagePage = 1;

            devices[1].Usage = 6;
            //WM_INPUT を受け取るウィンドウ
            devices[1].Target = this.Handle;
            // windowが最前面でなくても受け取れるように！
            devices[1].Flags = RIDEV_INPUTSINK;

            //WM_INPUT を有効にするデバイス群、devices の数、
            //  RawInputDevice の構造体サイズ
            RegisterRawInputDevices(devices, 2, size);
        }
        private void ProcessInputKey(ref Message m)
        {

            const int RidInput = 0x10000003;
            int headerSize = Marshal.SizeOf(typeof(RawInputHeader));
            int size = Marshal.SizeOf(typeof(RawInput));
            RawInput input;
            GetRawInputData(m.LParam, RidInput, out input, ref size, headerSize);
            string str = "";
            if (input.Header.Type == 0)
            {

                short ButtonData = input.c;//クリック
                sbyte RawButtons = (sbyte)(input.d >> 8);
                sbyte delta = (sbyte)(input.d);
                int LastX = input.f;
                int LastY = input.g;

                str = string.Format("{0}({1},{2},{3},{4},{5})\r\n", input.Header.Device, "Mouse", LastX, LastY, ButtonData, delta);
                if (str != "0,0,0,0,0\r\n")
                {
                    log(str);
                }
            }
            else if (input.Header.Type == 1)
            {

                string message = "";
                ushort VKey = input.d;
                if (input.e == 256)
                {
                    message = "keyDown";
                }
                else if (input.e == 257)
                {
                    message = "keyUp";
                }
                str = string.Format("{0}({1},{2})\r\n", input.Header.Device, message, VKey);
                log(str);
            }
        }
        private void log(string msg)
        {
            writeConsole(msg);
            Console.WriteLine(msg);
            textBox1.Text = msg;
        }

        protected override void WndProc(ref Message m)
        {
            const int WmInput = 0xFF;
            if (m.Msg == WmInput)
                this.ProcessInputKey(ref m);
            base.WndProc(ref m);
        }

        [DllImport("user32.dll")]
        private static extern int RegisterRawInputDevices(
       RawInputDevice[] devices, int number, int size);
        [DllImport("user32.dll")]
        private static extern int GetRawInputData(
            IntPtr rawInput, int command, out RawInput data,
            ref int size, int headerSize);
        private struct RawInputDevice
        {
            public short UsagePage;
            public short Usage;
            public int Flags;
            public IntPtr Target;
        }
        private struct RawInputHeader
        {
            public int Type;
            public int Size;
            public IntPtr Device;
            public IntPtr WParam;
        }
        private struct RawInput
        {
            public RawInputHeader Header;

            public short a;//Makecode,Flags
            public short b;//Flags,ButtonFlags
            public short c;//Reserved,ButtonData
            public ushort d;//VKey,RawButtons
            public ushort e;//Message,RawButtons
            public int f;//Message,LastX
            public int g;//ExtraInformation,LastY
            public int h;//ExtraInformation
        }


        private struct RawMouse
        {
            public short Flags;
            public short ButtonFlags;
            public short ButtonData;
            public int RawButtons;
            public int LastX;
            public int LastY;
            public int Extra;
        }
        private struct Rawkeyboard
        {
            public ushort Makecode;                 // Scan code from the key depression
            public ushort Flags;                    // One or more of RI_KEY_MAKE, RI_KEY_BREAK, RI_KEY_E0, RI_KEY_E1
            public ushort Reserved;                 // Always 0    
            public ushort VKey;                     // Virtual Key Code
            public uint Message;                    // Corresponding Windows message for exmaple (WM_KEYDOWN, WM_SYASKEYDOWN etc)
            public uint ExtraInformation;           // The device-specific addition information for the event (seems to always be zero for keyboards)
        }
        // [C#] WinFormのプログラムからコンソール(標準出力)に文字を出力する
        // http://nanoappli.com/blog/archives/2363
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        //*********************************************************************
        /// <summary> コンソールに文字を出力する
        /// 
        /// </summary>
        /// <param name="msg">  出力する文字列</param>
        /// <returns>           true:成功, false:失敗</returns>
        //*********************************************************************
        private bool writeConsole(string msg)
        {

            if (!AttachConsole(System.UInt32.MaxValue))
            {
                return false;
            }

            // stdoutのストリームを取得
            System.IO.Stream stream = System.Console.OpenStandardOutput();
            System.IO.StreamWriter stdout = new System.IO.StreamWriter(stream);

            // 指定された文字列を出力
            stdout.WriteLine(msg);
            stdout.Flush();

            FreeConsole();
            return true;
        }


    }
}
