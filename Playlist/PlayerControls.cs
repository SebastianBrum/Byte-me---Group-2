using AxWMPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Byte_me___Group_2.Playlist
{
    public partial class PlayerControls : UserControl
    {
        private bool isPlaying = false;
        private bool Dragging = false;
        private double width;

        private AxWindowsMediaPlayer songPlayer;

        public PlayerControls()
        {
            InitializeComponent();
        }

        public void setSongPlayer(AxWindowsMediaPlayer player)
        {
            songPlayer = player;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!isPlaying) 
            {
                songPlayer.Ctlcontrols.play();
                isPlaying = true;
            }
            else
            {
                songPlayer.Ctlcontrols.pause();
                isPlaying = false;
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {

        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {

        }

        private void tmrTrackbarTime_Tick(object sender, EventArgs e)
        {
            TimeSpan duration = TimeSpan.FromSeconds(songPlayer.currentMedia.duration);
            TimeSpan secondsElapsed = TimeSpan.FromSeconds(songPlayer.Ctlcontrols.currentPosition);

            if (duration.TotalSeconds > 0 && !(double.IsNaN(duration.TotalSeconds)))
            {
                double done = secondsElapsed.TotalSeconds / duration.TotalSeconds;
                width = Math.Ceiling(done * pnlTrackParent.Width);
                pnlTrackChild.Width = Convert.ToInt32(width);

                UpdateTimeElapsed(secondsElapsed);
                //button2.Left = panel1.Left + pnlTime.Width - Convert.ToInt32(button2.Width * 0.5);
                //button2.Top = panel1.Top + pnlTime.Top - Convert.ToInt32(button2.Height * 0.4);
            }
        }

        private void UpdateTimeElapsed(TimeSpan secondsElapsed)
        {
            lblElpased.Text = $"{(int)secondsElapsed.TotalMinutes:D2}:{secondsElapsed.Seconds:D2}";
        }
    }
}
