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
        public BindingList<Song> Songs;

        // The class for the song that is currently playing
        CurrentlyPlaying currentlyPlayingSong;


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
            playerControls1.setPlaylistPanel(this);
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

            playlistVisible(false);
            homeVisible(false);
            addPLaylistButton(false);
            changeCoverButton(false);
            loadImage(title, pbxPlaylistCoverPhoto);

            lblPlalistCount.Text = $"This playlist has {Songs.Count} songs";

            //Gets the creation date of the playlist and displays it
            string creationDate = File.GetCreationTime(Path.Combine(playlistsFolder, $"{title}.txt")).ToString("dd MMMM yyyy");
            addToCreationLabel(creationDate);
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

        // Makes the button to add a new playlist visible on the sidebar
        private void addPLaylistButton(bool home)
        {
            btnNewPlaylist.Visible = !home;
            btnNewPlaylist.Enabled = !home;
        }

        // Makes the button to change the cover photo visible on the sidebar
        private void changeCoverButton(bool home)
        {
            btnChangeCoverPhoto.Visible = !home;
            btnChangeCoverPhoto.Enabled = !home;
        }

        /// <summary>
        /// Shows the the that the playlist was created
        /// </summary> 
        /// <param name="val"> The value to add to the display </param>
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

        /// <summary>
        /// Reads the songs from the playlist. Format: SongName|SongArtist|SongDuration|SongFilePath
        /// </summary>
        /// <param name="playlistName"> The name of the playlist </param>
        private void readSongs(string playlistName)
        {
            // Get the filepath to where the playlist's textfile is saved.
            string filepath = Path.Combine(
            dataFolder,
            this.username,
            "Playlists",
            playlistName + ".txt"
            );

            //Removes the old playlist's songs from the list
            if (Songs != null) Songs.Clear();

            //Create a new binding list
            Songs = new BindingList<Song>();

            //Reads from the textfile
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    // Keep track of the total sonngs in the playlist
                    int totalSongs = 0;

                    // The current line being read
                    string line;

                    // Reads through the textfile
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Adds the currentline to the Songs binding list by initialiing a new Song Object
                        Songs.Add( new Song(line) );

                        // Set the dustbin icon in the delete column of the datagridview
                        dgvDisplaySongs.Columns["SongDelete"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvDisplaySongs.Columns["SongDelete"].DefaultCellStyle.ForeColor = Color.Red;

                        // Increases the total songs in the playlist
                        totalSongs++;
                    }

                    // Display the total songs in the playlist on a label
                    addToCreationLabel($"{totalSongs} tracks");
                    dgvDisplaySongs.DataSource = Songs;
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

        // Deletes the playlist that was clicked
        private void dgvDisplaySongs_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0)
            {
                SortPlaylistSongs(dgvDisplaySongs.Columns[e.ColumnIndex].Name, false, lblPlaylistName.Text);
                return;
            }

            // First check is to check if the column on the far left is clicked
            // The second check is to check if the song needs to be deleted
            if (e.ColumnIndex > 0 && dgvDisplaySongs.Columns[e.ColumnIndex].Name == "SongDelete")
            {
                DeleteSong(e.RowIndex, lblPlaylistName.Text);
            } 
            else
            {
                wmpSongPlay.URL = Songs[e.RowIndex].SongFilePath;

                playSong(e.RowIndex, Songs[e.RowIndex].SongFilePath);
            }
        }

        /// <summary>
        /// Plays the song
        /// </summary>
        /// <param name="rowIndex"> The index in the list that saves the songs in the current playlist </param>
        /// <param name="filepath"> The filepath to the current song being played </param>
        public void playSong(int rowIndex, string filepath)
        {
            //Set the URL of the mediaplayer equal to the song's filepath
            wmpSongPlay.URL = filepath;

            // Play the song
            wmpSongPlay.Ctlcontrols.play();


            currentlyPlayingSong = new CurrentlyPlaying(Songs[rowIndex]);

            setCurrentSongDisplay(rowIndex);
        }

        public void setCurrentSongDisplay(int rowIndex)
        {
            playerControls1.tmrTrackbarTime.Enabled = true;
            playerControls1.lblTime.Text = dgvDisplaySongs[2, rowIndex].Value.ToString();

            playerControls1.lblArtistName.Text = dgvDisplaySongs[1, rowIndex].Value.ToString();
            playerControls1.lblCurrentSong.Text = dgvDisplaySongs[0, rowIndex].Value.ToString();
        }

        private void btnAddsongs_Click(object sender, EventArgs e)
        {
            homeForm.btnUploadSong_Click(sender, e);
            Songs.Clear();
            readSongs(lblPlaylistName.Text);
            MessageBox.Show(Songs.Last().Name);
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
            using (StreamWriter writer = new StreamWriter(playlistPath))
            {
                foreach (Song song in Songs)
                {
                    writer.WriteLine($"{song.Name}|{song.Artist}|{song.Duration}|{song.SongFilePath}");
                }
            }

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
            // Create a new instance of the home class sending the user's username as paramater
            Home homePage = new Home(this.username);
            homePage.deletePlaylist(lblPlaylistName.Text, Path.Combine(playlistsFolder, lblPlaylistName.Text + ".txt"));
            btnBackHome_Click(sender, e);
        }

        //This method sorts the songs according to the selected option
        private void SortPlaylistSongs(string sortBy, bool ascending, string playlistName)
        {
            //Creates a list that stores the positions of the songs
            List<int> songIndexes = new List<int>();

            //Adds each song to the position list
            for (int i = 0; i < Songs.Count; i++)
            {
                songIndexes.Add(i);
            }
            //checks if user wants to sort by title
            if (sortBy == "songName")
            {
                if (ascending)
                {
                    songIndexes = songIndexes.OrderBy(i => Songs[i].Name).ToList();//from A to Z
                }
                else
                {
                    songIndexes = songIndexes.OrderByDescending(i => Songs[i].Name).ToList();//from Z to A
                }
            }
            else if (sortBy == "songArtist")
            {
                if (ascending)
                {
                    songIndexes = songIndexes.OrderBy(i => Songs[i].Artist).ToList();
                }
                else
                {
                    songIndexes = songIndexes.OrderByDescending(i => Songs[i].Artist).ToList();
                }
            }
            else if (sortBy == "songDuration")
            {
                if (ascending)
                {
                    songIndexes = songIndexes.OrderBy(i => GetDurationInSeconds(Songs[i].Duration)).ToList();
                }
                else
                {
                    songIndexes = songIndexes.OrderByDescending(i => GetDurationInSeconds(Songs[i].Duration)).ToList();
                }
            }

            //Creates temporary list to store the sorted information.
            List<Song> sortedSongs = new List<Song>();

            //Goes through their index in the new sorted order
            for (int i = 0; i < songIndexes.Count; i++)
            {
                //Gets original position of a song
                int index = songIndexes[i];

                //Adds info to the temporary list
                sortedSongs.Add(Songs[index]);
            }

            //clear the original lists
            Songs.Clear();

            //Add sorted info back to the original list
            foreach (Song song in sortedSongs)
            {
                Songs.Add(song);
            }

        }


        private int GetDurationInSeconds(string duration)
        {
            //Splits duration such as "3:45"into seconds.
            string[] parts = duration.Split(':');

            //checks if duration contains minutes and seconds
            if (parts.Length == 2)
            {
                int minutes;
                int seconds;

                //minutes snd seconds into numbers
                if (int.TryParse(parts[0], out minutes) && int.TryParse(parts[1], out seconds))
                {
                    return (minutes * 60) + seconds;
                }
            }
            //Return Zero if duration cannot be converted
            return 0;
        }
    }


}