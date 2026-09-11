using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Byte_me___Group_2
{
    public class Song
    {

        private string mName;
        private string mArtist;
        private string mDuration;

        private string mSongFilePath;


        /// <summary>
        /// It extracts the SongName, Artist, Duration and the FilePath of the song from the line paramater
        /// </summary>
        /// <param name="line"> The line that was read from the textfile </param>
        public Song(string line)
        {
            this.Name = getSongName(ref line);
            this.Artist = getSongArtist(ref line);
            this.Duration = getSongDuratiion(ref line);
            this.SongFilePath = line;
        }

        public string Name
        {
            get { return this.mName; }
            set { mName = value; }
        }

        public string Artist
        {
            get { return this.mArtist; }
            set { mArtist = value;}
        }

        public string Duration
        {
            get { return this.mDuration; }
            set { mDuration = value; }
        }

        public string SongFilePath
        {
            get { return this.mSongFilePath; }
            set { mSongFilePath = value; }
        }

        /// <summary>
        /// Gets the Name of the Song of the song
        /// </summary>
        /// <param name="line"> The text file line that was read, it subsets it so that Name of the Song is removed from the line and the remaining data is sent back </param>
        /// <returns> The Name of the song of the song</returns>
        private string getSongName(ref string line)
        {
            int characterPosition = line.IndexOf("|");
            string title = line.Substring(0, characterPosition);
            line = line.Substring(characterPosition + 1);
            return title;
        }

        /// <summary>
        /// Gets the Artist of the song
        /// </summary>
        /// <param name="line"> The text file line that was read, it subsets it so that Artist is removed from the line and the remaining data is sent back </param>
        /// <returns> The Artist of the song</returns>
        private string getSongArtist(ref string line)
        {
            int characterPosition = line.IndexOf("|");
            string artist = line.Substring(0, characterPosition);
            line = line.Substring(characterPosition + 1);
            return artist;
        }

        /// <summary>
        /// Gets the duration of the song
        /// </summary>
        /// <param name="line"> The text file line that was read, it subsets it so that duration is removed from the line and the remaining data is sent back </param>
        /// <returns> The duration of the song</returns>
        private string getSongDuratiion(ref string line)
        {
            int characterPosition = line.IndexOf("|");
            string duration = line.Substring(0, characterPosition);
            line = line.Substring(characterPosition + 1);
            return duration;
        }
    }
}
