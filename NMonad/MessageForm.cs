using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace NMonad
{
    public class MessageForm : Form
    {
        public static class Win32
        {
            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, WS_EX dsNewLong);

            [DllImport("user32.dll")]
            public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);
        }
        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        public enum GWL
        {
            ExStyle = -20
        }

        public enum WS_EX
        {
            Transparent = 0x20,
            Layered = 0x80000
        }
        public static void ShowMessage(string message, int interval)
        {

            MessageForm f = new MessageForm(message, interval);

            f.ShowDialog();
        }

        private readonly string _message;
        private readonly int _durationMs;

        private MessageForm(string message, int durationMs)
            : base()
        {
            _message = message;
            _durationMs = durationMs;

            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Controls.Add(new Label()
            {
                Text = _message,
                Font = new Font("Arial", 14.0f, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.FromArgb(0xCC, 0xCC, 0xCC),
                Padding = new Padding(30),
                Dock = DockStyle.Fill
            });
        }

        protected override void OnShown(EventArgs e)
        {
            Win32.SetWindowLong(this.Handle, GWL.ExStyle, WS_EX.Layered | WS_EX.Transparent);

            //Set the Alpha for our window to the percentage specified by our TransparentAlpha trackbar.
            //Note: This has NOTHING to do with making the form transparent to the mouse!  This is solely
            //for visual effect!
            Win32.SetLayeredWindowAttributes(this.Handle, 0, (byte)(0.75f * 255), LWA.Alpha);
            this.BackColor = Color.FromArgb(0x33, 0x33, 0x33);

            this.AutoSize = true;
            var t = new Timer
            {
                Interval = _durationMs
            };

            t.Tick += (sender, args) =>
            {
                this.Close();
            };
            t.Enabled = true;

            base.OnShown(e);
        }

    }
}