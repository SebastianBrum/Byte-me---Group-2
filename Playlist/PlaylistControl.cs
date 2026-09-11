using Byte_me___Group_2.Playlist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Byte_me___Group_2
{
    public partial class PlaylistControl : UserControl
    {
        private Home homeForm;

        private string username, coversFolder, playlistsFolder, dataFolder;
        OpenFileDialog ofdCoverPicture;
        Panel pnlMainContent, pnlSidebar;
        Button btnNewPlaylist, btnChangeCoverPhoto;

        // The list of songs in the playlist
        private BindingList<Song> Songs;

        // The class for the song that is currently playing
        CurrentlyPlaying currentlyPlayingSong;

        SongHistory SongsQueue;


        public PlaylistControl(Home homeForm, string username, string coversFolder, string playListsFolder, string dataFolder, OpenFileDialog ofdCoverPicture, Panel pnlMain, Panel Sidebar, Button btnNewPlayList, Button btnChangeCover)
        {
            InitializeComponent();

            this.username = username;
            this.coversFolder = coversFolder;
            this.playlistsFolder = playListsFolder;
            this.dataFolder = dataFolder;
            this.ofdCoverPicture = ofdCoverPicture;
            this.pnlMainContent = pnlMain;
            this.btnNewPlaylist = btnNewPlayList;
            this.btnChangeCoverPhoto = btnChangeCover;
            this.pnlSidebar = Sidebar;
            this.homeForm = homeForm;


            playerControls1.setSongPlayer(wmpSongPlay);
        }

        /// <summary>
        /// Basic display setup when the user opens a playlist
        /// </summary>
        /// <param name="title"> The playlist that the user opened </param>
        public void setupPlaylistPage(string title)
        {
            lblPlaylistName.Text = title;
            lblPlaylistPathName.Text = title;
            lblDateCreated.Text = null;
            lblWelecome.Text = $"Welcome back, {this.username}";
            
            // Read the songs from the textfile
            readSongs(title);
            dgvDisplaySongs.DataSource = Songs;

            playlistVisible(false);
            homeVisible(false);
            addPLaylistButton(false);
            changeCoverButton(false);
            loadImage(title, pbxPlaylistCoverPhoto);

            lblPlalistCount.Text = $"This playlist has {Songs.Count} songs";

            //Gets the creation date of the playlist and displays it
            string creationDate = File.GetCreationTime(Path.Combine(playlistsFolder, $"{title}.txt")).ToString("dd MMMM yyyy");
            addToCreationLabel(creationDate);

            SongsQueue = new SongHistory( playlistsFolder, title );
        }

        //Goes back to the home screen when the user clicks on the filepath
        private void lblBreadcrumb_Click(object sender, EventArgs e)
        {
            playlistVisible(true);
            homeVisible(true);
            addPLaylistButton(true);
            changeCoverButton(true);
        }

        //Goes back to the home screen when the user clicks back
        private void btnBackHome_Click(object sender, EventArgs e)
        {
            playlistVisible(true);
            homeVisible(true);
            addPLaylistButton(true);
            changeCoverButton(true);
        }

        /// <summary>
        /// Loads the picture into a picturebox
        /// </summary>
        /// <param name="playlist"> The playlist name </param>
        /// <param name="pictureBox"> The picturebox that needs to be updated </param>
        private void loadImage(string playlist, PictureBox pictureBox)
        {
            // Write all the image files to an array
            string[] coverImagesFiles = Directory.GetFiles(coversFolder);

            //Boolean to check if the user selected a file for the playlist
            bool hasImage = false;

            // Iterate through the images in the folder
            foreach (string coverImage in coverImagesFiles)
            {
                // Gets the image name without the extension
                string currentImage = Path.GetFileNameWithoutExtension(coverImage);


                //  Checks if the current image in the loop is the one corresponding with the playlist
                if (currentImage == playlist)
                {
                    pictureBox.Image = Image.FromFile(coverImage);
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

                    // The user has a image for the playlist
                    hasImage = true;
                }
            }

            // Sets the default image if the user didn't set an image themselves
            if (!hasImage)
            {
                string defaultImagePath = Path.Combine(dataFolder, "DefaultCover", "default.png");
                pictureBox.Image = Image.FromFile(defaultImagePath);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        //Makeing the active playlist visible on a panel, or making it invisible if the home screen is showing
        private void playlistVisible(bool home)
        {
            this.Visible = !home;
            this.Enabled = !home;

            Home frmHome = new Home(username);
            frmHome.Width = pnlMainContent.Width;
        }

        //Making the home screen visible on a panel, or making it invisible if a playlist is active
        private void homeVisible(bool home)
        {
            pnlMainContent.Visible = home;
            pnlMainContent.Enabled = home;

            if (home)
            { 
                homeForm.adjustFormHomeWidth(pnlMainContent.Width + 75);
            }
        }

        private void addPLaylistButton(bool home)
        {
            btnNewPlaylist.Visible = !home;
            btnNewPlaylist.Enabled = !home;
        }

        private void changeCoverButton(bool home)
        {
            btnChangeCoverPhoto.Visible = !home;
            btnChangeCoverPhoto.Enabled = !home;
        }

        private void addToCreationLabel(string val)
        {
            if (lblDateCreated.Text != "")
            {
                lblDateCreated.Text += $"•{val}";
            }
            else
            {
                lblDateCreated.Text = val;
            }
        }

        //Reads the songs from the textfile into the songs list
        private void readSongs(string playlistName)
        {
            string filepath = Path.Combine(
            dataFolder,
            this.username,
            "Playlists",
            playlistName + ".txt"
            );

            //Removes the old playlist's songs from the list
            if (Songs != null) Songs.Clear();


            Songs = new BindingList<Song>();

            //Reads from the textfile
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    int totalSongs = 0;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        MessageBox.Show(line);
                        Songs.Add( new Song(line) );

                        totalSongs++;
                    }

                    addToCreationLabel($"{totalSongs} tracks");
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Playlist file corrupted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //
        private void dgvDisplaySongs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Gaurd against the header being clicked
            if (e.RowIndex < 0)
            {
                return;
            }

            // Deletes the song which delete button is pressed
            if (e.ColumnIndex > 0 && dgvDisplaySongs.Columns[e.ColumnIndex].Name == "SongDelete")
            {
                DeleteSong(e.RowIndex, lblPlaylistName.Text);
            } 
            else
            {
                //Plays the song that is clicked
                wmpSongPlay.URL = Songs[e.RowIndex].SongFilePath;
                wmpSongPlay.Ctlcontrols.play();

                currentlyPlayingSong = new CurrentlyPlaying();

                playSong(e.RowIndex);
            }
        }

        public void playSong(int rowIndex)
        {
            playerControls1.lblTime.Text = dgvDisplaySongs[2, rowIndex].Value.ToString();

            playerControls1.tmrTrackbarTime.Enabled = true;
            
        }

       

        private void DeleteSong(int songIndex, string playlistName)
        {
            // Get datafolder
            string dataFolder = Path.Combine(
                Application.StartupPath,
                "Data");

            // Get the playlist file
            string[] files = Directory.GetFiles(
                dataFolder,
                playlistName + ".txt",
                SearchOption.AllDirectories);

            // Test if the playlist exists
            if (files.Length == 0)
            {
                MessageBox.Show("Playlist not found.");
                return;
            }

            // Saves the current playlist path
            string playlistPath = files[0];

            // Checks if the playlist does have songs
            if (Songs.Count == 0)
            {
                MessageBox.Show("There are no songs to delete.");
                return;
            }

            // Checks if the selected song actually exists in the playlist
            if (songIndex < 0 || songIndex >= Songs.Count)
            {
                return;
            }

            // Gets the song that the user selected
            string songToDelete = Songs[songIndex].Name;

            // Ask the user for confirmation
            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to delete \"{songToDelete}\"?",
                "Delete Song",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            // Stop if the user selects No
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            // Deletes the song from the list
            Songs.RemoveAt(songIndex);

            // Writes remaining songs back into the playlist
            //File.WriteAllLines(playlistPath, remainingSongs);
            using (StreamWriter writer = new StreamWriter(playlistPath))
            {
                foreach (Song song in Songs)
                {
                    writer.WriteLine($"{song.Name}|{song.Artist}|{song.Duration}|{song.SongFilePath}");
                }
            }

            // Rerenders all the remaining songs to display
            //readSongs(playlistName);

            // Shows a message for successful deletion
            MessageBox.Show("Song deleted successfully.");
        }

        public void changeCoverPhoto()
        {
            //Prevents the user from selected a file that isn't an image
            ofdCoverPicture.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            //Open the filedialog
            if (ofdCoverPicture.ShowDialog() == DialogResult.OK)
            {
                // Gets the image's filepath
                string filepath = ofdCoverPicture.FileName;

                //Gets the image extension
                string extension = Path.GetExtension(filepath);

                // Changes the coverPhoto visually
                pbxPlaylistCoverPhoto.Image = Image.FromFile(filepath);

                //Makes the image fit
                pbxPlaylistCoverPhoto.SizeMode = PictureBoxSizeMode.StretchImage;

                //Copies the coverphoto over to the covers file.
                File.Copy(filepath, Path.Combine(coversFolder, lblPlaylistName.Text + extension), true);
            }
        }

        //Deletes the current playlist
        private void btnDelete_Click(object sender, EventArgs e)
        {
            Home homePage = new Home(this.username);
            homePage.deletePlaylist(lblPlaylistName.Text, Path.Combine(playlistsFolder, lblPlaylistName.Text + ".txt"));
            btnBackHome_Click(sender, e);
        }
    }
}
