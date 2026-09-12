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
        private bool isPlaying = true;
        private bool Dragging = false;
        private double width;
        private bool shufflePlay = false;

        private PlaylistControl PlaylistPanel;

        private AxWindowsMediaPlayer songPlayer;

        private CurrentlyPlaying songCurrentlyPlaying;

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

        public void setCurrentlyPlaying(CurrentlyPlaying song)
        {
            this.songCurrentlyPlaying = song;
        }

        /// Play or pause the song
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                songPlayer.Ctlcontrols.play();
                isPlaying = true;
                btnPlay.Text = "Ⅱ";
            }
            else
            {
                songPlayer.Ctlcontrols.pause();
                isPlaying = false;
                btnPlay.Text = "▶";
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (songCurrentlyPlaying == null)
                return; // nothing has played yet, nothing to go back to

            if (songCurrentlyPlaying.currentlyPlayingIndex > 0)
            {
                int previousIndex = songCurrentlyPlaying.currentlyPlayingIndex - 1;
                string previousFilePath = PlaylistPanel.Songs[previousIndex].SongFilePath;

                PlaylistPanel.playSong(previousIndex, previousFilePath);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            playNextSong();
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            // Gets the amount of songs in the playlist
            int SongCount = PlaylistPanel.Songs.Count;

            Random randomSongIndex = new Random();

            // Generate a random number to select a random song
            int songIndex = randomSongIndex.Next(0, SongCount);

            // If a song is currently playing it will not start playing a new one.
            if (songCurrentlyPlaying == null)
            {
                PlaylistPanel.playSong(songIndex, PlaylistPanel.Songs[songIndex].SongFilePath);
            }

            // Sets shuffle to true if it was false and false if it was true
            shufflePlay = !shufflePlay;

            // Changes the background color of the button to indicate if shuffle is active.
            if (shufflePlay)
            {
                btnShuffle.BackColor = Color.Lime;
            } 
            else
            {
                btnShuffle.BackColor = this.BackColor;
            }
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

                if (secondsElapsed >= duration)
                {
                    playNextSong();
                }
            }
        }

        /// <summary>
        /// Plays the next song
        /// </summary>
        private void playNextSong()
        {
            if (shufflePlay == true)
            {
                int SongCount = PlaylistPanel.Songs.Count;
                Random randomSongIndex = new Random();
                // Generate a random number to select a random song
                int songIndex = randomSongIndex.Next(0, SongCount);
                PlaylistPanel.playSong(songIndex, PlaylistPanel.Songs[songIndex].SongFilePath);
            }

            if (songCurrentlyPlaying.currentlyPlayingIndex < PlaylistPanel.Songs.Count - 1)
            {
                string nextFilePath = PlaylistPanel.Songs[songCurrentlyPlaying.currentlyPlayingIndex + 1].SongFilePath;
                int nextIndex = songCurrentlyPlaying.currentlyPlayingIndex + 1;

                PlaylistPanel.playSong(nextIndex, nextFilePath);
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

            // Updates the time elapsed on the CurrentlyPlaying class
            songCurrentlyPlaying.secondsElapsed = secondsElapsed;
        }
    }
}
