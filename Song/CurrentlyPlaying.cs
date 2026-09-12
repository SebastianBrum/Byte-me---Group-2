using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Byte_me___Group_2
{
    public class CurrentlyPlaying
    {
        private string mSongName;
        private string mArtist;
        private string mDuration;
        private string mFilePath;
        private TimeSpan msecondsRemaining;

        private TimeSpan msecondsElapsed;
        private TimeSpan msecondsDuration;

       public CurrentlyPlaying(Song song)
        {
            this.mSongName = song.Name;
            this.mArtist = song.Artist;
            this.mDuration = song.Duration;
            this.mFilePath = song.SongFilePath;

            msecondsDuration = ParseDuration(this.mDuration);
        }

        public TimeSpan secondsElapsed
        {
            get { return msecondsElapsed; }
            set { msecondsElapsed = value; }
        }

        public TimeSpan secondsRemaining
        {
            get
            {
                msecondsRemaining = msecondsDuration - msecondsElapsed;
                return msecondsRemaining > TimeSpan.Zero ? msecondsRemaining : TimeSpan.Zero
            }
        }

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
