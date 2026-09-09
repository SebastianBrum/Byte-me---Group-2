using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Byte_me___Group_2
{
    public class Song
    {

        private string mName;
        private string mArtist;
        private string mDuration;

        private string mSongFilePath;

        public Song(string line)
        {
            this.Name = getSongName(ref line);
            this.Artist = getSongArtist(ref line);
            this.Duration = getSongDuratiion(ref line);
        }

        public string Name
        {
            get { return mName;  }
            set { mName = value;  }
        }

        public string Artist
        {
            get { return mArtist; }
            set { mArtist = value;}
        }

        public string Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }

        //gets the song title from the textfile and saves it into the songs list
        private string getSongName(ref string line)
        {
            int characterPosition = line.IndexOf("|");
            string title = line.Substring(0, characterPosition);
            line = line.Substring(characterPosition + 1);
            return title;
        }

        //Gets the songs artist from the textfile and saves it into the artists list
        private string getSongArtist(ref string line)
        {
            int characterPosition = line.IndexOf("|");
            string artist = line.Substring(0, characterPosition);
            line = line.Substring(characterPosition + 1);
            return artist;
        }

        private string getSongDuratiion(ref string line)
        {
            int characterPosition = line.IndexOf("|");
            string duration = line.Substring(0, characterPosition);
            line = line.Substring(characterPosition + 1);
            return duration;
        }
    }
}
