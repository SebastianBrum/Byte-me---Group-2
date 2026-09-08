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

        public string duration
        {
            get { return mDuration}
            set { mDuration = value; }
        }
    }
}
