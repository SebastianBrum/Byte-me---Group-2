using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Byte_me___Group_2
{
    public partial class frmPlaylist : Form
    {
        private string playlistName;
        public frmPlaylist(string playlistName)
        {
            InitializeComponent();
            this.playlistName = playlistName;
        }
    }
}
