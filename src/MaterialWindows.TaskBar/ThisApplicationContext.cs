using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaterialWindows.TaskBar
{
    public class ThisApplicationContext : ApplicationContext
    {
        //Component declarations
        private NotifyIcon TrayIcon;
        private ContextMenuStrip TrayIconContextMenu;
        private ToolStripMenuItem CloseMenuItem;
        private ToolStripMenuItem PrintStateMenuItem;
        private readonly Actions actions;

        public ThisApplicationContext(Actions actions)
        {
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            this.actions = actions;
            InitializeComponent();
            TrayIcon.Visible = true;
        }

        private void InitializeComponent()
        {
            TrayIcon = new NotifyIcon();

            TrayIcon.BalloonTipIcon = ToolTipIcon.Info;
            TrayIcon.BalloonTipText =
                "I noticed that you double-clicked me! What can I do for you?";
            TrayIcon.BalloonTipTitle = "You called Master?";
            TrayIcon.Text = "My fabulous tray icon demo application";


            //The icon is added to the project resources.
            //Here I assume that the name of the file is 'TrayIcon.ico'
            TrayIcon.Icon = new Icon("MaterialWindows.TaskBar.ico");

            //Optional - handle doubleclicks on the icon:
            TrayIcon.DoubleClick += TrayIcon_DoubleClick;

            //Optional - Add a context menu to the TrayIcon:
            TrayIconContextMenu = new ContextMenuStrip();
            CloseMenuItem = new ToolStripMenuItem();
            PrintStateMenuItem = new ToolStripMenuItem();
            TrayIconContextMenu.SuspendLayout();

            // 
            // TrayIconContextMenu
            // 
            this.TrayIconContextMenu.Items.AddRange(new ToolStripItem[] {
                this.PrintStateMenuItem,
                this.CloseMenuItem
                });
            this.TrayIconContextMenu.Name = "TrayIconContextMenu";
            this.TrayIconContextMenu.Size = new Size(153, 70);
            // 
            // CloseMenuItem
            // 
            this.CloseMenuItem.Name = "CloseMenuItem";
            this.CloseMenuItem.Size = new Size(152, 22);
            this.CloseMenuItem.Text = "Close the tray icon program";
            this.CloseMenuItem.Click += new EventHandler(this.CloseMenuItem_Click);


            // PrintState Menu Item

            this.PrintStateMenuItem.Name = "PrintStateMenuItem";
            this.PrintStateMenuItem.Size = new Size(152, 22);
            this.PrintStateMenuItem.Text = "Print All State";
            this.PrintStateMenuItem.Click += new EventHandler(this.actions.dumpWindowList);

            TrayIconContextMenu.ResumeLayout(false);
            TrayIcon.ContextMenuStrip = TrayIconContextMenu;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
            TrayIcon.Visible = false;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            //Here you can do stuff if the tray icon is doubleclicked
            TrayIcon.ShowBalloonTip(10000);
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to close me?",
                "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}