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

        private AxWindowsMediaPlayer songPlayer;

        public PlayerControls()
        {
            InitializeComponent();
        }

        public void setSongPlayer(AxWindowsMediaPlayer player)
        {
            songPlayer = player;
            MessageBox.Show(songPlayer.Name);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            MessageBox.Show(songPlayer.Name);
            songPlayer.Ctlcontrols.pause();
        }
    }
}
