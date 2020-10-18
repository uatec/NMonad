using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialWindows.TaskBar.Win32Interop;

namespace MaterialWindows.TaskBar
{
    public class MessageForm : Form
    {
       
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
            Win32.SetWindowLong(this.Handle, Win32.GWL.ExStyle, Win32.WS_EX.Layered | Win32.WS_EX.Transparent);

            //Set the Alpha for our window to the percentage specified by our TransparentAlpha trackbar.
            //Note: This has NOTHING to do with making the form transparent to the mouse!  This is solely
            //for visual effect!
            Win32.SetLayeredWindowAttributes(this.Handle, 0, (byte)(0.75f * 255), Win32.LWA.Alpha);
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