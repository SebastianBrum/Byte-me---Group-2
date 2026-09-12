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

        private PlaylistControl PlaylistPanel;

        private AxWindowsMediaPlayer songPlayer;

        public PlayerControls()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the mediaplayer to a local variable so that it is accesible in this userControl
        /// </summary>
        /// <param name="player"> The mediaplayer </param>
        public void setSongPlayer(AxWindowsMediaPlayer player)
        {
            songPlayer = player;
        }

        public void setPlaylistPanel(PlaylistControl playlistPanel)
        {
            this.PlaylistPanel = playlistPanel;
        }

        /// Play or pause the song
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!isPlaying) 
            {
                songPlayer.Ctlcontrols.play();
                isPlaying = true;
                btnPlay.Text = "▶";
            }
            else
            {
                songPlayer.Ctlcontrols.pause();
                isPlaying = false;
                btnPlay.Text = "Ⅱ";
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {

        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            int SongCount = PlaylistPanel.Songs.Count;

            Random randomSongIndex = new Random();
            int songIndex = randomSongIndex.Next(0, SongCount);

            PlaylistPanel.playSong(songIndex, PlaylistPanel.Songs[songIndex].SongFilePath);
        }

        /// <summary>
        /// Updates the trackbar display and the display that indicates how much time the song has been playing
        /// </summary>
        /// <param name="sender"> The element that activated the eventhandler </param>
        /// <param name="e"> A variable that saves the column and row index of the cell that was clicked </param>
        private void tmrTrackbarTime_Tick(object sender, EventArgs e)
        {
            // Get the duration of the song
            TimeSpan duration = TimeSpan.FromSeconds(songPlayer.currentMedia.duration);

            //Get the time that the song has been playing
            TimeSpan secondsElapsed = TimeSpan.FromSeconds(songPlayer.Ctlcontrols.currentPosition);

            // Check if time has passed, and that the duration is not null. It will be null because it takes a few milliseconds to set the Mediaplayer's url
            if (duration.TotalSeconds > 0 && !(double.IsNaN(duration.TotalSeconds)))
            {
                // Calculates the percentage that the song has completed
                double done = secondsElapsed.TotalSeconds / duration.TotalSeconds;

                // Calculates the width that the child panel needs to be with respect to the parent panel
                width = Math.Ceiling(done * pnlTrackParent.Width);

                // Sets the width of the child panel
                pnlTrackChild.Width = Convert.ToInt32(width);

                // Updates lblElapsed
                UpdateTimeElapsed(secondsElapsed);

                //button2.Left = panel1.Left + pnlTime.Width - Convert.ToInt32(button2.Width * 0.5);
                //button2.Top = panel1.Top + pnlTime.Top - Convert.ToInt32(button2.Height * 0.4);
            }
        }

        /// <summary>
        /// Updates lblElapsed to show the total time that the song has been playing
        /// </summary>
        /// <param name="secondsElapsed"> The total seconds the song has been playing </param>
        private void UpdateTimeElapsed(TimeSpan secondsElapsed)
        {
            // Formats the time displayed so that it is minutes : seconds
            lblElpased.Text = $"{(int)secondsElapsed.TotalMinutes:D2}:{secondsElapsed.Seconds:D2}";
        }
    }
}
