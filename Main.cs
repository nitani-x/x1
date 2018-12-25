﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace x1
{
    public partial class Main : Form
    {
        #region epic dLLimportz
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern ushort GetAsyncKeyState(int vKey);

        public static bool IsKeyPushedDown(System.Windows.Forms.Keys vKey)
        {
            return 0 != (GetAsyncKeyState((int)vKey) & 0x8000);
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        private static IntPtr m_hTaskBar;

        public static void HideTaskBar()
        {
            m_hTaskBar = FindWindow("Shell_TrayWnd", null);

            if ((int)m_hTaskBar != 0)
            {
                ShowWindow(m_hTaskBar, 0);
            }
        }
        #endregion

        public Main()
        {
            InitializeComponent();
        }

        bool helpdialog = false;
        PowerStatus battery = SystemInformation.PowerStatus;
        Help help = new Help();


        private void Main_Load(object sender, EventArgs e)
        {
            spotifytrack.Anchor = AnchorStyles.Right;
            seperator.Anchor = AnchorStyles.Right;
            btry.Anchor = AnchorStyles.Right;
            time.Anchor = AnchorStyles.Right;
            panel_StuffHere.Anchor = AnchorStyles.Right;

            Width = Screen.PrimaryScreen.Bounds.Width;

            t1.Start();

            HideTaskBar();

            CheckBattery();

        }

        void CheckBattery()
        {
            int batterynumber = Convert.ToInt32(battery.BatteryLifePercent.ToString("P0").Trim(new Char[] { '%' }));
            if (battery.BatteryChargeStatus.ToString() != "NoSystemBattery")
            {
                if (batterynumber <= 100 && batterynumber >=80)
                {
                    btry.Text = "[----]";
                }

                if (batterynumber <= 80 && batterynumber >= 70)
                {
                    btry.Text = "[--- ]";
                }

                if (batterynumber <= 70 && batterynumber >= 50)
                {
                    btry.Text = "[--  ]";
                }

                if (batterynumber <= 50 && batterynumber >= 20)
                {
                    btry.Text = "[-   ]";
                }

                if (batterynumber <= 20 && batterynumber >= 0)
                {
                    btry.Text = "[    ]";
                }

            }
            else
            {
                spotifytrack.Location = new Point(98, 10);
                btry.Hide();
                p_Media.Location = new Point(366, 6);
            }
        }

        public string GetSpotifyTrackInfo()
        {
            var proc = Process.GetProcessesByName("Spotify").FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));
            if (string.Equals(proc.MainWindowTitle, "Spotify", StringComparison.InvariantCultureIgnoreCase))
            {
                return "       Paused";
            }
            return proc.MainWindowTitle;
        }


        #region haha im epic
        private void t1_Tick(object sender, EventArgs e)
        {
            var ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
            Location = new Point(0, ScreenHeight - 40);

            if (battery.BatteryChargeStatus.ToString() != "NoSystemBattery")
                CheckBattery();

            title.Text = GetActiveWindowTitle();
            time.Text = DateTime.Now.ToString("HH:mm");

            if (IsKeyPushedDown(Keys.F7) && helpdialog == false)
            {
                help.Show();
                helpdialog = true;
            }

            if (IsKeyPushedDown(Keys.F8) && helpdialog == true)
            {
                helpdialog = false;
                help.Hide();
            }

            if (IsKeyPushedDown(Keys.CapsLock))
            {
                if (IsKeyLocked(Keys.CapsLock))
                {
                    nt1.ShowBalloonTip(5, "x1", "capslock enabled", ToolTipIcon.None);
                }
                else
                {
                    nt1.ShowBalloonTip(5, "x1", "capslock disabled", ToolTipIcon.None);
                }
            }

            Process[] spotify = Process.GetProcessesByName("Spotify");

            if (spotify.Length == 0)
            {
                p_Media.Visible = false;
                spotifytrack.Visible = false;
            }
            else
            {
                spotifytrack.Text = GetSpotifyTrackInfo();
                spotifytrack.Visible = true;
                p_Media.Visible = true;
            }

        }
        #endregion

        private void p_Forward_Click(object sender, EventArgs e)
        {
            var KEYEVENTF_KEYUP = 0x0002;
            var mediaNextTrack = (byte)Keys.MediaNextTrack;
            keybd_event(mediaNextTrack, mediaNextTrack, 0, 0);
            keybd_event(mediaNextTrack, mediaNextTrack, KEYEVENTF_KEYUP, 0);
        }

        private void p_Back_Click(object sender, EventArgs e)
        {
            var KEYEVENTF_KEYUP = 0x0002;
            var mediaPreviousTrack = (byte)Keys.MediaPreviousTrack;
            keybd_event(mediaPreviousTrack, mediaPreviousTrack, 0, 0);
            keybd_event(mediaPreviousTrack, mediaPreviousTrack, KEYEVENTF_KEYUP, 0);
        }

        private void btry_MouseHover(object sender, EventArgs e)
        {
            ttp.Show(battery.BatteryLifePercent.ToString("P0"), btry);
        }
    }
}