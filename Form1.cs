using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiCursors
{
    public partial class Form1 : Form
    {

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        };

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Int32 vKey);

        MemoryStream cursor;
        Image cursorImage;

        int cursorWidth = 0;
        int cursorHeight = 0;

        Thread t;

        public Form1()
        {
            cursorImage = Image.FromFile(@"cursor1.png");
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            this.Focus();
            t = new Thread(() =>
            {
                bool test = false;
                while (true)
                {

                    try
                    {
                        DrawCursorsOnForm(GetMousePosition());

                        bool mousePress = (GetAsyncKeyState(0x01) & (1 << 15)) != 0;
                        if (mousePress)
                        {
                            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, sec_cursor.Location.X, sec_cursor.Location.Y, 0, 0);
                        }

                    }
                    catch (Exception ex)
                    {

                       
                    }
        

                    Thread.Sleep(10);
                }
            });

            t.Start();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Set the form click-through
                cp.ExStyle |= 0x80000 /* WS_EX_LAYERED */ | 0x20 /* WS_EX_TRANSPARENT */;
                return cp;
            }
        }

        private MemoryStream getStream(string file)
        {

            byte[] f = File.ReadAllBytes(file);
            MemoryStream stream = new MemoryStream(f);
            return stream;
        }

        private void DrawCursorsOnForm(Point p)
        {
            p.X = (int)(p.X / 1.1);

            MethodInvoker invoker = delegate
            {

                if (sec_cursor.Width == 0 || sec_cursor.Height == 0)
                {
                    this.sec_cursor.Width = this.cursorWidth;
                    this.sec_cursor.Height = this.cursorHeight;
                }

                sec_cursor.Location = p;

                if (sec_cursor.Image != null)
                {
                    sec_cursor.Image.Dispose();
                }

                sec_cursor.Image = (Image)cursorImage.Clone();
            };

            this?.Invoke(invoker);


        }


     
     
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t != null && t.IsAlive) {
                t.Abort();
            }
        }
    }
}
