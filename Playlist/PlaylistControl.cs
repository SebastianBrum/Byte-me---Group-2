using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        }

        //Lists for the songNames, artists, and durations
        List<string> songs = new List<string>();
        List<string> artists = new List<string>();
        List<string> songDurations = new List<string>();

        //Basic display setup when the user opens a playlist
        public void setupPlaylistPage(string title)
        {
            lblPlaylistName.Text = title;
            lblPlaylistPathName.Text = title;
            lblDateCreated.Text = null;
            lblWelecome.Text = $"Welcome back, {this.username}";
            playlistVisible(false);
            homeVisible(false);
            addPLaylistButton(false);
            changeCoverButton(false);
            readSongs(title);
            loadImage(title);

            lblPlalistCount.Text = $"This playlist has {songs.Count} songs";

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

        private void loadImage(string playlist)
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


                //  Checks if the current image in the loop is the correct one
                if (currentImage == playlist)
                {
                    pbxPlaylistCoverPhoto.Image = Image.FromFile(coverImage);
                    pbxPlaylistCoverPhoto.SizeMode = PictureBoxSizeMode.StretchImage;

                    // The user has a image for the playlist
                    hasImage = true;
                }
            }

            //Sets the default image if the user didn't set an image themselves
            if (!hasImage)
            {
                string defaultImagePath = Path.Combine(dataFolder, "DefaultCover", "default.png");
                pbxPlaylistCoverPhoto.Image = Image.FromFile(defaultImagePath);
                pbxPlaylistCoverPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
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

            //Clears the lists from old values
            songs.Clear();
            artists.Clear();
            songDurations.Clear();

            //Clears the display panel from old songs
            flpSongs.Controls.Clear();
            Panel titlePanel = createTitlePanel();
            flpSongs.Controls.Add(titlePanel);

            //Reads from the textfile
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    int totalSongs = 0;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        getSongTitle(line);
                        totalSongs++;
                    }

                    addToCreationLabel($"{totalSongs} tracks");
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Playlist file corrupted.");
            }
            catch (Exception)
            {
                MessageBox.Show("Please try again");
            }

            //Displays the songs in the panel
            displaySongs(playlistName);
        }

        //gets the song title from the textfile and saves it into the songs list
        private void getSongTitle(string line)
        {
            int characterPosition = line.IndexOf("|");
            songs.Add(line.Substring(0, characterPosition));

            getSongArtist(line.Substring(characterPosition + 1));
        }

        //Gets the songs artist from the textfile and saves it into the artists list
        private void getSongArtist(string line)
        {
            int characterPosition = line.IndexOf("|");
            artists.Add(line.Substring(0, characterPosition));
            getSongDuration(line.Substring(characterPosition + 1));
        }

        //Gets the song duration from the textfile and saves it into the songDurations List
        private void getSongDuration(string line)
        {
            songDurations.Add(line);
        }

        //Displays the songs into the display panel
        private void displaySongs(string playlistName)
        {
            for (int i = 0; i < songs.Count; i++)
            {
                Panel songPanel = createSongPanel(songs[i], artists[i], songDurations[i], playlistName, i);
                flpSongs.Controls.Add(songPanel);
            }
        }

        //Creates the headers for the display panel
        private Panel createTitlePanel()
        {
            Panel headingPanel = new Panel();
            headingPanel.Width = 784;
            headingPanel.Height = 30;
            headingPanel.Cursor = Cursors.Hand;

            Label Title = new Label();
            Title.Text = "Names";
            Title.Font = new Font("Microsoft Sans Sarif", 10, FontStyle.Bold);
            Title.Width = 53;
            Title.Height = 16;
            Title.AutoSize = false;
            Title.TextAlign = ContentAlignment.MiddleLeft;
            Title.Left = 17;
            Title.Top = 13;

            Label Artist = new Label();
            Artist.Text = "Artists";
            Artist.Font = new Font("Microsoft Sans Sarif", 10, FontStyle.Bold);
            Artist.Width = 53;
            Artist.Height = 16;
            Artist.AutoSize = false;
            Artist.TextAlign = ContentAlignment.MiddleLeft;
            Artist.Left = 241;
            Artist.Top = 13;

            Label Duration = new Label();
            Duration.Text = "Duration";
            Duration.Font = new Font("Microsoft Sans Sarif", 10, FontStyle.Bold);
            Duration.Width = 70;
            Duration.Height = 16;
            Duration.AutoSize = false;
            Duration.TextAlign = ContentAlignment.MiddleLeft;
            Duration.Left = 423;
            Duration.Top = 13;

            headingPanel.Controls.Add(Title);
            headingPanel.Controls.Add(Artist);
            headingPanel.Controls.Add(Duration);

            return headingPanel;
        }

        //Creates the song display
        private Panel createSongPanel(string songName, string artist, string duration, string playlistName, int songIndex)
        {
            //Song panel
            Panel songPanel = new Panel();
            songPanel.Width = 784;
            songPanel.Height = 30;
            songPanel.Cursor = Cursors.Hand;

            //Title
            Label songTitle = new Label();
            songTitle.Text = songName;
            songTitle.Font = new Font("Microsoft Sans Sarif", 8, FontStyle.Regular);
            songTitle.Width = 53;
            songTitle.Height = 16;
            songTitle.AutoSize = false;
            songTitle.TextAlign = ContentAlignment.MiddleLeft;
            songTitle.Left = 17;
            songTitle.Top = 13;

            //Artist
            Label songArtist = new Label();
            songArtist.Text = artist;
            songArtist.Font = new Font("Microsoft Sans Sarif", 8, FontStyle.Regular);
            songArtist.Width = 53;
            songArtist.Height = 16;
            songArtist.AutoSize = false;
            songArtist.TextAlign = ContentAlignment.MiddleLeft;
            songArtist.Left = 241;
            songArtist.Top = 13;

            //Duration
            Label songDuration = new Label();
            songDuration.Text = duration;
            songDuration.Font = new Font("Microsoft Sans Sarif", 8, FontStyle.Regular);
            songDuration.Width = 53;
            songDuration.Height = 16;
            songDuration.AutoSize = false;
            songDuration.TextAlign = ContentAlignment.MiddleLeft;
            songDuration.Left = 423;
            songDuration.Top = 13;

            //Delete button
            Button buttonDelete = new Button();
            buttonDelete.Width = 40;
            buttonDelete.Height = 24;
            buttonDelete.Cursor = Cursors.Hand;
            buttonDelete.Left = flpSongs.Width - buttonDelete.Width - 40;
            buttonDelete.Top = 3;
            buttonDelete.Text = "🗑️";
            buttonDelete.Font = new Font("Microsoft Sans Sarif", 8, FontStyle.Regular);
            buttonDelete.ForeColor = Color.Black;
            buttonDelete.BackColor = Color.Red;

            // Delete functionality. Sends the index in the array as paramater
            buttonDelete.Click += (sender, e) =>
            {
                DeleteSong(songIndex, playlistName);
            };

            // Adds the controls to the song panel
            songPanel.Controls.Add(songTitle);
            songPanel.Controls.Add(songArtist);
            songPanel.Controls.Add(songDuration);
            songPanel.Controls.Add(buttonDelete);


            return songPanel;
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

            // Reads the songs into an array
            string[] songs = File.ReadAllLines(playlistPath);

            // Checks if the playlist does have songs
            if (songs.Length == 0)
            {
                MessageBox.Show("There are no songs to delete.");
                return;
            }

            // Checks if the selected song actually exists in the playlist
            if (songIndex < 0 || songIndex >= songs.Length)
            {
                return;
            }

            // Gets the song that the user selected
            string songToDelete = songs[songIndex];

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

            // Converts array into a list so that it is easier to delete
            List<string> remainingSongs = songs.ToList();

            // Deletes the song from the list
            remainingSongs.RemoveAt(songIndex);

            // Writes remaining songs back into the playlist
            File.WriteAllLines(playlistPath, remainingSongs);

            // Rerenders all the remaining songs to display
            readSongs(playlistName);

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
