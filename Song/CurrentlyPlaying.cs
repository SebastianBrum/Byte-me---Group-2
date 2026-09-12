using Byte_me___Group_2.Playlist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Byte_me___Group_2
{
    public class CurrentlyPlaying
    {
        private PlayerControls MediaPlayerControls;

        private string mSongName;
        private string mArtist;
        private string mDuration;
        private string mFilePath;
        private int mcurrentlyPlayingIndex;

        // Timespan for the seconds remaining of the current song
        private TimeSpan msecondsRemaining;

        // Timespan for the seconds that the current song has been playing
        private TimeSpan msecondsElapsed;

        // Timespan for the time remaining of the current song
        private TimeSpan msecondsDuration;

        /// <summary>
        /// Constructor to set the default values
        /// </summary>
        /// <param name="song"> The current song being played </param>
       public CurrentlyPlaying(Song song)
        {
            this.mSongName = song.Name;
            this.mArtist = song.Artist;
            this.mDuration = song.Duration;
            this.mFilePath = song.SongFilePath; 

            msecondsDuration = ParseDuration(this.mDuration);

        }

        public void setMediaPlayer(PlayerControls mediaPlayerControls)
        {
            this.MediaPlayerControls = mediaPlayerControls;

            // Sets the CurrentlyPlayingSong object
            MediaPlayerControls.setCurrentlyPlaying(this);
        }

        public string Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }
        public string Artist
        {
            get { return mArtist; }
            set { mArtist = value; }
        }
        public string SongName
        {
            get { return mSongName; }
            set { mSongName = value; }
        }
        public string FilePath
        {
            get { return mFilePath; }
            set { mFilePath = value; }
        }

        /// <summary>
        /// The seconds that the song has been playing
        /// </summary>
        public TimeSpan secondsElapsed
        {
            get { return msecondsElapsed; }
            set { msecondsElapsed = value; }
        }

        /// <summary>
        /// The seconds remaining of the current song
        /// </summary>
        public TimeSpan secondsRemaining
        {
            get
            {
                this.msecondsRemaining = msecondsDuration - msecondsElapsed;
                return msecondsRemaining > TimeSpan.Zero ? msecondsRemaining : TimeSpan.Zero;
            }
        }

        public int currentlyPlayingIndex
        {
            get { return mcurrentlyPlayingIndex; }
            set { this.mcurrentlyPlayingIndex = value; }
        }


        /// <summary>
        /// Converts the duration time from a string into a TimeSpan
        /// </summary>
        /// <param name="duration"> The duration as a string </param>
        /// <returns> The duration as a TimeSpan </returns>
        private TimeSpan ParseDuration(string duration)
        {
            bool minutesTime = false;
            bool secondsTime = false;
            string[] timeParts = mDuration.Split(':');

            if (timeParts.Length == 2)
            {
                minutesTime = int.TryParse(timeParts[0], out int minutes);
                secondsTime = int.TryParse(timeParts[1], out int seconds);

                if (minutesTime && secondsTime)
                {
                    return new TimeSpan(0, minutes, seconds);
                }
            }

            return TimeSpan.Zero;
        }
    }
}
